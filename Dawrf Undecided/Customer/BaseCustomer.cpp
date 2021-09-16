//Author: Hjalmar Andersson
// Co-Author: Justus H�rberg
// Johan Liljedahl

#include "BaseCustomer.h"
#include "Components/CapsuleComponent.h"
#include "CustomerComponent.h"
#include "CustomComponents/ItemHolderComponent.h"
#include "Item/BaseItem_DataAsset.h"
#include "Item/ItemCounter.h"
#include "Item/BaseItem.h"
#include "CustomComponents/InteractableComponent.h"
#include "CustomComponents/ZoomingComponent.h"
#include "Player/PlayerCharacter.h"
#include "Materials/MaterialInstance.h"
#include "Components/SplineComponent.h"
#include "Components/SplineMeshComponent.h"

//GameMode
#include "GameSystem/GP3GameMode.h"
#include "GameSystem/GM_BargainComponent.h"
#include "GameSystem/GM_CustomizationComponent.h"
#include "GameSystem/GM_CustomerController.h"
#include "Kismet/GamePlayStatics.h"
//#include "UI/Subtitles/CustomerPersonalDialogAsset.h"
#include "UI/Subtitles/DialogWidget.h"
#include "UI/HUD/GameHUD.h"


ABaseCustomer::ABaseCustomer() 
{
	PrimaryActorTick.bCanEverTick = true;
	Capsule = CreateDefaultSubobject<UCapsuleComponent>(TEXT("Capsule"));
	Capsule->SetCapsuleSize(42.0f, 96.0f, true);
	Capsule->SetCollisionProfileName("BlockAllDynamic");
	RootComponent = Capsule;

	InteractComp = CreateDefaultSubobject<UInteractableComponent>(TEXT("Interact Component"));
	InteractComp->OnPlayerInteract.AddDynamic(this, &ABaseCustomer::OnPlayerInteract);

	SkeletalMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("Skeletal Mesh"));
	SkeletalMesh->SetupAttachment(Capsule);

	ItemHolder = CreateDefaultSubobject<USceneComponent>(TEXT("Item Holder"));
	ItemHolder->SetupAttachment(SkeletalMesh, TEXT("ItemSocket"));

	HatPosition = CreateDefaultSubobject<USceneComponent>(TEXT("Hat Position"));
	HatPosition->SetupAttachment(SkeletalMesh, TEXT("HatSocket"));

	BeardPosition = CreateDefaultSubobject<USceneComponent>(TEXT("Beard Position"));
	BeardPosition->SetupAttachment(SkeletalMesh, TEXT("BeardSocket"));

	EarsPosition = CreateDefaultSubobject<USceneComponent>(TEXT("Ear Position"));
	EarsPosition->SetupAttachment(SkeletalMesh, TEXT("EarSocket"));
	
	NosePosition = CreateDefaultSubobject<USceneComponent>(TEXT("Nose Position"));
	NosePosition->SetupAttachment(SkeletalMesh, TEXT("NoseSocket"));

	ZoomPosition = CreateDefaultSubobject<USceneComponent>(TEXT("Zoom Position"));
	ZoomPosition->SetupAttachment(Capsule);
	ZoomComp = CreateDefaultSubobject<UZoomingComponent>(TEXT("Zoom Component"));
	
	InteractComp->OnPlayerHover.AddDynamic(this, &ABaseCustomer::OnPlayerHover);
	InteractComp->OnPlayerStopHover.AddDynamic(this, &ABaseCustomer::OnPlayerStopHover);
}

void ABaseCustomer::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void ABaseCustomer::BeginPlay()
{
	Super::BeginPlay();
	SpawnPos = GetActorLocation();

	GetWorld()->GetAuthGameMode<AGP3GameMode>();

	auto* GM = Cast<AGP3GameMode>(UGameplayStatics::GetGameMode(this));
	if (GM == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("No GameMode detected"));
		return;
	}

	Controller = GM->GetCustomerController();
	Controller->AssignCustomer(this);
	BargainComp = GM->GetBargainComponent();
	APlayerController* PlayerCtrl = UGameplayStatics::GetPlayerController(GetWorld(), 0);
	GameHUD = Cast<AGameHUD>(PlayerCtrl->GetHUD());

	if(SplineHolder != nullptr)
		MoveSpline = SplineHolder->FindComponentByClass<USplineComponent>();
	if (MoveSpline == nullptr)
		UE_LOG(LogTemp, Log, TEXT("You have no spline...sadge"));
}

void ABaseCustomer::OnPlayerInteract(AActor* Player)
{
	if (!Interactable || InteractComp->Interacted)
	{
		return;
	}
	if(bActivateZoom)
	{
		if(!ZoomComp->IsSetUp())
		{
			PlayerRef = Cast<APlayerCharacter>(Player);
			ZoomComp->SetUpZooming(ZoomPosition, PlayerRef->GetHeadSocket(), PlayerRef);
		}

		if (InteractComp->Interacted || ZoomComp->GetZooming())
			return;
		InteractComp->Interacted = true;
		ZoomComp->StartZooming();
	}
	if (GameHUD)
	{
		float Deal = BargainComp->CompareItems();
		FOnDialogBoxEvent TextEvent;
		if(Deal <= 1)
		{
			if(Deal > 0.5f)
			{
				TextEvent.BindDynamic(this, &ABaseCustomer::OnBuyItem);
				GameHUD->InitiateTextbox(Controller->GetGoodDialog(GetLevel()), TextEvent, StatNameList);
			}
			else
			{
				TextEvent.BindDynamic(this, &ABaseCustomer::OnBuyItem);
				GameHUD->InitiateTextbox(Controller->GetBadDialog(GetLevel()), TextEvent, StatNameList);
			}
			EmojiEvent(Deal);
		}
		else
		{
			AssignDisplayInfo();	
			TextEvent.BindDynamic(this, &ABaseCustomer::OnDialogOver);
			GameHUD->InitiateTextbox(Controller->GetDialog(GetLevel()), TextEvent, StatNameList);
			StatNameList.Empty();
		}
	}
	OnPlayerStopHover(Player);
}

void ABaseCustomer::OnPlayerBreakInteract(AActor* Player)
{
}

void ABaseCustomer::ResetCustomer() 
{
	if(MyItem != nullptr)
	{
		MyItem->DeleteThisItem();
		Grabbed = false;
	}

	//<JH>
	UE_LOG(LogTemp, Log, TEXT("Customer is Level %i"), GetLevel());
	if (GetLevel() == 1)
	{
		FString WType = Level2WantType == ELevel2WantType::DamageType ? TEXT("want DamageType") : TEXT("want WeaponType");
		UE_LOG(LogTemp, Log, TEXT("%s"), *WType);
	}
	//</JH>
}

void ABaseCustomer::Move(float Timer)
{
	if (MoveSpline != nullptr) 
	{
		float Distance = MoveSpline->GetSplineLength() * Timer;
		FVector MovePos = MoveSpline->GetLocationAtDistanceAlongSpline(Distance, ESplineCoordinateSpace::World);
		if(!SidewayWalking)
		{
			FVector LookAtVector = (GetActorLocation() - MovePos) * -1.0f;
			SetActorRotation(LookAtVector.Rotation());
		}

		SetActorLocation(MovePos);
	}
	else
		SetActorLocation(FMath::Lerp(SpawnPos, ShopPos, Timer));
}

void ABaseCustomer::OnDialogOver(int Decision)
{
	if (!InteractComp->Interacted)
		return;
	InteractComp->Interacted = false;
	OnPlayerHover(PlayerRef);
	if(bActivateZoom)
	{
		ZoomComp->StartZooming();
	}
}

void ABaseCustomer::OnBuyItem(int value)
{
	BargainComp->Bargain();
	MyItem->OnPlayerStopHover(PlayerRef);
}

void ABaseCustomer::AssignDisplayInfo()
{
	// <JH>
	// Change if level 2, based on if weapontype or damagetype. Otherwise, always weapontype.
	FString SecondArgument;
	if (CurrentLevel != 1 || GetWantType() == ELevel2WantType::WeaponType)
	{
		SecondArgument = StaticEnum<EWeaponType>()->GetNameStringByValue((int)TargetItem->WeponType);
	}
	else
	{
		SecondArgument = StaticEnum<EDamageType>()->GetNameStringByValue((int)TargetItem->DamageType);
	}
	// </JH>

	StatNameList.Add(NameOfCustomer); //0
	
	StatNameList.Add(StaticEnum<EWieldType>()->GetNameStringByValue((int)TargetItem->WieldType)); // 1 {is unused}
	StatNameList.Add(SecondArgument); // 2 {shifts between damage/weapontype if level 2}
	StatNameList.Add(StaticEnum<EDamageType>()->GetNameStringByValue((int)TargetItem->DamageType)); // 3
	StatNameList.Add(StaticEnum<EUtilityType>()->GetNameStringByValue((int)TargetItem->UtilityType)); // 4
	// 4 + X;
	FString NameString;
	if(TargetItem->StrongAgainst.Num() == 1)
	{
		NameString = StaticEnum<EMonsterTypes>()->GetNameStringByValue((int)TargetItem->StrongAgainst[0]);
		NameString = NameString.ToUpper();
		NameString += TEXT("S");

		StatNameList.Add(NameString);
	}
	else
	{
		for (int X = 0; X < TargetItem->StrongAgainst.Num() - 1; X++)
		{
			NameString = StaticEnum<EMonsterTypes>()->GetNameStringByValue((int)TargetItem->StrongAgainst[X]);
			NameString = NameString.ToUpper();
			NameString += TEXT("S");

			StatNameList.Add(NameString);
		}
	}

	// 4 + X + Y
	if (TargetItem->WorksAgainst.Num() == 1)
	{
		StatNameList.Add(StaticEnum<EMonsterTypes>()->GetNameStringByValue((int)TargetItem->WorksAgainst[0]));
	}
	else
	{
		for (int Y = 0; Y < TargetItem->WorksAgainst.Num() - 1; Y++)
		{
			StatNameList.Add(StaticEnum<EMonsterTypes>()->GetNameStringByValue((int)TargetItem->WorksAgainst[Y]));
		}
	}
}

void ABaseCustomer::OnPlayerHover(AActor* Player)
{
	UItemDisplayWidget* ItemDisplayWidget = GameHUD->GetItemDisplayWidget();
	ItemDisplayWidget->DisplayWidget(GenerateDisplayItemInfo());
}

void ABaseCustomer::OnPlayerStopHover(AActor* Player)
{
	if (!Player) return;

	UItemDisplayWidget* ItemDisplayWidget = GameHUD->GetItemDisplayWidget();
	if (ItemDisplayWidget)
	{
		ItemDisplayWidget->HideWidget();
	}
}

FDisplayLabelData ABaseCustomer::GenerateDisplayItemInfo()
{
	FDisplayLabelData OutData;
	const FText NullData(FText::FromString(TEXT("ERROR: NO ITEM STATS")));
	FString MainLabel = NameOfCustomer;
	OutData.MainLabel = FText::FromString(MainLabel);

	return OutData;
}

void ABaseCustomer::AssignNewName(FString NewName)
{
	NameOfCustomer = NewName;
}

void ABaseCustomer::EquipHat(AActor* NewHat)
{
	if (NewHat == nullptr)
		return;
	if (CurrentHat != nullptr)
		Controller->ActivateActor(CurrentHat, false);
	CurrentHat = NewHat;
	Controller->ActivateActor(CurrentHat, true);
	NewHat->AttachToComponent(HatPosition, FAttachmentTransformRules::SnapToTargetIncludingScale);
}

void ABaseCustomer::EquipBeard(AActor* NewBeared)
{
	if (NewBeared == nullptr)
		return;
	if (CurrentBeard != nullptr)
		Controller->ActivateActor(CurrentBeard, false);
	CurrentBeard = NewBeared;
	Controller->ActivateActor(CurrentBeard, true);
	NewBeared->AttachToComponent(BeardPosition, FAttachmentTransformRules::SnapToTargetIncludingScale);	
}

void ABaseCustomer::EquipEars(AActor* NewEars)
{
	if (NewEars == nullptr)
		return;
	else if(CurrentEars != nullptr)
		Controller->ActivateActor(CurrentEars, false);
	CurrentEars = NewEars;
	Controller->ActivateActor(CurrentEars, true);
	NewEars->AttachToComponent(EarsPosition, FAttachmentTransformRules::SnapToTargetIncludingScale);

}

void ABaseCustomer::EquipNose(AActor* NewNose)
{
	if (NewNose == nullptr)
		return;
	else if (CurrentNose != nullptr)
		Controller->ActivateActor(CurrentNose, false);
	CurrentNose = NewNose;
	Controller->ActivateActor(CurrentNose, true);
	NewNose->AttachToComponent(NosePosition, FAttachmentTransformRules::SnapToTargetIncludingScale);
}

void ABaseCustomer::ChangeColor(UMaterialInstance* NewColor)
{
	if (NewColor != nullptr)
	{
		CurrentColor = NewColor;
	}
	SkeletalMesh->SetMaterial(0, CurrentColor);
	CurrentEars->FindComponentByClass<UStaticMeshComponent>()->SetMaterial(0, CurrentColor);
	CurrentNose->FindComponentByClass<UStaticMeshComponent>()->SetMaterial(0, CurrentColor);
}

void ABaseCustomer::SetLevel(int Level)
{
	CurrentLevel = Level;
}

int ABaseCustomer::GetLevel() const
{
	return CurrentLevel;
}

void ABaseCustomer::SetLevel2WantType(ELevel2WantType WantType)
{
	Level2WantType = WantType;
}

ELevel2WantType ABaseCustomer::GetWantType() const
{
	return Level2WantType;
}

void ABaseCustomer::BuyItem(ABaseItem* Item)
{
	InteractComp->Interacted = false;
	if (bActivateZoom)
	{
		ZoomComp->StartZooming();
	}
	MyItem = Item;
	MyItem->CurrentItemHolder->MakeItemNull();
	Controller->ItemBought(MyItem->ItemStats);
	MyItem->OnBought();
	Interactable = false;
}

void ABaseCustomer::GrabBoughtItem()
{
	if (Grabbed)
		return;
	Grabbed = true;
	MyItem->OnPlayerInteract();
	MyItem->PlayInteractionSound(EItemInteractType::Pickup);
	MyItem->SetActorLocation(ItemHolder->GetComponentLocation());
	MyItem->AttachToComponent(ItemHolder, FAttachmentTransformRules::SnapToTargetNotIncludingScale);
	BargainComp->AssignCustomer(nullptr);
	MyItem->OnPlayerStopHover(PlayerRef);
}

UBaseItem_DataAsset* ABaseCustomer::GetItemStats()
{
	return TargetItem;
}

void ABaseCustomer::AssignNewItemStats(UBaseItem_DataAsset* NewStats)
{
	if (NewStats == nullptr)
	{
		UE_LOG(LogTemp, Log, TEXT("Assigned item was a nullptr")); 
		return;
	}
	
	TargetItem = NewStats;
	BargainComp->AssignCustomer(this);
}