## TArray

TArray是虚幻c++中的动态数组，TArray特点：速度快，内存消耗小，安全性高。并且TArray所有元素均完全为相同类型，不能进行不同元素类型的混合


https://dev.epicgames.com/documentation/en-us/unreal-engine/array-containers-in-unreal-engine?application_version=5.2


在C++中创建数组
```cpp
/**
 *
 */
UCLASS()
class DEMO_API AMyGameMode : public AGameMode
{
	GENERATED_BODY()
	AMyGameMode();

public:
	// 游戏开始时执行一次
	virtual void BeginPlay() override;
	// 游戏运行时每帧执行一次
	virtual void Tick(float DeltaTime) override;
	// 游戏结束或者切换关卡时执行一次
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	//TArray是虚幻c++中的动态数组TArray特点：速度快，内存消耗小，安全性高。并且TArray所有元素均完全为相同类型，不能进行不同元素类型的混合
	TArray<int32> m_Int32Array;

	void PrintTArray();
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
    // 数组操作
    // 添加元素到数组中
    m_Int32Array.Add(10);
    m_Int32Array.Add(20);
    m_Int32Array.Add(30);
    m_Int32Array.AddUnique(30); // 数组中不存在30，添加成功，存在30，添加失败
    // 删除所有等值的元素
    m_Int32Array.Remove(20);
    // 删除指定索引的元素
    m_Int32Array.RemoveAt(0);
    // 删除数组中第一个30,如果数组中有多个30，只删除第一个
    m_Int32Array.RemoveSingle(30);
    m_Int32Array.Empty(); // 清空数组
    m_Int32Array.Reset(); // 重置我们的元素所有变为0
    // 修改
    m_Int32Array.Insert(60,0); // 在索引0处插入60
    int32& value = m_Int32Array[0]; // 获取索引0处的值
    value = 100; // 修改索引0处的值为100    
    //查找
    int32 index = m_Int32Array.Find(100); // 查找100的索引  
    m_Int32Array.Contains(100); // 查找数组中是否包含100
    int32 index = m_Int32Array.FindLast(100);// 查找最后一个100的索引

    PrintTArray();
}

void AMyGameMode::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
}

void AMyGameMode::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
    Super::EndPlay(EndPlayReason);
}

void AMyGameMode::PrintTArray()
{
    // 遍历数组
    for (int32 i = 0; i < m_Int32Array.Num(); i++)
    {
        // 打印数组元素 到日志
        UE_LOG(LogTemp, Warning, TEXT("m_Int32Array[%d] = %d"), i, m_Int32Array[i]);
    }
    // 使用迭代器遍历数组
    for (auto It = m_Int32Array.CreateIterator(); It; ++It)
    {
        // 打印数组元素到屏幕上
        GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Green, FString::Printf(TEXT("m_Int32Array[%d] = %d"), It.GetIndex(), *It));
    }
}

```

## TMap


https://dev.epicgames.com/documentation/en-us/unreal-engine/map-containers-in-unreal-engine?application_version=5.3


TMap-是一种键值对容器，里面的数据都是成对出现的（Key，Value），Value通过Key值来获取，且Key值不能重复，key值唯一。

C+=中创建TMap
```cpp
UCLASS()
class DEMO_API AMyGameMode : public AGameMode
{
	GENERATED_BODY()
	AMyGameMode();

public:
	// 游戏开始时执行一次
	virtual void BeginPlay() override;
	// 游戏运行时每帧执行一次
	virtual void Tick(float DeltaTime) override;
	// 游戏结束h或者切换关卡时执行一次
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	// TMap-是一种键值对容器，里面的数据都是成对出现的（Key，Value），Value通过Key值来获取，且Key值不能重复，key值唯一。
	TMap<int32, int32> m_Map;

	void PrintTMap();
};

```

```cpp
// Fill out your copyright notice in the Description page of Project Settings.
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
    // TMap操作
    // 添加元素到TMap中
    m_Map.Add(1, 10);
    m_Map.Emplace(2, 20);
    m_Map.Emplace(3, 30);
    
    // 删除元素
    m_Map.Remove(2); // 删除key为2的元素
    m_Map.Empty();   // 清空TMap

    // 查找
    m_Map.Contains(1);                          // 查找TMap中是否包含key为1的元素
    int32 *findValue = m_Map.Find(1);           // 查找key为1的元素
    const int32 *findValue2 = m_Map.FindKey(1); // 查找value为1的元素
    m_Map.FindRef(1);                           // 查找key为1的元素的引用
    //分别获取所有的Keys和Values
    TArray<int32> keys;
    TArray<int32> values;
    m_Map.GenerateKeyArray(keys);
    m_Map.GenerateValueArray(values);
    
    PrintTMap();
}

void AMyGameMode::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
}

void AMyGameMode::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
    Super::EndPlay(EndPlayReason);
}
void AMyGameMode::PrintTMap()
{
  for(auto& elem : m_Map)
  {
  //打印日志
    UE_LOG(LogTemp, Warning, TEXT("m_Map[%d] = %d"), elem.Key, elem.Value);
    //打印到屏幕上
    GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Green, FString::Printf(TEXT("m_Map[%d] = %d"), elem.Key, elem.Value));
  }
}

```


## TSet

- TSet是一种快速容器类，（通常）用于在排序不重要的情况下存储唯一元素。
- TSet类似于TMap和TMultiMap，但有一个重要区别：TSet是通过对元素求值的可覆盖函数，使用数据值本身作为键，而不是将数据值与独立的键相关联。
- TSet可以非常快速地添加、查找和删除元素（恒定时间）。
- TSet也是值类型，支持常规复制、赋值和析构函数操作，以及其元素较强的所有权。

https://dev.epicgames.com/documentation/en-us/unreal-engine/set-containers-in-unreal-engine?application_version=5.3

```cpp
/**
 *
 */
UCLASS()
class DEMO_API AMyGameMode : public AGameMode
{
	GENERATED_BODY()
	AMyGameMode();

public:
	// 游戏开始时执行一次
	virtual void BeginPlay() override;
	// 游戏运行时每帧执行一次
	virtual void Tick(float DeltaTime) override;
	// 游戏结束h或者切换关卡时执行一次
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	TSet<FString> m_Set;
	void PrintTSet();
};

```

```cpp
// Fill out your copyright notice in the Description page of Project Settings.

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
    // TSet操作
    // 添加元素到TSet中
    m_Set.Add(TEXT("A"));
    m_Set.Add(TEXT("B"));
    m_Set.Emplace(TEXT("C")); // add和Emplace的区别是，Emplace可以避免在插入集时创建的临时对象，从而提高性能

    // 合并元素
    TSet<FString> setTemp;
    setTemp.Add(TEXT("D"));
    setTemp.Add(TEXT("E"));
    m_Set.Append(setTemp);

    // 删除元素
    m_Set.Remove(TEXT("A")); // 删除元素A
    m_Set.Reset();           // 重置TSet 清空集合元素但会保留内存
    m_Set.Empty();           // 清空TSet 并释放内存

    // 查找元素
    int32 count = m_Set.Num();                  // 获取TSet中元素的个数
    bool isContain = m_Set.Contains(TEXT("A")); // 查找TSet中是否包含元素A
    FString *value = m_Set.Find(TEXT("A"));     // 查找元素A

    // 转化为TArray
    TArray<FString> array = m_Set.Array(); // 填充了Tset中的所有元素副本

    // 排序 从小到大
    TSet<FString> setTemp2 = {TEXT("a"), TEXT("aa"), TEXT("aaa")};
    setTemp2.Sort([](const FString &A, const FString &B)
                  { return A.Len() < B.Len(); });

    // 运算符
    TSet<FString> setTemp3;
    setTemp3 = m_Set;
    setTemp3.Add(TEXT("F"));

    //[]
    FSetElementId id = setTemp3.Add(TEXT("G")); // 根据FSetElementId访问集合对应元素的引用
    m_Set[id] = TEXT("H");

    // ReServe
    TSet<FString> setTemp4;
    setTemp4.Reserve(10); // 预留10个元素的空间 ,若输入的参数大于当前元素个数，则重新分配内存，否则不做任何操作

    // Shrink
    for (int32 i = 0; i < 10; i++)
    {
        setTemp4.Add(FString::FromInt(i));
    }
    for (int32 i = 0; i < 10; i++)
    {
        setTemp4.Remove(FString::FromInt(i));
    }
    // 删除元素后，释放内存
    setTemp4.Shrink();

    // Compact 将容器中的所有空白的元素集合到一起，然后一起删除
    setTemp4.Compact();
    setTemp4.Shrink();

    PrintTSet();
    PrintTSet();
}

void AMyGameMode::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
}

void AMyGameMode::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
    Super::EndPlay(EndPlayReason);
}

void AMyGameMode::PrintTSet()
{
     UE_LOG(LogTemp, Warning, TEXT("%s"), "PrintTSet");
    for (auto &elem : m_Set)
    {
        UE_LOG(LogTemp, Warning, TEXT("%s"), *elem);
        GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Blue, FString::Printf(TEXT("%s"), *elem));
    }
}

```
