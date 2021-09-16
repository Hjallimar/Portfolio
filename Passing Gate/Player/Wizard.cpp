//Author Olof Wallentin - Hazelight (Movement based on his code)
//This class was taken from one of his courses and additions to the original code will be documented
//Class, function and variables may have their names changed without documentation
//Author - Johan Liljedahl
//Author - Hjalmar Andersson

#include "Wizard.h"

#include "DrawDebugHelpers.h"
#include "Camera/CameraComponent.h"
#include "Components/CapsuleComponent.h"
#include "Components/InputComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Components/SkeletalMeshComponent.h"
#include "CustomComponents/Movement/PlayerMovementComponent.h"
#include "CustomComponents/Movement/MovementStatics.h"
#include "GameFramework/SpringArmComponent.h"
#include "Kismet/KismetSystemLibrary.h"

//Hjalmar
#include "Player/CustomComponents/HealthComponent.h"
#include "Player/CustomComponents/ResetableComponent.h"
#include "MagicSpells/AbilityStats.h"
#include "GP4PlatformerGameModeBase.h"
#include "Kismet/KismetMathLibrary.h"
#include "MagicSpells/IceSpells/IceBlock.h"
/// -------


AWizard::AWizard()
{
	PrimaryActorTick.bStartWithTickEnabled = true;

	Capsule = CreateDefaultSubobject<UCapsuleComponent>(TEXT("Capsule"));
	RootComponent = Capsule;
	Capsule->SetCollisionProfileName("BlockAllDynamic");

	Mesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("CharacterMesh"));
	Mesh->SetupAttachment(Capsule);
	Mesh->bCastDynamicShadow = true;
	Mesh->CastShadow = true;
	
	FirePos = CreateDefaultSubobject<USceneComponent>(TEXT("Fire Position"));
	FirePos->SetupAttachment(RootComponent);
	
	SpawnPosition = CreateDefaultSubobject<USceneComponent>(TEXT("SPAWN POSITION"));
	
	MovementComponent = CreateDefaultSubobject<UPlayerMovementComponent>(TEXT("Movement Component"));
	MovementComponent->SetUpdatedComponent(Capsule);
	
	//Hjalmar Andersson
	MagicComponents.Add(CreateDefaultSubobject<UMagicComponent>(TEXT("Fire Magic Component")));
	MagicComponents.Add(CreateDefaultSubobject<UMagicComponent>(TEXT("Ice Magic Component")));
	MagicComponents.Add(CreateDefaultSubobject<UMagicComponent>(TEXT("Void Magic Component")));

	HealthComponent = CreateDefaultSubobject<UHealthComponent>(TEXT("Health Component"));
	HealthComponent->OnTakeDamage.AddDynamic(this, &AWizard::OnDamageTaken);
	//----
}

void AWizard::BeginPlay()
{
	Super::BeginPlay();
	MovementData = MovementComponent->GetMovementData();
	if(MovementData != nullptr)
	{
		Speed = MovementData->MoveSpeed;
		Gravity = MovementData->Gravity;	
	}
	else
	{
		UE_LOG(LogTemp, Log, TEXT("No Movement Data-Asset Found"))
	}

	CachedDashAnimationGrace = DashAnimationGrace;
	CachedFakeSecondaryCD = FakeSecondaryCD;
	CachedDashDur = DashDuration;
	CachedKBDur = KnockbackDuration;
	CachedStaggerDur = StaggerDur;
	
	Camera = Cast<UCameraComponent>(GetComponentByClass(UCameraComponent::StaticClass()));
	Springarm = Cast<USpringArmComponent>(GetComponentByClass(USpringArmComponent::StaticClass()));

	Orb = GetWorld()->SpawnActor<AActor>(MouseOrb);
	if(!OrbActive)
	{
		Orb->SetActorHiddenInGame(true);
	}
	
	ResetComponent = FindComponentByClass<UResetableComponent>();
	if(ResetComponent != nullptr)
		ResetComponent->TimeToReset.AddDynamic(this, &AWizard::OnCheckpointReset);

	//Hjalmar
	if(MagicActive)
		EquipRobe();
	ImmunityTimer = ImmuneTime;
}

void AWizard::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	CheckDash();
	CheckKnockback();
	CheckStagger();
	CheckFakeSecondaryCD();
	
	FMovementStatics FrameMovement = MovementComponent->CreateFrameMovement();
	FrameMovement.AddGravity(Gravity * DeltaTime);

	if (!InputVector.IsNearlyZero())
	{
		FVector Forward = FVector::ForwardVector - FVector::RightVector;
		FVector ForwardMovement = Forward * InputVector.X;
		FVector Right = (FVector::ForwardVector + FVector::RightVector) * InputVector.Y;
		Velocity = (ForwardMovement + Right).GetSafeNormal() * Speed * DeltaTime;
		if(isStaggered)
		{
			InputVector = FVector::ZeroVector;
		}
		else
		{
			if(Dashing && !KnockedBack) // Player is now dashing
				{
					Gravity = 0;
					FrameMovement.AddDelta(Velocity * DashPower);	
				}
			else // Player move speed is now normal
				{
					Gravity = MovementData->Gravity;
					FrameMovement.AddDelta(Velocity);
				}
		}
		if(!KnockedBack && bIsAllowedToMove) //Move player unless stunned
			{
				MovementComponent->Move(FrameMovement);	
			}	
		}
	if(bIsAllowedToMove)
	{
		UpdateMousePosition();		
	}


	FakeSecondaryCD -= DeltaTime;
	DashAnimationGrace -= DeltaTime;
	KnockbackDuration -= DeltaTime;
	StaggerDur -= DeltaTime;
	DashDuration -= DeltaTime;
	DeltaTimeCopy = DeltaTime;

	//Hjalmar Fuckery
	FVector Direction = InputVector.GetSafeNormal();
	if (Direction.Size() > 0)
	{
		FVector FakeForward = GetActorForwardVector();
		FakeForward *= FVector::ForwardVector;
		float angleDiff = FMath::RadiansToDegrees(FMath::Acos(FVector::DotProduct(FakeForward.GetSafeNormal(), Direction)));
		BlendWalkAnimation(angleDiff);
	}
	else
		BlendWalkAnimation(-100);
	if (Immune)
		CheckImmunity(DeltaTime);
	//----
}

//Hjalmar
void AWizard::SawpMagic()
{
	if (!bIsAllowedToMove)
		return;
	CurrentMagic++;
	if(CurrentMagic > MagicComponents.Num() -1)
	{
		CurrentMagic = -1;
	}
	UpdateMagic(CurrentMagic);
}
void AWizard::UpdateMagic(int i)
{
	CurrentMagic = i;
	if(CurrentMagic < 0)
	{
		MagicActive = false;
		if(NoviceMaterial != nullptr)
		{
			Mesh->SetMaterial(0, NoviceMaterial);
			Mesh->SetMaterial(1, NoviceMaterial);
			Mesh->SetMaterial(2, NoviceMaterial);
		}
		return;
	}
	else if (CurrentMagic > MagicComponents.Num() - 1)
		CurrentMagic = MagicComponents.Num() - 1;
	EquipRobe();
	OnMagicUpdate();
	CancelPlacingIceblock();
}
void AWizard::OnDamageTaken(UAbilityStats* Stats)
{
	if(Immune || Stats->bPlayerProjectile)
	{
		return;
	}

	if(ShieldActive)
	{
		ShieldActive = false;
		OnShieldBreak();
	}
	else
	{
		HealthComponent->Health -= Stats->GetDamage();
		if (HealthComponent->Health <= 0)
		{
			AGP4PlatformerGameModeBase* GameModeBase = Cast<AGP4PlatformerGameModeBase>(GetWorld()->GetAuthGameMode());
			GameModeBase->CheckPointReset(0);
		}
	}
	DamageTaken(Stats);
	Immune = true;
}
void AWizard::ShieldPickUp()
{
	if (!ShieldActive)
	{
		ShieldPickUpCount++;
		OnShieldActivate(ShieldPickUpCount);
		if (ShieldPickUpCount >= 4)
		{
			ShieldPickUpCount = 0;
			ShieldActive = true;
		}
	}
}
void AWizard::TakeDamageFromBP(UAbilityStats* Stats)
{
	OnDamageTaken(Stats);
}
void AWizard::CheckImmunity(float DeltaTime)
{
	ImmunityTimer -= DeltaTime;
	if (ImmunityTimer < 0)
	{
		Immune = false;
		ImmunityTimer = ImmuneTime;
	}
}
void AWizard::OnCheckpointReset(int i)
{
	HealthComponent->Health = HealthComponent->MaxHealth;
	ShieldActive = true;
	OnShieldActivate(4);
	ShieldPickUpCount = 0;
	CancelPlacingIceblock();
	OnReset();
}
void AWizard::EquipRobe()
{
	MagicActive = true;
	if (MagicComponents[CurrentMagic]->ElemantalMaterials.Num() > 0)
	{
		UE_LOG(LogTemp, Log, TEXT("Changeing material"));
		Mesh->SetMaterial(0, MagicComponents[CurrentMagic]->ElemantalMaterials[0]);
		Mesh->SetMaterial(1, MagicComponents[CurrentMagic]->ElemantalMaterials[1]);
		Mesh->SetMaterial(2, MagicComponents[CurrentMagic]->ElemantalMaterials[2]);
	}
}
void AWizard::RemovePlayerShield()
{
	ShieldPickUpCount = 0;
	OnShieldActivate(ShieldPickUpCount);
	ShieldActive = false;
}
//-------
void AWizard::SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent)
{
	check(PlayerInputComponent);
	PlayerInputComponent->BindAxis("MoveForward", this, &AWizard::MoveForward);
	PlayerInputComponent->BindAxis("MoveRight", this, &AWizard::MoveRight);
	PlayerInputComponent->BindAction("PrimaryFire",IE_Pressed, this, &AWizard::PrimaryFire);
	PlayerInputComponent->BindAction("SecondaryFire",IE_Pressed, this, &AWizard::SecondaryFire);
	PlayerInputComponent->BindAction("Dash", IE_Pressed, this, &AWizard::Dash);
	//HARDCODED PLEASE FIX ASP - Hjalmar
	PlayerInputComponent->BindAction("Swap", IE_Pressed, this, &AWizard::SawpMagic);
}
float AWizard::GetDeltaTime()
{
	return DeltaTimeCopy;
}
void AWizard::PrimaryFire()
{
	if (!bIsAllowedToMove)
		return;
	CancelPlacingIceblock();
	if(MagicActive && MagicComponents[CurrentMagic] != nullptr)
	{
		SetStaggerDir(Orb->GetActorLocation() - GetActorLocation());
		Stagger();
		MagicComponents[CurrentMagic]->PrimaryFire(FirePos->GetComponentTransform());
		PrimaryAttack();
	}
}
void AWizard::SecondaryFire()
{
	if (!bIsAllowedToMove)
		return;
	if (MagicActive && MagicComponents[CurrentMagic] != nullptr)
	{
		MagicComponents[CurrentMagic]->SecondaryFire(FirePos->GetComponentTransform());
		SecondaryAttack();
		if(FakeSecondaryCD + MagicComponents[CurrentMagic]->SecondSpell->CoolDown < 0)
		{
			FakeSecondaryCD = CachedFakeSecondaryCD;
		}
	}
}
void AWizard::MoveForward(float Value)
{
	if (!bIsAllowedToMove)
		return;
	InputVector.X = Value;		
}
void AWizard::MoveRight(float Value)
{
	if (!bIsAllowedToMove)
		return;
	InputVector.Y = Value;
}
void AWizard::UpdateMousePosition()
{
	FVector2D mousePosition = FVector2D(0, 0);
	GetWorld()->GetFirstPlayerController()->GetMousePosition(mousePosition.X, mousePosition.Y);
	FVector CameraLocation, targetDirection;
	GetWorld()->GetFirstPlayerController()->DeprojectScreenPositionToWorld(mousePosition.X, mousePosition.Y, CameraLocation, targetDirection);
	targetDirection *= Springarm->TargetArmLength;
	FVector  TargetPos = targetDirection += CameraLocation;
	FVector Intersection = FMath::LinePlaneIntersection(CameraLocation, TargetPos, GetActorLocation(), FVector(0, 0, 1));
	
	Orb->SetActorRelativeLocation(Intersection); // Set Orb Location
	
	CurrentRot = GetActorRotation();
	if(!isStaggered)
	{
		SetActorRotation(FMath::Lerp(CurrentRot, Velocity.ToOrientationRotator(),TurnRate)); // Rotate Player	
	}
	HandleSpawnPosition();
}
void AWizard::CheckDash()
{
	if(DashDuration > 0)
	{
		Dashing = true;
		RichardsDashBool = true;
	}
	else
	{
		Dashing = false;
		if(DashDuration + DashAnimationGrace < 0)
		{
			RichardsDashBool = false;
		}
	}
}
void AWizard::Dash()
{
	if (!bIsAllowedToMove)
		return;
	if(DashDuration + DashCooldown < 0 && !isStaggered && bIsAllowedToMove && (InputVector.X != 0 || InputVector.Y != 0) && bDashLegal)
	{
		OnDash(GetActorLocation(), GetActorRotation());
		DashDuration = CachedDashDur;
		if(DashAnimationGrace < 0)
		{
			DashAnimationGrace = CachedDashAnimationGrace;
		}
	}
}
void AWizard::KnockBack(FVector Direction, float Power)
{
	if(KnockbackDuration < 0)
	{
		KnockedBack = true;
		KnockbackDuration = CachedKBDur;
	}
	KnockBackDir = Direction;
	KnockBackPower = Power;
}
void AWizard::CheckKnockback()
{
	if(KnockbackDuration < 0)
	{
		KnockedBack = false;
	}
}
void AWizard::Stagger()
{
	StaggerDur = CachedStaggerDur;
	SetActorRotation(UKismetMathLibrary::FindLookAtRotation(GetActorLocation(), Orb->GetActorLocation()));
}
void AWizard::CheckStagger()
{
	if(StaggerDur > 0)
	{
		if(!bIsFireSlashing)
		{
			bIsCastingPrimary = true;
		}
		isStaggered = true;
		SetActorRotation(StaggerDir.ToOrientationQuat());
	}
	else
	{
		bIsFireSlashing = false;
		bIsCastingPrimary = false;
		isStaggered = false;
	}
}
void AWizard::CheckFakeSecondaryCD()
{
	if(FakeSecondaryCD > 0)
	{
		bIsCastingSecondary = true;
	}
	else
	{
		bIsCastingSecondary = false;
	}
}
void AWizard::SetStaggerDir(FVector Dir)
{
	StaggerDir = Dir;
}
void AWizard::MakeOrbVisible(bool Status)
{
	OrbActive = Status;
	if(Orb != nullptr)
		Orb->SetActorHiddenInGame(!Status);
}
void AWizard::CancelPlacingIceblock()
{
	AIceBlock* IceBlock = Cast<AIceBlock>(UGameplayStatics::GetActorOfClass(this,AIceBlock::StaticClass()));
	if(IceBlock != nullptr && IceBlock->CurrentBlock != nullptr)
	{
		IceBlock->CancelSpell();
	}
}
void AWizard::HandleSpawnPosition()
{
	float DistanceToSpawnTransform = FVector::Distance(GetActorLocation(), Orb->GetActorLocation());
	FVector DirToOrb = (Orb->GetActorLocation() - GetActorLocation()).GetSafeNormal();
	SpawnPosition->SetWorldRotation(DirToOrb.ToOrientationQuat());
	
	if(DistanceToSpawnTransform < SpawnPosMinDistance) 
	{
		SpawnPosition->SetWorldLocation(GetActorLocation() + DirToOrb * SpawnPosMinDistance); //SpawnPos is to close!
	}
	else if(DistanceToSpawnTransform > SpawnPosMaxDistance)
	{
		SpawnPosition->SetWorldLocation(GetActorLocation() + DirToOrb * SpawnPosMaxDistance); //SpawnPos is to far away!
	}
	else
	{
		SpawnPosition->SetWorldLocation(Orb->GetActorLocation()); // SpawnPos is within allowed area
	}
}

UCameraComponent* AWizard::GetCamera()
{
	return Camera;
}
USceneComponent* AWizard::GetSpawnPos()
{
	return SpawnPosition;
}
AActor* AWizard::GetOrb()
{
	return Orb;
}