// Author: Hjalmar Andersson
// Co-Author: Johan Liljedahl
// Co-Author: Justus H�rberg

#pragma once

#include "GameFramework/Actor.h"
#include "ItemStats.h"
#include "CustomComponents/ItemHolderComponent.h"


#include "BaseItem.generated.h"

class UInteractableComponent;
class UStaticMeshComponent;
class USphereComponent;
class UBoxComponent;
class UBaseItem_DataAsset;
class UItemDisplayWidget;
class UAudioHandlerComponent;
//========================= JUSTUS -->
struct FDisplayLabelData;

UENUM()
enum class EItemInteractType
{
	Pickup = 0,
	Placedown
};
//========================= JUSTUS <--


UCLASS()
class ABaseItem : public AActor
{
	GENERATED_BODY()
public:
	ABaseItem();
	virtual void Tick(float DeltaSeconds) override; //JL
	virtual void BeginPlay() override;
	void DeleteThisItem();
	
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	USceneComponent* Root;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	UInteractableComponent* Interactable;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	UStaticMeshComponent* Mesh;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	UStaticMeshComponent* OverlayMesh;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	UMaterialInstance* Allowed;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	UMaterial* Denied;
	UPROPERTY(EditAnywhere)
	UBoxComponent* BoxCol;
	//========================= JUSTUS -->
	/* Item's audio handler. Primarily used for sounds that play when picking up and placing down. */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
		UAudioHandlerComponent* AudioHandlerComponent;
	//========================= JUSTUS <--

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	USphereComponent* Sphere;

	UFUNCTION(BlueprintCallable)
	void OnBought();
	UFUNCTION(BlueprintCallable)
	void OnPlayerInteract();
	UFUNCTION(BlueprintCallable)
	void OnPlayerStopInteract();
	UFUNCTION(BlueprintCallable)
	void OnPlayerHover(AActor* Player);
	UFUNCTION(BlueprintCallable)
	void OnPlayerStopHover(AActor* Player);
	bool bIsBeingCarried = false;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly)
	UBaseItem_DataAsset* ItemStats;
	UItemHolderComponent* CurrentItemHolder = nullptr;
	//========================= JUSTUS -->
	void PlayInteractionSound(EItemInteractType InteractionType);
protected:
	//TWeakPtr<UItemDisplayWidget> ItemWidget = nullptr;
	
	/* Safe short-hand widget returner */
	UItemDisplayWidget* GetItemDisplayWidget() const;

	FDisplayLabelData GenerateDisplayItemInfo() const;

	/* Makes the weapon play a sound based on its interaction. */
	//========================= JUSTUS <--
};