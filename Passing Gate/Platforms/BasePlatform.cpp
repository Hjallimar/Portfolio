//Author: Hjalmar Andersson

#include "BasePlatform.h"
#include "Components/SceneComponent.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Player/CustomComponents/ResetableComponent.h"

ABasePlatform::ABasePlatform()
{
	PrimaryActorTick.bCanEverTick = true;

	Root = CreateDefaultSubobject<USceneComponent>(TEXT("Root"));
	RootComponent = Root;

	Mesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Base Mesh"));
	Mesh->SetupAttachment(Root);
	Mesh->SetEnableGravity(false);
	Mesh->SetGenerateOverlapEvents(false);

	TriggerZone = CreateDefaultSubobject<UBoxComponent>(TEXT("Trigger Box"));
	TriggerZone->SetupAttachment(Root);
	TriggerZone->OnComponentBeginOverlap.AddDynamic(this, &ABasePlatform::OverlapBegin);
	TriggerZone->OnComponentEndOverlap.AddDynamic(this, &ABasePlatform::OverlapEnd);

	ResetComponent = CreateDefaultSubobject<UResetableComponent>(TEXT("Reset Component"));
}

void ABasePlatform::BeginPlay()
{
	Super::BeginPlay();
	//Register A player ref that can be used in a check if needed
	PlayerRef = GetWorld()->GetFirstPlayerController()->GetPawn();
	ResetComponent->TimeToReset.AddDynamic(this, &ABasePlatform::TimeToReset);
}

void ABasePlatform::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void ABasePlatform::ManualReset()
{
	OnReseted(0);
}

void ABasePlatform::TimeToReset(int i)
{
	if(ResetComponent->MyResetIndex == i && Activated)
	{
		OnReseted(i);
		OnReset();
		OnSpawn();
	}
}

void ABasePlatform::OverlapBegin(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (MovePlayer && OtherActor == PlayerRef)
		OtherActor->AttachToComponent(Root, FAttachmentTransformRules::KeepWorldTransform, NAME_None);
	OnBeginOverlap(OtherActor);
}

void ABasePlatform::OverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex) {
	if (MovePlayer && OtherActor == PlayerRef)
		OtherActor->DetachFromActor(FDetachmentTransformRules::KeepWorldTransform);
	OnEndOverlap(OtherActor);
}

void ABasePlatform::OnReseted(int i) {}
void ABasePlatform::OnBeginOverlap(AActor* OtherActor) {}
void ABasePlatform::OnEndOverlap(AActor* OtherActor) {}