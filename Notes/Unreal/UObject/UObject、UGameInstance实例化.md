## 在Default Pawn中实现

```cpp
#include "MyObject.h"

UCLASS()
class DEMO_API AMyPawn : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	AMyPawn();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	UPROPERTY(EditAnywhere)
	UMyObject *MyCustomObject;
};

```

```cpp
// Called when the game starts or when spawned
void AMyPawn::BeginPlay()
{
	Super::BeginPlay();
	// 使用模板类TSubclassOf创建对象
	TSubclassOf<UMyObject> MyObjectClass = UMyObject::StaticClass();
	MyCustomObject = NewObject<UMyObject>(GetWorld(), MyObjectClass);
	if (MyCustomObject)
	{
		UE_LOG(LogTemp, Warning, TEXT("MyCustomObject is %s"),
			   *MyCustomObject->GetName());
	}
}

```

## MyObject中参数调用

在MyObject.h中实现

```cpp
#pragma once

#include "CoreMinimal.h"
#include "Engine/DataTable.h"
#include "UObject/NoExportTypes.h"
#include "MyObject.generated.h"

USTRUCT(BlueprintType)
struct FDateTableStruct : public FTableRowBase
{
	GENERATED_USTRUCT_BODY()
	//构造函数
	FDateTableStruct();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "DateTableStruct")
	FString ID;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "DateTableStruct")
	FString Name;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "DateTableStruct")
	int32 Age;
};

/**
 *
 */
UCLASS()
class DEMO_API UMyObject : public UObject
{
	GENERATED_BODY()

public:
	// 隐式创建并调用无参构造器
	// 隐式创建方式在进程虚拟地址空间中的栈中分配内存，它的分配和释放由系统决定，
	// 函数内局部变量的存储单元可以在栈上创建，函数执行完毕，系统会自动释放这些存储单元。
	UPROPERTY()
	FDateTableStruct MyDateTableStruct;
	// 显式创建并调用无参构造器
	// 这种创建方式和第一种一样，在进程虚拟地址空间中的栈中分配内存，
	// 它的分配和释放由系统决定，函数内局部变量的存储单元可以在栈上创建，函数执行完毕，系统会自动释放这些存储单元。
	// FDateTableStruct MyDateTableStruct = FDateTableStruct();
};


```

MyObject.cpp 实现
```cpp
#include "MyObject.h"

FDateTableStruct::FDateTableStruct()
{
    ID = "1001";
    Name = "Kurenai";
    Age = 18;
}

```

调用参数

```cpp
// Called when the game starts or when spawned
void AMyPawn::BeginPlay()
{
	Super::BeginPlay();
	// 使用模板类TSubclassOf创建对象
	TSubclassOf<UMyObject> MyObjectClass = UMyObject::StaticClass();
	MyCustomObject = NewObject<UMyObject>(GetWorld(), MyObjectClass);
	if (MyCustomObject)
	{
		UE_LOG(LogTemp, Warning, TEXT("MyCustomObject is %s"), *MyCustomObject->GetName());
	
		UE_LOG(LogTemp, Warning, TEXT("MyCustomObject  ID is %s"), *MyCustomObject->MyDateTableStruct.ID);
		UE_LOG(LogTemp, Warning, TEXT("MyCustomObject  Name is %s"), *MyCustomObject->MyDateTableStruct.Name);
		UE_LOG(LogTemp, Warning, TEXT("MyCustomObject  Age is %d"), MyCustomObject->MyDateTableStruct.Age);
	}
}

```

## UGameInstance实例化

GameInstance是全局唯一单例，这个在引擎初始化的时候就已经生成，一直存在到引擎关闭。
主要用于为游戏保存全局临时数据，想要持久化保存需要用SaveGame。


### 创建GameInstance

UMyGameInstance.h
```cpp
#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "MyGameInstance.generated.h"

/**
 *
 */
UCLASS()
class DEMO_API UMyGameInstance : public UGameInstance
{
	GENERATED_BODY()

public:
//构造函数
	UMyGameInstance();

//添加数据成员
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyGameInstance")
	FString AppID;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyGameInstance")
	FString AppKey;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyGameInstance")
	FString AppName;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyGameInstance")
	FString AppVersion;
};

```

UMyGameInstance.cpp
```cpp
#include "MyGameInstance.h"

UMyGameInstance::UMyGameInstance()
{
    AppID = "123456789";
    AppKey = "qwertyuiop";
    AppName = "MyGame";
    AppVersion = "1.0.0";
}

```

在MyPawn中调用
```cpp
#include "MyGameInstance.h"

UCLASS()
class DEMO_API AMyPawn : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	AMyPawn();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	UPROPERTY()
	UMyGameInstance *MyGameInstance;
};

//----

// Called when the game starts or when spawned
void AMyPawn::BeginPlay()
{
	Super::BeginPlay();
	MyGameInstance = Cast<UMyGameInstance>(GetWorld()->GetFirstPlayerController()->GetGameInstance());
	if (MyGameInstance)
	{
		UE_LOG(LogTemp, Warning, TEXT("MyGameInstance AppName is %s"), *MyGameInstance->AppName);
		UE_LOG(LogTemp, Warning, TEXT("MyGameInstance AppID is %s"), *MyGameInstance->AppID);
		UE_LOG(LogTemp, Warning, TEXT("MyGameInstance AppKey is %s"), *MyGameInstance->AppKey);
		UE_LOG(LogTemp, Warning, TEXT("MyGameInstance AppVersion is %s"), *MyGameInstance->AppVersion);
	}
}

```