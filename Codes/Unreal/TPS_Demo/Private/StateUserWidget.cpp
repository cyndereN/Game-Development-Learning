// Fill out your copyright notice in the Description page of Project Settings.


#include "StateUserWidget.h"
#include <string>
#include "Components/Image.h"
#include "Components/ProgressBar.h"
#include "Components/TextBlock.h"

void UStateUserWidget::NativeConstruct()
{
	Super::NativeConstruct();
	//UpdateAmmo(45,999);
}

void UStateUserWidget::UpdateHP(float Percent)
{
	HPBar->SetPercent(Percent);
	
}

void UStateUserWidget::UpdateAmmo(int32 WeaponAmmo, int32 PlayerAmmo)
{
FString Ammo_1 = FString::Printf(TEXT("%03d"),WeaponAmmo);
	FString Ammo_2 = FString::Printf(TEXT("%03d"),PlayerAmmo);
	
AmmoText->SetText(FText::FromString("AMMO: "+Ammo_1+" / "+Ammo_2));
	
	
}

