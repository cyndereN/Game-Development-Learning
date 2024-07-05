https://docs.unrealengine.com/4.26/zh-CN/ProgrammingAndScripting/ProgrammingWithCPP/UnrealArchitecture/Actors/ActorLifecycle/

## 生命周期

常用声明周期函数：

- BeginPlay() ：游戏开始时执行一次
- Tick(float DeltaTime) ：游戏运行时每帧执行一次
- EndPlay(const EEndPlayReason::Type EndPlayReason) ：游戏结束或者切换关卡时执行一次

集成自AGameMode实现类AMyGameMode

```cpp
#pragma once

#include "CoreMinimal.h"
#include "MyGameState.h"
#include "MyHUD.h"
#include "MyPawn.h"
#include "MyPlayerController.h"
#include "MyPlayerState.h"
#include "GameFramework/GameMode.h"
#include "MyGameMode.generated.h"

UCLASS()
class DEMO_API AMyGameMode : public AGameMode
{
	GENERATED_BODY()
	AMyGameMode();

public:
	// 游戏开始时执行一次
	virtual void BeginPlay() override;
	//游戏运行时每帧执行一次
	virtual void Tick(float DeltaTime) override;
	//游戏结束h或者切换关卡时执行一次
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;
};
```

```cpp

#include "MyGameMode.h"

AMyGameMode::AMyGameMode()
{
    DefaultPawnClass = AMyPawn::StaticClass();
    HUDClass = AMyHUD::StaticClass();
    PlayerControllerClass = AMyPlayerController::StaticClass();
    GameStateClass = AMyGameState::StaticClass();
    PlayerStateClass = AMyPlayerState::StaticClass();
}

void AMyGameMode::BeginPlay()
{
    Super::BeginPlay();
}

void AMyGameMode::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
}

void AMyGameMode::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
    Super::EndPlay(EndPlayReason);
}
```

## 日志打印

### OutLog打印

```cpp
void AMyGameMode::BeginPlay()
{
    Super::BeginPlay();

    // LogTemp : 临时日志，不会保存到文件中
    // Warning : 警告日志，黄色 记录级别
    // TEXT : 打印内容
    // 日志级别常用三种：Display、Warning、Error
    UE_LOG(LogTemp, Display, TEXT("Hello World!"));
    UE_LOG(LogTemp, Warning, TEXT("Hello World!"));
    UE_LOG(LogTemp, Error, TEXT("Hello World!"));
}
```


### 屏幕打印

```cpp
void AMyGameMode::BeginPlay()
{
    Super::BeginPlay();
    //打印到屏幕上
    GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Red, TEXT("Hello World!"));
}
```

## FString、FName 和 FText，三者之间的区别

- FName：表示名称，不区分大小写，不可更改
引擎中的资源名称都是FName类型，

    - 通过一个轻型系统重复使用字符串，FName创建时会根据内容创建一个Hash值，且同样的内容只会存储一次。

    - 通过Hash值在进行FName的查找和访问时速度较快，而在比较的时，也不需要比较字符串内容，直接比较Hash值来区分不同FName字符串。

    - 另外两种字符串到FName的转换（特别注意的是，FText不能直接转换到FName，可先转换为FString，再转换为FName）

    ```cpp
    FString str = TEXT("Hello world~");
    FText txt = LOCTEXT("keyName", "theValue");
    FText txtNS = NSLOCTEXT("TextNameSpace", "keyName", "theValue");
    
    // FString 转 FName：不可靠，丢失大小写信息
    FName name1 = FName(*str);

    FString strFromTxt = txt.ToString();
    // FText 先转 FString，再转 FName：不可靠，丢失大小写信息，丢失本地化信息可能导致语言转换的潜在风险
    FName name2 = FName(*strFromTxt);
    ```


- FString：最接近std::string，着重在于字符串的操作
    - 提供了大量对字符串的操作接口，是三者中唯一可修改的字符串类型，

    - 也正因如此，FString对比其它两种字符串来说消耗更高，性能更低。

    - 另外两种字符串到FString的转换

    ```cpp
    FName name = TEXT("I am Frank~");
    FText txt = LOCTEXT("keyName", "theValue");
    FText txtNS = NSLOCTEXT("TextNameSpace", "keyName", "theValue");

    // FName 转 FString：可靠
    FString str1 = name.ToString();
    // FText 转 FString：不可靠，丢失本地化信息可能导致语言转换的潜在风险
    FString str2 = txt.ToString();

    ```

- FText：着重在于显示与本地化，显示可理解玩家能直接看到的信息
    - 本地化即多语言的处理，不可更改。
    - 相较于另外两种类型，FText会更加的臃肿，但提供了优秀的本地化功能。
    - 另外两种字符串到FText的转换：

    ```cpp
    FString str = TEXT("Hello world~");
    FName name = TEXT("I am Hello~");

    // FString 转 FText：可靠
    FText txt1 = FText::FromString(str);
    // FName 转 FText：可靠
    FText txt2 = FText::FromName(name);

    ```

## 基础数据类型打印

```cpp
//基础数据类型的输出打印
int32 MyInt=10;
float MyF1oat=5.0f;
bool MyBool=true;
char MyChar=’A';
FString MyStringl=TEXT("MyStringl");
FVector MyVector=FVector(0,0,0);
//打印
UE_LOG(LogTemp,Warning,TEXT("%d"),MyInt);
UE_LOG(LogTemp,Warning,TEXT("%f"),MyF1oat);
UE_LOG(LogTemp,Warning,TEXT("%d"),MyBoo1);
UE_LOG(LogTemp,Warning,TEXT("%c"),MyChar);
UE_LOG(LogTemp,Warning,TEXT("%s"),*MyString1);
UE_LOG(LogTemp,Warning,TEXT("%s"),*MyVector.ToString());
```