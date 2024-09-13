// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include "GameFramework/Character.h"
#include "BasePlayer.generated.h"

struct FInputActionValue;

//定义弹药类型枚举
UENUM(BlueprintType)
enum class E_Ammo:uint8
{
	Enum_Pistol UMETA(Displayname = "Pistol"),
	Enum_SMG UMETA(Displayname = "SMG"),
	Enum_AR UMETA(Displayname = "AR"),
};



UCLASS()
class TPS_C_API ABasePlayer : public ACharacter
{
	GENERATED_BODY()

public:
	// Sets default values for this character's properties
	ABasePlayer();
	

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

private:

	UPROPERTY()
	class ABasePlayerController* PlayerControl;

	UPROPERTY()
	class UAnimInstance* PlayerAnimInstance;
	
	//摄像机
	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="PlayerCamera",meta=(AllowPrivateAccess="true"))
	class USpringArmComponent* PlayerSpringArm;

	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="PlayerCamera",meta=(AllowPrivateAccess="true"))
	class UCameraComponent* PlayerCamera;

	//输入
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputMappingContext* PlayerInputMapping;

	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerMove;
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerLook;
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerJump;
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerShoot;
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerAim;
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerPickup;
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerDrop;
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="PlayerInput",meta=(AllowPrivateAccess="true"))
	class UInputAction* PlayerReload;

	//音效
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class USoundCue* ShootSound;

	//粒子
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class UParticleSystem* P_ShootMuzzle;
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class UParticleSystem* P_ShootHit;
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class UParticleSystem* P_ShootBeam;

	//动画
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class UAnimMontage* AM_Shoot;

	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class UAnimMontage* AM_Reload;
	
	//FOV
	float DefaultFOV;
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	float AimFOV;
	float CurrentFOV;
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	float InterpSpeedFOV;
	bool b_iSAim;
	bool isAiming;

	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"),meta=(ClampMin="0",ClampMax="1",UIMin="0",UIMax="1"))
	float AimLookRate;
	float DefaultLookRate;
	float CurrentLookRate;

//弹药类型枚举
	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Shoot",meta=(AllowPrivateAccess="true"))
	E_Ammo E_Ammos;

	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Shoot",meta=(AllowPrivateAccess="true"))
	TMap<E_Ammo,int32> Map_Ammo;

	UPROPERTY()
	int32 Default_SMGAmmo;
	

	
	//瞄准准星相关内容
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class UCurveFloat* AimCurve;
	
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="Shoot",meta=(AllowPrivateAccess="true"))
	class UCurveFloat* ShootCurve;
	
	class UTimelineComponent* AimTimeLine;
	class UTimelineComponent* ShootTimeLine;

	
	UFUNCTION()
	void AimFovUpdate(float Alpha);

	UFUNCTION()
	void AimFovFinish();

	UFUNCTION()
	void ShootCHUpdate(float Alpha);

	UFUNCTION()
	void ShootCHFinish();

	
	


	//基础动作
	void Move(const FInputActionValue& Value);
	void Look(const FInputActionValue& Value);
	void Shoot();
	void ShootComplete();
	void AimIn();
	void AimOut();
	void InterpFOV(float DeltaTime);
	void Pickup();
	void Drop();
	void UseDrop();
	void Reload();


	//UI
	UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="HUD",meta=(AllowPrivateAccess="true"))
	TSubclassOf<class UBaseUserWidget> BaseUserWidget;
	class UBaseUserWidget* BaseHUD;


	


	//射击定时器
	float ShootDuration;
	bool bFire; 
	bool isShooting;
	FTimerHandle ShootTimer;
	FTimerHandle ShootFinishTimer;
	

	UFUNCTION()
	void Fire_Start();
	UFUNCTION()
	void Fire_Finish();
	UFUNCTION()
	void CheckCH();

	UFUNCTION()
	bool ItemsTraceLine(FHitResult& HitResult);

	
	class AActor_Items* BeforeLookedItems;

	bool b_CheckItems;
	int CheckItemsCount;

	//打开关闭检测线
	UFUNCTION()
	void OpenCloseChekLine();

	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Weapons",meta=(AllowPrivateAccess="true"))
	class AActor_Weapons* Weapons;

	//UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="Weapons",meta=(AllowPrivateAccess="true"))
	//TSubclassOf<AActor_Weapons> WeaponsClass;

	//产生武器
	UFUNCTION()
	void SpawnWeapons(AActor_Weapons* EquipWeapon);

	bool EquipedWeapons;



public:
	//内联函数
	FORCEINLINE bool GetIsAim() const {return b_iSAim;}
	UFUNCTION()
	void UpdateCheckItemsCount(int count);

	UFUNCTION()
	void InitMap_Ammo();

	UFUNCTION(BlueprintCallable)
	void ReloadAmmo();
	



	


	
	


	





};
