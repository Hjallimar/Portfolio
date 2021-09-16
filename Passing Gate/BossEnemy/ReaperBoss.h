//Author Hjalmar Andersson

#pragma once
#include "AIController.h"
#include "GameFramework/Character.h"
#include "GameFramework/Actor.h"
#include "GameFramework/PawnMovementComponent.h"
#include  "MagicSpells/AbilityStats.h"
#include "ReaperBoss.generated.h"

class UHealthComponent;
class USkeletalMeshComponent;
class UCapsuleComponent;
class USceneComponent;
class UResetableComponent;
class AForwardProjectile;
class UMaterialInterface;

UCLASS()
class AReaperBoss : public ACharacter
{
	GENERATED_BODY()
public:
	AReaperBoss();
	UPROPERTY(EditAnywhere)
	USceneComponent* Root;
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UCapsuleComponent* Capsule;
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USkeletalMeshComponent* BossMesh;
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* FireRotator;
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* FirePoint;
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UHealthComponent* HealthComponent;
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UResetableComponent* ResetComponent;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	AAIController* AIController;

	UFUNCTION(BlueprintImplementableEvent)
	void OnDamageTaken(UAbilityStats* Stats);	
	UFUNCTION(BlueprintImplementableEvent)
	void OnDeath();
	UFUNCTION(BlueprintImplementableEvent)
	void OnSlash();
	UFUNCTION(BlueprintImplementableEvent)
	void OnRun();
	UFUNCTION(BlueprintImplementableEvent)
	void SpawnBP_Projectile(FVector Location, FRotator Rotation);
	UFUNCTION(BlueprintImplementableEvent)
	void ActivatePhase(int i);
	UFUNCTION(BlueprintImplementableEvent)
	void StartIntermission();
	UFUNCTION(BlueprintImplementableEvent)
	void OnPlatformThreshold(int i);

	UFUNCTION(BlueprintCallable)
	void ActivateBoss();
	UFUNCTION(BlueprintCallable)
	void AddProjectileToPool(AForwardProjectile* Projectile);
	UFUNCTION(BlueprintCallable)
	void ReturnProjectileToPool(AForwardProjectile* Projectile);
	UFUNCTION(BlueprintCallable)
	void ReduceMaxProjectiles(int i);

protected:
	virtual void Tick(float DeltaTime) override;
	virtual void BeginPlay() override;
	void ChangePhase();
	void PhaseOne(float DeltaTime);
	void PhaseTwo(float DeltaTime);
	void PhaseThree(float DeltaTime);

	//Phase 1
	void MoveBoss(float DeltaTime);
	void MoveSlash(float DeltaTime);
	void SlashForward(float DeltaTime);

	UPROPERTY(EditAnywhere, Category = Attack)
	bool DrawDebug = false;

	UPROPERTY(EditAnywhere, Category = Attack)
	UAbilityStats* SlashStats;
	UPROPERTY(EditAnywhere, Category = Attack)
	float SlashCD = 0.0f;
	UPROPERTY(EditAnywhere, Category = Attack)
	float SlashRadius = 500.0f;
	float TimeSinceLastSlash = 0.0f;

	UPROPERTY(EditAnywhere, Category = Attack)
	float MoveDuration = 8.0f;
	UPROPERTY(EditAnywhere, Category = Attack)
	float SlashDuration = 8.0f;
	float MoveTimer = 0.0f;
	bool Slashing = false;

	UPROPERTY(EditAnywhere, Category = Attack)
	float MoveSpeed = 150.0f;
	UPROPERTY(EditAnywhere, Category = Attack)
	float MoveSlashSpeed = 450.0f;

	//Phase 2
	void Teleport();
	UPROPERTY(EditAnywhere, Category = Attack)
	float PhaseDelay = 2.5f;
	bool DelayDone = false;
	float DelayTimer = 0.0f;
	UPROPERTY(EditAnywhere)
	float PhaseTwoRotation = 10.0f;
	void SpawnProjectile(float DeltaTime);
	UPROPERTY(EditAnywhere, Category = Attack)
	float WaveInterwall = 2.5f;
	UPROPERTY(EditAnywhere, Category = Attack)
	int MinAmmountProjectile = 12;	
	UPROPERTY(EditAnywhere, Category = Attack)
	int MaxAmmountProjectile = 36;

	float WaveTimer = 0.0f;
	TArray<AForwardProjectile*> ActiveProjectiles;
	TArray<AForwardProjectile*> ProjectilePool;
	UPROPERTY(EditAnywhere, Category = Health)
	float PhaseTwoThreshold = 40.0f;

	//Phase 3
	
	void Beams();
	void CheckHit(TArray<FHitResult> Hit);
	void PhaseThreeWaitTime(float DeltaTime);
	UPROPERTY(EditAnywhere)
	float BeamDistance = 1000.0f;

	UPROPERTY(EditAnywhere)
	float RotationSpeed = 30.0f;
	UPROPERTY(EditAnywhere, Category = Health)
	float PhaseThreeThreshold = 20.0f;
	//-----
	UPROPERTY(EditAnywhere, Category = Health)
	TArray<float> PlatformThresholds;
	int PlatformThreshold = 0;
	UPROPERTY(EditAnywhere, Category = Stats)
	TArray<UMaterialInterface*> ElementMaterials;
	int CurrentElement = 0;

	FTransform SpawnTrans;
	bool Active = false;
	APawn* PlayerRef;
	int Phase = 1;
	bool Immune = false;
	AActor* MyActor;

	UFUNCTION()
	void Reseting(int i);
	UFUNCTION()
	void TakeDamage(UAbilityStats* Stats);
};