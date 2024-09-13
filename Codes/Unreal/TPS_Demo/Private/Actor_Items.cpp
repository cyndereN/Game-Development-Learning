// Fill out your copyright notice in the Description page of Project Settings.


#include "Actor_Items.h"

#include "Components/BoxComponent.h"
#include "Components/SphereComponent.h"
#include "Components/WidgetComponent.h"
#include "BasePlayer.h"
#include "ItemsUserWidget.h"


// Sets default values
AActor_Items::AActor_Items()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
	
	
	SkeletalMesh = CreateDefaultSubobject<USkeletalMeshComponent>("SkeletalMesh");
	SetRootComponent(SkeletalMesh);

	ItemsPickUpWidget = CreateDefaultSubobject<UWidgetComponent>("ItemsPickUpWidget");
	ItemsPickUpWidget->SetupAttachment(SkeletalMesh);
	
	Box = CreateDefaultSubobject<UBoxComponent>("Box");
	Box->SetupAttachment(SkeletalMesh);

	//Box->SetCollisionResponseToAllChannels(ECR_Ignore);
	//Box->SetCollisionResponseToChannel(ECC_Visibility,ECR_Block);

	Sphere = CreateDefaultSubobject<USphereComponent>("Sphere");
	Sphere->SetupAttachment(SkeletalMesh);
	Sphere->SetSphereRadius(150.f);
	//Sphere->SetCollisionResponseToAllChannels(ECR_Ignore);
	//Sphere->SetCollisionResponseToChannel(ECC_Pawn,ECR_Overlap);

	WeaponsName = "DefaultName";
	SetItemsEnum(E_Items::Enum_Pickup);

	//E_Item  = E_Items::Enum_Pickup;

	
	
	
	
	
	
	
}

// Called when the game starts or when spawned
void AActor_Items::BeginPlay()
{
	Super::BeginPlay();
	SetWidgetVisibility(false);

	Sphere->OnComponentBeginOverlap.AddDynamic(this,&AActor_Items::Beginoverlap);
	Sphere->OnComponentEndOverlap.AddDynamic(this,&AActor_Items::Endoverlap);

	ItemsWidget = Cast<UItemsUserWidget>(ItemsPickUpWidget->GetWidget());
	if(ItemsWidget)
	{
		ItemsWidget->SetWeaponName(WeaponsName);
		
	}
	
	
	
}

// Called every frame
void AActor_Items::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

//设置交互UI可视性
void AActor_Items::SetWidgetVisibility(bool Value)
{
	ItemsPickUpWidget->SetVisibility(Value);
}



void AActor_Items::SetItemsEnum(E_Items SetItems)
{
	E_Item = SetItems;
	switch (E_Item)
	{
	case E_Items::Enum_Pickup:
		SkeletalMesh->SetCollisionResponseToAllChannels(ECR_Ignore);
		SkeletalMesh->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
		SkeletalMesh->SetCollisionResponseToChannel(ECC_WorldStatic,ECR_Block);
		SkeletalMesh->SetSimulatePhysics(true);
		SkeletalMesh->SetEnableGravity(true);
		Box->SetCollisionResponseToAllChannels(ECR_Ignore);
		Box->SetCollisionResponseToChannel(ECC_Visibility,ECR_Block);
		Sphere->SetCollisionResponseToAllChannels(ECR_Ignore);
		Sphere->SetCollisionResponseToChannel(ECC_Pawn,ECR_Overlap);
break;
	case E_Items::Enum_Equip:
		SkeletalMesh->SetSimulatePhysics(false);
		SkeletalMesh->SetEnableGravity(false);
		SkeletalMesh->SetCollisionResponseToAllChannels(ECR_Ignore);
		SkeletalMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
		Box->SetCollisionResponseToAllChannels(ECR_Ignore);
		Box->SetCollisionEnabled(ECollisionEnabled::NoCollision);
		Sphere->SetCollisionResponseToAllChannels(ECR_Ignore);
		Sphere->SetCollisionEnabled(ECollisionEnabled::NoCollision);

		break;
	case E_Items::Enum_Drop:
		SkeletalMesh->SetCollisionResponseToAllChannels(ECR_Ignore);
		SkeletalMesh->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
		SkeletalMesh->SetCollisionResponseToChannel(ECC_WorldStatic,ECR_Block);
		SkeletalMesh->SetSimulatePhysics(true);
		SkeletalMesh->SetEnableGravity(true);
		Box->SetCollisionResponseToAllChannels(ECR_Ignore);
		Box->SetCollisionResponseToChannel(ECC_Visibility,ECR_Block);
		Box->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
		Sphere->SetCollisionResponseToAllChannels(ECR_Ignore);
		Sphere->SetCollisionResponseToChannel(ECC_Pawn,ECR_Overlap);
		Sphere->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
		break;
	}
}

//给物品增加冲击力
void AActor_Items::AddForce(FVector force)
{

	
	GetItemsMesh()->AddImpulse(force);
}




//开始重叠
void AActor_Items::Beginoverlap(UPrimitiveComponent* OverlapComp, AActor* OtherActor, UPrimitiveComponent* OtherComp,
	int OtherBodyIndex, bool FromSweep, const FHitResult& SweepResult)
{
	if(OtherActor)
	{
		if(OtherActor->ActorHasTag("Player"))
		{
			ABasePlayer* ActorPlayer = Cast<ABasePlayer>(OtherActor);
            		if(ActorPlayer)
            		{
            			ActorPlayer->UpdateCheckItemsCount(1);
            		}
		}
		
	}
}

//结束重叠
void AActor_Items::Endoverlap(UPrimitiveComponent* OverlapComp, AActor* OtherActor, UPrimitiveComponent* OtherComp,
	int OtherBodyIndex)
{
	if(OtherActor)
	{
		ABasePlayer* ActorPlayer = Cast<ABasePlayer>(OtherActor);
		if(ActorPlayer)
		{
			ActorPlayer->UpdateCheckItemsCount(-1);
		}
	}
	
}






