// Copyright Epic Games, Inc. All Rights Reserved.

#include "GP3GameMode.h"
#include "CoolGP3ProjectCharacter.h"
#include "UObject/ConstructorHelpers.h"
#include "GameSystem/GM_BargainComponent.h"
#include "GameSystem/GM_CustomizationComponent.h"
#include "GM_CustomerController.h"
#include "GM_StockComponent.h"
#include "GM_AdventurerInformationComponent.h"
#include "GM_DayCycleComponent.h"

AGP3GameMode::AGP3GameMode()
{
	// set default pawn class to our Blueprinted character
	static ConstructorHelpers::FClassFinder<APawn> PlayerPawnBPClass(TEXT("/Game/ThirdPersonCPP/Blueprints/ThirdPersonCharacter"));
	if (PlayerPawnBPClass.Class != NULL)
	{
		DefaultPawnClass = PlayerPawnBPClass.Class;
	}

	Bargain = CreateDefaultSubobject<UGM_BargainComponent>(TEXT("Bargain Component"));
	Customize = CreateDefaultSubobject<UGM_CustomizationComponent>(TEXT("Cutomize Component"));
	CustomerController = CreateDefaultSubobject<UGM_CustomerController>(TEXT("Customer Controller Component"));
	AdventurerInfo = CreateDefaultSubobject<UGM_AdventurerInformationComponent>(TEXT("Adventurer Information Component"));
	Stock = CreateDefaultSubobject<UGM_StockComponent>(TEXT("Store Stock Component"));
	DayCycle = CreateDefaultSubobject<UGM_DayCycleComponent>(TEXT("Day Cycle Component"));
}

UGM_BargainComponent* AGP3GameMode::GetBargainComponent()
{
	return Bargain;
}

UGM_AdventurerInformationComponent* AGP3GameMode::GetAdventurerInformation()
{
	return AdventurerInfo;
}

UGM_CustomizationComponent* AGP3GameMode::GetCustomizeComponent()
{
	return Customize;
}

UGM_CustomerController* AGP3GameMode::GetCustomerController()
{
	return CustomerController;
}

UGM_StockComponent* AGP3GameMode::GetStockComponent()
{
	return Stock;
}

UGM_DayCycleComponent* AGP3GameMode::GetDayCycleComponent()
{
	return DayCycle;
}

int AGP3GameMode::GetGold() const
{
	return Gold;
}

void AGP3GameMode::SetGold(int NewGoldValue)
{
	const int OldGoldValue = Gold;

	//if (NewGoldValue < 0)
	//	Gold = 0;
	//else
		Gold = NewGoldValue;

	// Add the change in gold to today's gain/loss.
	GoldChangeToday += (Gold - OldGoldValue);

	if (OldGoldValue != Gold && OnGoldChangedDelegate.IsBound())
		OnGoldChangedDelegate.Broadcast(OldGoldValue, Gold);

	UE_LOG(LogTemp, Log, TEXT("%i"), Gold);
	if(Gold < 0)
	{
		UE_LOG(LogTemp, Warning, TEXT("GOLD UNDER ZERO, GAME OVER"));
		GameOver();
	}
}

void AGP3GameMode::AddGold(int AddGoldValue)
{
	SetGold(GetGold() + AddGoldValue);
	TotalGold += AddGoldValue;
}

void AGP3GameMode::AddFine(int Value)
{
	FinedGold += Value;
}

int  AGP3GameMode::GetTotalGold() const { return TotalGold; }
int  AGP3GameMode::GetFinedGold() const { return FinedGold; }

void AGP3GameMode::Console_SetDay(int32 arg1)
{
	if (DayCycle)
	{
		DayCycle->SetCurrentDay(arg1);
	}
}

void AGP3GameMode::Console_SetGold(int32 arg1)
{
	SetGold(arg1);
}

void AGP3GameMode::Console_SetCustomersLeftCount(int32 arg1)
{
	if (arg1 < 0)
		arg1 = 0;

	if (CustomerController)
		CustomerController->SetCustomersLeftCount(arg1);
}
