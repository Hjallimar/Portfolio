//Author: Hjalmar Andersson

#pragma once

#include "GameFrameWork/Actor.h"
#include "Player/Wizard.h"

#include "VoidBomb.generated.h"

class UAbilityStats;

UCLASS()
class AVoidBomb : public AActor
{
	GENERATED_BODY()
public:

	AVoidBomb();

	UPROPERTY(EditAnywhere)
	UAbilityStats* BombStats;

	UPROPERTY(EditAnywhere)
	float Delay = 0.5;

	UPROPERTY(EditAnywhere)
	float AreaOfEffect = 0.5;

	UFUNCTION(BlueprintImplementableEvent)
	void OnExplode();
private:
	virtual void Tick(float DeltaTime) override;
	void Explode();
	void DealDamage(TArray<AActor*> Objects);


	AWizard* Player;
	float Timer = 0.0f;
};