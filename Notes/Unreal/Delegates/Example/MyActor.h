// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "MyActor.generated.h"


//单播代理
DECLARE_DELEGATE(FTestDelegateNoparam);
DECLARE_DELEGATE_TwoParams(FTestDelegateTwoparams, float, const FString&);
DECLARE_DELEGATE_RetVal_TwoParams(int32, FTestDelegateTwoparamsRetVal, float, const FString&);
//多播代理
//注意
//1.多播代理上绑定的函数的执行顺序是不确定的
//2.多播代理没有返回值
DECLARE_MULTICAST_DELEGATE_TwoParams(FTestMutliDelegate, float, const FString&);

//动态单播代理
DECLARE_DYNAMIC_DELEGATE(FTestDyDelegate);
DECLARE_DYNAMIC_DELEGATE_RetVal_OneParam(float, FTestDyDelegate1, int32, a);
//动态多播代理（蓝图里的Event Dispatcher）
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FTestDyMuDelegate);


UCLASS()
class HELLOUE4PRO_API AMyActor : public AActor
{
	GENERATED_BODY()
public:	
	
	AMyActor();
	UFUNCTION(BlueprintCallable)
	void TestDyDelegate(FTestDyDelegate1 TestDyDelegate);
	UPROPERTY(BlueprintAssignable)
	FTestDyMuDelegate TestDyMuDelegate;

	FTestDyDelegate1 TestDyDelegate1;
public:
	UFUNCTION()
	void AADDD();
	UFUNCTION()
	int32 Fun(float a, const FString& s);
	void TestFun(float a, const FString& s);
protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	FTestDelegateTwoparamsRetVal DelegateTwoparamsRetVal;

	FTestMutliDelegate MutliDelegate;
public:	

	// Called every frame
	virtual void Tick(float DeltaTime) override;

};
