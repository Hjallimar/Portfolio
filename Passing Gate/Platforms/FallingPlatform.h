//Author: Hjalmar Andersson

#pragma once

#include "BasePlatform.h"
#include "FallingPlatform.generated.h"

class UBoxComponent;

UCLASS()
class AFallingPlatform : public ABasePlatform
{
	GENERATED_BODY()
public:
	AFallingPlatform();

	UPROPERTY(EditAnywhere)
	float FallSpeed = 10;

protected:
	virtual void OnReseted(int i) override;
	virtual void OnEndOverlap(AActor* OtherActor) override;

	bool Status = false;
	FVector StartPos;
	float FallDistance = 0;
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
};