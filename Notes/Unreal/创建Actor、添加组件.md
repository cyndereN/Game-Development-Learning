https://zhuanlan.zhihu.com/p/22833151

## Actor

所有可以放入关卡的对象都是 Actor，比如摄像机、静态网格体、玩家起始位置。Actor支持三维变换，例如平移、旋转和缩放。你可以通过游戏逻辑代码（C++或蓝图）创建（生成）或销毁Actor。

在C++中，AActor是所有Actor的基类。

## 组件（Component）

组件（Component） 是可以添加到Actor上的一项功能。

当你为Actor添加组件后，该Actor便获得了该组件所提供的功能。例如：

- 聚光灯组件（Spot Light Component）允许你的Actor像聚光灯一样发光，
- 旋转移动组件（Rotating Movement Component）能使你的Actor四处旋转，
- 音频组件（Audio Component）将使你的Actor能够播放声音。

组件必须绑定在Actor身上，它们无法单独存在。

MyCustomActor.h
```cpp
#pragma once

#include "CoreMinimal.h"
// 引入组件
#include "Components/SceneComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Components/BoxComponent.h"
#include "Components/AudioComponent.h"
#include "Particles/ParticleSystemComponent.h"

#include "GameFramework/Actor.h"
#include "MyCustomActor.generated.h"

UCLASS()
class DEMO_API AMyCustomActor : public AActor
{
	GENERATED_BODY()

public:
	// Sets default values for this actor's properties
	AMyCustomActor();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// 自定义组件
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "MySceneComponent")
	USceneComponent *SceneComponent;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "MySceneComponent")
	UStaticMeshComponent *StaticMeshComponent;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "MySceneComponent")
	UBoxComponent *BoxComponent;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "MySceneComponent")
	UAudioComponent *AudioComponent;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "MySceneComponent")
	UParticleSystemComponent *ParticleSystemComponent;
};

```

MyCustomActor.cpp
```cpp

#include "MyCustomActor.h"

// Sets default values
AMyCustomActor::AMyCustomActor()
{
	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	// 创建组件
	SceneComponent = CreateDefaultSubobject<USceneComponent>(TEXT("CustomScene"));
	StaticMeshComponent = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("CustomStaticMesh"));
	BoxComponent = CreateDefaultSubobject<UBoxComponent>(TEXT("CustomBox"));
	AudioComponent = CreateDefaultSubobject<UAudioComponent>(TEXT("CustomAudio"));
	ParticleSystemComponent = CreateDefaultSubobject<UParticleSystemComponent>(TEXT("CustomParticleSystem"));

	// 把组件添加到根组件
	RootComponent = SceneComponent;
	StaticMeshComponent->SetupAttachment(SceneComponent);
	BoxComponent->SetupAttachment(SceneComponent);
	AudioComponent->SetupAttachment(BoxComponent);
	ParticleSystemComponent->SetupAttachment(SceneComponent);
}

// Called when the game starts or when spawned
void AMyCustomActor::BeginPlay()
{
	Super::BeginPlay();
}

// Called every frame
void AMyCustomActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

```