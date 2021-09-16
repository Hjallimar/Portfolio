//Author Hjalmar Andersson
// Johan Liljedahl
#pragma once
#include "GameFramework/Actor.h"

#include "GameSystem/DayStateDelegate.h"
#include "DayStateCaller.generated.h"

class UGM_DayCycleComponent;
class UInteractableComponent;

UCLASS()
class ADayStateCaller : public AActor
{
	GENERATED_BODY()
public:
	ADayStateCaller();

	UFUNCTION()
	void DayStateHandler(EDayState State);

	UFUNCTION(BlueprintCallable)
	void SendDayStateEvent(EDayState State);

	UFUNCTION(BlueprintImplementableEvent)
	void OnDayEventIndex(int State);

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool AcivateButton = true;
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool AllCratesOpened = false;
protected:
	virtual void BeginPlay() override;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UInteractableComponent* InteractComp;
	UGM_DayCycleComponent* DayCycle;
	
};