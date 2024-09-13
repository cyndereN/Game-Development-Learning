// Fill out your copyright notice in the Description page of Project Settings.


#include "Actor_Weapons.h"

#include "ItemsUserWidget.h"

AActor_Weapons::AActor_Weapons()
{
	
	
}

void AActor_Weapons::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

//使用武器的子弹
void AActor_Weapons::UseAmmo()
{
		--Ammo;
}

//同步UI的子弹数量
void AActor_Weapons::UpdateWidgetAmmo()
{
	if(ItemsWidget)
	{
		ItemsWidget->SetWeaponAmmo(Ammo);
	}
}

//获取所需要填充的子弹数
int32 AActor_Weapons::GetNeedAmmo()
{
	NeededAmmo = Ammo_Max - Ammo;
	
	return NeededAmmo;
}

//设置武器子弹
void AActor_Weapons::SetAmmo(int32 NeedAmmo)
{
	Ammo  = Ammo + NeedAmmo;
}

void AActor_Weapons::BeginPlay()
{
	Super::BeginPlay();
	Ammo_Max = 50;
	Ammo = FMath::RandRange(30,Ammo_Max);
	UpdateWidgetAmmo();
}


