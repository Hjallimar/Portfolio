//Hjalmar Andersson

#include "ReaperBoss.h"
#include "Components/CapsuleComponent.h"
#include "Components/SkeletalMeshComponent.h"
#include "Player/CustomComponents/HealthComponent.h"
#include "Player/CustomComponents/ResetableComponent.h"
#include "MagicSpells/Projectile/ForwardProjectile.h"

#include "Engine/World.h"
#include "NavigationSystem.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "DrawDebugHelpers.h"

AReaperBoss::AReaperBoss()
{
	PrimaryActorTick.bCanEverTick = true;

	Capsule = GetCapsuleComponent();
	if(Capsule == nullptr)
	{
		Capsule = CreateDefaultSubobject<UCapsuleComponent>(TEXT("Capsule Collider"));
		Capsule->SetupAttachment(RootComponent);
	}
	Capsule->SetCollisionProfileName("OverlappAll");
	Capsule->SetGenerateOverlapEvents(true);

	BossMesh = GetMesh();
	if(BossMesh == nullptr)
	{
		BossMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("Mesh"));
		BossMesh->SetupAttachment(RootComponent);
	}
	BossMesh->SetGenerateOverlapEvents(false);
	BossMesh->SetCollisionProfileName("BlockAll");

	FireRotator = CreateDefaultSubobject<USceneComponent>(TEXT("Fire Rotator"));
	FireRotator->SetupAttachment(BossMesh);
	FirePoint = CreateDefaultSubobject<USceneComponent>(TEXT("Fire Point"));
	FirePoint->SetupAttachment(FireRotator);

	HealthComponent = CreateDefaultSubobject<UHealthComponent>("Health Component");
	HealthComponent->OnTakeDamage.AddDynamic(this, &AReaperBoss::TakeDamage);
	ResetComponent = CreateDefaultSubobject<UResetableComponent>("Reset Component");
	ResetComponent->TimeToReset.AddDynamic(this, &AReaperBoss::Reseting);
}

void AReaperBoss::BeginPlay()
{
	Super::BeginPlay();
	CurrentElement = 1;
	PlayerRef = GetWorld()->GetFirstPlayerController()->GetPawn();
	SpawnTrans = GetActorTransform();

	AIController = Cast<AAIController>(GetController());
	Cast<UCharacterMovementComponent>(GetMovementComponent())->MaxWalkSpeed = MoveSpeed;
	HealthComponent->Health = HealthComponent->MaxHealth;
	UE_LOG(LogTemp, Log, TEXT("Max health is: %f"), HealthComponent->MaxHealth);
	UE_LOG(LogTemp, Log, TEXT("Current health is: %f"), HealthComponent->Health);
}

void AReaperBoss::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if (!Active)
		return;
	switch(Phase)
	{
	case 1:
		PhaseOne(DeltaTime);
		break;
	case 2:
		PhaseTwo(DeltaTime);
		break;
	case 3:
		PhaseThree(DeltaTime);
		break;
	case 4:
		break;
	}
}

//--------Phase one--------
void AReaperBoss::PhaseOne(float DeltaTime)
{
	if(Slashing)
	{
		MoveSlash(DeltaTime);
		SlashForward(DeltaTime);
	}
	else
	{
		MoveBoss(DeltaTime);
	}
}

void AReaperBoss::MoveBoss(float DeltaTime)
{
	MoveTimer += DeltaTime;
	if(PlayerRef == nullptr)
	{
		PlayerRef = GetWorld()->GetFirstPlayerController()->GetPawn();
		return;
	}

	AIController->MoveToActor(PlayerRef);
	if(MoveTimer >= MoveDuration)
	{
		TimeSinceLastSlash = 0.0f;
		MoveTimer = 0.0f;
		Slashing = true;
		Cast<UCharacterMovementComponent>(GetMovementComponent())->MaxWalkSpeed = MoveSlashSpeed;
	}
}

void AReaperBoss::MoveSlash(float DeltaTime)
{
	MoveTimer += DeltaTime;
	if (PlayerRef == nullptr)
	{
		PlayerRef = GetWorld()->GetFirstPlayerController()->GetPawn();
		return;
	}
	AIController->MoveToActor(PlayerRef);
	if (MoveTimer >= SlashDuration)
	{
		OnRun();
		MoveTimer = 0.0f;
		Slashing = false;
		Cast<UCharacterMovementComponent>(GetMovementComponent())->MaxWalkSpeed = MoveSpeed;
	}
}

void AReaperBoss::SlashForward(float DeltaTime)
{
	TimeSinceLastSlash += DeltaTime;
	if (TimeSinceLastSlash >= SlashCD)
	{
		OnSlash();
		TimeSinceLastSlash = 0.0f;
		FCollisionShape MyShpere = FCollisionShape::MakeSphere(SlashRadius);
		TArray<FHitResult> Hits;
		if(DrawDebug)
		{
			DrawDebugSphere(GetWorld(), GetActorLocation(), SlashRadius, 64, FColor::Red, false, 1.0f, 0, 1.0f);
		}
		bool isHit = GetWorld()->SweepMultiByChannel(Hits, GetActorLocation(), GetActorLocation(), FQuat::Identity, ECC_WorldStatic, MyShpere);
		if (isHit)
		{
			for (FHitResult Hit : Hits)
			{
				if(Hit.GetActor() != nullptr) 
				{
					if(Hit.GetActor() == PlayerRef)
					{
						UHealthComponent* Health = Hit.GetActor()->FindComponentByClass<UHealthComponent>();
						Health->OnTakeDamage.Broadcast(SlashStats);
					}
				}
			}
		}
	}
}
//--------Phase two--------
void AReaperBoss::PhaseTwo(float DeltaTime)
{
	if(!DelayDone)
	{
		DelayTimer += DeltaTime;
		if (DelayTimer >= PhaseDelay)
			Teleport();
	}
	else
	{
		AddActorLocalRotation(FRotator(0, PhaseTwoRotation * DeltaTime, 0));
		SpawnProjectile(DeltaTime);
	}
}

void AReaperBoss::Teleport()
{
	Immune = false;
	DelayDone = true;
	SetActorLocation(SpawnTrans.GetLocation());
	ActivatePhase(Phase);
}

void AReaperBoss::SpawnProjectile(float DeltaTime)
{
	WaveTimer += DeltaTime;
	if (WaveTimer >= WaveInterwall)
	{
		FRotator OriginRot = GetActorRotation();
		int rand = (int)FMath::RandRange(MinAmmountProjectile, MaxAmmountProjectile);
		for (int i = 0; i < rand; i++)
		{
			AddActorLocalRotation(FRotator(0, (360.0f/rand), 0));
			FVector Loc = FirePoint->GetComponentLocation();

			if (ProjectilePool.Num() > 0)
			{
				ProjectilePool[0]->SetActorLocation(Loc);
				ProjectilePool[0]->SetActorRotation(GetActorRotation());
				ProjectilePool[0]->SpawnFromPool();
				ActiveProjectiles.Add(ProjectilePool[0]);
				ProjectilePool.RemoveAt(0);
			}
			else
			{
				SpawnBP_Projectile(Loc, GetActorRotation());
			}
		}
		WaveTimer = 0.0f;
		SetActorRotation(OriginRot);
	}
}
//--------Phase Three-------
void AReaperBoss::PhaseThree(float DeltaTime)
{
	if (!DelayDone)
	{
		PhaseThreeWaitTime(DeltaTime);
	}
	else
	{
		AddActorLocalRotation(FRotator(0, RotationSpeed * DeltaTime, 0));
		Beams();
		//SpawnProjectile(DeltaTime/2);
	}
}

void AReaperBoss::PhaseThreeWaitTime(float DeltaTime)
{
	DelayTimer += DeltaTime;
	float PrepDistance = BeamDistance * (DelayTimer / PhaseDelay);
	FVector BaseLocation = GetActorLocation();
	BaseLocation.Z = FirePoint->GetComponentLocation().Z;
	FVector Forward = BaseLocation + GetActorForwardVector() * PrepDistance;
	FVector Backwards = BaseLocation + GetActorForwardVector() * -PrepDistance;
	FVector Right = BaseLocation + GetActorRightVector() * PrepDistance;
	FVector Left = BaseLocation + GetActorRightVector() * -PrepDistance;
	if(DrawDebug)
	{
		DrawDebugLine(GetWorld(), Forward, Backwards, FColor::Red, false, 0.0f, 0, 5.0f);
		DrawDebugLine(GetWorld(), Right, Left, FColor::Red, false, 0.0f, 0, 5.0f);
	}
	if (DelayTimer >= PhaseDelay)
	{
		DelayDone = true;
		Immune = false;
		ActivatePhase(Phase);
	}
}

void AReaperBoss::Beams()
{
	FVector BaseLocation = GetActorLocation();
	BaseLocation.Z = FirePoint->GetComponentLocation().Z;
	FVector Forward = BaseLocation + GetActorForwardVector() * BeamDistance;
	FVector Backwards = BaseLocation + GetActorForwardVector() * -BeamDistance;
	FVector Right = BaseLocation + GetActorRightVector() * BeamDistance;
	FVector Left = BaseLocation + GetActorRightVector() * -BeamDistance;
	TArray<FHitResult> Hit;
	GetWorld()->LineTraceMultiByChannel(Hit, Forward, BaseLocation, ECollisionChannel::ECC_Visibility);
	CheckHit(Hit);
	GetWorld()->LineTraceMultiByChannel(Hit, Backwards, BaseLocation, ECollisionChannel::ECC_Visibility);
	CheckHit(Hit);
	GetWorld()->LineTraceMultiByChannel(Hit, Right, BaseLocation, ECollisionChannel::ECC_Visibility);
	CheckHit(Hit);
	GetWorld()->LineTraceMultiByChannel(Hit, Left, BaseLocation, ECollisionChannel::ECC_Visibility);
	CheckHit(Hit);
	if(DrawDebug)
	{
		DrawDebugLine(GetWorld(), Forward, BaseLocation, FColor::Red, false, 0.0f, 0, 5.0f);
		DrawDebugLine(GetWorld(), Right, BaseLocation, FColor::Red, false, 0.0f, 0, 5.0f);
		DrawDebugLine(GetWorld(), Backwards, BaseLocation, FColor::Red, false, 0.0f, 0, 5.0f);
		DrawDebugLine(GetWorld(), Left, BaseLocation, FColor::Red, false, 0.0f, 0, 5.0f);
	}
}

void AReaperBoss::CheckHit(TArray<FHitResult> Hits)
{
	for ( FHitResult Hit : Hits )
	{
		if (Hit.bBlockingHit && Hit.GetActor() != nullptr)
		{
			if (Hit.GetActor() != PlayerRef)
				return;
			UHealthComponent* Health = Hit.GetActor()->FindComponentByClass<UHealthComponent>();
			if (Health != nullptr)
			{
				Health->OnTakeDamage.Broadcast(SlashStats);
			}
		}
	}
}

//-----------------------

void AReaperBoss::ReduceMaxProjectiles(int i = 1)
{
	MaxAmmountProjectile -= i;
	if(MaxAmmountProjectile <= MinAmmountProjectile)
	{
		MaxAmmountProjectile = MinAmmountProjectile;
	}
}

void AReaperBoss::TakeDamage(UAbilityStats* Stats)
{
	if ((int)Stats->GetElement() != CurrentElement || Immune)
	{
		return;
	}

	if(PlatformThreshold < PlatformThresholds.Num())
	{
		if (HealthComponent->Health <= PlatformThresholds[PlatformThreshold])
		{
			OnPlatformThreshold(PlatformThreshold);
			PlatformThreshold++;
		}
	}

	HealthComponent->Health -= Stats->GetDamage();
	if(HealthComponent->Health <= PhaseThreeThreshold && Phase < 3)
	{
		ChangePhase();
	}
	else if(HealthComponent->Health <= PhaseTwoThreshold && Phase < 2)
	{
		ChangePhase();	
	}

	if(HealthComponent->Health <= 0)
	{
		Immune = true;
		OnDeath();
		Active = false;
		Capsule->SetGenerateOverlapEvents(false);
	}
}
void AReaperBoss::Reseting(int i)
{
	HealthComponent->Health = HealthComponent->MaxHealth;
	Phase = 1;
	PlatformThreshold = 0;
	Active = false;
	CurrentElement = 1;
	BossMesh->SetMaterial(0, ElementMaterials[0]);
	SetActorTransform(SpawnTrans);

	DelayDone = false;
	DelayTimer = 0.0f;
	Immune = false;
	//Pahse 1
	MoveTimer = 0.0f;
	Slashing = false;
	TimeSinceLastSlash = 0.0f;
	Cast<UCharacterMovementComponent>(GetMovementComponent())->MaxWalkSpeed = MoveSpeed;
	//Phase 2
	WaveTimer = 0.0f;
	int Num = ActiveProjectiles.Num();
	UE_LOG(LogTemp, Log, TEXT("ProjectilePool size before: %i"), ActiveProjectiles.Num());
	for(int i = 0; i < ActiveProjectiles.Num(); )
	{
		if(ActiveProjectiles[0] != nullptr)
			ActiveProjectiles[0]->DestroyActorInGame();
	}
	UE_LOG(LogTemp, Log, TEXT("ProjectilePool size after: %i"), ActiveProjectiles.Num());
	ActiveProjectiles.Empty();
	ActivatePhase(Phase);
	//Phase 3
}
void AReaperBoss::ActivateBoss()
{
	Active = true;
	Immune = false;
}
void AReaperBoss::AddProjectileToPool(AForwardProjectile* Projectile)
{
	ActiveProjectiles.Add(Projectile);
}
void AReaperBoss::ReturnProjectileToPool(AForwardProjectile* Projectile)
{
	if(ActiveProjectiles.Contains(Projectile))
	{
		ActiveProjectiles.Remove(Projectile);
		ProjectilePool.Add(Projectile);
	}
}
void AReaperBoss::ChangePhase()
{
	AIController->StopMovement();
	DelayDone = false;
	DelayTimer = 0.0f;
	Phase++;
	CurrentElement++;
	BossMesh->SetMaterial(0, ElementMaterials[CurrentElement -1]);
	StartIntermission();
	Immune = true;
}