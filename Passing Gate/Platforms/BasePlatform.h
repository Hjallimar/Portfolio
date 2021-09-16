//Author: Hjalmar Andersson
#pragma once

#include "GameFramework/Actor.h"

#include "BasePlatform.generated.h"

class APawn;
class UStaticMeshComponent;
class USceneComponent;
class UBoxComponent;
class UResetableComponent;

UCLASS(ABSTRACT)
class ABasePlatform : public AActor
{
	GENERATED_BODY()
public:
	ABasePlatform();
	UPROPERTY(EditDefaultsOnly)
	USceneComponent* Root;

	UPROPERTY(EditDefaultsOnly)
	UStaticMeshComponent* Mesh;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	UBoxComponent* TriggerZone;

	UPROPERTY(EditAnywhere)
	UResetableComponent* ResetComponent;

	UPROPERTY(EditAnywhere)
	bool PlayerSpecific = true;

	UPROPERTY(EditAnywhere)
	bool MovePlayer = true;

	UFUNCTION(BlueprintCallable)
	void ManualReset();

protected:
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	UFUNCTION()
	void OverlapBegin(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
	UFUNCTION()
	void OverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);
	UFUNCTION()
	void TimeToReset(int i);

	UFUNCTION(BlueprintImplementableEvent)
		void OnSpawn();
	UFUNCTION(BlueprintImplementableEvent)
		void OnDespawn();

	UFUNCTION(BlueprintImplementableEvent)
	void OnReset();
	virtual void OnReseted(int i);
	bool Activated = false;
	virtual void OnBeginOverlap(AActor* OtherActor);
	virtual void OnEndOverlap(AActor* OtherActor);

	APawn* PlayerRef;
};