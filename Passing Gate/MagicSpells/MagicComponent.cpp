//Author: Hjalmar Andersson

#include "MagicComponent.h"
#include "BaseSpell.h"
#include "Materials/MaterialInstance.h"

UMagicComponent::UMagicComponent()
{
}

void UMagicComponent::BeginPlay()
{
	Super::BeginPlay();
	FirstSpell = GetWorld()->SpawnActor<ABaseSpell>(PrimarySpell);
	SecondSpell = GetWorld()->SpawnActor<ABaseSpell>(SecondarySpell);
}

void UMagicComponent::PrimaryFire(FTransform SpawnTrans)
{
	if (FirstSpell != nullptr)
	{
		FirstSpell->Activate(SpawnTrans);
	}
}

void UMagicComponent::SecondaryFire(FTransform SpawnTrans)
{
	if(SecondSpell != nullptr)
	{
		SecondSpell->Activate(SpawnTrans);
	}
}