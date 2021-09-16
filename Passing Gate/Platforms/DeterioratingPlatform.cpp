//Author: Hjalmar Andersson

#include "DeterioratingPlatform.h"

ADeterioratingPlatform::ADeterioratingPlatform()
{
}

void ADeterioratingPlatform::BeginPlay() 
{
	Super::BeginPlay();
	StartPos = Mesh->GetRelativeLocation();
	ResetPos = GetActorLocation();
	UE_LOG(LogTemp, Log, TEXT("Assigning resetPos: %f, %f, %f"), ResetPos.X, ResetPos.Y, ResetPos.Z);
	ShakeFrequency = 1.0f / ShakeFrequency;
}

void ADeterioratingPlatform::TriggerDeteriorating()
{
	Status = true;
	Activated = true;
}

void ADeterioratingPlatform::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if(Falling)
	{
		FallDistance += FallSpeed * DeltaTime;
		FVector NewPos = GetActorLocation() + (FVector::DownVector * FallSpeed) * DeltaTime;
		SetActorLocation(NewPos);
		if (FallDistance > 350.f)
		{
			SetActorHiddenInGame(true);
			Mesh->SetCollisionProfileName("NoCollision");
			OnDespawn();
			Falling = false;
		}
	}
	else if(Status)
	{
		ShakeTimer += DeltaTime;
		FrequencyTimer += DeltaTime;
		if(FrequencyTimer >= ShakeFrequency)
		{
			Shake();
			FrequencyTimer -= ShakeFrequency;
		}
		if (ShakeTimer > DeterioratingTime)
		{
			Falling = true;
			Status = false;
			Mesh->SetRelativeLocation(StartPos);
		}
	}
}

void ADeterioratingPlatform::OnReseted(int i)
{

	FrequencyTimer = 0.0f;
	FallDistance = 0.0f;
	ShakeTimer = 0.0f;
	UE_LOG(LogTemp, Log, TEXT("ResetPos: %f, %f, %f"), ResetPos.X, ResetPos.Y, ResetPos.Z);
	SetActorLocation(ResetPos);
	UE_LOG(LogTemp, Log, TEXT("ResetPos: %f, %f, %f"), ResetPos.X, ResetPos.Y, ResetPos.Z);
	SetActorHiddenInGame(false);
	Mesh->SetCollisionProfileName("BlockAllDynamic");
	Mesh->SetRelativeLocation(StartPos);
	Falling = false;
	Status = false;
	Activated = false;
}

void ADeterioratingPlatform::OnBeginOverlap(AActor* OtherActor)
{
	if (PlayerSpecific && OtherActor != PlayerRef)
		return;
	Status = true;
	Activated = true;
}

void ADeterioratingPlatform::Shake()
{
	FVector ShakeVect = FVector(FMath::RandRange(-1.0f, 1.0f), FMath::RandRange(-1.0f, 1.0f), 0.0f);
	ShakeVect *= ShakeVect.GetSafeNormal() * ShakeStrenght;
	FVector Diff = StartPos - (Mesh->GetRelativeLocation() + ShakeVect);
	if(Diff.Size() > ShakeStrenght)
	{
		ShakeVect = Diff.GetSafeNormal() * ShakeStrenght;
	}
	Mesh->AddLocalOffset(ShakeVect);
}