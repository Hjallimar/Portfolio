//Author Hjalmar Andersson
// Co-Author: Justus Hörberg

#include "GM_DayCycleComponent.h"

UGM_DayCycleComponent::UGM_DayCycleComponent()
{
}

// <JH>
void UGM_DayCycleComponent::BeginPlay()
{
	Super::BeginPlay();
	Day = 1;
}
// </JH>

void UGM_DayCycleComponent::StartDay()
{
	// <JH>
	Day++;
	UE_LOG(LogTemp, Log, TEXT("Starting day %i"), Day);
	// </JH>
	DayStateDelegate.Broadcast(EDayState::StartDay);
}

void UGM_DayCycleComponent::StartWorkDay()
{
	DayStateDelegate.Broadcast(EDayState::StartWorkDay);
}

void UGM_DayCycleComponent::EndWorkDay()
{
	DayStateDelegate.Broadcast(EDayState::EndWorkDay);
}

void UGM_DayCycleComponent::EndDay()
{
	DayStateDelegate.Broadcast(EDayState::EndDay);
}

void UGM_DayCycleComponent::AfterNews()
{
	DayStateDelegate.Broadcast(EDayState::AfterNewspaper);
}

void UGM_DayCycleComponent::OrderMade()
{
	DayStateDelegate.Broadcast(EDayState::OrderDelivery);
}