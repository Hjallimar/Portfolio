// Author Johan Liljedahl
// Author Hjalmar Andersson
// Co-Author: Justus Hörberg
#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "ItemStats.h"
#include "Item/Level2WantType.h"
#include "BaseItem_DataAsset.generated.h"

class UStaticMeshComponent;

UCLASS()
class COOLGP3PROJECT_API UBaseItem_DataAsset : public UDataAsset
{
	GENERATED_BODY()
public:

	UPROPERTY(EditAnywhere, Category = ItemProperties) 
	FString Name;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	EWieldType WieldType;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	EWeaponType WeponType;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	EDamageType DamageType;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	EUtilityType UtilityType;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	TArray<EMonsterTypes> StrongAgainst;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	TArray<EMonsterTypes> WorksAgainst;

	UPROPERTY(EditAnywhere, Category = ItemProperties) 
	int Value = 10;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	EStockType StockType;
	
	UPROPERTY(EditAnywhere, Category = ItemProperties)
	bool CanHangOnWalls = false;
	
	UPROPERTY(EditAnywhere, Category = ItemProperties) 
	UStaticMeshComponent* Mesh;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	USoundWave* PickupSound;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	USoundWave* PlacedownSound;

	float CompareTo(UBaseItem_DataAsset* Other, int CustomerLevel, ELevel2WantType Level2Type, UWorld* World = nullptr);

	// <JH>
protected:
	float CompareTo_Level1(UBaseItem_DataAsset* Other);
	float CompareTo_Level2(UBaseItem_DataAsset* Other, ELevel2WantType Level2Type);
	float CompareTo_Level3(UBaseItem_DataAsset* Other);
};
