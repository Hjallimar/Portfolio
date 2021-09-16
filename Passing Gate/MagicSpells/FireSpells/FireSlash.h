//Author: Hjalmar Andersson

#pragma once
#include "MagicSpells/BaseSpell.h"
#include "Player/Wizard.h"

#include "FireSlash.generated.h"

class UAbilityStats;
class APawn;

UCLASS()
class AFireSlash : public ABaseSpell
{
	GENERATED_BODY()
public:
	AFireSlash();

	virtual void Activate(FTransform SpawnPos) override;

	UPROPERTY(EditAnywhere)
		UAbilityStats* SlashStats;
	UPROPERTY(EditAnywhere)
	bool DrawDebugLines = true;
	UPROPERTY(EditAnywhere)
	float Radius = 100.0f;
	UFUNCTION(BlueprintImplementableEvent)
	void OnSlash(FVector Pos, FRotator Rotation);
	AWizard* Player;

protected:
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

};