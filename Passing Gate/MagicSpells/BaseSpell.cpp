//Primary author: Hjalmar Andersson

#include "BaseSpell.h"
#include "AbilityStats.h"
#include "Projectile/ForwardProjectile.h"

ABaseSpell::ABaseSpell() 
{
	PrimaryActorTick.bCanEverTick = true;
}

void ABaseSpell::BeginPlay()
{
	Super::BeginPlay();
	CoolDownTimer = CoolDown;
}

void ABaseSpell::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if (CoolDownTimer < CoolDown)
	{
		CoolDownTimer += DeltaTime;
		//UE_LOG(LogTemp, Log, TEXT("Counting up cd"));		
	}

}

void ABaseSpell::Activate(FTransform SpawnTrans)
{
	UE_LOG(LogTemp, Log, TEXT("BaseSpell Activate"));
}