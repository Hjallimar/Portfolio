//Author: Hjalmar Andersson

#pragma once
#include "GameFramework/Pawn.h"
#include "Item/BaseItem.h"
#include "UI/ItemDisplayWidget.h"
#include "Item/Level2WantType.h"
#include "BaseCustomer.generated.h"

class UCapsuleComponent;
class UBaseItem_DataAsset;
class UInteractableComponent;
class AItemCounter;
class UGM_BargainComponent;
class UGM_CustomerController;
class UZoomingComponent;
//enum class ELevel2WantType;
//class UDialogAsset;
//class UCustomerPersonalDialogAsset;
class AGameHUD;
class UMaterialInstance;
class USkeletalMeshComponent;
class USplineComponent;

UCLASS()
class ABaseCustomer: public APawn
{
	GENERATED_BODY()
public:
	ABaseCustomer();
private:
	virtual void Tick(float DeltaTime) override;
	virtual void BeginPlay() override;

public:
	UFUNCTION()
	void OnPlayerInteract(AActor* Player);
	UFUNCTION()
	void OnDialogOver(int Decision);
	UFUNCTION()
	void OnBuyItem(int Decision);

	UFUNCTION(BlueprintImplementableEvent)
	void PlayAnimationIndex(int i);

	UFUNCTION()
	void OnPlayerBreakInteract(AActor* Player);

	UPROPERTY(EditAnywhere)
	AActor* SplineHolder;

	USplineComponent* MoveSpline;

protected:
	UPROPERTY(EditAnywhere)
	UCapsuleComponent* Capsule;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UInteractableComponent* InteractComp;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* ZoomPosition;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* ItemHolder;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* HatPosition;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* BeardPosition;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* EarsPosition;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USceneComponent* NosePosition;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	USkeletalMeshComponent* SkeletalMesh;

	APlayerCharacter* PlayerRef;;

	UPROPERTY(EditAnywhere)
	int CurrentLevel = 0; // 0 = 1, 1 = 2, 2 = 3
	UPROPERTY(EditAnywhere)
	ELevel2WantType Level2WantType = ELevel2WantType::DamageType;

public:
	UFUNCTION(BlueprintCallable)
	void BuyItem(ABaseItem* BoughtItem);

	UFUNCTION(BlueprintImplementableEvent)
	void EmojiEvent(float Happy);

	UFUNCTION(BlueprintImplementableEvent)
	void AboutToLeaveEvent();
	

	UPROPERTY(EditAnywhere)
		bool SidewayWalking = false;

	UBaseItem_DataAsset* GetItemStats();
	void AssignNewItemStats(UBaseItem_DataAsset* NewStats);
	void AssignNewName(FString NewName);
	void Move(float Timer);
	void ResetCustomer();
	void EquipHat(AActor* NewHat);
	void EquipBeard(AActor* NewBeard);
	void EquipEars(AActor* NewEars);
	void EquipNose(AActor* NewNose);
	void ChangeColor(UMaterialInstance* NewColor);
	// <JH>
	void SetLevel(int Level);
	int GetLevel() const;
	void SetLevel2WantType(ELevel2WantType WantType);
	ELevel2WantType GetWantType() const;
	// </JH>
	void GrabBoughtItem();
	bool Interactable = false;
	FString NameOfCustomer = "";
	ABaseItem* MyItem;
protected:
	UPROPERTY(EditAnywhere)
	UZoomingComponent* ZoomComp;

	UPROPERTY(EditAnywhere)
	bool bActivateZoom = true;
	bool Grabbed = false;

	UGM_BargainComponent* BargainComp;
	UGM_CustomerController* Controller;
	UPROPERTY(EditAnywhere)
	UBaseItem_DataAsset* TargetItem;

	// <JH>
	/* The dialog that this customer can say. */
	//UCustomerPersonalDialogAsset* DialogAsset;
	// </JH>

	void AssignDisplayInfo();
	TArray<FStringFormatArg> StatNameList;
	AGameHUD* GameHUD;
	AActor* CurrentHat = nullptr;
	AActor* CurrentBeard = nullptr;
	AActor* CurrentNose = nullptr;
	AActor* CurrentEars = nullptr;
	UMaterialInstance* CurrentColor = nullptr;

	UPROPERTY(EditAnywhere)
	FVector SpawnPos;
	UPROPERTY(EditAnywhere)
	FVector ShopPos;
	
	UFUNCTION(BlueprintCallable)
	void OnPlayerHover(AActor* Player);
	UFUNCTION(BlueprintCallable)
    void OnPlayerStopHover(AActor* Player);
	FDisplayLabelData GenerateDisplayItemInfo();
};
