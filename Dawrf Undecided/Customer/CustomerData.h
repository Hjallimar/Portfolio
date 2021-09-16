//Author Hjalmar Andersson
#pragma once

#include "Engine/DataAsset.h"
//#include "UI/Subtitles/CustomerPersonalDialogAsset.h"
#include "Item/Level2WantType.h"
#include "CustomerData.generated.h"

class UBaseItem_DataAsset;
class AActor;
class UMaterialInstance;

USTRUCT(BlueprintType)
struct FCustomerData
{
	GENERATED_USTRUCT_BODY()
public:
	FCustomerData() = default;

	UPROPERTY(EditAnywhere)
		FString CustomerName = "";
	UPROPERTY(EditAnywhere)
		UBaseItem_DataAsset* CustomerItem = nullptr;
	UPROPERTY(EditAnywhere)
		int HatIndex = 0;
	UPROPERTY(EditAnywhere)
		int BeardIndex = 0;
	UPROPERTY(EditAnywhere)
		int NoseIndex = 0;
	UPROPERTY(EditAnywhere)
		int EarIndex = 0;
	UPROPERTY(EditAnywhere)
		int ColorIndex = 0;
	UPROPERTY(EditAnywhere)
		float SurvivalChance = 0.0f;
	UPROPERTY(EditAnywhere)
		FString BoughtWeaponName;
	// <JH>
	//UPROPERTY(EditAnywhere)
	//	UCustomerPersonalDialogAsset* CustomerDialogs = nullptr;
	UPROPERTY(EditAnywhere)
	ELevel2WantType Level2WantType = ELevel2WantType::DamageType;

	UPROPERTY(EditAnywhere)
	int CustomerLevel = 0;
	//</JH>
	
	//FCustomerData(FString Name, AActor* Hat, AActor* Beard, UBaseItem_DataAsset* Item);
};