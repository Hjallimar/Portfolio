//Author: Hjalmar Andersson

#include "TogglePlatform.h"

ATogglePlatform::ATogglePlatform()
{
	
}

void ATogglePlatform::BeginPlay()
{
	Super::BeginPlay();
	ActiveStatus = StartActive;
	CurrentTimer = ActiveStatus ? ActiveTime : DeactivateTime;
	ToggleActive(ActiveStatus);
	Activated = true;
}

void ATogglePlatform::OnReseted(int i)
{
	ActiveStatus = StartActive;
	CurrentTimer = ActiveStatus ? ActiveTime : DeactivateTime;
	ToggleActive(ActiveStatus);
}

void ATogglePlatform::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	CurrentTimer -= DeltaTime;
	if(CurrentTimer < 0)
	{
		ActiveStatus = !ActiveStatus;
		ToggleActive(ActiveStatus);
		CurrentTimer = ActiveStatus ? ActiveTime : DeactivateTime;
	}
}

void ATogglePlatform::ToggleActive(bool Status)
{
	Mesh->SetCollisionProfileName(Status ? "BlockAllDynamic" : "NoCollision");
	Mesh->SetHiddenInGame(!Status);
}