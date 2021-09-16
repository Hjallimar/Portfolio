//Author Hjalmar Andersson

#pragma once
#include "Components/ActorComponent.h"
#include "DayStateDelegate.h"
#include "GM_BargainComponent.generated.h"

class ABaseItem;
class ABaseCustomer;
class APlayerCharacter;
class AGP3GameMode;
class ADeliveryService;
class AGoldChest;
class APlayerJournal;

UCLASS()
class UGM_BargainComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	void Bargain();
	void AssignItem(ABaseItem* NewItem);
	void AssignCustomer(ABaseCustomer* NewCustomer);
	void AssignPlayer(APlayerCharacter* NewPlayer);
	void LeaveStore();
	float CompareItems();

	UFUNCTION(BlueprintCallable)
	void MakeDeliviery(int Index, int Cost = 20);

	UFUNCTION(BlueprintCallable)
	bool BuyLicence(int Cost, int Index);

	UFUNCTION()
	void HandleDayState(EDayState State);
	
	UFUNCTION(BlueprintCallable)
	ABaseItem* GetCurrentItem();


	AGoldChest* GoldChest;

	ADeliveryService* PostService;
	APlayerJournal* Journal;
	ABaseCustomer* CurrentCustomer;
protected:
	virtual void BeginPlay() override;
	APlayerCharacter* Player;
	ABaseItem* CurrentItem;
	AGP3GameMode* MyGameMode;
	int BoughtItems = 0;
private:
};