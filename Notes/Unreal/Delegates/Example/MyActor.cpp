// Fill out your copyright notice in the Description page of Project Settings.


#include "MyActor.h"



AMyActor::AMyActor()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

void AMyActor::TestDyDelegate(FTestDyDelegate1 TestDyDelegate)
{

}

void AMyActor::AADDD()
{

}

int32 AMyActor::Fun(float a, const FString& s)
{
	return 1;
}

void AMyActor::TestFun(float a, const FString& s)
{

}

int32 AAAA()
{
	return 0;
}
// Called when the game starts or when spawned
class FTestA
{
public:
	int32 Fun(float a, const FString& s)
	{
		return 5;
	}
	static int32 FunStatic(float a, const FString& s)
	{
		return 5;
	}
};
static int32 Fun1(float a, const FString& s)
{
	return 5;
}
void AMyActor::BeginPlay()
{

	Super::BeginPlay();

	
	//绑定ambda(匿名函数)的代理
	DelegateTwoparamsRetVal.BindLambda([this](float a, const FString& s) ->int32
	{
		return 0;
	});
	//绑定原生C++类的方法
	FTestA A;
	DelegateTwoparamsRetVal.BindRaw(&A, &FTestA::Fun);

	TSharedPtr<FTestA> A1 = MakeShareable(new FTestA);

	//绑定一个TSharedPtr指向对象的函数，即纯C++类的函数。因为纯C++类一般会使用TSharedPtr用于管理内存
	DelegateTwoparamsRetVal.BindSP(A1.ToSharedRef(), &FTestA::Fun);

	//绑定一个静态函数
	DelegateTwoparamsRetVal.BindStatic(&FTestA::FunStatic);

	//绑定一个线程安全的TSharedPtr包裹着的纯C++类里面的函数
	TSharedPtr<FTestA,ESPMode::ThreadSafe> A2 = MakeShareable(new FTestA);
	DelegateTwoparamsRetVal.BindThreadSafeSP(A2.ToSharedRef(), &FTestA::Fun);

	// ͨ通过函数名去绑定代理，用到了反射
	DelegateTwoparamsRetVal.BindUFunction(this,FName("Fun"));

	//绑定继承自UObject类的类里面的成员函数
	DelegateTwoparamsRetVal.BindUObject(this, &AMyActor::Fun);

	if(DelegateTwoparamsRetVal.IsBound())
		DelegateTwoparamsRetVal.Execute(23.f,"Hello Delegate");

	DelegateTwoparamsRetVal.Unbind();

	//多播代理
	MutliDelegate.AddUObject(this, &AMyActor::TestFun);

	MutliDelegate.Broadcast(5.2, "Hello Delegate");

	//对于动态单播代理
	//TestDyDelegate1.BindUFunction();
	//TestDyDelegate1.Execute();

	//对于动态多播代理
	//动态代理绑定的函数必须在头上面加UFUNCTION()
	//TestDyMuDelegate.AddDynamic(this, &AMyActor::AADDD);

	//TestDyMuDelegate.Broadcast();
}


// Called every frame
void AMyActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

