//Author Olof Wallentin - Hazelight
//This class was taken from one of his courses and additions to the original code will be documented
//Class, function and variables may have their names changed without documentation
//Secondary Author - Johan Liljedahl

#pragma once

#include "CoreMinimal.h"
#include "CustomComponents/Movement/MovementData.h"
#include "GameFramework/Pawn.h"
#include "Camera/CameraComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "MagicSpells/MagicComponent.h"


#include "Wizard.generated.h"

class UInputComponent;
class USkeletalMeshComponent;
class UCapsuleComponent;
class UCameraComponent;
class UPlayerMovementComponent;

class UHealthComponent;
class UResetableComponent;
class UAbilityStats;

class AForwardProjectile;

UCLASS()
class AWizard : public APawn
{
	GENERATED_BODY()
public:
	AWizard();
	UPROPERTY(VisibleDefaultsOnly, Category = Collision)
	UCapsuleComponent* Capsule;

	UPROPERTY(VisibleDefaultsOnly, BlueprintReadOnly, Category = Mesh)
	USkeletalMeshComponent* Mesh;

	//Hjalmar 
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Mesh)
	UHealthComponent* HealthComponent;

	UPROPERTY(EditAnywhere, Category = Mesh)
	UResetableComponent* ResetComponent;
	//-------

	UPROPERTY(VisibleAnywhere, Category = Movement)
	UPlayerMovementComponent* MovementComponent;

	UPROPERTY(EditAnywhere, Category = Movement)
	float DashPower;

	UPROPERTY(EditAnywhere, Category = Movement)
	float DashDuration;

	UPROPERTY(EditAnywhere, Category = Movement)
	float DashCooldown = 0.5;
	UPROPERTY(EditAnywhere, Category = Movement)
	float DashAnimationGrace = 0.5;

	UPROPERTY(EditAnywhere, Category = Movement)
	float StaggerDur = 0.3;

	UPROPERTY(EditAnywhere, Category = Movement)
	float KnockbackDuration;

	UPROPERTY(EditAnywhere, Category = Movement)
	float TurnRate = 0.3;
	
	UPROPERTY(EditAnywhere, Category = SpawnPos)
	float SpawnPosMinDistance = 200;
	UPROPERTY(EditAnywhere, Category = SpawnPos)
	float SpawnPosMaxDistance = 600;
	
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
	UCameraComponent* Camera;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category= Combat)
	TArray<UMagicComponent*> MagicComponents;

	virtual void Tick(float DeltaTime) override;

	UPROPERTY(EditAnywhere)
	USceneComponent* FirePos;

	UPROPERTY(EditAnywhere)
	USceneComponent* SpawnPosition;

	UPROPERTY(EditAnywhere)
	TSubclassOf<AActor> MouseOrb;
	AActor* Orb;

	UFUNCTION(BlueprintCallable)
	void MakeOrbVisible(bool status);

	UFUNCTION(BlueprintCallable)
	void UpdateMagic(int i);

	//Hjalmar
	UFUNCTION(BlueprintImplementableEvent)
	void OnShieldActivate(int i);

	UFUNCTION(BlueprintCallable)
	void ShieldPickUp();

	UFUNCTION(BlueprintImplementableEvent)
	void OnShieldBreak();

	UFUNCTION(BlueprintCallable)
	void RemovePlayerShield();

	UPROPERTY(EditAnywhere, Category = Mesh)
	float ImmuneTime = 1.0f;
	//----

	UCapsuleComponent* GetCapsule() const { return Capsule; }
	UCameraComponent* GetCamera();
	USceneComponent*  GetSpawnPos();
	AActor* GetOrb();
	void Stagger();
	void SetStaggerDir(FVector Dir);
	void HandleSpawnPosition();
	
	UFUNCTION(BlueprintCallable)
	void KnockBack(FVector Direction, float Power);
	
	UPROPERTY(BlueprintReadOnly)
	bool RichardsDashBool = false;
	
	bool Dashing = false;
	
	UPROPERTY(BlueprintReadWrite)
	bool bIsAllowedToMove = true;
	UPROPERTY(BlueprintReadWrite)
	bool bDashLegal = true;
	
	bool bIsFireSlashing = false;
	bool KnockedBack = false;
	bool PlacingBlock = false;
	FVector KnockBackDir;
	FVector StaggerDir;
	float KnockBackPower;

protected:
	virtual void BeginPlay() override;
	virtual void SetupPlayerInputComponent(UInputComponent* InputComponent) override;

	USpringArmComponent* Springarm;
	TArray<AActor*> IgnoreList;
	UMovementData* MovementData;

	bool ShieldActive = true;
	
	UPROPERTY(BlueprintReadOnly)
	bool isStaggered;

	UPROPERTY(BlueprintReadOnly)
	bool bIsCastingPrimary = false;
	UPROPERTY(BlueprintReadOnly)
	bool bIsCastingSecondary = false;
	UPROPERTY(EditAnywhere, Category = Combat)
	float FakeSecondaryCD = 0.4;
	float CachedFakeSecondaryCD;
	
	UPROPERTY(EditAnywhere, Category = Mesh)
	UMaterialInterface* NoviceMaterial;
	
	int ShieldPickUpCount = 0;
	float CachedDashAnimationGrace;
	float CachedDashDur;
	float CachedKBDur;
	float CachedStaggerDur;
	float Speed;
	float Gravity;
	float DeltaTimeCopy;
	UPROPERTY(BlueprintReadOnly)
	int CurrentMagic = -1;
	
	float ImmunityTimer = 0.0f;
	bool Immune = false;
	bool OrbActive = true;
	bool MagicActive = false;

	FVector InputVector = FVector::ZeroVector;
	FVector LookVector = FVector::ZeroVector;
	FRotator CurrentRot;
	UPROPERTY(BlueprintReadWrite)
	FVector Velocity;
	//Hjalmar
	UFUNCTION(BlueprintCallable)
	void OnDamageTaken(UAbilityStats* Stats);

	UFUNCTION(BlueprintImplementableEvent)
	void DamageTaken(UAbilityStats* Stats);

	UFUNCTION(BlueprintCallable)
	void TakeDamageFromBP(UAbilityStats* Stats);
	void CheckImmunity(float DeltaTime);

	UFUNCTION()
	void OnCheckpointReset(int ResetIndex);

	UFUNCTION(BlueprintImplementableEvent)
	void OnReset();

	UFUNCTION(BlueprintImplementableEvent)
	void OnMagicUpdate();

	//Animation stuff
	UFUNCTION(BlueprintImplementableEvent)
	void BlendWalkAnimation(float angle);
	UFUNCTION(BlueprintImplementableEvent)
	void PrimaryAttack();
	UFUNCTION(BlueprintImplementableEvent)
	void SecondaryAttack();
	//-------
	UFUNCTION(BlueprintImplementableEvent)
    void OnDash(FVector Pos, FRotator Rot);
	void PrimaryFire();
	void SecondaryFire();
	void MoveForward(float Val);
	void MoveRight(float Val);
	void Dash();
	void CheckDash();
	void CheckKnockback();
	void CheckStagger();
	void CheckFakeSecondaryCD();
	void UpdateMousePosition();
	void SawpMagic();
	float GetDeltaTime();
	void EquipRobe();
	void CancelPlacingIceblock();
};
