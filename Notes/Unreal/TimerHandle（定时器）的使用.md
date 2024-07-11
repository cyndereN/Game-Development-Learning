## 设置定时器

定时器（Timer） 可用于执行延迟类型的操作，或让某些操作在一段时间内重复执行。定时器的设置只

需三步即可完成：声明定时器句柄 FTimerHandle、定义执行函数、设置定时器，其中前两步是第三步的预备工作。

### 1. 声明FTimerHandle

```cpp
#include "TimerManager.h"

UCLASS()
class DEMO_API AMyCharacter : public ACharacter, public IMyInterface
{
	GENERATED_BODY()

public:
	// Sets default values for this character's properties
	AMyCharacter();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent *PlayerInputComponent) override;
	//声明定时器变量
	FTimerHandle TimerHandle;
	void TimerFunction();
};

```

### 2. 定义执行函数

```cpp
void AMyCharacter::TimerFunction()
{
	UE_LOG(LogTemp, Warning, TEXT("TimerFunction"));
	GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Red, TEXT("TimerFunction"));
}
```

### 3. 设置定时器

注意：不可以在Actor的构造函数中设置定时器

```cpp
// Called when the game starts or when spawned
void AMyCharacter::BeginPlay()
{
	Super::BeginPlay();

	// 设置定时器 5秒后执行TimerFunction函数，每5秒执行一次
	GetWorldTimerManager().SetTimer(TimerHandle, this, &AMyCharacter::TimerFunction, 5.f, true);
}
```

SetTimer() 各个参数的作用如下：

- InOutHandle：定时器绑定的句柄，如果该句柄已指向其他定时器，则取消这个其他定时器；
- InObj：调用执行函数的对象；
- InTimerMethod：定时器所执行的函数；
- InRate：函数执行的时间间隔，如果<=0，则清除现存定时器，即 InOutHandle 所绑定的定时器；
- InbLoop：该定时器是否循环，若不循环则只执行一次；
- InFirstDelay：从设置定时器到执行定时器的时间间隔，若<0，则使用 InRate 代替。

### 4. 清除定时器

设置定时器后，一定不要忘了清除定时器。这就像申请了资源，一定要归还一样。取消定时器的话，建议直接使用ClearTimer()，其用法示例如下：

```cpp
if (TimeHandle.IsValid())
{
    GetWorldTimerManager().ClearTimer(TimerHandle);
}
```

清除定时器可在 Actor 的 EndPlay() 函数中执行，但在其他地方也是可以的，只是不要忘记清除定时器。