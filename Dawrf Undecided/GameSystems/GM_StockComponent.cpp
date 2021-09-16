//Author Hjalmar Andersson

#include "GM_StockComponent.h"
#include "Item/BaseItem_DataAsset.h"
#include "GM_DayCycleComponent.h"

UGM_StockComponent::UGM_StockComponent()
{

}

void UGM_StockComponent::BeginPlay()
{
	Super::BeginPlay();
	CurrentItemTargets = ItemsDay1;
	GetOwner()->FindComponentByClass<UGM_DayCycleComponent>()->DayStateDelegate.AddDynamic(this, &UGM_StockComponent::HandleDayState);
}

UBaseItem_DataAsset* UGM_StockComponent::GetRandomItemInStock()
{
	float RandInt = FMath::RandRange(0, CurrentItemTargets.Num() - 1);
	if(StoreStock.Contains(CurrentItemTargets[RandInt]))
	{
		return CurrentItemTargets[RandInt];
	}
	else
	{
		if(OddRequest >= 2)
		{
			if (StoreStock.Num() < 1)
			{
				UE_LOG(LogTemp, Warning, TEXT("No ITEMS IN STOCK!"));
				return nullptr;
			}
			float Rand = FMath::RandRange(0, StoreStock.Num() - 1);
			return StoreStock[Rand];
		}
		else
		{
			OddRequest++;
			return CurrentItemTargets[RandInt];
		}
	}
}

void UGM_StockComponent::RemoveItemFromStock(UBaseItem_DataAsset* Remove)
{
	if(StoreStock.Contains(Remove))
	{
		StoreStock.RemoveSingle(Remove);
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("THIS ITEM IS NOT REGISTERD IN STOCK!"));
	}
}

int UGM_StockComponent::StoreSize()
{
	return StoreStock.Num();
}

void UGM_StockComponent::RegisterItem(UBaseItem_DataAsset* Register)
{
	if(Register != nullptr)
	{
		StoreStock.Add(Register);
	}
}

void UGM_StockComponent::HandleDayState(EDayState State)
{
	if(State == EDayState::EndWorkDay)
	{
		OddRequest = 0;
		CurrentDay++;
		switch(CurrentDay)
		{
		case 1:
			CurrentItemTargets.Append(ItemsDay2);
			break;
		case 2:
			CurrentItemTargets.Append(ItemsDay3);
			break;
		}
	}
}