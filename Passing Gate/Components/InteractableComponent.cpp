//Author Hjalmar Andersson

#include "InteractableComponent.h"
#include "Components/SphereComponent.h"
#include "Components/InputComponent.h"

UInteractableComponent::UInteractableComponent()
{

}

void UInteractableComponent::BeginPlay()
{
	Super::BeginPlay();
	if(CollisionSphere == nullptr)
	{
		CollisionSphere = Cast<USphereComponent>(GetOwner()->FindComponentByClass<USphereComponent>());
		if(CollisionSphere == nullptr)
		{
			UE_LOG(LogTemp, Warning, TEXT("Interactable Object: %s Doesn't have a sphere collider"), *GetName());
			return;
		}
	}

	PlayerRef = GetWorld()->GetFirstPlayerController()->GetPawn();
	if (PlayerRef == nullptr)
		return;
	
	UInputComponent* InputComp = Cast<UInputComponent>(PlayerRef->FindComponentByClass<UInputComponent>());
	if (InputComp == nullptr)
		return;

	InputComp->BindAction("Interact", IE_Pressed, this, &UInteractableComponent::Interact);
	CollisionSphere->OnComponentBeginOverlap.AddDynamic(this, &UInteractableComponent::OverlapBegin);
	CollisionSphere->OnComponentEndOverlap.AddDynamic(this, &UInteractableComponent::OverlapEnd);
}

void UInteractableComponent::OverlapBegin(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if(OtherActor == PlayerRef)
	{
		Interactable = true;
		UE_LOG(LogTemp, Log, TEXT("Player has Entered"));
	}
}

void UInteractableComponent::OverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex)
{
	if (OtherActor == PlayerRef)
	{
		Interactable = false;
		UE_LOG(LogTemp, Log, TEXT("Player Has Left"));
	}
}

void UInteractableComponent::DeactivateInteraction()
{
	CollisionSphere->SetGenerateOverlapEvents(false);
	Interactable = false;
}

void UInteractableComponent::Interact()
{
	if (Interactable)
		OnInteracted.Broadcast();
	else
	{
		UE_LOG(LogTemp, Log, TEXT("Player not close enough to interact"));
	}
}

void UInteractableComponent::AssignNewSphere(USphereComponent* Sphere)
{
	if(CollisionSphere != nullptr)
	{
		CollisionSphere->OnComponentBeginOverlap.RemoveDynamic(this, &UInteractableComponent::OverlapBegin);
		CollisionSphere->OnComponentEndOverlap.RemoveDynamic(this, &UInteractableComponent::OverlapEnd);
	}
	CollisionSphere = Sphere;

	CollisionSphere->OnComponentBeginOverlap.AddDynamic(this, &UInteractableComponent::OverlapBegin);
	CollisionSphere->OnComponentEndOverlap.AddDynamic(this, &UInteractableComponent::OverlapEnd);
}