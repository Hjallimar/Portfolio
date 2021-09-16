//Author: Hjalmar Andersson

#pragma once

#include "MagicSpells/BaseSpell.h"
#include "VoidPulse.generated.h"

class AVoidBomb;

UCLASS()
class AVoidPulse : public ABaseSpell
{
	GENERATED_BODY()
public:
	AVoidPulse();

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	virtual void Activate(FTransform SpawnTrans);
private:
	UPROPERTY(EditDefaultsOnly)
	TSubclassOf<AVoidBomb> VoidBomb;
};