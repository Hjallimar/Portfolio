//Author Hjalmar Andersson

#pragma once
#include "Components/ActorComponent.h"
#include "DayStateDelegate.h"
#include "CustomerAI/CustomerData.h"
#include "GM_AdventurerInformationComponent.generated.h"

class AActor;
class UBaseItem_DataAsset;
class UNewsArticle_DataAsset;
class UCustomerPersonalDialogAsset;
class AGameHUD;

UCLASS()
class UGM_AdventurerInformationComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	bool DoesCustomerExist();
	FCustomerData* GetRandomCustomer();
	void AssignNewCustomer(FString Name, int Hat, int Beard, UBaseItem_DataAsset* Item, int Nose, int Ears, int Color, int Level);
	void AssignSurvival(float Survival, FString BoughtItemName = "");
	void CreateNewspapperNews();
	UFUNCTION()
	void HandleDayState(EDayState State);
	UFUNCTION(BlueprintCallable)
	void FinePlayer();
	UFUNCTION(BlueprintCallable)
    void RewardPlayer();
	UFUNCTION(BlueprintCallable)
	FString GetAllDeadNames() const;

protected:
	UFUNCTION()
	void OpenNewsPaper();
	UFUNCTION()
	void CloseNewsPaper();
	UFUNCTION()
	void NewArticle();
	UPROPERTY(EditAnywhere, Category = "Good News")
	UNewsArticle_DataAsset* GoodNews;
	UPROPERTY(EditAnywhere, Category = "Ok News")
	UNewsArticle_DataAsset* OkNews;
	UPROPERTY(EditAnywhere, Category = "Fail News")
	UNewsArticle_DataAsset* TerribleNews;
	UPROPERTY(EditAnywhere, Category = "No News")
	UNewsArticle_DataAsset* NoNews;

	UPROPERTY(EditAnywhere)
	int RewardAmount = 5;
	UPROPERTY(EditAnywhere)
	int FineAmount = 5;
	AGameHUD* GameHUD;
	int ArticleCounter = 0;


	virtual void BeginPlay() override;
	TArray<FCustomerData> ExistingCustomers;
	TArray<FCustomerData> DeadAdventurers;
	TArray<FCustomerData> CurrentAdventurers;
	TArray<FCustomerData> TempCustomerArray;
	FCustomerData* CurrentCustomerData;
};
