//Author Hjalmar Andersson

#pragma once

#include "GameFramework/Actor.h"
#include "GameSystem/DayStateDelegate.h"
#include "PlayerJournal.generated.h"

class UInteractableComponent;
class UCameraComponent;
class UStaticMeshComponent;
class APlayerCharacter;
class UZoomingComponent;

UCLASS()
class APlayerJournal : public AActor
{
	GENERATED_BODY()
public:
	APlayerJournal();
	virtual void Tick(float DeltaTime) override;
	virtual void BeginPlay() override;

	UFUNCTION(BlueprintCallable)
	void OnPlayerInteract(AActor* Player);
	UFUNCTION(BlueprintCallable)
	void OnPlayerStopInteract(AActor* Player);
	UFUNCTION(BlueprintCallable)
	void OnPlayerHover(AActor* Player);
	UFUNCTION(BlueprintCallable)
	void OnPlayerStopHover(AActor* Player);

	UFUNCTION(BlueprintCallable)
	void OnPlayerBreakInteract(AActor* Player);

	UFUNCTION(BlueprintCallable)
	void JumpToChapter(int Index);

	UFUNCTION(BlueprintImplementableEvent)
	void OnPageTurn();
	UFUNCTION(BlueprintImplementableEvent)
	void PlayYawn();

	bool bOrderBought = false;

	UPROPERTY(BlueprintReadWrite)
	bool bInteracted = false;

	UFUNCTION(BlueprintImplementableEvent)
	void TabIndex(int Index);

	UFUNCTION(BlueprintCallable)
	void ChangePage(int Index);

	UFUNCTION(BlueprintImplementableEvent)
	void ActivateOrderPage(int Index);
	UFUNCTION(BlueprintImplementableEvent)
	void UpdatedJournal();
	UFUNCTION(BlueprintImplementableEvent)
	void RefreshOrderPage();

	void AddPage(int Index);

	UFUNCTION()
	void HandleDayState(EDayState State);

	UFUNCTION()
	void UpdateWeaponIndex(int Pages);

protected:
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	UInteractableComponent* InteractComponent;

	UPROPERTY(EditAnywhere, Category = "Chapters")
	UMaterialInstance* StartPage;

	UPROPERTY(EditAnywhere, Category = "Chapters")
	TArray<UMaterialInstance*> WeaponCompendium;

	UPROPERTY(EditAnywhere, Category = "Chapters")
	TArray<UMaterialInstance*> BeastCompendium;

	UPROPERTY(EditAnywhere, Category = "Chapters")
	UMaterialInstance* UtilityPage;

	UPROPERTY(EditAnywhere, Category = "Chapters")
	TArray<UMaterialInstance*> OrderPages;

	TArray<UMaterialInstance*> AllPages;

	UPROPERTY(EditAnywhere)
	USceneComponent* Root;

	UPROPERTY(EditAnywhere)
	USceneComponent* ReadingView;
	
	UPROPERTY(EditAnywhere)
	UStaticMeshComponent* BookMesh;
	UPROPERTY(EditAnywhere)
	UStaticMeshComponent* OutLine;
	UPROPERTY(EditAnywhere)
	UZoomingComponent* ZoomingComp;
private:

	void UpdateJournal();
	void CheckIfMomsNote();

	// <JH>
	UFUNCTION()
	void PreviousPage();
	UFUNCTION()
	void NextPage();
	// </JH>

	UPROPERTY(EditAnywhere)
	int MonsterIndex = 2;
	UPROPERTY(EditAnywhere)
	int WeaponIndex = 2;
	int OrderIndex = 2;

	bool bPlayedSound = false;
	int CurrentIndex = 0;
	int WorkIndex = 0;
	bool AfterWork = true;
};