// Fill out your copyright notice in the Description page of Project Settings.


#include "BaseUserWidget.h"

#include "BasePlayer.h"
#include "Components/Image.h"
#include "Components/ProgressBar.h"

float Default_ch_l_x;
float Default_ch_r_x;
float Default_ch_u_y;
float Default_ch_d_y;
float CrossHair_L_x;
float CrossHair_R_x;
float CrossHair_U_y;
float CrossHair_D_y;
float Current_ch_l_x;
float Current_ch_r_x;
float Current_ch_u_y;
float Current_ch_d_y;

void UBaseUserWidget::NativeConstruct()
{
	Super::NativeConstruct();
	Default_ch_l_x = CrossHair_L->GetRenderTransform().Translation.X;
	Default_ch_r_x = CrossHair_R->GetRenderTransform().Translation.X;
	Default_ch_u_y = CrossHair_U->GetRenderTransform().Translation.Y;
	Default_ch_d_y = CrossHair_D->GetRenderTransform().Translation.Y;
	

	/*CrossHair_L->SetRenderTranslation(FVector2d(Default_ch_l_x,0.f));
	CrossHair_R->SetRenderTranslation(FVector2d(20.f,0.f));
	CrossHair_U->SetRenderTranslation(FVector2d(0.f,-20.f));
	CrossHair_D->SetRenderTranslation(FVector2d(0.f,20.f));	*/
	
}



//瞄准时候准星变换
void UBaseUserWidget::UpdateAimCHTranslate(float Alpha,float Value)
{
	
		CrossHair_L_x = FMath::Lerp(Default_ch_l_x,Default_ch_l_x+Value,Alpha);
		CrossHair_R_x = FMath::Lerp(Default_ch_r_x,Default_ch_r_x-Value,Alpha);
		CrossHair_U_y = FMath::Lerp(Default_ch_u_y,Default_ch_u_y+Value,Alpha);
		CrossHair_D_y = FMath::Lerp(Default_ch_d_y,Default_ch_d_y-Value,Alpha);
	
	
	//GEngine->AddOnScreenDebugMessage(1,1.0f,FColor::Red,FString::Printf(TEXT("cross:%f"),Alpha));

	CrossHair_L->SetRenderTranslation(FVector2d(CrossHair_L_x,0.f));
	CrossHair_R->SetRenderTranslation(FVector2d(CrossHair_R_x,0.f));
	CrossHair_U->SetRenderTranslation(FVector2d(0.f,CrossHair_U_y));
	CrossHair_D->SetRenderTranslation(FVector2d(0.f,CrossHair_D_y));
	
}

//射击时候准星变换
void UBaseUserWidget::UpdateShootCHTranslate(float Alpha, float Value)
{
	CrossHair_L_x = FMath::Lerp(Current_ch_l_x,Current_ch_l_x+Value,Alpha);
	CrossHair_R_x = FMath::Lerp(Current_ch_r_x,Current_ch_r_x-Value,Alpha);
	CrossHair_U_y = FMath::Lerp(Current_ch_u_y,Current_ch_u_y+Value,Alpha);
	CrossHair_D_y = FMath::Lerp(Current_ch_d_y,Current_ch_d_y-Value,Alpha);
	CrossHair_L->SetRenderTranslation(FVector2d(CrossHair_L_x,0.f));
	CrossHair_R->SetRenderTranslation(FVector2d(CrossHair_R_x,0.f));
	CrossHair_U->SetRenderTranslation(FVector2d(0.f,CrossHair_U_y));
	CrossHair_D->SetRenderTranslation(FVector2d(0.f,CrossHair_D_y));
	//GEngine->AddOnScreenDebugMessage(1,1.0f,FColor::Red,FString::Printf(TEXT("cross:%f"),Current_ch_l_x));

}

//获取最新准星变换
void UBaseUserWidget::GetCurrentCHTranslate()
{
	Current_ch_l_x = CrossHair_L->GetRenderTransform().Translation.X;
	Current_ch_r_x = CrossHair_R->GetRenderTransform().Translation.X;
	Current_ch_u_y = CrossHair_U->GetRenderTransform().Translation.Y;
	Current_ch_d_y = CrossHair_D->GetRenderTransform().Translation.Y;
	//GEngine->AddOnScreenDebugMessage(1,1.0f,FColor::Red,FString::Printf(TEXT("cross:%f"),Current_ch_l_x));

}

void UBaseUserWidget::ClearCHTranslate()
{
	Current_ch_l_x = 0;
	Current_ch_r_x = 0;
	Current_ch_u_y = 0;
	Current_ch_d_y = 0;
}

