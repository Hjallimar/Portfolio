//Author: Hjalmar Andersson
#include "VoidPulse.h"
#include "CoreMinimal.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetSystemLibrary.h"
#include "Player/CustomComponents/HealthComponent.h"
#include "VoidBomb.h"

AVoidPulse::AVoidPulse()
{
	
}

void AVoidPulse::BeginPlay()
{
	Super::BeginPlay();
	CoolDownTimer = CoolDown + 0.1f;
}

void AVoidPulse::Tick(float DeltaTime) 
{
	Super::Tick(DeltaTime);
}

void AVoidPulse::Activate(FTransform SpawnTrans)
{
	UE_LOG(LogTemp, Log, TEXT("Active Void"));
	if (CoolDownTimer <= CoolDown)
		return;
	CoolDownTimer = 0;
	FHitResult hitResult;
	GetWorld()->GetFirstPlayerController()->GetHitResultUnderCursorByChannel(UEngineTypes::ConvertToTraceType(ECC_Visibility), true, hitResult);
	if(hitResult.bBlockingHit)
	{
		UE_LOG(LogTemp, Log, TEXT("Spawning Void Bomb"));
		FTransform SpawnTrans;
		SpawnTrans.SetLocation(hitResult.Location);
		GetWorld()->SpawnActor<AVoidBomb>(VoidBomb, SpawnTrans);
	}
}