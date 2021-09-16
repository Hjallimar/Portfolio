//Author Hjalmar Andersson

#include "PlayerJournal.h"
#include "CustomComponents/InteractableComponent.h"
#include "Camera/CameraComponent.h"
#include "Player/PlayerCharacter.h"
#include "CustomComponents/ZoomingComponent.h"
#include "CustomComponents/Movement/MakeFloating.h"
#include "GameSystem/GP3GameMode.h"
#include "Kismet/GamePlayStatics.h"
#include "GameSystem/GM_DayCycleComponent.h"
#include "GameSystem/PlayerMultiController.h"
#include "GameSystem/GM_BargainComponent.h"
#include "GameSystem/GM_DayCycleComponent.h"

APlayerJournal::APlayerJournal()
{
	PrimaryActorTick.bCanEverTick = true;
	InteractComponent = CreateDefaultSubobject<UInteractableComponent>(TEXT("Interact Component"));

	InteractComponent->OnPlayerInteract.AddDynamic(this, &APlayerJournal::OnPlayerInteract);
	InteractComponent->OnPlayerStopInteract.AddDynamic(this, &APlayerJournal::OnPlayerStopInteract);
	InteractComponent->OnPlayerHover.AddDynamic(this, &APlayerJournal::OnPlayerHover);
	InteractComponent->OnPlayerStopHover.AddDynamic(this, &APlayerJournal::OnPlayerStopHover);
	InteractComponent->OnPlayerBreakInteract.AddDynamic(this, &APlayerJournal::OnPlayerBreakInteract);

	Root = CreateDefaultSubobject<USceneComponent>(TEXT("Root"));
	RootComponent = Root;

	BookMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("BookMesh"));
	BookMesh->SetupAttachment(Root);
	OutLine = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Overlay Mesh"));
	OutLine->SetCollisionProfileName("NoCollision");
	OutLine->SetGenerateOverlapEvents(false);
	OutLine->bHiddenInGame = true;
	OutLine->SetupAttachment(Root);

	ZoomingComp = CreateDefaultSubobject<UZoomingComponent>(TEXT("Zooming Comp"));

	ReadingView = CreateDefaultSubobject<USceneComponent>(TEXT("Reading view"));
	ReadingView->SetupAttachment(Root);
}

void APlayerJournal::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if(CurrentIndex == 0)
	{
		TabIndex(0);
	}else if(CurrentIndex > 0 && CurrentIndex < WeaponIndex)
	{
		TabIndex(1);
	}
	else if (CurrentIndex > 0 && CurrentIndex < WeaponIndex + MonsterIndex)
	{
		TabIndex(2);
	}
	else if (CurrentIndex > 0 && CurrentIndex < WeaponIndex + MonsterIndex + 1)
	{
		TabIndex(3);
	}
	else
	{
		TabIndex(4);
	}
}

void APlayerJournal::BeginPlay() 
{
	Super::BeginPlay();
	UpdateJournal();

	auto* GM = Cast<AGP3GameMode>(UGameplayStatics::GetGameMode(this));
	UGM_BargainComponent* Bargain = GM->GetBargainComponent();
	GM->GetDayCycleComponent()->DayStateDelegate.AddDynamic(this, &APlayerJournal::HandleDayState);
	Bargain->Journal = this;
}

void APlayerJournal::OnPlayerInteract(AActor* Player)
{
	if (InteractComponent->Interacted || ZoomingComp->GetZooming())
		return;
	if (!ZoomingComp->IsSetUp())
	{
		APlayerCharacter* PlayerRef = Cast<APlayerCharacter>(Player);
		if (PlayerRef)
		{
			PlayerRef->AssignJournal(this);
			ZoomingComp->SetUpZooming(ReadingView, PlayerRef->GetHeadSocket(), PlayerRef);

			// <JH>
			APlayerMultiController* PMC = PlayerRef->GetController<APlayerMultiController>();
			if (PMC)
			{
				PMC->OnNextPage.AddDynamic(this, &APlayerJournal::NextPage);
				PMC->OnPreviousPage.AddDynamic(this, &APlayerJournal::PreviousPage);
			}
			// </JH>
		}
	}

	bInteracted = true;
	InteractComponent->Interacted = true;
	ZoomingComp->StartZooming();
	if(ActorHasTag("MomsNote")) // Cursed quickfix for MomsNote
	{
		UE_LOG(LogTemp,Log,TEXT("THIS IS NOTE"));
		return;
	}
	
	ChangePage(0);
	Cast<UMakeFloating>(FindComponentByClass(UMakeFloating::StaticClass()))->Active = false;
	UE_LOG(LogTemp,Log,TEXT("Interact, Stop hover"));
}

void APlayerJournal::OnPlayerHover(AActor* Player)
{
	OutLine->SetHiddenInGame(false);
}

void APlayerJournal::OnPlayerStopHover(AActor* Player)
{
	OutLine->SetHiddenInGame(true);
}

void APlayerJournal::OnPlayerStopInteract(AActor* Player)
{
	if (!InteractComponent->Interacted || ZoomingComp->GetZooming())
		return;

	bInteracted = false;
	InteractComponent->Interacted = false;
	ZoomingComp->StartZooming();

	if (bOrderBought && !bPlayedSound)
	{
		bPlayedSound = true;
		PlayYawn();
	}

	// <JH>
	APlayerCharacter* PlayerRef = Cast<APlayerCharacter>(Player);
	if (PlayerRef)
	{
		APlayerMultiController* PMC = PlayerRef->GetController<APlayerMultiController>();
		if (PMC)
		{
			PMC->OnNextPage.RemoveDynamic(this, &APlayerJournal::NextPage);
			PMC->OnPreviousPage.RemoveDynamic(this, &APlayerJournal::PreviousPage);
		}
	}
	// </JH>
	Cast<UMakeFloating>(FindComponentByClass(UMakeFloating::StaticClass()))->Active = true;
}

void APlayerJournal::OnPlayerBreakInteract(AActor* Player)
{
	if (!InteractComponent->Interacted || ZoomingComp->GetZooming())
		return;

	bInteracted = false;
	InteractComponent->Interacted = false;
	ZoomingComp->StartZooming();

	if (bOrderBought && !bPlayedSound)
	{
		bPlayedSound = true;
		PlayYawn();
	}

	if(ActorHasTag("MomsNote")) // Cursed quickfix for MomsNote
	{
		UE_LOG(LogTemp,Log,TEXT("THIS IS NOTE"));
		return;
	}
	
	Cast<UMakeFloating>(FindComponentByClass(UMakeFloating::StaticClass()))->Active = true;
}

void APlayerJournal::ChangePage(int Index)
{
	if (!bInteracted || ZoomingComp->GetZooming() || AllPages.Num() < 1)
	{
		return;
	}
	CurrentIndex += Index;

	if(AfterWork)
	{
		if (CurrentIndex >= WorkIndex + OrderIndex)
			CurrentIndex = WorkIndex + OrderIndex;
		else if (CurrentIndex < 0)
			CurrentIndex = 0;
		int Diff = (WorkIndex + OrderIndex) - CurrentIndex;
		if (Diff < 2)
		{
			ActivateOrderPage(Diff);
		}
		else
		{
			ActivateOrderPage(-1);
		}
	}
	else
	{
		if (CurrentIndex >= WorkIndex)
			CurrentIndex = WorkIndex;
		else if (CurrentIndex < 0)
			CurrentIndex = 0;
	}
	OnPageTurn();
	BookMesh->SetMaterial(0, AllPages[CurrentIndex]);
}

void APlayerJournal::UpdateJournal()
{
	AllPages.Empty();
	AllPages.Add(StartPage);
	int counter = 0;
	if (WeaponIndex >= WeaponCompendium.Num())
		WeaponIndex = WeaponCompendium.Num();
	for (int i = 0; i < WeaponIndex; i++)
	{
		AllPages.Add(WeaponCompendium[i]);
		counter++;
	}

	if (MonsterIndex >= BeastCompendium.Num())
		MonsterIndex = BeastCompendium.Num();
	for (int i = 0; i < MonsterIndex; i++)
	{
		AllPages.Add(BeastCompendium[i]);
		counter++;
	}

	AllPages.Add(UtilityPage);
	counter++;

	for (int i = 0; i < OrderPages.Num(); i++)
	{
		AllPages.Add(OrderPages[i]);
	}

	WorkIndex = counter;
	OrderIndex = OrderPages.Num();
	CurrentIndex = 0;
	BookMesh->SetMaterial(0, AllPages[CurrentIndex]);
}

void APlayerJournal::PreviousPage()
{
	ChangePage(-1);
}

void APlayerJournal::NextPage()
{
	ChangePage(1);
}

void APlayerJournal::JumpToChapter(int Index)
{
	switch(Index)
	{
	case 1:
		//WeaponIndex
		CurrentIndex = 1;
		break;
	case 2:
		//Monster
		CurrentIndex = WeaponIndex + 1;
		break;
	case 3:
		//Utility
		CurrentIndex = WeaponIndex + MonsterIndex + 1;
		break;
	case 4:
		//Order
		CurrentIndex = WeaponIndex + MonsterIndex + 2;
		break;
	}
	ChangePage(0);
}

void APlayerJournal::AddPage(int Index)
{
	switch(Index)
	{
	case 1:
		WeaponIndex++;
		break;
	case 2:
		MonsterIndex++;
		break;
	}
	UpdateJournal();
}

void APlayerJournal::UpdateWeaponIndex(int Pages)
{
	WeaponIndex += Pages;
	int NewIndex = CurrentIndex + Pages;
	UpdateJournal();
	UpdatedJournal();
	ChangePage(NewIndex);
}

void APlayerJournal::HandleDayState(EDayState State)
{
	if(State == EDayState::AfterNewspaper)
	{
		bPlayedSound = false;
		AfterWork = true;
		RefreshOrderPage();
	}
	else if(State != EDayState::OrderDelivery)
	{
		AfterWork = false;
		if (!InteractComponent->Interacted)
		{
			UpdateJournal();
		}
	}
}