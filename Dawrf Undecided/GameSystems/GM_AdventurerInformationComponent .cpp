//Author Hjalmar Andersson;
//Author Johan Liljedahl
#include "GM_AdventurerInformationComponent.h"
#include "GM_DayCycleComponent.h"
#include "GP3GameMode.h"
#include "Item/GoldChest.h"
#include "UI/Newspaper/NewsArticle_DataAsset.h"
#include "UI/Subtitles/CustomerPersonalDialogAsset.h"
#include "UI/HUD/GameHUD.h"
#include "Kismet/GameplayStatics.h"
#include "UI/Newspaper/ArticleData.h"
#include "Item/BaseItem_DataAsset.h"

void UGM_AdventurerInformationComponent::BeginPlay()
{
	Super::BeginPlay();
	GetOwner()->FindComponentByClass<UGM_DayCycleComponent>()->DayStateDelegate.AddDynamic(this, &UGM_AdventurerInformationComponent::HandleDayState);
	APlayerController* PlayerCtrl = UGameplayStatics::GetPlayerController(GetWorld(), 0);
	GameHUD = Cast<AGameHUD>(PlayerCtrl->GetHUD());
}

void UGM_AdventurerInformationComponent::HandleDayState(EDayState State)
{
	if(State == EDayState::EndWorkDay)
	{
		ArticleCounter = 0;
		UE_LOG(LogTemp, Log, TEXT("Day is over, getting news"));
		CreateNewspapperNews();
	}
}

void UGM_AdventurerInformationComponent::FinePlayer()
{
	auto Owner = Cast<AGP3GameMode>(GetOwner());
	Owner->AddFine(FineAmount);
	Owner->SetGold(Owner->GetGold()-FineAmount);
	TArray<AActor*> GoldChest;
	UGameplayStatics::GetAllActorsOfClass(GetWorld(), AGoldChest::StaticClass(), GoldChest);
	Cast<AGoldChest>(GoldChest.Last())->UpdateGoldInStorage();
	GoldChest.Empty();
}

void UGM_AdventurerInformationComponent::RewardPlayer()
{
	auto Owner = Cast<AGP3GameMode>(GetOwner());
	Owner->AddGold(RewardAmount);
	TArray<AActor*> GoldChest;
	UGameplayStatics::GetAllActorsOfClass(GetWorld(), AGoldChest::StaticClass(), GoldChest);
	Cast<AGoldChest>(GoldChest.Last())->UpdateGoldInStorage();
	GoldChest.Empty();
}

bool UGM_AdventurerInformationComponent::DoesCustomerExist()
{
	UE_LOG(LogTemp, Log, TEXT("Checking if customers exists"));
	if (ExistingCustomers.Num() >= 1)
		return true;
	return false;
}

FCustomerData* UGM_AdventurerInformationComponent::GetRandomCustomer()
{
	if (ExistingCustomers.Num() > 0)
	{
		int Rand = FMath::RandRange(0, (ExistingCustomers.Num() - 1));
		CurrentAdventurers.Add(ExistingCustomers[Rand]);
		ExistingCustomers.RemoveAt(Rand);
		UE_LOG(LogTemp,Log, TEXT("Adding customer, Total of %i"), CurrentAdventurers.Num());
		return &CurrentAdventurers.Last();
	}
	return nullptr;
}
void UGM_AdventurerInformationComponent::AssignNewCustomer(FString Name, int Hat, int Beard, UBaseItem_DataAsset* Item, int Nose, int Ears, int Color, int Level/*, ELevel2WantType Level2WantType*/)
{ 
	FCustomerData NewCustomer = {Name, Item, Hat, Beard};
	NewCustomer.CustomerName = Name;
	NewCustomer.HatIndex = Hat;
	NewCustomer.BeardIndex = Beard;
	NewCustomer.CustomerItem = Item;
	NewCustomer.NoseIndex = Nose;
	NewCustomer.EarIndex = Ears;
	NewCustomer.ColorIndex = Color;
	NewCustomer.CustomerLevel = Level;

	// If customer is level 2, randomize whether they 
	// prioritize damage type or weapon type.
	if (NewCustomer.CustomerLevel == 1)
	{
		int Rand = FMath::RandRange(0, 1);
		NewCustomer.Level2WantType = Rand == 0 ? ELevel2WantType::DamageType : ELevel2WantType::WeaponType;
	}

	CurrentAdventurers.Add(NewCustomer);
	UE_LOG(LogTemp,Log, TEXT("Assigning customer, Total of %i"), CurrentAdventurers.Num());
	CurrentCustomerData = &CurrentAdventurers.Last(); 
}

void UGM_AdventurerInformationComponent::AssignSurvival(float Survival, FString BoughtItemName)
{
	CurrentCustomerData->SurvivalChance = Survival;
	CurrentCustomerData->BoughtWeaponName = BoughtItemName;
}

void UGM_AdventurerInformationComponent::CreateNewspapperNews()
{
	TArray<FArticleData> TempArticleArray;
	TArray<FStringFormatArg> CharacterInfo;
	for(int i = 0; i < CurrentAdventurers.Num(); i++)
	{
		CharacterInfo.Add((CurrentAdventurers[i].CustomerName)); // 0 - Customer Name
		CharacterInfo.Add(CurrentAdventurers[i].BoughtWeaponName); // 1 - Customer Item
		// 2 - Monster Type
		CharacterInfo.Add(StaticEnum<EMonsterTypes>()->GetNameStringByValue((int)CurrentAdventurers[i].CustomerItem->StrongAgainst[0]));
		if(CurrentAdventurers[i].BoughtWeaponName.IsEmpty())
		{
			DeadAdventurers.Add(CurrentAdventurers[i]);
			FArticleData Data = NoNews->GetArticleData();
			Data.AssignCharacterInfo(CharacterInfo);
			TempArticleArray.Add(Data);
		}
		else if(CurrentAdventurers[i].SurvivalChance >= 0.9f)
		{
			ExistingCustomers.Add(CurrentAdventurers[i]);
			FArticleData Data = GoodNews->GetArticleData();
			Data.AssignCharacterInfo(CharacterInfo);
			TempArticleArray.Add(Data);
		}
		else if(CurrentAdventurers[i].SurvivalChance >= 0.1f)
		{
			ExistingCustomers.Add(CurrentAdventurers[i]);
			FArticleData Data = OkNews->GetArticleData();
			Data.AssignCharacterInfo(CharacterInfo);
			TempArticleArray.Add(Data);
		}
		else if (CurrentAdventurers[i].SurvivalChance < 0.1f)
		{
			DeadAdventurers.Add(CurrentAdventurers[i]);
			FArticleData Data = TerribleNews->GetArticleData();
			Data.AssignCharacterInfo(CharacterInfo);
			TempArticleArray.Add(Data);
		}
		CharacterInfo.Empty();
	}
	TempCustomerArray = CurrentAdventurers;
	CurrentAdventurers.Empty();

	//Displays the news paper and assigns if anything should happen on open/close
	FOnNewspaperEvent OpenNews;
	OpenNews.BindDynamic(this, &UGM_AdventurerInformationComponent::OpenNewsPaper);
	FOnNewspaperEvent CloseNews;
	CloseNews.BindDynamic(this, &UGM_AdventurerInformationComponent::CloseNewsPaper);
	FOnNewspaperEvent ArticleSpawn;
	ArticleSpawn.BindDynamic(this, &UGM_AdventurerInformationComponent::NewArticle);
	GameHUD->DisplayNewspaper(TempArticleArray, OpenNews, CloseNews, ArticleSpawn);
}

void UGM_AdventurerInformationComponent::OpenNewsPaper()
{
	GameHUD->GetNewspaperWidget()->ActivateWidget();
	UE_LOG(LogTemp, Log, TEXT("I'm Opening the news"));
}
void UGM_AdventurerInformationComponent::CloseNewsPaper()
{
	GameHUD->GetNewspaperWidget()->DeactivateWidget();
	UE_LOG(LogTemp, Log, TEXT("I'm Closing the news"));
}

void UGM_AdventurerInformationComponent::NewArticle()
{
	auto Owner = Cast<AGP3GameMode>(GetOwner());
	if(TempCustomerArray[ArticleCounter].SurvivalChance >= 0.9f)
	{
		Owner->PlayArticleSoundGoodNews();
		RewardPlayer();
		UE_LOG(LogTemp, Warning, TEXT("GOOD NEWS!"));
		UE_LOG(LogTemp, Warning, TEXT("Player Rewarded!"));
		UE_LOG(LogTemp, Warning, TEXT("%i"), ArticleCounter);
	}
	else if(TempCustomerArray[ArticleCounter].SurvivalChance >= 0.1f)
	{
		Owner->PlayArticleSoundOkNews();
		UE_LOG(LogTemp, Warning, TEXT("OK NEWS!"));
		UE_LOG(LogTemp, Warning, TEXT("%i"), ArticleCounter);
	}
	else if(TempCustomerArray[ArticleCounter].SurvivalChance < 0.1f)
	{
		Owner->PlayArticleSoundTerribleNews();
		FinePlayer();
		UE_LOG(LogTemp, Warning, TEXT("TERRIBLE NEWS!"));
		UE_LOG(LogTemp, Warning, TEXT("Player Fined!"));
		UE_LOG(LogTemp, Warning, TEXT("%i"), ArticleCounter);
	}
	ArticleCounter++;
}

FString UGM_AdventurerInformationComponent::GetAllDeadNames() const
{
	FString Temp;
	for(int i = 0; i < DeadAdventurers.Num(); i++)
	{
		Temp.Append(DeadAdventurers[i].CustomerName + "\n");
	}
	return Temp;
}