# 第1章 导论

## 1.1 典型游戏团队结构

Engineer, Artist, Designer, Producer，其他支持和管理人员（市场，法律，it，行政）

Content is King

市场、制造和分销一般由发行商负责

## 1.2 游戏是什么

在设计的虚拟环境中，用确定的约束规则，由游戏参与者决策和控制的一种体验流程

## 1.3 游戏引擎是什么

把游戏开发中的基本核心功能抽象出来，供游戏开发者调用，这样的组件系统被称为游戏引擎

## 1.4 不同游戏类型的引擎差异

## 1.5 游戏引擎概览

## 1.6 运行时引擎架构

游戏引擎通常由工具套件和运行时组件两部分组成

### 1.6.1 目标硬件，设备驱动程序和操作系统

### 1.6.2 第三方软件开发包和中间件

数据结构，图形，碰撞物理，角色动画，人工智能，生物力学角色模型

### 1.6.3 平台独立层

### 1.6.4 核心系统

assertion，内存分配，数学库等

### 1.6.5 资源管理器

### 1.6.6 渲染引擎

### 1.6.7 剖析和调试工具

### 1.6.8 碰撞和物理

### 1.6.9 动画

### 1.6.10 人体学接口设备

### 1.6.11 音频

### 1.6.12 网络

### 1.6.13 游戏性基础系统

### 1.6.16 游戏专用子系统

如摄像机，人工智能，专用渲染，武器等

## 1.7 工具及资产管道

### 1.7.1 数字内容创作工具（DCC）

### 1.7.2 资产调节管道

### 1.7.3 世界编辑器，资源数据库

# 第2章 专业工具

# 第3章 游戏软件工程师基础

# 第4章 游戏所需的三维数学

# 第5章 游戏支持系统

## 5.1 子系统的启动和终止

### 5.1.1 C++ 静态初始化次序（是不可用的）

在函数内声明的静态变量不会在main()之前构建。把全局单例改为静态变量就可控制全局单例的构建次序

```cpp
class RenderManager{
public:
    // 取得唯一实例
    static RenderMannager& get()
    {
        // 此函数中的静态变量将于函数被首次调用时构建
        static RenderManager sSingleton;
        return sSingleton;
    }

    RenderManager()
    {
        VideoManager::get();
        TextureManager::get();
        // ...
    }

    ~RenderManager()
    {
        // 终止管理器
    }

};
```

变种：
```cpp
static RenderManager& get()
{
    static RenderManager* gpSingleton = NULL;
    if(gpSingleton == NULL)
        gpSingleton = new RenderManager;
    ASSERT(gpSingleton);
    returnm *gpSingleton;
}
```

但此方法不可控制析构次序，且难以预计单例确切构建时间

最简单的“蛮力”方法就是，明确定义启动和终止函数，而构造和析构函数完全不做任何事，这样我们可以在main()中按需明确次序调用

## 5.2 内存管理

以malloc或者new进行内存分配是非常慢的操作，要尽量避免动态内存分配，不然也可利用自制的内存分配器降低分配成本

把数据置于细小的连续内存块更高效

### 5.2.1 优化内存分配

用malloc或者new很慢的原因：
1. 大量管理开销
2. 从user mode切换到kernel mode的context-switch浪费时间

### 5.2.2 内存碎片

以堆栈和池分配器避免内存碎片

## 5.3 容器

p++后置递增不造成数据依赖，而++p会，且可能造成流水线停顿（stall）

## 5.4 字符串

提前对字符串扣留一次并把结果储存备用更好

```cpp
static StringId sid_foo = internString("foo");

// stringid.h

typedef U32 StringId;
extern StringId internString(const char* str);

// stringid.cpp
static HashTable<StringId, const char*> gStringIdTable;

StringId internString(const char* str)
{
    StringId sid = hasCrc32(str);
    HashTable<StringId, const char*>::iterator it = gStringIdTable.find(sid);

    if (it == gStringTable.end())
    {
        // 加入表
        // 记得要复制，以防原来的字符串是动态分配的并将被释放
        gStringTable[sid] = strdup(str);
    }

    return sid;
}
```

## 5.5 引擎配置

# 第6章 资源及文件系统

处理资源间的交叉引用：
1. 使用全局统一标识符做交叉引用
2. 指针修正表
3. 存储C++对象为二进制映像：构造函数

# 第7章 游戏循环及实时模拟

## 7.1 渲染循环

Rectangle Invalidation, 仅重绘屏幕中有改动的内容

```
while(!quit){
    updateCamera();
    updateSceneElements();

    // 把静止的场景渲染到屏幕外的帧缓冲
    renderScene();

    // 交换背景缓冲和前景缓冲
    swapBuffers();
}
```

## 7.2 游戏循环

## 7.3 游戏循环的架构风格

消息泵，回调驱动，基于事件的

## 7.4 抽象时间线

## 7.5 测量及处理时间

30 FPS: delta time = 33.3ms

60 FPS: delta time = 16.6ms

毫秒是游戏中常用的时间单位

游戏有时候会用上一帧的delta t估计下一帧，会造成不好的效果。帧率调控可以减少误差

在Unity中，Update和FixedUpdate是两个常用的游戏循环函数，它们之间有以下区别：

调用频率：Update函数在每一帧渲染之前被调用，因此其调用频率是不确定的，取决于每秒帧数（FPS）。而FixedUpdate函数是在固定时间间隔内调用，通常是每秒固定调用几次。默认情况下，FixedUpdate每秒调用次数为50次，但可以通过修改Time.fixedDeltaTime来改变。

时间间隔：Update函数的时间间隔是可变的，根据帧率的变化而变化。而FixedUpdate函数的时间间隔是固定的，不受帧率的影响，因此在物理模拟等需要固定时间间隔的情况下更加可靠。

适用场景：Update函数适用于处理与用户输入、游戏逻辑和动画相关的操作。因为其调用频率不固定，所以适合处理相对较轻量级的操作。而FixedUpdate函数适用于处理物理模拟、刚体运动和碰撞检测等需要固定时间间隔的操作。由于其时间间隔固定，所以适合处理较重量级的操作。

需要注意的是，由于物理引擎在FixedUpdate中运行，所以涉及刚体和碰撞的代码最好放在FixedUpdate中，以确保物理模拟的准确性。而其他游戏逻辑和动画相关的代码可以放在Update中。

综上所述，Update和FixedUpdate函数在调用频率、时间间隔和适用场景上有所区别，开发者应根据具体的需求和功能选择合适的函数来编写代码。

总结来说，按照一定时间间隔更新游戏适用于较简单的、不需要实时响应的游戏类型，而帧间隔更新适用于对实时性要求更高的游戏类型。

## 7.6 多处理器的游戏循环

比如

```cpp

RayCastResult result;
requestRayCast(playerPos, enemyPos, &result);

// 当等待其他核做光线投射时，做其他无关的工作

// 不能做更多了，等待光线投射作业的结果

waitForRayCastResuilts(&result);

//处理结果
if(result.hitSth() && isEnemy(r.getHitObject()))
{
    // ...
}

```

有时也加入bool rayJobPending以等待上一帧光线投射结果


## 7.7 网络多人游戏循环

# 第8章 人体学接口设备

## 8.1 各种人体学接口设备

## 8.2 人体学接口设备的接口技术

轮询，中断

## 8.3 输入类型

## 8.4 输出类型

## 8.5 游戏引擎的人体学接口设备系统

迅速连续按按钮
```cpp
class ButtonTapDetector{
    U32 m_buttonMask; // 需要检测的按钮（位掩码）
    F32 m_dtMax; // 按下事件之间的最长允许时间
    F32 m_tLast; // 最后按下按钮的事件，以s为单位

public:
    ButtonTapDetector(U32 buttonMask, F32 dtMax)
        : m_buttonMask(1U<<buttonMask)
        , m_dtMax(dtMax)
        , m_tLast(CurrentTime()-dtMax) // 开始时是无效的
    {}

    bool isGestureValid(U32 buttonMask, F32 dtMax)
    {
        F32 t = CurrentTime();
        F32 dt = t-m_tLast;
        return (dt < m_dtMax);
    }

    void Update(){
        if(ButtonJustWentDown(m_buttonMask)){
            m_tLast = CurrentTime();
        }
    }
};

```

多按钮序列

```cpp
class ButtonSequenceDetector{
    U32* m_aButtonIds; // 检测的序列
    U32 m_ButtonCount;  // 序列中的按钮数
    F32 m_dtMax; // 按下事件之间的最长允许时间
    EventId m_eventId; // 检测到序列时发送的事件
    U32 m_iButton; // 要检测的下一个按钮
    F32 m_tStart; // 序列开始时间，以s为单位
public:
    // 构建一个对象，用于检测指定的按钮序列
    // 当成功检测到序列时，就会广播指定事件，令整个游戏能适当回应事件

    ButtonSequenceDetectir(U32* aButtonIds, U32 buttonCount, F32 dtMax, EventId eventIdToSend)
        : m_aButtonIds(aButtonIds)
        , m_nButtonCount(buttonCount)
        , m_dtMax(dtMax)
        , m_eventId(eventIdToSend)
        , m_iButton(0)
        , m_tStart(0) // 开始时是无效的
    {}

    void Update(){
        ASSERT(m_iButton < m_nButtonCount);

        // 计算下一个预期的按钮，以位掩码表示
        U32 buttonMask = (1U << m_aButtonIds[m_iButton]);

        if (ButtonJustWentDown(~buttonMask)){
            m_iButton = 0; // 重置序列
        }
        else if (ButtonJustWentDown(buttonMask)){
            if(m_iButton == 0){
                m_tStart = CurrentTime();
                m_iButton++;
            }
        
            else{
                F32 dt = CurrentTime() - m_tStart;

                if(dt<m_dtMax){
                    // 序列仍然有效
                    m_iButton++;

                    if(m_iButton == m_nButtonCount){
                        // 序列已完成
                        BroadcastEvent(m_eventId);
                        m_iButton = 0;
                    }
                }
                else{
                    // 对不起，按的不够快
                    m_iButton = 0;
                }
            }
        }
    }
};
```
