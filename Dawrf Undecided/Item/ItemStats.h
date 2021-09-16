//Author: Hjalmar Andersson

#pragma once
#include <stdbool.h>
#include "ItemStats.generated.h"

UENUM(BlueprintType)
enum class EWieldType : uint8
{
	OneHanded = 0 UMETA(DisplayName = "One Handed"),
	TwoHanded = 1 UMETA(DisplayName = "Two Handed")
};

UENUM(BlueprintType)
enum class EDamageType : uint8
{
	NotSpecified = 0 UMETA(DisplayName = "Not Specified"),
	Sharp = 1 UMETA(DisplayName = "Sharp"),
	Blunt = 2 UMETA(DisplayName = "Blunt"),
	Magic = 3 UMETA(DisplayName = "Magic"),
	Melody = 4 UMETA(DisplayName = "Melody")
};

UENUM(BlueprintType)
enum class EWeaponType : uint8
{
	NotSpecified = 0 UMETA(DisplayName = "Not Specified"),
	Meele = 1 UMETA(DisplayName = "Meele"),
	ShortRange = 2 UMETA(DisplayName = "Short Range"),
	LongRange = 3 UMETA(DisplayName = "Long Range")
};

UENUM(BlueprintType)
enum class EUtilityType : uint8
{
	NotSpecified = 0 UMETA(DisplayName = "Not Specified"),
	Versatile = 1 UMETA(DisplayName = "Versatile"),
	Pierce = 2 UMETA(DisplayName = "Pierce"),
	Daze = 3 UMETA(DisplayName = "Daze"),
	Rapid = 4 UMETA(DisplayName = "Rapid"),
	Heavy = 5 UMETA(DisplayName = "Heavy"),
	ShieldBreak = 6 UMETA(DisplayName = "Shield Break")
};

UENUM(BlueprintType)
enum class EMonsterTypes : uint8
{
	NotSpecified = 0 UMETA(DisplayName = "Not Specified"),
	Humanoid = 1 UMETA(DisplayName = "Humanoid"),
	Dragon = 2 UMETA(DisplayName = "Dragon"),
	Undead = 3 UMETA(DisplayName = "Undead"),
	Demon = 4 UMETA(DisplayName = "Demon"),
	Automaton = 5 UMETA(DisplayName = "Automaton"),
	Beast = 6 UMETA(DisplayName = "Beast")
};

UENUM(BlueprintType)
enum class EStockType : uint8
{
	NotSpecified = 0 UMETA(DisplayName = "Not Specified"),
	Small = 1 UMETA(DisplayName = "Small"),
	Medium = 2 UMETA(DisplayName = "Medium"),
	Large = 3 UMETA(DisplayName = "Large"),
	Currency = 4 UMETA(DisplayName = "Currency")
};