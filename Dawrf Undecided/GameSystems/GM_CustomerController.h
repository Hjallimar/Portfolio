//Author Hjalmar Andersson
//Secondary Author Johan Liljedahl

#pragma once
#include "Components/ActorComponent.h"
#include "Item/BaseItem.h"
#include "DayStateDelegate.h"
#include "GM_CustomerController.generated.h"

class ABaseCustomer;
class UBaseItem_DataAsset;
class UGM_CustomizationComponent;
class UGM_AdventurerInformationComponent;
class AItemCounter;
class UGM_StockComponent;
class AActor;
class UDialogAsset;


UCLASS()
class UGM_CustomerController : public UActorComponent
{
	GENERATED_BODY()
public:
	UGM_CustomerController();

	void StartCustomersAgain();
	void AssignCustomer(ABaseCustomer* Customer);
	void AssignItemCounter(AItemCounter* Counter);
	void AssignCurrencyCounter(UItemHolderComponent* Counter); //JL
	void ItemBought(UBaseItem_DataAsset* BoughtItem);
	void CustomerOutside();
	void ActivateActor(AActor* Actor, bool status);
	UDialogAsset* GetDialog(int Level);
	UDialogAsset* GetGoodDialog(int Level);
	UDialogAsset* GetOKDialog(int Level);
	UDialogAsset* GetBadDialog(int Level);
	UFUNCTION()
	void DayStateHandler(EDayState State);

	UFUNCTION(BlueprintCallable)
	int GetRandomCustomerLevel(int CurrentDay) const;

	UFUNCTION()
	/* DEBUG */
	void SetCustomersLeftCount(int32 arg1);

protected:
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;
	virtual void BeginPlay() override;
	UPROPERTY(EditDefaultsOnly)
	bool bHaveWaitTime = true;


	UPROPERTY(EditAnywhere)
	int TimeTilLeaving = 20;
	bool bLeaving = false;


	UPROPERTY(EditAnywhere)
	int AmmountOfCustomers = 3;
	int CustomersLeft = 0;
	UPROPERTY(EditAnywhere)
	float MinStoreTime = 40.0f;	
	UPROPERTY(EditAnywhere)
	float MaxStoreTime = 80.0f;	
	UPROPERTY(EditAnywhere)
	float MinOutTime = 10.0f;
	UPROPERTY(EditAnywhere)
	float MaxOutTime = 20.0f;

	void GetCustomer();
	void GenerateNewCustomer();
	void WaitNextCustomer();
	void WaitInStore();
	void MoveTheCustomer();
	void PayGoldAnim();
	void TakeItemAnim();
	void SpawnGold(); //JL

	ABaseCustomer* CurrentCustomer;
	AItemCounter* ItemCounter;
	UItemHolderComponent* CurrencyCounter;

private:
	UPROPERTY(EditAnywhere)
	TArray<FString> CustomerNames;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> MyDialogs_Level1;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> MyDialogs_Level2;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> MyDialogs_Level3;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> GoodDialog;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> BadDialogs;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> OKDialog;

	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> MyDialogs_HardMode;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> GoodDialog_HardMode;
	UPROPERTY(EditAnywhere)
	TArray<UDialogAsset*> BadDialog_HardMode;

	TArray<FString> NameList;

	UPROPERTY(VisibleAnywhere)
	UGM_StockComponent* Stock;
	UBaseItem_DataAsset* CurrentDataAsset;
	UGM_CustomizationComponent* Customizer;
	UGM_AdventurerInformationComponent* AdventurerInfo;
	bool Done = false;
	float TickTimer = 0.0f;
	float EndTimer = 0.0f;
	bool Walk;
	bool WaitStore;
	bool WaitNext;
	bool Grab;
	bool Pay;

	//JL----->
	UPROPERTY(EditAnywhere)
	TSubclassOf<ABaseItem> GoldBP;
	//JL<----
};