 // Fill out your copyright notice in the Description page of Project Settings.


#include "BasePlayerController.h"
#include "StateUserWidget.h"
#include "Blueprint/UserWidget.h"


 ABasePlayerController::ABasePlayerController()
 {
  
 }



 void ABasePlayerController::BeginPlay()
 {
  Super::BeginPlay();
 	
  if(StateWidgetClass)
  {
StateWidget = CreateWidget<UStateUserWidget>(this,StateWidgetClass);
  	if(StateWidget)
  	{
	StateWidget->AddToViewport();
  	}
   
  }
  
  }
  


