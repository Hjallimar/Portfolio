//Author: Hjalmar Andersson

#pragma once

#include "MagicSpells/BaseSpell.h"
#include "VoidTeleport.generated.h"

UCLASS()
class AVoidTeleport : public ABaseSpell
{
	GENERATED_BODY()
public:
	AVoidTeleport();

	UPROPERTY(EditAnywhere)
	bool DrawDebugLines = false;

	UPROPERTY(EditAnywhere)
	float MaxDistance = 100.0f;
	UFUNCTION(BlueprintImplementableEvent)
	void OnTeleportFrom(FTransform From, FVector To);
	UFUNCTION(BlueprintImplementableEvent)
	void OnTeleportTo(FVector To);

	UPROPERTY(EditAnywhere)
	float TeleportDelay = 0.4f;
	UPROPERTY(EditAnywhere)
	float UpOffset = 60.0f;
	
	virtual void Activate(FTransform SpawnPoint) override;
protected:
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	APawn* PlayerRef;
	FVector ToPos;
	FHitResult Hit;
	bool TeleportDelayed = false;
	float DelayTimer = 0.0f;
	void ActivateTeleport(FVector To);
	void PerformCapsuleCast(FVector From, FVector To, FColor Color = FColor::Red);
	void RescaleTeleport();
	void Teleport(FVector TeleportPoint);
};
