//Author: Hjalmar Andersson

#pragma once

#include "Components/ActorComponent.h"
#include "ResetableComponent.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FResetEvent, int, ResetIndex);

UCLASS(Meta = (BlueprintSpawnableComponent))
class UResetableComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UResetableComponent();

	UFUNCTION()
	void OnReset(int i);

	UPROPERTY(BlueprintAssignable)
	FResetEvent TimeToReset;
	virtual void BeginPlay() override;
	virtual void BeginDestroy() override;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	int MyResetIndex = 0;

};