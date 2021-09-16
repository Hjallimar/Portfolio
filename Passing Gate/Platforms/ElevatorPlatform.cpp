//Author: Hjalmar Andersson

#include "ElevatorPlatform.h"
#include "Components/SceneComponent.h"

AElevatorPlatform::AElevatorPlatform()
{
	End = CreateDefaultSubobject<USceneComponent>(TEXT("End Position"));
	End->SetupAttachment(Root);
}

void AElevatorPlatform::BeginPlay()
{
	Super::BeginPlay();
	StartPos = GetActorLocation();
	EndPos = End->GetRelativeLocation() + GetActorLocation();
	Move(0.0f);
	if (!ManualTrigger)
		Activated = true;
}

void AElevatorPlatform::OnReseted(int i) 
{
	SetActorLocation(StartPos);
	CurrentTime = 0.0f;
	Direction = 1;
	StopTime = 0;
	Triggered = false;
	if (ManualTrigger)
		Activated = false;
}

void AElevatorPlatform::OnBeginOverlap(AActor* OtherActor)
{
	if(ManualTrigger && OtherActor == PlayerRef)
	{
		Triggered = true;
		Activated = true;
	}
}

void AElevatorPlatform::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if(ManualTrigger)
	{
		if (Triggered)
		{
			if(ManualWaitTime)
			{
				if (StopTime < WaitTime)
					Wait(DeltaTime);
				else
					Travel(DeltaTime);
			}
			else
			{
				Travel(DeltaTime);
			}
		}
	}
	else 
	{
		if (StopTime < WaitTime)
			Wait(DeltaTime);
		else
			Travel(DeltaTime);
	}
}

void AElevatorPlatform::Wait(float DeltatTime)
{
	StopTime += DeltatTime;
}

void AElevatorPlatform::Move(float t)
{
	FVector TravelDistance = FMath::Lerp(StartPos, EndPos, t);
	SetActorLocation(TravelDistance);
}

void AElevatorPlatform::Travel(float DeltaTime)
{
	CurrentTime += Direction * DeltaTime;
	float Distance = 0;

	if (CurrentTime >= TravelTime)
	{
		Distance = 1;
		Direction = -1;
		StopTime = 0;
		Triggered = false;
		OnDestinationReached();
	}
	else if(CurrentTime <= 0)
	{
		Distance = 0;
		Direction = 1;
		StopTime = 0;
		Triggered = false;
		OnDestinationReached();
	}

	Distance = CurrentTime / TravelTime;
	Move(Distance);
}