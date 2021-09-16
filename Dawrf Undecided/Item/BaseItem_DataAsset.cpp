// Author Johan Liljedahl
// Author Hjalmar Andersson
#include "Item/BaseItem_DataAsset.h"
#include "GameSystem/GP_GameInstance.h"
#include "Kismet/GamePlayStatics.h"

float UBaseItem_DataAsset::CompareTo(UBaseItem_DataAsset* Other, int CustomerLevel, ELevel2WantType Level2Type, UWorld* World)
{
	if (World && World->IsGameWorld())
	{
		auto* Inst = World->GetGameInstance<UGP_GameInstance>();
		//UE_LOG(LogTemp, Log, TEXT("instant noodles"));
		if (Inst->bHardMode)
		{
			if (Other->Name == Name)
			{
				return 1;
			}
			return 0;
		}
	}

	float Result;
	switch (CustomerLevel)
	{
	case 0:
		Result = CompareTo_Level1(Other);
		break;
		
	case 1:
		Result = CompareTo_Level2(Other, Level2Type);
		break;

	default:
	case 2:
		Result = CompareTo_Level3(Other);
		break;
	}

	return Result;
}

// <JH>
float UBaseItem_DataAsset::CompareTo_Level1(UBaseItem_DataAsset* Other)
{
	float Correct = 0.0f;
	float NotSpecified = 0.0f;
	float DivideValue = 0;

	// For level 1, we just need to find one enemy type that matches.
	for (int i = 0; i < StrongAgainst.Num(); i++)
	{
		for (int j = 0; j < Other->StrongAgainst.Num(); j++)
		{
			// If we find one that matches, rate is 100%.
			if (StrongAgainst[0] == Other->StrongAgainst[j])
			{
				Correct = 1;
				DivideValue = 1;
				break;
			}
		}

		// If we failed to find one strong against, test works against instead.
		if (Correct == 0)
		{
			for (int j = 0; j < Other->WorksAgainst.Num(); j++)
			{
				if (StrongAgainst[i] == Other->WorksAgainst[j])
				{
					Correct = 0.5f;
					DivideValue = 1;
					break;
				}
			}
		}
	}

	float Divider = (DivideValue - NotSpecified);
	if (Divider != 0)
	{
		float Result = Correct / Divider;

		UE_LOG(LogTemp, Log, TEXT("Level 1 Comparison rate: %f"), Result);

		return Result;
	}

	return 0;
}

float UBaseItem_DataAsset::CompareTo_Level2(UBaseItem_DataAsset* Other, ELevel2WantType Level2Type)
{
	float Correct = 0.0f;
	float NotSpecified = 0.0f;
	float DivideValue = 2;

	//if (WieldType == Other->WieldType)
	//{
	//	UE_LOG(LogTemp, Log, TEXT("WieldType is correct."));
	//	Correct++;
	//}
	//else
	//{
	//	UE_LOG(LogTemp, Log, TEXT("WieldType is NOT correct."));
	//}
		

	// In contrast to Level 3 customers who want both weapon type AND damage
	// type right, Level 2s may only want one of these correct.
	if (Level2Type == ELevel2WantType::DamageType)
	{
		if (DamageType == EDamageType::NotSpecified)
		{
			NotSpecified++;
		}
		else if (DamageType == Other->DamageType)
		{
			Correct++;
			UE_LOG(LogTemp, Log, TEXT("DamageType is correct."));
		}
		else
		{
			UE_LOG(LogTemp, Log, TEXT("DamageType is NOT correct: %i != %i"), (int)DamageType, (int)Other->DamageType);
		}
	}
	else
	{
		if (WeponType == EWeaponType::NotSpecified)
		{
			NotSpecified++;
		}
		else if (WeponType == Other->WeponType)
		{
			Correct++;
			UE_LOG(LogTemp, Log, TEXT("WeaponType is correct."));
		}
		else
		{
			UE_LOG(LogTemp, Log, TEXT("WeaponType is NOT correct: %i != %i"), (int)WeponType, (int)Other->WeponType);//
		}
	}

	if (UtilityType == EUtilityType::NotSpecified)
	{
		NotSpecified++;
	}
	else if (UtilityType == Other->UtilityType)
	{
		Correct++;
		UE_LOG(LogTemp, Log, TEXT("UtilityType is correct."));
	}
	else
	{
		UE_LOG(LogTemp, Log, TEXT("UtilityType is NOT correct: %i != %i"), (int)UtilityType, (int)Other->UtilityType);
	}

	for (int i = 0; i < StrongAgainst.Num(); i++)
	{
		for (int j = 0; j < Other->StrongAgainst.Num(); j++)
		{
			if (StrongAgainst[i] == Other->StrongAgainst[j])
			{
				Correct++;
			}
				
		}
		DivideValue++;
	}

	for (int i = 0; i < StrongAgainst.Num(); i++)
	{
		bool bIncreaseDivideValue = false;
		for (int j = 0; j < Other->WorksAgainst.Num(); j++)
		{
			UE_LOG(LogTemp, Log, TEXT("%i"), (int)Other->WorksAgainst[j]);
			if (StrongAgainst[i] == Other->WorksAgainst[j])
			{
				Correct += 0.5f;
				bIncreaseDivideValue = true;
			}
		}

		if (bIncreaseDivideValue)
		{
			DivideValue++;
		}
	}

	float Divider = (DivideValue - NotSpecified);
	if (Divider != 0)
	{
		float Result = Correct / Divider;
		UE_LOG(LogTemp, Log, TEXT("Correct = %f"), Correct);
		UE_LOG(LogTemp, Log, TEXT("Divider = %f"), Divider);

		UE_LOG(LogTemp, Log, TEXT("Level 2 Comparison rate: %f"), Result);
		UE_LOG(LogTemp, Log, TEXT("Customer was looking for a: %s, you gave them a: %s"), *Other->Name, *Name);//
		//UtilityType
		return Result;
	}

	return 0;
}

float UBaseItem_DataAsset::CompareTo_Level3(UBaseItem_DataAsset* Other)
{
	float Correct = 0.0f;
	float NotSpecified = 0.0f;
	float DivideValue = 3;

	//if (WieldType == Other->WieldType)
	//	Correct++;

	if (WeponType == EWeaponType::NotSpecified)
	{
		NotSpecified++;
	}
	else if (WeponType == Other->WeponType)
	{
		Correct++;
	}

	if (DamageType == EDamageType::NotSpecified)
	{
		NotSpecified++;
	}
	else if (DamageType == Other->DamageType)
	{
		Correct++;
	}

	if (UtilityType == EUtilityType::NotSpecified)
	{
		NotSpecified++;
	}
	else if (UtilityType == Other->UtilityType)
	{
		Correct++;
	}

	for (int i = 0; i < StrongAgainst.Num(); i++)
	{
		for (int j = 0; j < Other->StrongAgainst.Num(); j++)
		{
			if (StrongAgainst[i] == Other->StrongAgainst[j])
				Correct++;
		}
		DivideValue++;
	}

	for (int i = 0; i < StrongAgainst.Num(); i++)
	{
		bool bIncreaseDivideValue = false;
		for (int j = 0; j < Other->WorksAgainst.Num(); j++)
		{
			if (StrongAgainst[i] == Other->WorksAgainst[j])
			{
				Correct += 0.5f;
				bIncreaseDivideValue = true;
			}
				
		}

		if (bIncreaseDivideValue)
			DivideValue++;
	}

	float Divider = (DivideValue - NotSpecified);
	if (Divider != 0)
	{
		float Result = Correct / Divider;

		UE_LOG(LogTemp, Log, TEXT("Level 3 Comparison rate: %f"), Result);

		return Result;
	}

	return 0;
}
// </JH>