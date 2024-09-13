// Fill out your copyright notice in the Description page of Project Settings.


#include "BasePlayer.h"

#include "Actor_Items.h"
#include "Actor_Weapons.h"
#include "BasePlayerController.h"
#include "BaseUserWidget.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "StateUserWidget.h"
#include "Blueprint/UserWidget.h"
#include "Camera/CameraComponent.h"
#include "Engine/SkeletalMeshSocket.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Particles/ParticleSystemComponent.h"
#include "Sound/SoundCue.h"
#include "Components/TimelineComponent.h"


FOnTimelineFloat AimTimeLineFloat;
FOnTimelineFloat ShootTimeLineFloat;
FOnTimelineEvent ShootTimeLineFinish;
FOnTimelineEvent AimTimeLineFinish;
FHitResult Items_Hitresult;

// Sets default values
ABasePlayer::ABasePlayer()
{
	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	//摄像机
	PlayerSpringArm = CreateDefaultSubobject<USpringArmComponent>("PlayerSpringArm");
	PlayerSpringArm->SetupAttachment(RootComponent);
	PlayerSpringArm->TargetArmLength = 150.f;
	PlayerSpringArm->bUsePawnControlRotation = true;
	PlayerSpringArm->SocketOffset = FVector(0.f, 50.f, 80.f);


	PlayerCamera = CreateDefaultSubobject<UCameraComponent>("PlayerCamera");
	PlayerCamera->SetupAttachment(PlayerSpringArm);
	PlayerCamera->bUsePawnControlRotation = false;

	//摄像机控制
	bUseControllerRotationPitch = false;
	bUseControllerRotationRoll = false;
	bUseControllerRotationYaw = true;
	GetCharacterMovement()->bOrientRotationToMovement = false;
	GetCharacterMovement()->RotationRate = FRotator(0.f, 540.f, 0.f);

	//设置默认值

	//视场相关
	DefaultFOV = PlayerCamera->FieldOfView;
	AimFOV = 65.f;
	CurrentFOV = DefaultFOV;
	InterpSpeedFOV = 25.f;

	//瞄准
	b_iSAim = false;
	isAiming = false;
	DefaultLookRate = 1.0f;
	AimLookRate = 0.4f;
	CurrentLookRate = DefaultLookRate;

	//时间轴
	AimTimeLine = CreateDefaultSubobject<UTimelineComponent>("AimTimeLine");
	ShootTimeLine = CreateDefaultSubobject<UTimelineComponent>("ShootTimeLine");

	b_CheckItems = false;
	ShootDuration = .2f;
	bFire = false;
	isShooting = false;

	Tags.Add("Player");
	EquipedWeapons = false;

	Default_SMGAmmo = 100;

	
}

// Called when the game starts or when spawned
void ABasePlayer::BeginPlay()
{
	Super::BeginPlay();

	//获取动画实例
	PlayerAnimInstance = GetMesh()->GetAnimInstance();
	//注册增强输入子系统
	//APlayerController* PlayerControl = Cast<APlayerController>(Controller);
	PlayerControl = Cast<ABasePlayerController>(Controller);
	if (PlayerControl)
	{
		UEnhancedInputLocalPlayerSubsystem* LocalSS = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(
			PlayerControl->GetLocalPlayer());
		if (LocalSS)
		{
			LocalSS->AddMappingContext(PlayerInputMapping, 0);
		}
		//设置摄像机pitch角度
		PlayerControl->PlayerCameraManager->ViewPitchMax = 30.f;
		PlayerControl->PlayerCameraManager->ViewPitchMin = -60.f;
	}


	
	//时间轴捆绑
	if (AimCurve)
	{
		AimTimeLineFloat.BindUFunction(this, "AimFovUpdate");
		AimTimeLine->AddInterpFloat(AimCurve, AimTimeLineFloat);
		AimTimeLineFinish.BindUFunction(this, "AimFovFinish");
		AimTimeLine->SetTimelineFinishedFunc(AimTimeLineFinish);
	}
	if (ShootCurve)
	{
		ShootTimeLineFloat.BindUFunction(this, "ShootCHUpdate");
		ShootTimeLine->AddInterpFloat(ShootCurve, ShootTimeLineFloat);
		ShootTimeLineFinish.BindUFunction(this, "ShootCHFinish");
		ShootTimeLine->SetTimelineFinishedFunc(ShootTimeLineFinish);
	}

	InitMap_Ammo();
	//准星HUD
	BaseHUD = CreateWidget<UBaseUserWidget>(PlayerControl, BaseUserWidget);
	if (BaseHUD)
	{
		BaseHUD->AddToViewport(0);
	}

	//状态HUD
	PlayerControl->StateWidget->UpdateHP(.5f);
	PlayerControl->StateWidget->UpdateAmmo(0,*Map_Ammo.Find(E_Ammo::Enum_SMG));
	
	
}

// Called every frame
void ABasePlayer::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

//检视可交互物品
	OpenCloseChekLine();
	
	
	
	
}

// Called to bind functionality to input 绑定输入设置
void ABasePlayer::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	UEnhancedInputComponent* PlayerInputC = Cast<UEnhancedInputComponent>(PlayerInputComponent);
	if (PlayerInputC)
	{
		PlayerInputC->BindAction(PlayerMove, ETriggerEvent::Triggered, this, &ABasePlayer::Move);
		PlayerInputC->BindAction(PlayerLook, ETriggerEvent::Triggered, this, &ABasePlayer::Look);
		PlayerInputC->BindAction(PlayerJump, ETriggerEvent::Started, this, &ABasePlayer::Jump);
		PlayerInputC->BindAction(PlayerJump, ETriggerEvent::Completed, this, &ABasePlayer::StopJumping);

		PlayerInputC->BindAction(PlayerShoot, ETriggerEvent::Triggered, this, &ABasePlayer::Fire_Start);
		PlayerInputC->BindAction(PlayerShoot, ETriggerEvent::Completed, this, &ABasePlayer::ShootComplete);

		PlayerInputC->BindAction(PlayerAim, ETriggerEvent::Started, this, &ABasePlayer::AimIn);
		PlayerInputC->BindAction(PlayerAim, ETriggerEvent::Completed, this, &ABasePlayer::AimOut);

		PlayerInputC->BindAction(PlayerPickup, ETriggerEvent::Started, this, &ABasePlayer::Pickup);
		PlayerInputC->BindAction(PlayerDrop, ETriggerEvent::Started, this, &ABasePlayer::UseDrop);
		PlayerInputC->BindAction(PlayerReload, ETriggerEvent::Started, this, &ABasePlayer::Reload);
		

	}
}

//瞄准视场更新
void ABasePlayer::AimFovUpdate(float Alpha)
{
	float UpdateFOV = FMath::Lerp(CurrentFOV, AimFOV, Alpha);
	PlayerCamera->SetFieldOfView(UpdateFOV);

	BaseHUD->UpdateAimCHTranslate(Alpha, 10.f);
}

//准星瞄准完成
void ABasePlayer::AimFovFinish()
{
	isAiming = false;
	BaseHUD->GetCurrentCHTranslate();
}


//角色移动
void ABasePlayer::Move(const FInputActionValue& Value)
{
	const FVector2d F2d = Value.Get<FVector2d>();
	if (Controller)
	{
		const FRotator Ro = Controller->GetControlRotation();
		const FRotator YawRo(0, Ro.Yaw, 0);
		const FVector PlayerF = FRotationMatrix(YawRo).GetUnitAxis(EAxis::X);
		const FVector PlayerR = FRotationMatrix(YawRo).GetUnitAxis(EAxis::Y);
		AddMovementInput(PlayerF, F2d.Y);
		AddMovementInput(PlayerR, F2d.X);
	}
}

//角色查看
void ABasePlayer::Look(const FInputActionValue& Value)
{
	
	const FVector2d F2d = Value.Get<FVector2d>();
	if (Controller)
	{
		AddControllerPitchInput(F2d.Y * CurrentLookRate);
		AddControllerYawInput(F2d.X * CurrentLookRate);
	}
}

//角色射击
void ABasePlayer::Shoot()
{
if(Weapons->GetAmmo() ==0)
{
	//播放射击蒙太奇动画
	
	if (PlayerAnimInstance)
	{
		PlayerAnimInstance->Montage_Play(AM_Shoot);
	}
}
else
{
	Weapons->UseAmmo();
	PlayerControl->StateWidget->UpdateAmmo(Weapons->GetAmmo(),*Map_Ammo.Find(E_Ammo::Enum_SMG));
	
	//播放射击音效
	if (ShootSound)
	{
		UGameplayStatics::PlaySound2D(this, ShootSound);
	}

	//播放射击粒子效果
	
	//const FVector SocketLocation = GetMesh()->GetSocketLocation("MuzzleSocket");
	const FVector SocketLocation = Weapons->GetItemsMesh()->GetSocketLocation("MuzzleSocket");
	const FRotator SocketRotation = Weapons->GetItemsMesh()->GetSocketRotation("MuzzleSocket");
	const FVector SocketScale(1, 1, 1);
	if (P_ShootMuzzle)
	{
	
		UGameplayStatics::SpawnEmitterAttached(P_ShootMuzzle, GetMesh(), "MuzzleSocket", SocketLocation, SocketRotation,
		                                       SocketScale, EAttachLocation::KeepWorldPosition);
	}

	//播放射击蒙太奇动画
	PlayerAnimInstance = GetMesh()->GetAnimInstance();
	if (PlayerAnimInstance)
	{
		PlayerAnimInstance->Montage_Play(AM_Shoot);
	}


	//根据屏幕准星进行射击判定
	FVector2D Size_ViewPort;
	if (GEngine->GameViewport)
	{
		GEngine->GameViewport->GetViewportSize(Size_ViewPort);
	}
	FVector2D CrossHair2D(Size_ViewPort.X / 2.f, Size_ViewPort.Y / 2.f);

	bool SetScreenToWorld;
	FVector CrossHairWP;
	FVector CrossHairWD;
	SetScreenToWorld = UGameplayStatics::DeprojectScreenToWorld(UGameplayStatics::GetPlayerController(this, 0),
	                                                            CrossHair2D, CrossHairWP, CrossHairWD);
	if (SetScreenToWorld)
	{
		FHitResult ShootHit;
		const FVector ShootStart = CrossHairWP;
		const FVector ShootEnd = CrossHairWP + CrossHairWD * 30000.f;
		FVector ShootBeamEnd = ShootEnd;
		GetWorld()->LineTraceSingleByChannel(ShootHit, ShootStart, ShootEnd, ECC_Visibility);
		//如果射击被阻挡则播放击中效果
		if (ShootHit.bBlockingHit)
		{
			if (P_ShootHit)
			{
				UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), P_ShootHit, ShootHit.Location);
			}
			ShootBeamEnd = ShootHit.Location;
		}

		//设置射击光束粒子
		if (P_ShootBeam)
		{
			

			const FTransform MuzzleSocketTransform = Weapons->GetItemsMesh()->GetSocketByName("MuzzleSocket")->GetSocketTransform(Weapons->GetItemsMesh());

			UParticleSystemComponent* P_Beam = UGameplayStatics::SpawnEmitterAtLocation(
				GetWorld(), P_ShootBeam, MuzzleSocketTransform);
			P_Beam->SetVectorParameter("Target", ShootBeamEnd);
		}
	}
}
}

//角色完成射击
void ABasePlayer::ShootComplete()
{
	bFire = false;
}

//射击准星更新
void ABasePlayer::ShootCHUpdate(float Alpha)
{
	BaseHUD->UpdateShootCHTranslate(Alpha, -5.f);
}

//射击时间轴完成
void ABasePlayer::ShootCHFinish()
{
}


//瞄准状态切换
void ABasePlayer::AimIn()
{
	if(EquipedWeapons)
	{
		b_iSAim = true;
        	AimTimeLine->Play();
        	isAiming = true;
	}
	else
	{
		
	}
	
}

void ABasePlayer::AimOut()
{
	if(EquipedWeapons)
	{
		b_iSAim = false;
        	isAiming = true;
        	AimTimeLine->Reverse();
	}
	else
	{
		
	}
	
}

//拾取武器
void ABasePlayer::Pickup()
{
	Drop();
	if(BeforeLookedItems)
	{
		//GEngine->AddOnScreenDebugMessage(1,0.2f,FColor::Red,FString::Printf(TEXT("111")),true,FVector2d(3,3));
		
	SpawnWeapons(Cast<AActor_Weapons>(BeforeLookedItems));
	}
	if(Weapons)
	{
		PlayerControl->StateWidget->UpdateAmmo(Weapons->GetAmmo(),*Map_Ammo.Find(E_Ammo::Enum_SMG));
	}
}

//丢弃武器
void ABasePlayer::Drop()
{
	if(Weapons)
	{
		
		FDetachmentTransformRules detach(EDetachmentRule::KeepWorld,true);
		Weapons->GetItemsMesh()->DetachFromComponent(detach);
		Weapons->SetItemsEnum(E_Items::Enum_Drop);
		EquipedWeapons = false;

		float f_force = 2555.f;
		FVector FinalForce = GetActorForwardVector() * f_force;
		Weapons->AddForce(FinalForce);
		Weapons->UpdateWidgetAmmo();
	
		
	}
}

//按下丢弃键丢弃武器
void ABasePlayer::UseDrop()
{
	PlayerControl->StateWidget->UpdateAmmo(0,*Map_Ammo.Find(E_Ammo::Enum_SMG));
	Drop();
}

//上子弹
void ABasePlayer::Reload()
{
	if(Weapons)
	{
		if (PlayerAnimInstance)
        	{
        		PlayerAnimInstance->Montage_Play(AM_Reload);
        	}
		
	}
	

}

//开火定时器开始
void ABasePlayer::Fire_Start()
{
if(PlayerAnimInstance->Montage_IsPlaying(AM_Reload))
{return;}

	
	if(EquipedWeapons)
	{
		//设置定时器
        	if (isShooting)
        	{
        	}
        	else
        	{
        		bFire = true;
        		isShooting = true;
        		Shoot();
        		//ShootDuration = ShootTimeLine->GetTimelineLength();
        		GetWorldTimerManager().SetTimer(ShootTimer, this, &ABasePlayer::Fire_Finish, ShootDuration);
        
        		//如果定时器没有停止情况下射击，不重新获取当前变换，如果停止情况下重新射击则重新获取变换值
        		if (isAiming)
        		{
        			ShootTimeLine->Stop();
        			//GEngine->AddOnScreenDebugMessage(1,0.2f,FColor::Red,FString::Printf(TEXT("%d"),ShootTimeLine->IsPlaying()),true,FVector2d(3,3));
        		}
        		else
        		{
        			if (ShootTimeLine->IsPlaying())
        			{
        				ShootTimeLine->PlayFromStart();
        			}
        			else
        			{
        				BaseHUD->GetCurrentCHTranslate();
        				ShootTimeLine->PlayFromStart();
        			}
        		}
        	}
	}
	else
	{
		
	}

}



//开火定时器结束
void ABasePlayer::Fire_Finish()
{
	isShooting = false;
	ShootTimeLine->Reverse();


	//bFire=false;
	if (bFire)
	{
		Fire_Start();
	}
	else
	{
		//GetWorldTimerManager().SetTimer(ShootFinishTimer,this,&ABasePlayer::CheckCH,.1f);
	}
}

//射击结束后准星矫正
void ABasePlayer::CheckCH()
{
	if (b_iSAim)
	{
		AimTimeLine->Play();
	}
	else
	{
		AimTimeLine->Reverse();
	}
}




bool ABasePlayer::ItemsTraceLine(FHitResult& HitResult)
{
	FVector2D Size_ViewPort;
	if (GEngine->GameViewport)
	{
		GEngine->GameViewport->GetViewportSize(Size_ViewPort);
	}
	FVector2D CrossHair2D(Size_ViewPort.X / 2.f, Size_ViewPort.Y / 2.f);
	bool SetScreenToWorld;
	FVector CrossHairWP;
	FVector CrossHairWD;
	SetScreenToWorld = UGameplayStatics::DeprojectScreenToWorld(
		UGameplayStatics::GetPlayerController(this, 0),
		CrossHair2D, CrossHairWP, CrossHairWD);
	if (SetScreenToWorld)
	{
		const FVector LookStart = CrossHairWP;
		const FVector LookEnd = CrossHairWP + CrossHairWD * 20000.f;
		GetWorld()->LineTraceSingleByChannel(HitResult, LookStart, LookEnd, ECC_Visibility);
		if(HitResult.bBlockingHit)
		{
			return true;
		}
	}
	
	return false;
}

//打开关闭检测线
void ABasePlayer::OpenCloseChekLine()
{
	if(b_CheckItems)
	{
		//交互物品检测射线
		bool b_LookedItems = ItemsTraceLine(Items_Hitresult);
                        	
		if(b_LookedItems)
		{
			AActor_Items* LookedActorItem =  Cast<AActor_Items>(Items_Hitresult.GetActor());
			if(LookedActorItem)
			{
				LookedActorItem->SetWidgetVisibility(true);
				if(BeforeLookedItems)
				{
					if(BeforeLookedItems == LookedActorItem)
					{
                                        				
					}
					else
					{
						BeforeLookedItems->SetWidgetVisibility(false);
						BeforeLookedItems = LookedActorItem;
					}
				}
				else
				{
					BeforeLookedItems = LookedActorItem;
				}
                        			
			}
			else
			{
				if(BeforeLookedItems)
				{
					BeforeLookedItems->SetWidgetVisibility(false);
				}
			}
		}
		else
		{
                        		
                        		
		}

	}
	else
	{
		if(BeforeLookedItems)
		{
			BeforeLookedItems->SetWidgetVisibility(false);
			BeforeLookedItems = nullptr;
		}
	}
	
}


//生成武器
void ABasePlayer::SpawnWeapons(AActor_Weapons* EquipWeapon)
{
	
	if(EquipWeapon)
	{
		
		//Weapons = GetWorld()->SpawnActor<AActor_Weapons>(WeaponsClass);
		Weapons = EquipWeapon;
		Weapons->SetItemsEnum(E_Items::Enum_Equip);
		const USkeletalMeshSocket* WeaponSocket = GetMesh()->GetSocketByName("WeaponSocket");
		WeaponSocket->AttachActor(Weapons,GetMesh());
		
		EquipedWeapons = true;
	}
	
}



//更新交互物品重叠判断的数量
void ABasePlayer::UpdateCheckItemsCount(int count)
{
	CheckItemsCount += count;
	if(CheckItemsCount >0)
	{
		b_CheckItems = true;
	}
	else
	{
		b_CheckItems = false;

	}
	
}

//初始化弹药映射
void ABasePlayer::InitMap_Ammo()
{
	Map_Ammo.Add(E_Ammo::Enum_SMG,Default_SMGAmmo);
}

//蓝图中可以调用的上弹方法
void ABasePlayer::ReloadAmmo()
{
	
	int32 NeedAmmo =Weapons->GetNeedAmmo();
	int32 PlayerAmmo = *Map_Ammo.Find(E_Ammo::Enum_SMG);
	if(PlayerAmmo ==0)
	{
        		
	}
	else
	{
		if(PlayerAmmo>=NeedAmmo)
		{
			PlayerAmmo = PlayerAmmo-NeedAmmo;
			Weapons->SetAmmo(NeedAmmo);
		}
		else
		{
			Weapons->SetAmmo(PlayerAmmo);
			PlayerAmmo = 0;
        			
		}
	}
        
	Map_Ammo.Add(E_Ammo::Enum_SMG,PlayerAmmo);
	PlayerControl->StateWidget->UpdateAmmo(Weapons->GetAmmo(),*Map_Ammo.Find(E_Ammo::Enum_SMG));
}




