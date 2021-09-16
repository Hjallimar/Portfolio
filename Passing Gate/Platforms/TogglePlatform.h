//Author: Hjalmar Andersson

#pragma once

#include "BasePlatform.h"
#include "TogglePlatform.generated.h"

UCLASS()
class ATogglePlatform : public ABasePlatform
{
	GENERATED_BODY()
public:
	ATogglePlatform();

	UPROPERTY(EditAnywhere)
	bool StartActive = true;

	UPROPERTY(EditAnywhere)
	float ActiveTime = 3.0f;

	UPROPERTY(EditAnywhere)
	float DeactivateTime = 3.0f;

protected:
	float CurrentTimer = 0;
	bool ActiveStatus = true;
	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	virtual void OnReseted(int i) override;
	void ToggleActive(bool Status);
};