//Author Hjalmar Andersson

#pragma once
#include "Components/ActorComponent.h"

#include "InteractableComponent.generated.h"

class USphereComponent;

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FInteractedEvent);

UCLASS(Meta = (BlueprintSpawnableComponent))
class UInteractableComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UInteractableComponent();

	UFUNCTION()
	void OverlapBegin(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
	UFUNCTION()
	void OverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);

	UPROPERTY(BlueprintAssignable)
	FInteractedEvent OnInteracted;

	UFUNCTION(BlueprintCallable)
		void DeactivateInteraction();

	UFUNCTION(BlueprintCallable)
	void AssignNewSphere(USphereComponent* Sphere);

private:

	virtual void BeginPlay() override;
	
	bool Interactable = false;
	APawn* PlayerRef;
	UPROPERTY(VisibleAnywhere)
	USphereComponent* CollisionSphere;

	

	UFUNCTION()
	void Interact();
};