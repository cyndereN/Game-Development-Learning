// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/TimelineComponent.h"
#include "GameFramework/Actor.h"
#include "Test.generated.h"

UCLASS()
class TPS_C_API ATest : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ATest();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	UPROPERTY()
	class UTimelineComponent* abc;
	UPROPERTY(EditAnywhere,BlueprintReadWrite)
	class UCurveFloat* mycure;
	UPROPERTY()
	FOnTimelineFloat tlfloat;
	//
	UFUNCTION()
	void update(float value);

};
