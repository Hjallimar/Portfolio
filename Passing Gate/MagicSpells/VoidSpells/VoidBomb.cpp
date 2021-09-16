//Author: Hjalmar Andersson

#include "VoidBomb.h"

#include "Kismet/GameplayStatics.h"
#include "Player/CustomComponents/HealthComponent.h"
#include "MagicSpells/AbilityStats.h"
#include "Player/Wizard.h"

AVoidBomb::AVoidBomb()
{
	PrimaryActorTick.bCanEverTick = true;
	Player = Cast<AWizard>(UGameplayStatics::GetActorOfClass(this, AWizard::StaticClass()));
}

void AVoidBomb::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	Timer += DeltaTime;
	if (Timer >= Delay)
		Explode();
}

void AVoidBomb::Explode()
{
	OnExplode();
	FCollisionShape MyColShpere = FCollisionShape::MakeSphere(AreaOfEffect);
	TArray<FHitResult> Hits;
	TArray<AActor*> HitActors;
	bool isHit = GetWorld()->SweepMultiByChannel(Hits, GetActorLocation(), GetActorLocation(), FQuat::Identity, ECC_WorldStatic, MyColShpere);
	for (FHitResult Hit : Hits)
	{
		if (Hit.GetActor() != nullptr)
		{
			if(HitActors.Num() > 0)
			{
				if(!HitActors.Contains(Hit.GetActor()))
				{
					HitActors.Add(Hit.GetActor());
				}
			}
			else
			{
				HitActors.Add(Hit.GetActor());
			}
			
		}
	}
	DealDamage(HitActors);
	Destroy();
}

void AVoidBomb::DealDamage(TArray<AActor*> Objects)
{
	for(AActor* Actor : Objects)
	{
		UHealthComponent* HitComp = Actor->FindComponentByClass<UHealthComponent>();
		if (HitComp != nullptr && Actor != Player)
		{
			HitComp->OnTakeDamage.Broadcast(BombStats);
		}	
	}
}