//Author: Hjalmar Andersson

#include "VoidTeleport.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/Character.h"
#include "TeleportableComponent.h"

#include "DrawDebugHelpers.h"
#include "Components/CapsuleComponent.h"


AVoidTeleport::AVoidTeleport()
{
	PrimaryActorTick.bCanEverTick = true;
}

void AVoidTeleport::BeginPlay()
{
	Super::BeginPlay();
	CoolDownTimer += 0.1f;
	PlayerRef = GetWorld()->GetFirstPlayerController()->GetPawn();
}

void AVoidTeleport::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if(TeleportDelayed)
	{
		DelayTimer += DeltaTime;
		Teleport(PlayerRef->GetActorLocation() + FVector::UpVector * -UpOffset);
		if(DelayTimer >= TeleportDelay)
		{
			Teleport(ToPos);
			OnTeleportTo(ToPos);
			DelayTimer = 0.0f;
			TeleportDelayed = false;
		}
	}
}

void AVoidTeleport::Activate(FTransform SpawnPoint)
{
	if (CoolDownTimer <= CoolDown)
	{
		return;
	}
	GetWorld()->GetFirstPlayerController()->GetHitResultUnderCursorByChannel(UEngineTypes::ConvertToTraceType(ECC_Visibility), true, Hit);
	if (Hit.bBlockingHit)
	{
		UTeleportableComponent* TeleportPlat = Hit.GetActor()->FindComponentByClass<UTeleportableComponent>();
		float Distance = FVector::Dist(Hit.Location, PlayerRef->GetActorLocation());
		if (TeleportPlat == nullptr || Distance > MaxDistance)
		{
			RescaleTeleport();
		}
		if (Distance < MaxDistance)
		{
			ActivateTeleport(Hit.ImpactPoint);
		}
	}
}

void AVoidTeleport::RescaleTeleport()
{
	FVector Direction = Hit.Location - PlayerRef->GetActorLocation();
	Direction.Z = 0;
	Direction = Direction.GetSafeNormal();
	FVector MaxPoint = PlayerRef->GetActorLocation() + Direction * MaxDistance;

	PerformCapsuleCast(MaxPoint + FVector::UpVector * 1000, MaxPoint + FVector::UpVector * -1000);
	UE_LOG(LogTemp, Log, TEXT("Rescaling the teleport"));
	
	if (Hit.GetActor() != nullptr && Hit.GetActor()->FindComponentByClass<UTeleportableComponent>() != nullptr)
	{
		ActivateTeleport(Hit.Location);
		return;
	}

	//perform a teleport to the correct location but get stuck
	PerformCapsuleCast(PlayerRef->GetActorLocation() + FVector::UpVector * 200, MaxPoint, FColor::Cyan);
	if (Hit.bBlockingHit)
	{
		DrawDebugLine(GetWorld(), PlayerRef->GetActorLocation() + FVector::UpVector * 200, Hit.Location, FColor::Cyan, false, 2.0f, 0, 5.0f);
		PerformCapsuleCast(Hit.Location, Hit.Location * FVector::UpVector * -500, FColor::Green);
		if (Hit.GetActor() != nullptr && Hit.GetActor()->FindComponentByClass<UTeleportableComponent>() != nullptr)
		{
			UE_LOG(LogTemp, Log, TEXT("hit on Sweep"));
			ActivateTeleport(Hit.Location);
			return;
		}
	}
}


void AVoidTeleport::PerformCapsuleCast(FVector From, FVector To, FColor Color)
{
	FCollisionShape Capsule = FCollisionShape::MakeCapsule(40, 60);
	GetWorld()->SweepSingleByChannel(Hit, From, To, FQuat::Identity, ECC_WorldStatic, Capsule);
	if(DrawDebugLines)
	{
		DrawDebugLine(GetWorld(), From, To, Color, false, 2.0f, 0, 5.0f);
		DrawDebugSphere(GetWorld(), Hit.Location, 15.0f, 12, Color, false, 2.0f, 0, 5.0f);
	}
}


void AVoidTeleport::ActivateTeleport(FVector To)
{
	CoolDownTimer = -TeleportDelay;
	TeleportDelayed = true;
	ToPos = To;
	OnTeleportFrom(PlayerRef->GetActorTransform(), ToPos);
}

void AVoidTeleport::Teleport(FVector TeleportPoint)
{
	FVector WizardPos = TeleportPoint + FVector::UpVector * UpOffset;
	PlayerRef->SetActorLocation(WizardPos);
}