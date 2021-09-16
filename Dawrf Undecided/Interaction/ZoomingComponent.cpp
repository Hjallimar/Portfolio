#include "ZoomingComponent.h"
#include "Camera/CameraComponent.h"
#include "Player/PlayerCharacter.h"

UZoomingComponent::UZoomingComponent()
{
	PrimaryComponentTick.bCanEverTick = true;
}

void UZoomingComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	if (bZooming)
	{
		if (ZoomTimer > ZoomTime)
		{
			ZoomTimer = ZoomTime;
			Zoom();
			bZooming = false;
			bZoomIn = !bZoomIn;
			if (!bZoomIn)
			{
				//PlayerRef->bRestrictMovement = false;
				PlayerRef->RestrictMovementInput(false);
				PlayerRef->SetPlayerControllerInactiveState(false);
			}
			ZoomTimer = 0.0f;
		}
		else
		{
			ZoomTimer += DeltaTime;
			Zoom();
		}
	}
}

void UZoomingComponent::SetUpZooming(USceneComponent* Start, USceneComponent* End, APlayerCharacter* Player)
{
	ZoomInComp = Start;
	ZoomOutComp = End;
	if (Player == nullptr)
		UE_LOG(LogTemp, Warning, TEXT("No Player was assigned for the zoom!"));
	
	PlayerRef = Player;
	Camera = PlayerRef->FindComponentByClass<UCameraComponent>();
}

bool UZoomingComponent::GetZooming()
{
	return bZooming;
}

bool UZoomingComponent::IsSetUp()
{
	if (ZoomInComp != nullptr && ZoomOutComp != nullptr && PlayerRef != nullptr)
		return true;
	return false;
}

void UZoomingComponent::StartZooming()
{
	if (bZooming)
		return;
	//PlayerRef->bRestrictMovement = true;
	PlayerRef->RestrictMovementInput(true);
	PlayerRef->SetPlayerControllerInactiveState(true);
	MyZoomPos = ZoomInComp->GetComponentLocation();
	MyZoomRot = ZoomInComp->GetComponentRotation();
	bZooming = true;
}

void UZoomingComponent::Zoom()
{
	float timer = ZoomTimer / ZoomTime;
	if (!bZoomIn)
	{
		FVector CurrentPos = FMath::Lerp(ZoomOutComp->GetComponentLocation(), MyZoomPos, timer);
		FQuat CurrentRot = FMath::Lerp(ZoomOutComp->GetComponentQuat(), MyZoomRot.Quaternion(), timer);
		Camera->SetWorldLocation(CurrentPos);
		Camera->SetWorldRotation(CurrentRot);
	}
	else
	{
		FVector CurrentPos = FMath::Lerp(MyZoomPos, ZoomOutComp->GetComponentLocation(), timer);
		FQuat CurrentRot = FMath::Lerp(MyZoomRot.Quaternion(), ZoomOutComp->GetComponentQuat(), timer);
		Camera->SetWorldLocation(CurrentPos);
		Camera->SetWorldRotation(CurrentRot);
	}
}

