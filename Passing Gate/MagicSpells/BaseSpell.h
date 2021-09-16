//Primary author: Hjalmar Andersson

#pragma once

#include "GameFramework/Actor.h"
#include "BaseSpell.generated.h"

class UAbilityStats;

UCLASS()
class ABaseSpell : public AActor
{
	GENERATED_BODY()
public:
	ABaseSpell();

	UPROPERTY(EditAnywhere)
	float CoolDown = 0.5f;
	float CoolDownTimer = 0.0f;

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	virtual void Activate(FTransform SpawnTrans);
};