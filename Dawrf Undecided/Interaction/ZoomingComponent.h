//Author Hjalmar Andersson

#pragma once
#include "Components/ActorComponent.h"
#include "ZoomingComponent.generated.h"

class UCameraComponent;
class APlayerCharacter;

UCLASS()
class UZoomingComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UZoomingComponent();
	bool GetZooming();
	bool IsSetUp();
	void SetUpZooming(USceneComponent* ZoomInComp, USceneComponent* ZoomOutComp, APlayerCharacter* Player = nullptr);
	void StartZooming();
protected:

	UPROPERTY(EditAnywhere)
	float ZoomTime = 1.0f;

	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;
private:
	void Zoom();
	UCameraComponent* Camera;
	bool bZooming = false;
	bool bZoomIn = false;
	float ZoomTimer = 0.0f;
	USceneComponent* ZoomInComp;
	FVector MyZoomPos;
	FRotator MyZoomRot;
	USceneComponent* ZoomOutComp;
	APlayerCharacter* PlayerRef;

public:
	bool IsZoomedIn() const { return bZoomIn; }
};