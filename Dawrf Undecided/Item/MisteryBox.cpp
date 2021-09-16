//Author Hjalmar Andersson
// Johan Liljedahl
#include "MisteryBox.h"
#include "Item/BaseItem.h"
#include "Player/PlayerCharacter.h"
#include "Components/StaticMeshComponent.h"
#include "CustomComponents/InteractableComponent.h"
#include "Kismet/GameplayStatics.h"

AMisteryBox::AMisteryBox()
{
	Root = CreateDefaultSubobject<USceneComponent>(TEXT("Root"));
	RootComponent = Root;

	SpawnPos = CreateDefaultSubobject<USceneComponent>(TEXT("Spawn Pos"));
	SpawnPos->SetupAttachment(Root);

	Mesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	Mesh->SetupAttachment(Root);

	OutLine = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Outline"));
	OutLine->SetupAttachment(Root);
	OutLine->SetGenerateOverlapEvents(false);
	OutLine->SetCollisionProfileName("NoCollision");
	OutLine->SetHiddenInGame(true);

	InteractComp = CreateDefaultSubobject<UInteractableComponent>("Interactable");

	InteractComp->OnPlayerHover.AddDynamic(this, &AMisteryBox::OnPlayerHover);
	InteractComp->OnPlayerInteract.AddDynamic(this, &AMisteryBox::OnPlayerInteract);
	InteractComp->OnPlayerStopInteract.AddDynamic(this, &AMisteryBox::OnPlayerStopInteract);
	InteractComp->OnPlayerStopHover.AddDynamic(this, &AMisteryBox::OnPlayerStopHover);
}

void AMisteryBox::BeginPlay()
{
	Super::BeginPlay();
	TArray<AActor*> DeliveryServiceRefList;
	UGameplayStatics::GetAllActorsOfClass(GetWorld(), ADeliveryService::StaticClass(), DeliveryServiceRefList);
	DeliveryServiceRef = Cast<ADeliveryService>(DeliveryServiceRefList.Last());
}

void AMisteryBox::OnPlayerHover(AActor* Player)
{
	OutLine->SetHiddenInGame(false);
}
void AMisteryBox::OnPlayerInteract(AActor* Player)
{
	auto CarryPos = Cast<UItemCarryPositionComponent>(Player->GetComponentByClass(UItemCarryPositionComponent::StaticClass()));
	if(CarryPos->CurrentItem == nullptr)
	{
		UE_LOG(LogTemp, Log, TEXT("Spawning item"));
		SpawnItem(Player);	
	}
}
void AMisteryBox::OnPlayerStopInteract(AActor* Player)
{
}
void AMisteryBox::OnPlayerStopHover(AActor* Player)
{
	OutLine->SetHiddenInGame(true);
}

void AMisteryBox::SpawnItem(AActor* Player)
{
	ABaseItem* NewItem = GetWorld()->SpawnActor<ABaseItem>(Container[0], SpawnPos->GetComponentTransform());
	Container.RemoveAt(0, 1, true);
	UInteractableComponent* Interact = NewItem->FindComponentByClass<UInteractableComponent>();
	Interact->OnPlayerInteract.Broadcast(Player);
	auto CarryPos = Cast<UItemCarryPositionComponent>(Player->GetComponentByClass(UItemCarryPositionComponent::StaticClass()));
	CarryPos->InteractWithItem(NewItem);
	if (Container.Num() < 1)
	{
		DeliveryServiceRef->BoxesStillUnOpened--;
		if(DeliveryServiceRef->BoxesStillUnOpened == 0)
		{
			DeliveryServiceRef->DayCallerRef->AllCratesOpened = true;
			UE_LOG(LogTemp, Log, TEXT("ALL CRATES OPENED, CAN OPEN STORE"))
		}
		Destroy();
	}
}