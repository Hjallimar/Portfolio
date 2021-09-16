//Author: Hjalmar Andersson
//Author: Johan Liljedahl

#include "BaseItem.h"
#include "Components/SphereComponent.h"
#include "Components/StaticMeshComponent.h"
#include "CustomComponents/InteractableComponent.h"
#include "BaseItem_DataAsset.h"
#include "GameSystem/GM_StockComponent.h"


#include "CustomComponents/ItemHolderComponent.h" //JL
#include "CustomComponents/ItemCarryPositionComponent.h" //JL

//========================= JUSTUS -->
#include "ItemCounter.h"
#include "Components/BoxComponent.h"
#include "GameSound/AudioHandlerComponent.h"
#include "UI/ItemDisplayWidget.h"
#include "UI/HUD/GameHUD.h"
#include "GameSystem/GP3GameMode.h"
#include "Kismet/GameplayStatics.h"
//========================= JUSTUS <--


ABaseItem::ABaseItem()
{
	PrimaryActorTick.bCanEverTick = true;
	Root = CreateDefaultSubobject<USceneComponent>(TEXT("Root"));
	RootComponent = Root;

	Mesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	Mesh->SetCollisionProfileName("BlockAllDynamic");
	Mesh->SetupAttachment(Root);
	OverlayMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Overlay Mesh"));
	OverlayMesh->SetCollisionProfileName("NoCollision");
	OverlayMesh->SetGenerateOverlapEvents(false);
	OverlayMesh->bHiddenInGame = true;
	OverlayMesh->SetupAttachment(Root);
	Interactable = CreateDefaultSubobject<UInteractableComponent>(TEXT("Interactable"));
	BoxCol = CreateDefaultSubobject<UBoxComponent>(TEXT("BoxCollision"));
	BoxCol->SetCollisionProfileName("NoCollision");
	BoxCol->SetupAttachment(Root);
	

	//========================= JUSTUS -->
	AudioHandlerComponent = CreateDefaultSubobject<UAudioHandlerComponent>(TEXT("Audio Handler Component"));
	AudioHandlerComponent->SetupAttachment(Root);
	//========================= JUSTUS <--

	Interactable->OnPlayerHover.AddDynamic(this, &ABaseItem::OnPlayerHover);
	Interactable->OnPlayerStopHover.AddDynamic(this, &ABaseItem::OnPlayerStopHover);

}

void ABaseItem::Tick(float DeltaSeconds) //JL
{
	Super::Tick(DeltaSeconds);
}

void ABaseItem::BeginPlay()
{
	Super::BeginPlay();
	if (ItemStats == nullptr)
		UE_LOG(LogTemp, Log, TEXT("NO DATA ASSET FOUND FOR '%s'"), *GetName());
	auto* GM = Cast<AGP3GameMode>(UGameplayStatics::GetGameMode(this));
	if (GM == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("No GameMode detected"));
		return;
	}
	if(ItemStats->StockType != EStockType::Currency)
	{
		GM->GetStockComponent()->RegisterItem(ItemStats);	
	}
	if(this->ItemStats->StockType == EStockType::Currency)
	{
		BoxCol->SetCollisionProfileName("BlockAllDynamic");
	}
}

void ABaseItem::OnBought()
{
	Mesh->SetCollisionProfileName("NoCollision");
	OverlayMesh->SetCollisionProfileName("NoCollision");
}

void ABaseItem::OnPlayerInteract() //JL
{
	bIsBeingCarried = true;
	Mesh->SetCollisionProfileName("NoCollision");
	OverlayMesh->SetCollisionProfileName("NoCollision");
	UItemDisplayWidget* ItemWidget = GetItemDisplayWidget();
	if (ItemWidget)
	{
		ItemWidget->DisplayWidget(GenerateDisplayItemInfo());
	}
	if(this->ItemStats->StockType == EStockType::Currency)
	{
		BoxCol->SetCollisionProfileName("NoCollision");
	}
	PlayInteractionSound(EItemInteractType::Pickup);
}

void ABaseItem::OnPlayerStopInteract() //JL
{
	bIsBeingCarried = false;
	Mesh->SetCollisionProfileName("BlockAllDynamic");
	OverlayMesh->SetCollisionProfileName("BlockAllDynamic");
	if(this->ItemStats->StockType == EStockType::Currency)
	{
		BoxCol->SetCollisionProfileName("BlockAllDynamic");
	}
	PlayInteractionSound(EItemInteractType::Placedown);
}

void ABaseItem::DeleteThisItem()
{
	if(CurrentItemHolder != nullptr)
	{
		CurrentItemHolder->MakeItemNull();	
	}
	Destroy();
}

void ABaseItem::OnPlayerHover(AActor* Player)
{
	auto CarryComp = Cast<UItemCarryPositionComponent>(Player->FindComponentByClass(UItemCarryPositionComponent::StaticClass()));
	auto CarriedItem = Cast<ABaseItem>(CarryComp->CurrentItem);
	auto MyHolder = this->CurrentItemHolder;
	if(MyHolder != nullptr && CarriedItem != nullptr && CarriedItem != this && !MyHolder->GetOwner()->IsA(AItemCounter::StaticClass()))
	{
		if(!MyHolder->AllowedStockTypes.Contains(CarriedItem->ItemStats->StockType) || MyHolder->WallSlot != CarriedItem->ItemStats->CanHangOnWalls)
		{
			OverlayMesh->SetMaterial(0,Denied);
		}
	}
	else
	{
		OverlayMesh->SetMaterial(0,Allowed);
	}	
	OverlayMesh->SetHiddenInGame(false);
	
	//========================= JUSTUS -->
	// Display the item's name, etc. on the item widget.
	if (!Player)
		return;
	
	UItemDisplayWidget* ItemWidget = GetItemDisplayWidget();
	if (ItemWidget)
	{
		ItemWidget->DisplayWidget(GenerateDisplayItemInfo());
	}
	//========================= JUSTUS <--
}

void ABaseItem::OnPlayerStopHover(AActor* Player)
{
	OverlayMesh->SetMaterial(0,Allowed);
	OverlayMesh->SetHiddenInGame(true);


	//========================= JUSTUS -->
	// Hides the item widget.
	if (!Player)
		return;

	UItemDisplayWidget* ItemWidget = GetItemDisplayWidget();
	if (ItemWidget)
	{
		ItemWidget->HideWidget();
	}
	//========================= JUSTUS <--
}

UItemDisplayWidget* ABaseItem::GetItemDisplayWidget() const
{
	APlayerController* PC = UGameplayStatics::GetPlayerController(GetWorld(), 0);
	if (PC)
	{
		AGameHUD* GameHUD = Cast<AGameHUD>(PC->GetHUD());
		if (GameHUD)
		{
			return GameHUD->GetItemDisplayWidget();
		}
	}

	return nullptr;
}

//========================= JUSTUS -->
FDisplayLabelData ABaseItem::GenerateDisplayItemInfo() const
{
	FDisplayLabelData OutData;
	
	const FText NullData(FText::FromString(TEXT("ERROR: NO ITEM STATS")));

	if (ItemStats)
	{
		FString MainLabel = ItemStats->Name;
		
		OutData.MainLabel = FText::FromString(MainLabel);
	}
	else
	{
		OutData.MainLabel = NullData;
	}

	return OutData;
}

void ABaseItem::PlayInteractionSound(EItemInteractType InteractionType)
{
	if (!ItemStats || !AudioHandlerComponent)
		return;

	USoundWave* SoundToPlay;
	switch (InteractionType)
	{
	case EItemInteractType::Pickup:
		SoundToPlay = ItemStats->PickupSound;
		break;

	case EItemInteractType::Placedown:
		SoundToPlay = ItemStats->PlacedownSound;
		break;

	default:
		UE_LOG(LogTemp, Log, TEXT("Uh oh something went wrong"));
		return;
	}

	AudioHandlerComponent->PlaySound(SoundToPlay);
}
//========================= JUSTUS <--