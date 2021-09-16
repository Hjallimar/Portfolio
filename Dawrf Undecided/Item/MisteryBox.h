//Author Hjalmar Andersson
// Johan Liljedahl
#pragma once
#include "DeliveryService.h"
#include "GameFramework/Actor.h"
#include "MisteryBox.generated.h"

class UInteractableComponent;
class ABaseItem;
class APlayerCharacter;
class UStaticMeshComponent;

UCLASS()
class AMisteryBox : public AActor
{
GENERATED_BODY()
public:
	AMisteryBox();
	virtual void BeginPlay() override;
	UFUNCTION(BlueprintCallable)
	void OnPlayerHover(AActor* Player);
	UFUNCTION(BlueprintCallable)
	void OnPlayerInteract(AActor* Player);
	UFUNCTION(BlueprintCallable)
	void OnPlayerStopInteract(AActor* Player);
	UFUNCTION(BlueprintCallable)
	void OnPlayerStopHover(AActor* Player);
protected:

	UPROPERTY(EditAnywhere)
	TArray<TSubclassOf<ABaseItem>> Container;

	UPROPERTY(EditAnywhere)
	UInteractableComponent* InteractComp;
	UPROPERTY(EditAnywhere)
	USceneComponent* Root;
	UPROPERTY(EditAnywhere)
	USceneComponent* SpawnPos;
	UPROPERTY(EditAnywhere)
	UStaticMeshComponent* Mesh;

	UPROPERTY(EditAnywhere)
	UStaticMeshComponent* OutLine;
	void SpawnItem(AActor* Player);
	APlayerCharacter* PlayerRef;
	ADeliveryService* DeliveryServiceRef;
};