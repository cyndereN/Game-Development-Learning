https://zhuanlan.zhihu.com/p/158717151

## 前言

![](导图.png)

The Singleton design pattern has a lot of baggage, but its utility can't be ignored. Luckily Unreal provides a way to get the benefits of a Singleton, with less of the drawbacks.

## The Bad Way: C++ Static Singleton

```cpp
class UMySingleton : public UObject
{
public:
	static UMySingleton* GetInstance() { return Instance; }

private:
	static UMySingleton* Instance;
};
```

### Benefits and drawbacks of C++ static class singletons:

- Pro: Interface programmers are used to

- Con: Interacts badly with the editor: Without work, instances are preserved between running game through the editor multiple times.

- Con: Interacts badly class Class Default Objects: Without work, instances are created when CDOs are created.

- Con: Lifetime unclear: Requires careful programming and clear intentions to manage the lifetime of singletons.

## Unreal Subsystems

Unreal has something it calls Subsystems, that can be used to create globally-accessible modules that have explicitly-defined lifecycles.

Your subsystem's lifetime will match the lifetime of its parent. There are 5 different parent classes to choose from (see below). More info on their lifecycle can be found in the documentation.

| Subsystem    | Parent Class        | Lifetime                                                                 |
|--------------|---------------------|--------------------------------------------------------------------------|
| Engine       | `UEngineSubsystem`    | Both in editor and in-game, I think.                                    |
| Editor       | `UEditorSubsystem`    | When the Editor starts.                                                 |
| GameInstance | `UGameInstanceSubsystem` | As soon as your game starts, stays alive until the game is closed.      |
| LocalPlayer  | `ULocalPlayerSubsystem` | Matches the lifetime of its parent `ULocalPlayer`, can move between levels. |
| World        | `UWorldSubsystem`     | Matches its parent `UWorld`, is effectively per-level.   |

### AInfo与USubsystem

AInfo 来自AActor，支持replication

### Pimpl

可以防止bp直接访问subsystem?

### 继承结构

![](继承结构.png)

### Benefits over vanilla C++ singletons:

- Pro: Lifetime is automatically managed: Subclassing the correct Subsystem ensures that the instance will be created and destroyed for you.
- Pro: Desired lifetime is made explicit: It is clear that a Subsystem that inherits from UWorldSubsystem will only exist as long as a World.
- Pro: Cleaner Blueprint access.
- Pro: Accessible in Python scripts.
- Con: Requires some understanding of the lifecycles of a few Unreal classes.
- Con: Have to learn Unreal's access style instead of MyClass::GetInstance()


### Accessing Subsystems from C++

```cpp
UGameInstance* GameInstance = ...;
UMyGameSubsystem* MySubsystem = GameInstance->GetSubsystem<UMyGameSubsystem>();

ULocalPlayer* LocalPlayer = ...;
UMyPlayerSubsystem* MySubsystem = LocalPlayer->GetSubsystem<UMyPlayerSubsystem>();
```

### Example Usage

Imagine that we want to save player telemetry as they progress through the game, from the main menu to in-game. We could create a subclass of `UGameInstanceSubsystem` called `UTelemetrySubsystem`. Our telemetry class would be instantiated as soon as the game starts, and all logic related to telemetry would be stored within that subsystem.

```cpp
UCLASS(Abstract)
class ENGINE_API USubsystem : public UObject
{
	GENERATED_BODY()

public:
	USubsystem();

	/** Override to control if the Subsystem should be created at all.
	 * For example you could only have your system created on servers.
	 * It's important to note that if using this is becomes very important to null check whenever getting the Subsystem.
	 *
	 * Note: This function is called on the CDO prior to instances being created!
	 */
	virtual bool ShouldCreateSubsystem(UObject* Outer) const { return true; }

	/** Implement this for initialization of instances of the system */
	virtual void Initialize(FSubsystemCollectionBase& Collection) {}

	/** Implement this for deinitialization of instances of the system */
	virtual void Deinitialize() {}

private:
	friend class FSubsystemCollectionBase;
	FSubsystemCollectionBase* InternalOwningSubsystem;

};

```