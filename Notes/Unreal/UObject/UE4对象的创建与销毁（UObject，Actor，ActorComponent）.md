# UE4对象的创建与销毁（UObject，Actor，ActorComponent）

## 简介

在C++中，创建一个对象可以分为两种方法，在栈中创建或在堆中创建。

```cpp
//一：在栈中分配
	int num = 3;
//二：在堆中分配，需要手动delete释放
	int* num = new int(3);
	delete num;

```

UE4使用的是C++，所以引擎底层还是上面两种方法。但是对于开发者来讲，则需要使用UE4提供的方法来创建相关对象，而对于一般的UObject、AActor和AActorComponent，创建方法又有一定差异。

## UObject

UE4中所有的类都继承于UObject，对UObject不使用new来直接创建，而使用UE4提供的工厂方法NewObject()来创建。

- 运行时动态创建

```cpp
UMyObject* MyObject = NewObject<UMyObject>(this);
```

NewObject()有多种重载方法，其中一个关键的参数为Outer*，表示这个对象的外部对象，通常可传this指针进去。

- 如果是在构造函数中创建，请使用CreateDefaultSubobject()来创建，内部将this指针当作Outer*传进去

```cpp
MyObejct = CreateDefaultSubobject<UMyObject>(TEXT("MyObject"));
```

- 蓝图中动态创建一个UObject

Construct My Object

- 销毁，由于UE4完善的垃圾回收功能，所以将全部引用指针置为nullptr即可由UE4自动回收掉。如果需要强制销毁，可使用MarkPendingKill()方法。

```cpp
MyObject->MarkPendingKill();
MyObject = nulltpr;
```

## AActor

- 运行时动态创建

运行时动态创建Actor使用SpawnActor方法，有多种重载方法。SpawnActor是World的方法，需要提供一个有效的Actor，一般的UObject都有GetWorld()方法，但是一般只有AActor或UActorComponent才会返回有效的World。

```cpp
AMyActor* MyActor = GetWorld()->SpawnActor<AMyActor>();
```

- 销毁

Actor提供一个显示销毁的方法。

```cpp
MyActor->Destroy();
```

## UActorComponent

- Actor的构造函数中创建，也是最常见创建的地方

```cpp
UActorComponent* MyActorComponent = CreateDefaultSubobject<UActorComponent>(TEXT("MyActorComponentName"));
```

对于场景组件，可以依附在其它组件上。

```cpp
MySceneComponent->AttachToComponent(RootComponent, FAttachmentTransformRules::KeepRelativeTransform);
```

- 动态创建

因为创建ActorComponent需要在Actor中注册，所以使用NewObject()动态创建的时候，需要将外部的Actor对象作为Outer传进去，并调用RegisterComponent()方法。

- 蓝图中创建
  
对特定的组件使用AddXXXComponent节点

- 销毁组件

同Actor一样，可以手动调用DestroyComponent()方法。

- 原生C++对象

对于不是继承自UObject的C++对象，则通过C++的方式创建即可。考虑到使用new容易造成内存泄漏，还是推荐使用智能指针的方式创建和管理对象，并使用UE4自己的智能指针。

```cpp
TSharedPtr<FMyObjectType> NewSharedPtr = MakeShared<FMyObjectType>();
```