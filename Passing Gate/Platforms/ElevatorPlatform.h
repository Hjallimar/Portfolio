//Author: Hjalmar Andersson

#pragma once

#include "BasePlatform.h"
#include "ElevatorPlatform.generated.h"

class UBoxComponent;

UCLASS()
class AElevatorPlatform : public ABasePlatform
{
	GENERATED_BODY()
public:
	AElevatorPlatform();

	UPROPERTY(EditAnyWhere)
	float TravelTime = 0;

	UPROPERTY(EditAnywhere)
	bool ManualTrigger = false;

	UPROPERTY(EditAnywhere)
	bool ManualWaitTime = false;

	UPROPERTY(EditAnyWhere)
	float WaitTime = 0;

	UPROPERTY(EditAnywhere)
	USceneComponent* End;

	UFUNCTION(BlueprintImplementableEvent)
	void OnDestinationReached();

protected:
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override; 
	virtual void OnReseted(int i) override;
	virtual void OnBeginOverlap(AActor* OtherActor) override;
	bool Triggered = false;
	float StopTime = 0;
	float Direction = 1;
	float CurrentTime = 0;
	FVector StartPos;
	FVector EndPos;

	void Wait(float DeltaTime);
	void Travel(float DeltaTime);
	void Move(float t);
};