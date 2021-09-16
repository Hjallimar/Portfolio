//Author: Hjalmar Andersson

#pragma once
#include <stdbool.h>
#include "DayStateDelegate.generated.h"

UENUM(BlueprintType)
enum class EDayState : uint8
{
	StartDay = 0 UMETA(DisplayName = "Start Day"),
	StartWorkDay = 1 UMETA(DisplayName = "Start Work Day"),
	EndWorkDay = 2 UMETA(DisplayName = "End Work Day"),
	EndDay = 3 UMETA(DisplayName = "End Day"),
	AfterNewspaper = 4 UMETA(DisplayName = "After Newspaper"),
	OrderDelivery = 5 UMETA(DisplayName = "Oder Delivery")
};

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FDayStateEvent, EDayState, Decision);
