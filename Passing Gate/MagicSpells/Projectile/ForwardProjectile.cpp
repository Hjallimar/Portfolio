//Author: Hjalmar Andersson

#include "ForwardProjectile.h"
#include "Components/SphereComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetSystemLibrary.h"
#include "MagicSpells/AbilityStats.h"
#include "MagicSpells/BaseSpell.h"
#include "Player/CustomComponents/HealthComponent.h"

AForwardProjectile::AForwardProjectile()
{
	PrimaryActorTick.bCanEverTick = true;
	Sphere = CreateDefaultSubobject<USphereComponent>(TEXT("Collison Sphere"));
	RootComponent = Sphere;
	Sphere->SetCollisionProfileName("BlockAllDynamic");

	Mesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Projectile Mesh"));
	Mesh->SetupAttachment(Sphere);
	Mesh->SetCollisionProfileName("NoCollision");
	Mesh->SetEnableGravity(false);
}

void AForwardProjectile::BeginPlay()
{
	Super::BeginPlay();
	OnActive();
	active = true;
}

void AForwardProjectile::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if(!active)
	{
		return;
	}
	LifeTime += DeltaTime;
	if (LifeTime > MaxLifeTime)
	{
		SetActiveStatus(false);
	}
	else
	{
		MoveProjectile(DeltaTime);
	}
}

void AForwardProjectile::AssignParent(AActor* NewParent)
{
	Parent = NewParent;
}

void AForwardProjectile::DestroyActorInGame()
{
	SetActiveStatus(false);
}

void AForwardProjectile::MoveProjectile(float DeltaTime)
{
	FHitResult Hit;
	FVector Delta = GetActorForwardVector() * TravelSpeed * DeltaTime;
	FCollisionShape SphereShape = FCollisionShape::MakeSphere(Sphere->GetScaledSphereRadius());
	Sphere->SetCollisionProfileName("NoCollision");
	GetWorld()->SweepSingleByChannel(Hit, GetActorLocation(), GetActorLocation() + Delta, FQuat::Identity, ECC_WorldStatic, SphereShape);
	Sphere->SetCollisionProfileName("BlockAllDynamic");
	AddActorWorldOffset(Delta, false);
	if(Hit.bBlockingHit && Hit.GetActor() != nullptr)
	{
		if(Parent != nullptr && Parent == Hit.GetActor())
		{
			return;
		}
		if(Hit.GetActor()->IsA(AForwardProjectile::StaticClass()))
		{
			return;
		}
		UHealthComponent* HealthComp = Cast<UHealthComponent>(Hit.GetActor()->GetComponentByClass(UHealthComponent::StaticClass()));
		if (HealthComp != nullptr)
		{
			HealthComp->OnTakeDamage.Broadcast(ProjectileStats);
		}
		SetActiveStatus(false);
	}
}

void AForwardProjectile::SetActiveStatus(bool status)
{
	OnHit();
	if (Destroyable)
	{
		Destroy();
	}
	else
	{
		SetActorHiddenInGame(!status);
		PrimaryActorTick.bCanEverTick = status;
		Sphere->SetCollisionProfileName(status ? "BlockAllDynamic" : "NoCollision");
		Sphere->SetGenerateOverlapEvents(status);
		active = status;
		LifeTime = 0.0f;
		if (!status)
			ReturnToPool();
	}
}

void AForwardProjectile::SpawnFromPool()
{
	SetActiveStatus(true);
}