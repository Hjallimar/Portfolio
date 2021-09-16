//Author Hjalmar Andersson


#include "GM_BargainComponent.h"
#include "Item/BaseItem.h"
#include "Item/BaseItem_DataAsset.h"
#include "CustomerAI/BaseCustomer.h"
#include "Player/PlayerCharacter.h"
#include "GP3GameMode.h"
#include "GM_DayCycleComponent.h"
#include "Item/DeliveryService.h"
#include "Item/GoldChest.h"
#include "Journal/PlayerJournal.h"
#include "Item/Level2WantType.h"

void UGM_BargainComponent::BeginPlay()
{
	Super::BeginPlay();
	GetOwner()->FindComponentByClass<UGM_DayCycleComponent>()->DayStateDelegate.AddDynamic(this, &UGM_BargainComponent::HandleDayState);
}

void UGM_BargainComponent::HandleDayState(EDayState State)
{
	if (State == EDayState::StartDay)
		BoughtItems = 0;
}

ABaseItem* UGM_BargainComponent::GetCurrentItem()
{
	return CurrentItem;	
}

//Just filled with temporary logs
void UGM_BargainComponent::Bargain() 
{
	CurrentCustomer->BuyItem(CurrentItem);
	CurrentItem = nullptr;
}

void UGM_BargainComponent::LeaveStore()
{
	//The Customer leaves the store without buying an Item
	CurrentCustomer = nullptr;
}

void UGM_BargainComponent::AssignItem(ABaseItem* NewItem)
{
	CurrentItem = NewItem;
}

void UGM_BargainComponent::AssignCustomer(ABaseCustomer* NewCustomer)
{
	CurrentCustomer = NewCustomer;
}

void UGM_BargainComponent::AssignPlayer(APlayerCharacter* NewPlayer)
{
	Player = NewPlayer;
}

float UGM_BargainComponent::CompareItems()
{
	if (CurrentItem == nullptr)
		return 100.0f;
	UBaseItem_DataAsset* Target = CurrentCustomer->GetItemStats();
	// <JH>
	ELevel2WantType Level2Type = CurrentCustomer->GetWantType();
	int CustomerLevel = CurrentCustomer->GetLevel();
	// </JH>
	return Target->CompareTo(CurrentItem->ItemStats, CustomerLevel, Level2Type, GetWorld());
}

void UGM_BargainComponent::MakeDeliviery(int Index, int Cost)
{
	if(GoldChest == nullptr || PostService == nullptr)
	{
		return;
	}
	else
	{
		if(GoldChest->GetCurrentGold() >= Cost)
		{
			if(BoughtItems < 3)
			{
				Journal->bOrderBought = true;
				GoldChest->RemoveGold(Cost);
				PostService->AddCrateToList(Index);
				BoughtItems++;
				Cast<AGP3GameMode>(GetOwner())->GetDayCycleComponent()->OrderMade();
			}
		}
	}
}

bool UGM_BargainComponent::BuyLicence(int Cost, int Index)
{
	if (GoldChest == nullptr || PostService == nullptr)
	{
		return false;
	}
	else
	{
		if (GoldChest->GetCurrentGold() >= Cost) 
		{
			GoldChest->RemoveGold(Cost);
			Journal->bOrderBought = true;
			Journal->UpdateWeaponIndex(1);
			for(int i = 0 ; i < 2; i++)
			{
				if(BoughtItems < 3)
				{
					PostService->AddCrateToList(Index + i);
					BoughtItems++;
				}
			}
			Cast<AGP3GameMode>(GetOwner())->GetDayCycleComponent()->OrderMade();
			return true;
		}
		return false;
	}
}