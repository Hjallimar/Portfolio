//Author: Hjalmar Andersson

#include "AbilityStats.h"

FString UAbilityStats::GetName()
{
	return Name;
}

ElementType UAbilityStats::GetElement()
{
	return Element;
}

float UAbilityStats::GetDamage()
{
	return Damage;
}

int UAbilityStats::GetTier()
{
	return Tier;
}