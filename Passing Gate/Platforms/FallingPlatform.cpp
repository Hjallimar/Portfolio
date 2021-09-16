//Author: Hjalmar Andersson

#include "FallingPlatform.h"

AFallingPlatform::AFallingPlatform()
{
}

void AFallingPlatform::BeginPlay()
{
	Super::BeginPlay();
	StartPos = GetActorLocation();
}

void AFallingPlatform::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if (Status)
	{
		FallDistance += FallSpeed * DeltaTime;
		FVector NewPos = GetActorLocation() + (FVector::DownVector * FallSpeed) * DeltaTime;
		SetActorLocation(NewPos);
		if(FallDistance > 350.f)
		{
			SetActorHiddenInGame(true);
			Mesh->SetCollisionProfileName("NoCollision");
			Status = false;
			OnDespawn();
		}
	}
}

void AFallingPlatform::OnReseted(int i)
{
	Status = false;

	FallDistance = 0.0f;
	SetActorHiddenInGame(false);
	Mesh->SetCollisionProfileName("BlockAll");
	SetActorLocation(StartPos);
	Activated = false;
}

void AFallingPlatform::OnEndOverlap(AActor* OtherActor) 
{
	if (PlayerSpecific && OtherActor != PlayerRef)
		return;
	Status = true;
	Activated = true;
}