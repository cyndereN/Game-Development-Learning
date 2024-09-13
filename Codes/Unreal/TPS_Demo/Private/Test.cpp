// Fill out your copyright notice in the Description page of Project Settings.


#include "Test.h"
#include "Components/TimelineComponent.h"
FVector starlocation;
FVector endlocation;
// Sets default values
ATest::ATest()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	abc = CreateDefaultSubobject<UTimelineComponent>("abc");
	tlfloat.BindUFunction(this,"update");
	

}

// Called when the game starts or when spawned
void ATest::BeginPlay()
{
	Super::BeginPlay();
	abc->AddInterpFloat(mycure,tlfloat,"abc");
	abc->Play();
	
}

// Called every frame
void ATest::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void ATest::update(float value)
{
	SetActorLocation(FMath::Lerp(starlocation,endlocation,value));
	//GEngine->AddOnScreenDebugMessage(1,2.f,FColor::Red,FString::Printf(TEXT("111")));

}

