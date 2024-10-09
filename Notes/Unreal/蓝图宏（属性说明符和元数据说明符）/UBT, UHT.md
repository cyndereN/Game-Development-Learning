# UBT和UHT介绍

## UnrealBuildTool（UBT）

表示项目环境依赖信息，用到哪些模块（“Core”,“CoreUObject”,“Engine”,“InputCore”，这些是初始内置模块）。

```cs
// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class DEMO : ModuleRules
{
	public DEMO(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(new string[]
		{
			"Core",
		 	"CoreUObject",
		 	"Engine",
		 	"InputCore"
		 });

		PrivateDependencyModuleNames.AddRange(new string[] { });

		// Uncomment if you are using Slate UI
		// PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });

		// Uncomment if you are using online features
		// PrivateDependencyModuleNames.Add("OnlineSubsystem");

		// To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true
	}
}


```

.Build.cs 依赖模块；.Target.cs 为目标平台设置

## UnrealHeadTool（UHT）

用于收集头文件生成反射数据，头文件发生变化时，UE引擎会调用UHT生成新的反射数据。

## UEC++各个宏的作用

- GENERATED_BODY() ：表示我们不直接使用父类的构造函数，如果要定义类中初始化操作，需要在.h头文件中声明构造函数，然后在.cpp文件中实现构造函数，它之后的成员是private的。
- GENERATED_UCLASS_BODY() ：表示使用父类的构造，如果在自定义类中做一些初始化操作，可以直接在.cpp文件中实现构造函数，不要在.h头文件中声明，这个宏会自动生成带有特定参数的构造函数，它之后的成员是public。https://www.cnblogs.com/heben/p/5495280.html
- UCLASS()：类声明宏。反射数据。类必须派生自UObject。
- UPROPERTY ：属性声明宏。UE在标准的C++基础上实现了一套反射系统（Reflection System）,反射系统负责垃圾回收、引用更新，编辑器集成等一系列高级且有用的功能。而UPROPERTY的作用就是声明该属性在反射系统的行为
- UFUNCTION：函数声明宏。反射系统的可识别的C++函数。
- USTRUCT()：结构体声明宏。反射系统的可识别的C++结构体。
- UENUM()：枚举声明宏。反射系统的可识别的C++枚举。
