// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"


#include "GameFramework/PlayerController.h"
#include "BasePlayerController.generated.h"

/**
 * 
 */
UCLASS()
class TPS_C_API ABasePlayerController : public APlayerController
{
	GENERATED_BODY()

public:
	ABasePlayerController();
	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="HUD")
    class UStateUserWidget* StateWidget;

private:
UPROPERTY(EditAnywhere,BlueprintReadWrite,Category="HUD",meta=(AllowPrivateAccess="true"))
	TSubclassOf<class UStateUserWidget> StateWidgetClass;
	
	

	

protected:
	virtual void BeginPlay() override;
	
};
