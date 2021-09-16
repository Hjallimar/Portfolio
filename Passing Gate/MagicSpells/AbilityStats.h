//Author: Hjalmar Andersson

#pragma once
#include <stdbool.h>
#include "Engine/DataAsset.h"
#include "AbilityStats.generated.h"

UENUM(BlueprintType)
enum class ElementType : uint8
{
	NATURAL = 0 UMETA(DisplayName = "Natural"),
	FIRE = 1 UMETA(DisplayName = "Fire"),
	ICE = 2 UMETA(DisplayName = "Ice"),
	VOID = 3 UMETA(DisplayName = "Void")
};

UCLASS()
class UAbilityStats : public UDataAsset
{
	GENERATED_BODY()
public:
	UFUNCTION(BlueprintCallable)
	FString GetName();
	
	UFUNCTION(BlueprintCallable)
	ElementType GetElement();
	
	UFUNCTION(BlueprintCallable)
	float GetDamage();
	
	UFUNCTION(BlueprintCallable)
	int GetTier();

	UPROPERTY(EditAnywhere)
	bool bPlayerProjectile = true;
private:
	UPROPERTY(EditAnywhere, Category = ItemProperties)
	FString Name;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	ElementType Element;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	float Damage;

	UPROPERTY(EditAnywhere, Category = ItemProperties)
	int Tier;

};
