//Author: Hjalmar Andersson

#pragma once

#include "Components/ActorComponent.h"
#include "TeleportableComponent.generated.h"

UCLASS(meta = (BlueprintSpawnableComponent))
class UTeleportableComponent : public UActorComponent
{
	GENERATED_BODY()
public:
	UTeleportableComponent();
};
