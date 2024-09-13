// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "StateUserWidget.generated.h"

/**
 * 
 */
UCLASS()
class TPS_C_API UStateUserWidget : public UUserWidget
{
	GENERATED_BODY()

public:
	virtual  void NativeConstruct() override;
	UFUNCTION()
	void UpdateHP(float Percent);

	UFUNCTION()
	void UpdateAmmo(int32 WeaponAmmo,int32 PlayerAmmo);

private:
	UPROPERTY(Transient,meta=(BindWidget))
	class UProgressBar* HPBar;
	
	UPROPERTY(Transient,meta=(BindWidget))
	class UTextBlock* AmmoText;

	
	
};
