//Author Hjalmar Andersson

#include "ResetableComponent.h"
#include "GP4PlatformerGameModeBase.h"
#include "Kismet/GameplayStatics.h"


UResetableComponent::UResetableComponent()
{

}

void UResetableComponent::BeginPlay()
{
	Super::BeginPlay();
	AGP4PlatformerGameModeBase* GameModeBase = Cast<AGP4PlatformerGameModeBase>(GetWorld()->GetAuthGameMode());
	GameModeBase->ResetCall.AddDynamic(this, &UResetableComponent::OnReset);
}

void UResetableComponent::BeginDestroy()
{
	Super::BeginDestroy();
	//AGP4PlatformerGameModeBase* GameModeBase = Cast<AGP4PlatformerGameModeBase>(GetWorld()->GetAuthGameMode());
	//GameModeBase->ResetCall.RemoveDynamic(this, &UResetableComponent::OnReset);
}

void UResetableComponent::OnReset(int i)
{
	if (i == MyResetIndex)
		TimeToReset.Broadcast(i);
}