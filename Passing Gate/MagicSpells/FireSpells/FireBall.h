//Author Hjalmar Andersson

#pragma once
#include "MagicSpells/BaseSpell.h"
#include "Player/Wizard.h"

#include "FireBall.generated.h"

class AForwardProjectile;

UCLASS()
class AFireBall : public ABaseSpell
{
	GENERATED_BODY()
public:
	AFireBall();

	UPROPERTY(EditAnywhere)
	TSubclassOf<AForwardProjectile> FireProjectile;

	virtual void Activate(FTransform SpawnPos) override;

protected:
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	AWizard* Player;

};