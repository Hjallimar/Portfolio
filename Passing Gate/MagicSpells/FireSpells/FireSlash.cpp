//Author Hjalmar Andersson
//Secondary: Johan Liljedahl
#include "FireSlash.h"
#include "MagicSpells/Projectile/ForwardProjectile.h"
#include "DrawDebugHelpers.h"
#include "Kismet/GameplayStatics.h"
#include "Player/CustomComponents/HealthComponent.h"
#include "Components/SkeletalMeshComponent.h"
#include "MagicSpells/AbilityStats.h"
#include "GameFramework/Pawn.h"
#include "Kismet/KismetMathLibrary.h"

AFireSlash::AFireSlash()
{

}

void AFireSlash::BeginPlay()
{
	Super::BeginPlay();
	Player = Cast<AWizard>(UGameplayStatics::GetActorOfClass(this, AWizard::StaticClass()));
}

void AFireSlash::Activate(FTransform SpawnPos)
{
	Super::Activate(SpawnPos);
	if(CoolDownTimer >= CoolDown)
	{	
		CoolDownTimer = 0;		
		OnSlash(Player->GetActorLocation() + Player->GetActorForwardVector(), Player->GetActorRotation()); // hardcoded 150 :D
		TArray<FHitResult> Hits;
		FVector CenterPos = Player->GetActorLocation() + Player->GetActorForwardVector() * Radius;
		FCollisionShape MyShpere = FCollisionShape::MakeSphere(Radius);
		bool isHit = GetWorld()->SweepMultiByChannel(Hits, Player->GetActorLocation(), Player->GetActorLocation(), FQuat::Identity, ECC_WorldStatic, MyShpere);
		if (!isHit)
			return;
		TArray<AActor*> ActorHits;
		for (FHitResult Hit : Hits)
		{
			if (Hit.GetActor() != Player && Hit.GetActor() != nullptr)
			{
				if (ActorHits.Num() <= 0 || !ActorHits.Contains(Hit.GetActor()))
				{
					ActorHits.Add(Hit.GetActor());
				}
			}
		}

		for (AActor* Actor : ActorHits)
		{
			UHealthComponent* HealthComp = Actor->FindComponentByClass<UHealthComponent>();
			if (HealthComp != nullptr)
			{
				FVector Loc = Player->GetActorLocation() - Actor->GetActorLocation();
				float angle = FMath::RadiansToDegrees(FVector::DotProduct(Player->GetActorForwardVector(), Loc.GetSafeNormal()));
				if (angle < 0)
				{
					HealthComp->OnTakeDamage.Broadcast(SlashStats);
				}
			}
		}
	}
}

void AFireSlash::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if(DrawDebugLines)
	{
		DrawDebugLine(GetWorld(), Player->GetActorLocation() + Player->GetActorRightVector() * Radius, Player->GetActorLocation() + Player->GetActorRightVector() * -Radius, FColor::Blue, false, 0.0f, 0, 5.0f);
		DrawDebugSphere(GetWorld(), Player->GetActorLocation(), Radius, 8, FColor::Red, false, 0.0f, 0, 5.0f);
	}
}