//Author Hjalmar Andersson

#include "GM_CustomizationComponent.h"
#include "Materials/MaterialInstance.h"
#include "GameFramework/Actor.h"
//#include "UI/Subtitles/CustomerPersonalDialogAsset.h"


UGM_CustomizationComponent::UGM_CustomizationComponent()
{

}

AActor* UGM_CustomizationComponent::GetHatAt(int Index)
{
	if (Hats.Num() < 1)
		return nullptr;

	AActor* Actor = nullptr;
	if (ObjectPoolHat.Contains(Index))
	{
		Actor = ObjectPoolHat[Index];
	}
	else if(Hats[Index])
	{
		FTransform SpawnTrans = GetOwner()->GetActorTransform();
		AActor* Hat = GetWorld()->SpawnActor<AActor>(Hats[Index], SpawnTrans);
		ObjectPoolHat.Add(Index, Hat);
		Actor = Hat;
	}
	return Actor;
}
	
AActor* UGM_CustomizationComponent::GetShouldersAt(int Index)
{
	if (Shoulders.Num() < 1)
		return nullptr;
	AActor* Actor = nullptr;
	if (ObjectPoolShoulder.Contains(Index))
	{
		Actor = ObjectPoolShoulder[Index];
	}
	else if(Shoulders[Index])
	{
		FTransform SpawnTrans = GetOwner()->GetActorTransform();
		AActor* Shoulder = GetWorld()->SpawnActor<AActor>(Shoulders[Index], SpawnTrans);
		ObjectPoolShoulder.Add(Index, Shoulder);
		Actor = Shoulder;
	}
	return Actor;
}

AActor* UGM_CustomizationComponent::GetNoseAt(int Index)
{
	if (Noses.Num() < 1)
		return nullptr;
	AActor* Actor = nullptr;
	if (ObjectPoolNose.Contains(Index))
	{
		Actor = ObjectPoolNose[Index];
	}
	else if(Noses[Index])
	{
		FTransform SpawnTrans = GetOwner()->GetActorTransform();
		AActor* Nose = GetWorld()->SpawnActor<AActor>(Noses[Index], SpawnTrans);
		ObjectPoolNose.Add(Index, Nose);
		Actor = Nose;
	}
	return Actor;
}

AActor* UGM_CustomizationComponent::GetEarsAt(int Index)
{
	if (Ears.Num() < 1)
		return nullptr;
	AActor* Actor = nullptr;
	if (ObjectPoolEar.Contains(Index))
	{
		Actor = ObjectPoolEar[Index];
	}
	else if (Ears[Index])
	{
		FTransform SpawnTrans = GetOwner()->GetActorTransform();
		AActor* Ear = GetWorld()->SpawnActor<AActor>(Ears[Index], SpawnTrans);
		ObjectPoolEar.Add(Index, Ear);
		Actor = Ear;
	}
	return Actor;
}

AActor* UGM_CustomizationComponent::GetBeardAt(int Index)
{
	if (Beards.Num() < 1)
		return nullptr;

	AActor* Actor = nullptr;
	if (ObjectPoolBeard.Contains(Index))
	{
		Actor = ObjectPoolBeard[Index];
	}
	else if (Beards[Index])
	{
		FTransform SpawnTrans = GetOwner()->GetActorTransform();
		AActor* Beard = GetWorld()->SpawnActor<AActor>(Beards[Index], SpawnTrans);
		ObjectPoolBeard.Add(Index, Beard);
		Actor = Beard;
	}
	return Actor;
}

UMaterialInstance* UGM_CustomizationComponent::GetColorAt(int Index)
{
	if (Colors.Num() < 1)
		return nullptr;

	if (Colors[Index])
	{
		 UMaterialInstance* Color = Colors[Index];
		 return Color;
	}
	return nullptr;
}

//UCustomerPersonalDialogAsset* UGM_CustomizationComponent::GetCustomerPersonalDialogAssetAt(int Index, int Level)
//{
//	TArray<UCustomerPersonalDialogAsset*> AssetArray;
//
//	switch (Level)
//	{
//	default:
//	case 0:
//		AssetArray = DialogAssets_Level1;
//		break;
//
//	case 1:
//		AssetArray = DialogAssets_Level2;
//		break;
//
//	case 2:
//		AssetArray = DialogAssets_Level3;
//		break;
//	}
//
//	if (AssetArray.Num() < 1)
//		return nullptr;
//
//	if (AssetArray[Index])
//	{
//		UCustomerPersonalDialogAsset* Dialog = AssetArray[Index];
//		return Dialog;
//	}
//	return nullptr;
//}

int UGM_CustomizationComponent::GetHatIndex()
{
	int Rand = FMath::RandRange(0, Hats.Num() - 1);
	return Rand;
}
int UGM_CustomizationComponent::GetShouldersIndex() 
{
	int Rand = FMath::RandRange(0, Shoulders.Num() - 1);
	return Rand;
}
int UGM_CustomizationComponent::GetBeardIndex()
{
	int Rand = FMath::RandRange(0, Beards.Num() - 1);
	return Rand;
}
int UGM_CustomizationComponent::GetNoseIndex()
{
	int Rand = FMath::RandRange(0, Noses.Num() - 1);
	return Rand;
}
int UGM_CustomizationComponent::GetEarsIndex()
{
	int Rand = FMath::RandRange(0, Ears.Num() - 1);
	return Rand;
}
int UGM_CustomizationComponent::GetColorIndex()
{
	int Rand = FMath::RandRange(0, Colors.Num() - 1);
	return Rand;
}

//int UGM_CustomizationComponent::GetCustomerPersonalDialogAssetIndex(int Level)
//{
//	TArray<UCustomerPersonalDialogAsset*> AssetArray;
//
//	switch (Level)
//	{
//	default:
//	case 0:
//		AssetArray = DialogAssets_Level1;
//		break;
//
//	case 1:
//		AssetArray = DialogAssets_Level2;
//		break;
//
//	case 2:
//		AssetArray = DialogAssets_Level3;
//		break;
//	}
//
//	int Rand = FMath::RandRange(0, AssetArray.Num() - 1);
//	return Rand;
//}

void UGM_CustomizationComponent::ActivateActor(AActor* Actor, bool Status)
{
	if (Actor == nullptr)
		return;
	Actor->SetActorHiddenInGame(!Status);
}