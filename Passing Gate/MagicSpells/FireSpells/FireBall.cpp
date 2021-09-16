//Author Hjalmar Andersson

#include "FireBall.h"


#include "DrawDebugHelpers.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetMathLibrary.h"
#include "MagicSpells/Projectile/ForwardProjectile.h"

AFireBall::AFireBall()
{

}

void AFireBall::Activate(FTransform SpawnPos) 
{
	Super::Activate(SpawnPos);
	GetActorTransform();
	if(CoolDownTimer >= CoolDown)
	{
		if(FireProjectile != NULL)
		{
			AForwardProjectile* NewProjectile = GetWorld()->SpawnActor<AForwardProjectile>(FireProjectile, SpawnPos);
			//NewProjectile->SetActorRotation(UKismetMathLibrary::FindLookAtRotation(Player->GetActorLocation(), Player->GetSpawnPos()->GetComponentLocation()));
			CoolDownTimer = 0.0f;	
		}
	}
}

void AFireBall::BeginPlay()
{
	Super::BeginPlay();
	Player = Cast<AWizard>(UGameplayStatics::GetActorOfClass(this, AWizard::StaticClass()));
}

void AFireBall::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}