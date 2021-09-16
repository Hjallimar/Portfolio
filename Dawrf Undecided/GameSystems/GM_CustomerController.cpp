//Author Hjalmar Andersson

#include "GM_CustomerController.h"
#include "GameSystem/GP_GameInstance.h"
#include "CustomerAI/BaseCustomer.h"
#include "Item/BaseItem_DataAsset.h"
#include "GM_CustomizationComponent.h"
#include "Item/ItemCounter.h"
#include "GM_BargainComponent.h"
#include "GM_StockComponent.h"
#include "GameFramework/Actor.h"
#include "CustomerAI/CustomerData.h"
#include "GM_AdventurerInformationComponent.h"
#include "GM_DayCycleComponent.h"
#include "CustomComponents/ZoomingComponent.h"

UGM_CustomerController::UGM_CustomerController()
{
	PrimaryComponentTick.bCanEverTick = true;
}

void UGM_CustomerController::SetCustomersLeftCount(int32 arg1)
{
	CustomersLeft = arg1;
}

void UGM_CustomerController::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	if (Done)
		return;

	UZoomingComponent* ZoomingComp = CurrentCustomer->FindComponentByClass<UZoomingComponent>();
	// Don't tick down if we're zoomed in, a.k.a. interacting with the customer.
	if (!(ZoomingComp && ZoomingComp->IsZoomedIn()))
	{
		TickTimer += DeltaTime;
	}
	
	if (Walk)
		MoveTheCustomer(); //moving customer
	if (Pay)
		PayGoldAnim();
	else if (Grab)
		TakeItemAnim();
	else if (WaitNext)
		WaitNextCustomer(); // waitning outside store
	else if (WaitStore)
		WaitInStore();	//Waiting in store
}

void UGM_CustomerController::BeginPlay()
{
	Super::BeginPlay();
	Customizer = GetOwner()->FindComponentByClass<UGM_CustomizationComponent>();
	Stock = GetOwner()->FindComponentByClass<UGM_StockComponent>();
	AdventurerInfo = GetOwner()->FindComponentByClass<UGM_AdventurerInformationComponent>();
	WaitNext = true;
	Done = true;
	Walk = false;
	Pay = false;
	Grab = false;
	EndTimer = FMath::RandRange(5.0f, 10.0f);
	CustomersLeft = AmmountOfCustomers;
	NameList = CustomerNames;
	//This handles the different states of the day
	UGM_DayCycleComponent* DayComp = GetOwner()->FindComponentByClass<UGM_DayCycleComponent>();
	DayComp->DayStateDelegate.AddDynamic(this, &UGM_CustomerController::DayStateHandler);
}

void UGM_CustomerController::AssignCustomer(ABaseCustomer* Customer)
{
	CurrentCustomer = Customer;
}

void UGM_CustomerController::AssignItemCounter(AItemCounter* Counter)
{
	ItemCounter = Counter;
}

void UGM_CustomerController::AssignCurrencyCounter(UItemHolderComponent* Counter)
{
	CurrencyCounter = Counter;
}

void UGM_CustomerController::DayStateHandler(EDayState State)
{
	if(State == EDayState::StartWorkDay)
	{
		//Start day
		StartCustomersAgain();
		UE_LOG(LogTemp, Log, TEXT("Time to start the day"));
	}
	else if(State == EDayState::EndWorkDay)
	{
		//EndDay
		CustomersLeft = 0;
		UE_LOG(LogTemp, Log, TEXT("No more customers after this"));
	}
}

void UGM_CustomerController::GenerateNewCustomer()
{
	CurrentDataAsset = Stock->GetRandomItemInStock();
	if (CurrentDataAsset == nullptr)
	{
		Done = true;
		return;
	}

	int HatIndex = Customizer->GetHatIndex();
	CurrentCustomer->EquipHat(Customizer->GetHatAt(HatIndex));

	int BeardIndex = Customizer->GetBeardIndex();
	CurrentCustomer->EquipBeard(Customizer->GetBeardAt(BeardIndex));

	int NoseIndex = Customizer->GetNoseIndex();
	CurrentCustomer->EquipNose(Customizer->GetNoseAt(NoseIndex));

	int EarIndex = Customizer->GetEarsIndex();
	CurrentCustomer->EquipEars(Customizer->GetEarsAt(EarIndex));

	int ColorIndex = Customizer->GetColorIndex();
	CurrentCustomer->ChangeColor(Customizer->GetColorAt(ColorIndex));

	// Set the customer level. Depending on day, the difficulty will increase.
	int CustomerLevel;

	auto* DayCycleComp = GetOwner()->FindComponentByClass<UGM_DayCycleComponent>();
	if (DayCycleComp)
	{
		int Day = DayCycleComp->GetDay();
		CustomerLevel = GetRandomCustomerLevel(Day);
	}
	else
	{
		CustomerLevel = 0;
	}

	CurrentCustomer->SetLevel(CustomerLevel);

	// Set weapon want type
	ELevel2WantType WantType = FMath::RandRange(0, 1) == 0 ? ELevel2WantType::DamageType : ELevel2WantType::WeaponType;
	CurrentCustomer->SetLevel2WantType(WantType);

	CurrentCustomer->ResetCustomer();
	int RandName = FMath::RandRange(0, NameList.Num() - 1);
	AdventurerInfo->AssignNewCustomer(NameList[RandName], HatIndex, BeardIndex, CurrentDataAsset, NoseIndex, EarIndex, ColorIndex, CustomerLevel);
	CurrentCustomer->AssignNewName(NameList[RandName]);
	NameList.RemoveAt(RandName);
}

void UGM_CustomerController::GetCustomer()
{
	if (CustomersLeft > 0)
	{
		if (AdventurerInfo->DoesCustomerExist() && FMath::RandRange(0, 100) > 70 || NameList.Num() < 1)
		{
			FCustomerData* Data = AdventurerInfo->GetRandomCustomer();
			if(Data == nullptr)
			{
				GenerateNewCustomer();
			}
			else
			{
				CurrentDataAsset = Stock->GetRandomItemInStock();
				if (CurrentDataAsset == nullptr)
				{
					UE_LOG(LogTemp, Log, TEXT("Items left in store: %i, Item received was null"), Stock->StoreSize());
					Done = true;
				}
				else
				{
					CurrentCustomer->AssignNewName(Data->CustomerName);
					Data->CustomerItem = CurrentDataAsset;
					CurrentCustomer->EquipHat(Customizer->GetHatAt(Data->HatIndex));
					CurrentCustomer->EquipBeard(Customizer->GetBeardAt(Data->BeardIndex));
					CurrentCustomer->EquipNose(Customizer->GetNoseAt(Data->NoseIndex));
					CurrentCustomer->EquipEars(Customizer->GetEarsAt(Data->EarIndex));
					CurrentCustomer->ChangeColor(Customizer->GetColorAt(Data->ColorIndex));

					int CustomerLevel; //= Data->CustomerLevel
					auto* DayCycleComp = GetOwner()->FindComponentByClass<UGM_DayCycleComponent>();
					if (DayCycleComp)
					{
						int Day = DayCycleComp->GetDay();
						CustomerLevel = GetRandomCustomerLevel(Day);
					}
					else
					{
						CustomerLevel = 0;
					}

					CurrentCustomer->SetLevel(CustomerLevel);

					if (CurrentCustomer->GetLevel() == 1)
					{
						int Rand = FMath::RandRange(0, 1);
						CurrentCustomer->SetLevel2WantType(Rand == 0 ? ELevel2WantType::DamageType : ELevel2WantType::WeaponType);
					}

					CurrentCustomer->ResetCustomer();
				}
			}
		}
		else
		{
			GenerateNewCustomer();
		}
	}
	else
	{
		Done = true;
		TickTimer = 0; /////////////////
	}

	if (Done)
	{
		GetOwner()->FindComponentByClass<UGM_DayCycleComponent>()->EndWorkDay();
		CurrentCustomer->ResetCustomer();
		GenerateNewCustomer();
	}
}

void UGM_CustomerController::CustomerOutside()
{
}

void UGM_CustomerController::ActivateActor(AActor* Actor, bool Status)
{
	Customizer->ActivateActor(Actor, Status);
}

void UGM_CustomerController::ItemBought(UBaseItem_DataAsset* BoughtItem)
{
	//float survivalChance;
	//if(BoughtItem == nullptr)
	//{
	//	AdventurerInfo->AssignSurvival(survivalChance);
	//}
	//else
	//{
		float survivalChance = CurrentDataAsset->CompareTo(BoughtItem, CurrentCustomer->GetLevel(), CurrentCustomer->GetWantType(), GetWorld());
		AdventurerInfo->AssignSurvival(survivalChance, BoughtItem->Name);
	//}
	Stock->RemoveItemFromStock(BoughtItem);
	CurrentDataAsset = nullptr;
	TickTimer = 0.0f;
	EndTimer = 3.6f;
	bLeaving = false;
	CurrentCustomer->PlayAnimationIndex(2); //Pay
	Pay = true;
	CustomersLeft--;
}

void UGM_CustomerController::WaitNextCustomer()
{
	if(!bHaveWaitTime || TickTimer > EndTimer)
	{
		TickTimer = 0.0f;
		EndTimer = 4.0f;
		Walk = true;
		CurrentCustomer->PlayAnimationIndex(1);
		GetCustomer();
	}
}

void UGM_CustomerController::WaitInStore()
{
	if (!bHaveWaitTime)
		return;

	if(!bLeaving && EndTimer - TickTimer < TimeTilLeaving && Walk == false)
	{
		CurrentCustomer->AboutToLeaveEvent();
		bLeaving = true;
	}
	else if (TickTimer > EndTimer)
	{
		TickTimer = 0.0f;
		EndTimer = 4.0f;
		Walk = true;
		CurrentCustomer->PlayAnimationIndex(1);
		CustomersLeft--;
		bLeaving = false;
	}
}

void UGM_CustomerController::MoveTheCustomer()
{
	float MoveTimer = WaitStore ? (1 - (TickTimer / 4.0f)): (TickTimer / 4.0f);
	CurrentCustomer->Move(MoveTimer);
	if (TickTimer > EndTimer)
	{
		TickTimer = 0.0f;
		Walk = false;
		CurrentCustomer->PlayAnimationIndex(0);
		WaitStore = !WaitStore;
		WaitNext = !WaitNext;
		if(WaitNext)
		{
			EndTimer = FMath::RandRange(MinOutTime, MaxOutTime);
		}
		else if(WaitStore)
		{
			if(CurrentDataAsset == nullptr)
			{			
				CurrentDataAsset = Stock->GetRandomItemInStock();
			}
			CurrentCustomer->AssignNewItemStats(CurrentDataAsset);

			float RandTimer;
			UGP_GameInstance* Inst = GetWorld() ? GetWorld()->GetGameInstance<UGP_GameInstance>() : nullptr;
			if (Inst && Inst->bHardMode)
			{
				RandTimer = FMath::RandRange(Inst->HardModeMinTime, Inst->HardModeMaxTime);
			}
			else
			{
				RandTimer = FMath::RandRange(MinStoreTime, MaxStoreTime);
			}

			EndTimer = RandTimer;
		}
		CurrentCustomer->Interactable = WaitStore;
	}
}

void UGM_CustomerController::PayGoldAnim() 
{
	if (TickTimer >= 2.6)
	{
		Pay = false;
		Grab = true;
		CurrentCustomer->PlayAnimationIndex(3); //Grab
		SpawnGold();
		
	}
}

void UGM_CustomerController::TakeItemAnim()
{
	if(TickTimer > 3 && TickTimer < 3.1)
	{
		CurrentCustomer->GrabBoughtItem();
	}
	if (TickTimer > EndTimer)
	{
		Walk = true;
		Grab = false;
		TickTimer = 0;
		EndTimer = 4.0f;
		CurrentCustomer->PlayAnimationIndex(1);
	}
}

void UGM_CustomerController::SpawnGold()
{
	FTransform SpawnTransform;
	SpawnTransform.SetLocation(CurrencyCounter->ItemPosition);
	SpawnTransform.SetRotation(CurrencyCounter->GetOwner()->GetActorQuat());
	auto newGold = GetWorld()->SpawnActor<ABaseItem>(GoldBP, SpawnTransform);
	newGold->PlayInteractionSound(EItemInteractType::Placedown);
	CurrencyCounter->ScanForItems();
}

UDialogAsset* UGM_CustomerController::GetDialog(int Level)
{
	TArray<UDialogAsset*> MyDialogs;

	bool bNormal = true;

	UWorld* World = GetWorld();
	if (World && World->IsGameWorld())
	{
		auto* Inst = World->GetGameInstance<UGP_GameInstance>();
		if (Inst->bHardMode)
		{
			bNormal = false;
		}
	}
	
	if (bNormal)
	{
		switch (Level)
		{
		default:
		case 0:
			MyDialogs = MyDialogs_Level1;
			break;

		case 1:
			MyDialogs = MyDialogs_Level2;
			break;

		case 2:
			MyDialogs = MyDialogs_Level3;
			break;
		}
	}
	else
	{
		MyDialogs = MyDialogs_HardMode;
	}

	int rand = FMath::RandRange(0, MyDialogs.Num() - 1);
	return MyDialogs[rand];
}
// JH: Level currently does not do anything, it's only if we decide to have level-specific dialog text for good/bad.
UDialogAsset* UGM_CustomerController::GetGoodDialog(int Level)
{
	TArray<UDialogAsset*>* AssetArray = nullptr;
	UWorld* World = GetWorld();
	if (World && World->IsGameWorld())
	{
		auto* Inst = World->GetGameInstance<UGP_GameInstance>();
		if (Inst->bHardMode)
		{
			AssetArray = &GoodDialog_HardMode;
		}
	}

	if (!AssetArray)
		AssetArray = &GoodDialog;

	if (AssetArray && AssetArray->Num() > 0)
	{
		int rand = FMath::RandRange(0, AssetArray->Num() - 1);
		return (*AssetArray)[rand];
	}

	return nullptr;
}

// JH: Level currently does not do anything, it's only if we decide to have level-specific dialog text for good/bad.
UDialogAsset* UGM_CustomerController::GetOKDialog(int Level)
{
	if (OKDialog.Num() > 0)
	{
		int rand = FMath::RandRange(0, OKDialog.Num() - 1);
		return OKDialog[rand];
	}

	return nullptr;
}

// JH: Level currently does not do anything, it's only if we decide to have level-specific dialog text for good/bad.
UDialogAsset* UGM_CustomerController::GetBadDialog(int Level)
{
	TArray<UDialogAsset*>* AssetArray = nullptr;
	UWorld* World = GetWorld();
	if (World && World->IsGameWorld())
	{
		auto* Inst = World->GetGameInstance<UGP_GameInstance>();
		if (Inst->bHardMode)
		{
			AssetArray = &BadDialog_HardMode;
		}
	}

	if (!AssetArray)
		AssetArray = &BadDialogs;

	if (AssetArray && AssetArray->Num() > 0)
	{
		int rand = FMath::RandRange(0, AssetArray->Num() - 1);
		return (*AssetArray)[rand];
	}

	return nullptr;
}

void UGM_CustomerController::StartCustomersAgain()
{
	Done = false;
	TickTimer = 0;
	CustomersLeft = AmmountOfCustomers;
}

int UGM_CustomerController::GetRandomCustomerLevel(int CurrentDay) const
{
	int CustomerLevel;
	UGP_GameInstance* Inst = GetWorld() ? GetWorld()->GetGameInstance<UGP_GameInstance>() : nullptr;
	if (Inst && Inst->bHardMode)
	{
		CustomerLevel = 2;
		return CustomerLevel;
	}

	int Rand = FMath::RandRange(0, 100);
	if (CurrentDay <= 1)
	{
		CustomerLevel = 0;
	}
	else if (CurrentDay == 2)
	{
		CustomerLevel = Rand > 40 ? 1 : 0; // 60% chance
	}
	else if (CurrentDay == 3)
	{
		CustomerLevel = 1;
	}
	else if (CurrentDay == 4)
	{
		CustomerLevel = Rand > 40 ? 2 : 1; // 60% chance
	}
	else if (CurrentDay >= 5)
	{
		CustomerLevel = 2;
	}

	return CustomerLevel;
}