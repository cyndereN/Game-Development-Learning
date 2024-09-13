// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Actor_Items.h"
#include "Actor_Weapons.generated.h"

/**
 * 
 */
UCLASS()
class TPS_C_API AActor_Weapons : public AActor_Items
{
	GENERATED_BODY()

public:
	AActor_Weapons();
	virtual void Tick(float DeltaTime) override;
	FORCEINLINE int32 GetAmmo() const {return Ammo;}
	UFUNCTION()
	void UseAmmo();
	UFUNCTION()
	void UpdateWidgetAmmo();
	UFUNCTION()
	int32 GetNeedAmmo();
	UFUNCTION()
	void SetAmmo(int32 NeedAmmo);
	

private:
	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Attribute",meta=(AllowPrivateAccess="true",ClampMin="0"))
	int32 Ammo;
	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Attribute",meta=(AllowPrivateAccess="true",ClampMin="0"))
	int32 Ammo_Max;
	int32 NeededAmmo;
	

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
};
