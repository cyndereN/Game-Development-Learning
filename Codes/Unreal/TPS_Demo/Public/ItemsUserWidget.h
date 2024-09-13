// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "ItemsUserWidget.generated.h"

/**
 * 
 */




UCLASS()
class TPS_C_API UItemsUserWidget : public UUserWidget
{
	GENERATED_BODY()

private:
	UPROPERTY(Transient,meta=(BindWidget))
	class UTextBlock* ItemsName;
	UPROPERTY(Transient,meta=(BindWidget))
	class UTextBlock* WeaponAmmos;

	
	
public:
	UFUNCTION()
	void SetWeaponName(FString Name);

	UFUNCTION()
	void SetWeaponAmmo(int32 Ammo);
	
	
};
