# 大象无形

## 0 目录

```txt
第1章 开发之前——五个最常见基类

第2章 需求到实现

第3章 创建自己的C++类

第4章 对象

第5章 从C++到蓝图

第6章 UE4游戏性框架概述

第7章 引擎系统相关类

第8章 模块机制

第9章 重要核心系统

第10章 对象模型

第11章 渲染

第12章 Slate

第13章 蓝图

第14章 引擎独立程序

第15章 插件

第16章 自定义资源和编辑器
```

## 1 开发之前——五个最常见基类

- UObject
  - Garbage collection垃圾收集

    C++释放资源的解决方案，①继承UObject ②非UObject可以采用智能指针
  - Reference updating引用自动更新
  - Reflectio反射（TODO：第二章即将学习）
  - Serialization序列化：保存资源
  - Automatic updating of default property changes自动检测默认变量的 更改
  - Automatic property initialization自动变量初始化
  - Automatic editor integration和虚幻引擎编辑器的自动交互
  - Type information available at runtime运行时类型识别

    UE4有自己的类型转换方案，`Cast<TargetType>(OriginType)`
  - Network replication网络复制

- Actor
  它能够被挂载组件，但不是组件。组件都以U开头而不是A
- Pawn
国际象棋中的“棋子”，必须要被Controller所操控，可以是玩家也可以是AI
- Character（继承Pawn）

  有特殊的Movement组件，提供了一个基于胶囊体的角色移动，包括了移动和跳跃功能

- Controller

  控制Pawn和Character

  AIController：使用UE4的行为树/EQS环境查询系统

  PlyaerController：绑定输入，转化为对Pawn/Character的函数调用

- 总结
  
  M（Pawn）V（Pawn带的动态网格组件）C（Controller）

## 3 创建自己的C++类

继承UObject的类，类名上方需要加入UCLASS宏

虚幻引擎类命名规则

F：纯C++类
U：继承UObject
A：继承
S：Slate空间相关类
H：HitResult相关类

## 4 对象

类对象的获取

```cpp
for(TActorIterator <ClassType> Iterator(GetWorld());Iterator;++Iterator)
 { 
    ...//do something 
}
```

*Iterator访问对象，或者`Iterator->function()`

## 6 UE4游戏性框架概述

### 6.1 行为树

同样的AI模式，用状态机会涉及大量的跳转，但是用行为树就相对来说更加简化。
同时由于行为树的“退行”特点，也就是“逐个尝试，不行就换”的思路

### 6.2 虚幻引擎网络架构

客户端是对服务端的拙劣模仿，不再时刻去"同步"，而是去"预测"，“模仿”

这就是说，我们承认“延迟”客观存在，只要我们的客户端模仿得别太差劲，那么玩家是可以接受这样的效果的。客户端可以根据同步数据发送时的当前对象的位置与速度，加上数据发送
的时间，猜测出当前对象在服务端的可能位置。并且通过修正当前世界（比如调整当前对象的速度方向，指向新的位置），去模仿服务端位置。如果服务端的位置和客户端差距太大，就强行闪现修正。

## 7 引擎系统相关类

### 7.1 在虚幻引擎4中使用正则表达式

需要包含头文件#include "Regex.h"，通过FRegexPattern 来创建一个待查找或匹配的模式，然后通过FRegexMatcher来查找与匹配字符串中符合模式的部分，如下代码：

```cpp
#include "Regex.h"
FString TextStr("ABCDEFGHIJKLMN");
FRegexPattern TestPattern(TEXT("C.+H"));
FRegexMatcher TestMacher(TestPattern, TestStr);

if (TestMacher.FindNext())
{
  UE_LOG(MyLog, Warning, TEXT("找到匹配内容 %d ~ %d"), 
                TestMacher.GetMatchBeginning(),
                TestMacher.GetMatchEnding());
}
```

### 7.2 FPaths类的使用

在Core模块中，虚幻引擎提供了一个用于路径相关处理的类：FPaths。主要提供了3类“工具”性质API：

1，具体路径类： FPaths::GameDir() 获取游戏根目录

2，工具类：FPaths::FileExists()判断一个文件是否存在。

3，路径转换类：FPaths::ConvertRelativePathToFull()将相对路径转换为绝对路径。

### 7.3 XML与JSON

这里涉及到XML，JSON与INI等三种文件：

XML：可以使用FastXML与FXmlFile进行解析与处理。

Json：可以使用FJsonSerializer与FJsonValue等来进行解析与处理。

ini：使用GConfig类进行读写与解析。

### 7.4 文件读写与访问

使用FPlatformFileManager::Get()->GetPlatformFile()进行文件的读写相关。常用函数如下：

CopyDirctorTree 拷贝目录树

CopyFile 拷贝当前文件

CreateDirectory 创建目录

DeleteDirectory 删除目录

DeleteFile 删除指定文件

FileExists 检查文件是否存在

OpenRead 打开一个文件进行读取

OpenWrite 打开一个文件进行写入

等等。

### 7.5 读写配置GConfig文件（就是Unity中的.Asset）

### 7.6 UE_LOG

### 7.7 字符串处理

### 7.8 编译器相关技巧

## 8 模块机制
