#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "GP3GameMode.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnGoldChangedDelegate, int, OldGoldValue, int, NewGoldValue);

class UGM_BargainComponent;
class UGM_CustomizationComponent;
class UGM_CustomerController;
class UGM_StockComponent;
class UGM_AdventurerInformationComponent;
class UGM_DayCycleComponent;

UCLASS(minimalapi)
class AGP3GameMode : public AGameModeBase
{
	GENERATED_BODY()

public:
	AGP3GameMode();

	UFUNCTION(BlueprintCallable)
	UGM_BargainComponent* GetBargainComponent();
	UGM_CustomizationComponent* GetCustomizeComponent();
	UGM_CustomerController* GetCustomerController();
	UFUNCTION(BlueprintCallable)
	UGM_AdventurerInformationComponent* GetAdventurerInformation();
	UGM_StockComponent* GetStockComponent();
	UGM_DayCycleComponent* GetDayCycleComponent();
	UFUNCTION(BlueprintImplementableEvent)
	void GameOver();
	UFUNCTION(BlueprintImplementableEvent)
	void PlayArticleSoundGoodNews();
	UFUNCTION(BlueprintImplementableEvent)
    void PlayArticleSoundOkNews();
	UFUNCTION(BlueprintImplementableEvent)
    void PlayArticleSoundBadNews();
	UFUNCTION(BlueprintImplementableEvent)
    void PlayArticleSoundTerribleNews();
	
	/* Delegate that triggers when the player's gold value has changed. */
	FOnGoldChangedDelegate OnGoldChangedDelegate;

	/* The current amount of gold in the game. */
	UPROPERTY(EditAnywhere)
	int Gold = 10;
	UPROPERTY(EditAnywhere)
	int TotalGold = 10;
	UPROPERTY(EditAnywhere)
	int FinedGold = 0;
protected:
	UPROPERTY(EditDefaultsOnly)
	UGM_BargainComponent* Bargain;

	UPROPERTY(EditDefaultsOnly)
	UGM_CustomizationComponent* Customize;

	UPROPERTY(EditDefaultsOnly)
	UGM_CustomerController* CustomerController;

	UPROPERTY(EditDefaultsOnly)
	UGM_AdventurerInformationComponent* AdventurerInfo;

	UPROPERTY(EditDefaultsOnly)
	UGM_StockComponent* Stock;

	UPROPERTY(EditDefaultsOnly)
	UGM_DayCycleComponent* DayCycle;

	/* The amount of gold the player has gained/lost today. */
	int GoldChangeToday;

	/* The maximum amount of gold the player can carry at once. */
	UPROPERTY(EditDefaultsOnly)
	int MaxGold = 99999999;

public:
	/* Get the current amount of gold carried by the player. */
	UFUNCTION(BlueprintCallable)
	int GetGold() const;
	UFUNCTION(BlueprintCallable)
	int GetTotalGold() const;
	UFUNCTION(BlueprintCallable)
	int GetFinedGold() const;

	/* Get the amount of gold the player has gained/lost today. */
	int GetTodayGold() const { return GoldChangeToday; }

	void SetGold(int NewGoldValue);

	void AddGold(int AddGoldValue);

	void AddFine(int Value);

	UFUNCTION(Exec)
	void Console_SetDay(int32 arg1);

	UFUNCTION(Exec)
	void Console_SetGold(int32 arg1);

	UFUNCTION(Exec)
	void Console_SetCustomersLeftCount(int32 arg1);
};
