//Author Hjalmar Andersson

#pragma once
#include "DayStateCaller.h"
#include "GameFramework/Actor.h"
#include "GameSystem/DayStateDelegate.h"
#include "DeliveryService.generated.h"


class AMisteryBox;

UCLASS()
class ADeliveryService : public AActor
{
	GENERATED_BODY()
public:
	ADeliveryService();

	UFUNCTION()
	void HandleDayState(EDayState State);

	void AddCrateToList(int i);
	
	int BoxesStillUnOpened = 0;
	ADayStateCaller* DayCallerRef;
private:
	virtual void BeginPlay() override;
	void SpawnCrates();
	UPROPERTY(EditAnywhere)
	TArray<TSubclassOf<AMisteryBox>> AllCrates;
	int currentSpawnPos = 0;
	TArray<int> DeliveryList;

	UPROPERTY(EditAnywhere)
	USceneComponent* Root;

	UPROPERTY(EditAnywhere)
	USceneComponent* DeliveryPos1;

	UPROPERTY(EditAnywhere)
	USceneComponent* DeliveryPos2;

	UPROPERTY(EditAnywhere)
	USceneComponent* DeliveryPos3;
};