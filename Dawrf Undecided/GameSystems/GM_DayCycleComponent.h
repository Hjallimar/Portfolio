//Author Hjalmar Andersson
// Co-Author: Justus Hörberg

#pragma once
#include <stdbool.h>
#include "Components/ActorComponent.h"
#include "DayStateDelegate.h"
#include "GM_DayCycleComponent.generated.h"

UCLASS()
class UGM_DayCycleComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UGM_DayCycleComponent();

// <JH>
protected:
	virtual void BeginPlay() override;
	
	UPROPERTY()
	int Day = 0;
// </JH>
public:

	UPROPERTY()
	FDayStateEvent DayStateDelegate;
	//virtual void TickComponent(float DeltaTime) overide;
	UFUNCTION(BlueprintCallable)
	void StartDay();
	UFUNCTION(BlueprintCallable)
	void StartWorkDay();
	UFUNCTION(BlueprintCallable)
	void EndWorkDay();
	UFUNCTION(BlueprintCallable)
	void EndDay();
	UFUNCTION(BlueprintCallable)
	void AfterNews();
	UFUNCTION(BLueprintCallable)
	void OrderMade();

	int GetDay() const { return Day; }

	UFUNCTION()
	void SetCurrentDay(int32 arg1) { UE_LOG(LogTemp,Log,TEXT("Set current day")) Day = arg1; }


};