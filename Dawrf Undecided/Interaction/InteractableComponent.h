// Author: Hjalmar Andersson

#pragma once

#include "Components/ActorComponent.h"
#include "InteractableComponent.generated.h"

class AActor;

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FInteractEvent, AActor*, Interactor);

UCLASS(Meta = (BlueprintSpawnableComponent))
class UInteractableComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UInteractableComponent();

	bool Interacted = false;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	UMaterialInstance* OutlineMat;

	UPROPERTY(BlueprintAssignable,BlueprintCallable)
	FInteractEvent OnPlayerHover;

	UPROPERTY(BlueprintAssignable, BlueprintCallable)
	FInteractEvent OnPlayerInteract;

	UPROPERTY(BlueprintAssignable, BlueprintCallable)
	FInteractEvent OnPlayerStopInteract;

	UPROPERTY(BlueprintAssignable, BlueprintCallable)
	FInteractEvent OnPlayerStopHover;

	UPROPERTY(BlueprintAssignable, BlueprintCallable)
	FInteractEvent OnPlayerBreakInteract;
};