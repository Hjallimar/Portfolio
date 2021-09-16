//Author Hjalmar Andersson

#include "DeliveryService.h"
//#include "GameSystem/GM_DayCycleComponent.h"
#include "GameSystem/GP3GameMode.h"
#include "GameSystem/GM_BargainComponent.h"
#include "Kismet/GamePlayStatics.h"
#include "MisteryBox.h"
#include "GameSystem/GM_DayCycleComponent.h"

ADeliveryService::ADeliveryService()
{
	Root = CreateDefaultSubobject<USceneComponent>(TEXT("Root"));
	RootComponent = Root;

	DeliveryPos1 = CreateDefaultSubobject<USceneComponent>(TEXT("Delivery position 1"));
	DeliveryPos1->SetupAttachment(Root);

	DeliveryPos2 = CreateDefaultSubobject<USceneComponent>(TEXT("Delivery position 2"));
	DeliveryPos2->SetupAttachment(Root);

	DeliveryPos3 = CreateDefaultSubobject<USceneComponent>(TEXT("Delivery position 3"));
	DeliveryPos3->SetupAttachment(Root);
}

void ADeliveryService::BeginPlay()
{
	Super::BeginPlay();
	TArray<AActor*> CallerList;
	UGameplayStatics::GetAllActorsWithTag(GetWorld(), "StartWorkDay", CallerList);
	DayCallerRef = Cast<ADayStateCaller>(CallerList.Last());
	
	auto* GM = Cast<AGP3GameMode>(UGameplayStatics::GetGameMode(this));
	GM->GetDayCycleComponent()->DayStateDelegate.AddDynamic(this, &ADeliveryService::HandleDayState);
	GM->GetBargainComponent()->PostService = this;
	if(!UGameplayStatics::GetActorOfClass(this, AMisteryBox::StaticClass()))
	{
		DayCallerRef->AllCratesOpened = true;
		UE_LOG(LogTemp, Warning, TEXT("NO CRATES, CAN OPEN STORE"));
		BoxesStillUnOpened = 0;
	}
	else
	{
		TArray<AActor*> CrateList;
		UGameplayStatics::GetAllActorsWithTag(GetWorld(), "Crate", CrateList);
		DayCallerRef->AllCratesOpened = false;
		BoxesStillUnOpened = CrateList.Num();
		UE_LOG(LogTemp, Warning, TEXT("%i CRATES UNOPENED - CANNOT OPEN STORE"), BoxesStillUnOpened);
	}
}

void ADeliveryService::HandleDayState(EDayState State)
{
	if(State == EDayState::StartDay)
	{
		SpawnCrates();
		currentSpawnPos = 0;
	}
}

void ADeliveryService::AddCrateToList(int i)
{
	UE_LOG(LogTemp, Log, TEXT("Making an order"));
	DeliveryList.Add(i);
}

void ADeliveryService::SpawnCrates()
{
	if (AllCrates.Num() < 1)
		return;
	
	for(int i = 0; i < DeliveryList.Num(); i++)
	{
		FTransform SpawnTrans;

		UE_LOG(LogTemp, Log, TEXT("Spawning an order"));
		switch (currentSpawnPos)
		{
		case 0:
			SpawnTrans = DeliveryPos1->GetComponentTransform();
			break;
		case 1:
			SpawnTrans = DeliveryPos2->GetComponentTransform();
			break;
		case 2:
			SpawnTrans = DeliveryPos3->GetComponentTransform();
			break;
		}
		currentSpawnPos++;
		BoxesStillUnOpened++;
		DayCallerRef->AllCratesOpened = false;
		AMisteryBox* NewItem = GetWorld()->SpawnActor<AMisteryBox>(AllCrates[DeliveryList[i]], SpawnTrans);
	}
	DeliveryList.Empty();
}