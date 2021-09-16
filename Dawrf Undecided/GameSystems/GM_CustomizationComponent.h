//Author: Hjalmar Andersson

#pragma once
#include "Components/ActorComponent.h"
#include "Containers/Map.h"
#include "GM_CustomizationComponent.generated.h"

class AActor;
//class UCustomerPersonalDialogAsset;
class UMaterialInstance;

UCLASS()
class UGM_CustomizationComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UGM_CustomizationComponent();

	AActor* GetHatAt(int Index);
	AActor* GetShouldersAt(int Index);
	AActor* GetNoseAt(int Index);
	AActor* GetEarsAt(int Index);
	AActor* GetBeardAt(int Index);
	UMaterialInstance* GetColorAt(int Index);
	// <JH>
	//UCustomerPersonalDialogAsset* GetCustomerPersonalDialogAssetAt(int Index, int Level);
	// </JH>

	int GetHatIndex();
	int GetShouldersIndex();
	int GetNoseIndex();
	int GetEarsIndex();
	int GetBeardIndex();
	int GetColorIndex();
	// <JH>
	//int GetCustomerPersonalDialogAssetIndex(int Level);
	// </JH>

	void ActivateActor(AActor* Actor, bool status);
protected:

private:
	UPROPERTY(EditDefaultsOnly)
	TArray<TSubclassOf<AActor>> Hats;
	TMap<int32, AActor*> ObjectPoolHat;

	UPROPERTY(EditDefaultsOnly)
	TArray<TSubclassOf<AActor>> Beards;
	TMap<int32, AActor*> ObjectPoolBeard;

	UPROPERTY(EditDefaultsOnly)
	TArray<TSubclassOf<AActor>> Shoulders;
	TMap<int32, AActor*> ObjectPoolShoulder;

	UPROPERTY(EditDefaultsOnly)
	TArray<TSubclassOf<AActor>> Noses;
	TMap<int32, AActor*> ObjectPoolNose;

	UPROPERTY(EditDefaultsOnly)
	TArray<TSubclassOf<AActor>> Ears;
	TMap<int32, AActor*> ObjectPoolEar;

	UPROPERTY(EditDefaultsOnly)
	TArray<UMaterialInstance*> Colors;

	//UPROPERTY(EditDefaultsOnly)
	//TArray<UCustomerPersonalDialogAsset*> DialogAssets_Level1;

	//UPROPERTY(EditDefaultsOnly)
	//TArray<UCustomerPersonalDialogAsset*> DialogAssets_Level2;

	//UPROPERTY(EditDefaultsOnly)
	//TArray<UCustomerPersonalDialogAsset*> DialogAssets_Level3;
};