//Author Hjalmar Andersson

#pragma once

#include "Components/ActorComponent.h"
#include "DayStateDelegate.h"
#include "GM_StockComponent.generated.h"

class UBaseItem_DataAsset;

UCLASS()
class UGM_StockComponent : public UActorComponent
{
GENERATED_BODY()
public:
	UGM_StockComponent();
	UBaseItem_DataAsset* GetRandomItemInStock();
	void RemoveItemFromStock(UBaseItem_DataAsset* Remove);
	void RegisterItem(UBaseItem_DataAsset* Register);
	int StoreSize();

	UFUNCTION()
	void HandleDayState(EDayState State);
protected:
	virtual void BeginPlay() override;
	UPROPERTY(EditAnywhere)
	TArray<UBaseItem_DataAsset*> ItemsDay1;
	UPROPERTY(EditAnywhere)
	TArray<UBaseItem_DataAsset*> ItemsDay2;
	UPROPERTY(EditAnywhere)
	TArray<UBaseItem_DataAsset*> ItemsDay3;

	TArray<UBaseItem_DataAsset*> CurrentItemTargets;

	int OddRequest = 0;
	int CurrentDay = 0;

	UPROPERTY(VisibleAnywhere)
	TArray<UBaseItem_DataAsset*> StoreStock;
private:

};