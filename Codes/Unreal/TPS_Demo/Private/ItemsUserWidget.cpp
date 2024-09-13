// Fill out your copyright notice in the Description page of Project Settings.


#include "ItemsUserWidget.h"

#include "Components/TextBlock.h"

void UItemsUserWidget::SetWeaponName(FString Name)
{
	ItemsName->SetText(FText::FromString(Name));
	
}

void UItemsUserWidget::SetWeaponAmmo(int32 Ammo)
{
	FString AmmoString = FString::Printf(TEXT("%03d"),Ammo);

	WeaponAmmos->SetText(FText::FromString(AmmoString));
}


