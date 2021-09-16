//Author: Hjalmar Andersson

#pragma once
#include "GameFramework/Actor.h"
#include "ForwardProjectile.generated.h"

class USphereComponent;
class UStaticMeshComponent;
class UAbilityStats;

UCLASS()
class AForwardProjectile : public AActor
{
	GENERATED_BODY()
public:
	AForwardProjectile();
	
	UPROPERTY(EditAnywhere)
	UAbilityStats* ProjectileStats;

	UPROPERTY(EditAnywhere)
	USphereComponent* Sphere;

	UPROPERTY(EditAnywhere)
	UStaticMeshComponent* Mesh;

	UPROPERTY(EditAnywhere)
	float TravelSpeed = 100.0f;

	UPROPERTY(EditAnywhere)
	float MaxLifeTime = 5.0f;

	UFUNCTION(BlueprintImplementableEvent)
	void OnActive();

	UFUNCTION(BlueprintImplementableEvent)
	void OnHit();

	UFUNCTION(BlueprintCallable)
	void DestroyActorInGame();

	UFUNCTION(BlueprintCallable)
	void AssignParent(AActor* NewParent);

	UFUNCTION(BlueprintImplementableEvent)
	void ReturnToPool();
	UFUNCTION(BlueprintCallable)
	void SpawnFromPool();

	UPROPERTY(BlueprintReadOnly)
	AActor* Parent;
protected:
	float LifeTime = 0;
	UPROPERTY(EditAnywhere)
	bool Destroyable = true;
	bool active = true;
	void SetActiveStatus(bool status);
	TArray<AActor*> IgnoreList;
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	void MoveProjectile(float DeltaTime);
};