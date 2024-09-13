// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Math/Transform.h"
#include "Actor_Items.generated.h"

UENUM(BlueprintType)
enum class E_Items:uint8
{
	Enum_Pickup UMETA(Displayname = "Pickup"),
	Enum_Equip UMETA(Displayname = "Equip"),
	Enum_Drop UMETA(Displayname = "Drop"),
};


UCLASS()
class TPS_C_API AActor_Items : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AActor_Items();

	

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	UFUNCTION()
	void SetWidgetVisibility(bool Value);
	

	UFUNCTION()
	void SetItemsEnum(E_Items SetItems);

	FORCEINLINE USkeletalMeshComponent* GetItemsMesh() const {return SkeletalMesh;}

	UFUNCTION()
	void AddForce(FVector force);

	

	


	//重叠组件方法
	UFUNCTION()
	void Beginoverlap(
		UPrimitiveComponent* OverlapComp,
		AActor* OtherActor,
		UPrimitiveComponent* OtherComp,
		int OtherBodyIndex,
		bool FromSweep,
		const FHitResult& SweepResult
		);

	UFUNCTION()
	void Endoverlap(
	UPrimitiveComponent* OverlapComp,
		AActor* OtherActor,
		UPrimitiveComponent* OtherComp,
		int OtherBodyIndex
	);


	
private:
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="Component",meta=(AllowPrivateAccess="true"))
	class UBoxComponent* Box;
	
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="Component",meta=(AllowPrivateAccess="true"))
	USkeletalMeshComponent* SkeletalMesh;

	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="Component",meta=(AllowPrivateAccess="true"))
	class UWidgetComponent* ItemsPickUpWidget;
	
	

	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="Attribute",meta=(AllowPrivateAccess="true"))
	FString WeaponsName;


	
	UPROPERTY(EditAnywhere,BlueprintReadOnly,Category="Component",meta=(AllowPrivateAccess="true"))
	class USphereComponent* Sphere;

	UPROPERTY(VisibleAnywhere,BlueprintReadOnly,Category="Attribute",meta=(AllowPrivateAccess="true"))
	E_Items E_Item;

	
protected:
	UPROPERTY()
	class UItemsUserWidget* ItemsWidget;
	

	

	



	
	
	

};
