//Author Hjalmar Andersson
// Johan Liljedahl
#include "DayStateCaller.h"
#include "GameSystem/GM_DayCycleComponent.h"
#include "GameSystem/GP3GameMode.h"
#include "Kismet/GamePlayStatics.h"
#include "CustomComponents/InteractableComponent.h"


ADayStateCaller::ADayStateCaller()
{
	InteractComp = CreateDefaultSubobject<UInteractableComponent>(TEXT("Interact Component"));
}

void ADayStateCaller::BeginPlay()
{
	Super::BeginPlay();
	auto* GM = Cast<AGP3GameMode>(UGameplayStatics::GetGameMode(this));
	if (GM == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("No GameMode detected"));
		return;
	}
	DayCycle = GM->GetDayCycleComponent();
	DayCycle->DayStateDelegate.AddDynamic(this, &ADayStateCaller::DayStateHandler);
}

void ADayStateCaller::SendDayStateEvent(EDayState State)
{
	if(State == EDayState::StartDay)
	{
		DayCycle->StartDay();
	}
	else if (State == EDayState::StartWorkDay)
	{
		DayCycle->StartWorkDay();
	}
	else if(State == EDayState::EndWorkDay)
	{
		DayCycle->EndWorkDay();
	}
	else if(State == EDayState::EndDay)
	{
		DayCycle->EndDay();
	}
}

void ADayStateCaller::DayStateHandler(EDayState State)
{
	OnDayEventIndex((int)State);
}