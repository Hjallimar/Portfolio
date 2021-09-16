//Author: Hjalmar Andersson

#pragma once
#include "Components/ActorComponent.h"
#include "MagicComponent.generated.h"

class ABaseSpell;
class UMaterialInstance;

UCLASS(meta = (BlueprintSpawnableComponent))
class UMagicComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UMagicComponent();
	virtual void BeginPlay() override;
	UFUNCTION(BlueprintCallable)
	void PrimaryFire(FTransform SpawnTrans);
	UFUNCTION(BlueprintCallable)
	void SecondaryFire(FTransform SpawnTrans);

	UPROPERTY(EditAnywhere)
	TArray<UMaterialInstance*> ElemantalMaterials;

	ABaseSpell* FirstSpell;
	ABaseSpell* SecondSpell;
private:
	UPROPERTY(EditAnywhere)
	TSubclassOf<ABaseSpell> PrimarySpell;

	UPROPERTY(EditAnywhere)
	TSubclassOf<ABaseSpell> SecondarySpell;

};