//Author: Hjalmar Andersson

#pragma once

#include "Components/ActorComponent.h"
#include "HealthComponent.generated.h"

class UAbilityStats;

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FTakeDamageEvent, UAbilityStats*, AbilityStats);
//DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FTakeDamageEvent, float, Damage);

UCLASS(Meta = (BlueprintSpawnableComponent))
class UHealthComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UHealthComponent();

	UPROPERTY(BlueprintAssignable)
	FTakeDamageEvent OnTakeDamage;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Health;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	float MaxHealth;
};