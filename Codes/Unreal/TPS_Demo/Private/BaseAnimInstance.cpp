// Fill out your copyright notice in the Description page of Project Settings.


#include "BaseAnimInstance.h"

#include "BasePlayer.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "Kismet/KismetMathLibrary.h"

//初始化动画
void UBaseAnimInstance::NativeInitializeAnimation()
{
	Super::NativeInitializeAnimation();
	BasePlayer = Cast<ABasePlayer>(TryGetPawnOwner());
}

//更新动画
void UBaseAnimInstance::UpdateAnim(float DeltaTime)
{
	//判断角色类是否为空
	if(BasePlayer == nullptr)
	{
		BasePlayer = Cast<ABasePlayer>(TryGetPawnOwner());
	}

	else
	{
		
		//判断角色是否跳跃或者移动
		FVector PlayerVelocity = BasePlayer->GetVelocity();
		f_Speed = PlayerVelocity.Size();
		b_IsInAir = BasePlayer->GetCharacterMovement()->IsFalling();
		f_Acceleration = BasePlayer->GetCharacterMovement()->GetCurrentAcceleration().Size();
		if(f_Acceleration > 0.f)
		{
			b_IsMove = true;			
		}
		else
		{
			b_IsMove = false;
		}

		//计算角色角度偏移
		FRotator Player_AimRotator = BasePlayer->GetBaseAimRotation();
		FRotator Player_MoveRotator = UKismetMathLibrary::MakeRotFromX(PlayerVelocity);
		f_Direction = UKismetMathLibrary::NormalizedDeltaRotator(Player_MoveRotator,Player_AimRotator).Yaw;
		if(PlayerVelocity.Size()>0.f)
		{
			Last_Direction = f_Direction;
		}

		//同步角色状态
		b_iSAim = BasePlayer->GetIsAim();

		//获取角色瞄准偏移

		FRotator ActorRotator = BasePlayer->GetActorRotation();
		f_AimOffset = UKismetMathLibrary::NormalizedDeltaRotator(Player_AimRotator,ActorRotator).Pitch*-1.f;
		
	}
	
	
	
	
	
}
