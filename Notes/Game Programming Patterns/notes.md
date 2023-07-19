# 第1章 架构，性能和游戏

## 1.1 什么是软件架构

### 1.1.1 什么是好的软件架构

好的设计意味着当我作出改动，整个程序就好像正等着这种改动。 我可以仅调用几个函数就完成任务，而代码库本身无需改动。

架构是关于改动的。 总会有人改动代码。评价架构设计的好坏就是评价它应对改动有多么轻松。

### 1.1.2 你如何做出改变

### 1.1.3 我们如何从解耦中受益

最小化在编写代码前需要了解的信息


## 1.2 有什么代价？

抽象化&模块化 v.s. YAGNI

过度去关注设计模式和软件架构，会让一批人很容易地沉浸在代码中，而忽略要自己的最终目的是要发布游戏。无数的开发者听着加强可扩展性的“警世名言”，花费多年时间制作“引擎”， 却没有搞清楚做引擎是为了什么。

## 1.3 性能和速度

许多让代码更灵活的模式依靠虚拟调度、 接口、 指针、 消息，和其他机制， 而这些都会消耗运行时成本

一个有趣的反面例子是C++中的模板。模板编程有时可以给你抽象接口而无需运行时开销。

这是灵活性的两极。当写代码调用类中的具体方法时，你已经硬编码了调用的是哪个类：但通过虚方法或接口，直到运行时才知道调用的类。虽然这样更加灵活，但增加了运行时开销。

而模板编程是在两者之间——在编译时初始化模板，决定调用哪些类。


## 1.4 坏代码中的好代码

## 1.5 寻求平衡

- 良好的架构
- 快速的运行时性能
- 快速实现今天的功能


## 1.6 简单性

正确获得数据结构和算法（大致是这样的先后），追求简单代码更少，但编写时间不一定更少；追求优雅代码更通用


## 1.6 准备出发

- 抽象和解耦让扩展代码更快更容易，但除非确信需要灵活性，否则不要在这上面浪费时间。

- 在整个开发周期中为性能考虑并做好设计，但是尽可能推迟那些底层的，基于假设的优化，那会锁死代码。

- 快速地探索游戏的设计空间，但不要跑得太快，在身后留下烂摊子。毕竟你总得回来打扫。

- 如果打算抛弃这段代码，就不要尝试将其写完美。摇滚明星将旅店房间弄得一团糟，因为他们知道明天就走人了

- 如果要做一些有趣的东西，那就先让自己乐在其中

# 第2章 命令模式

命令模式将“请求”封装成对象，以便使用不同的请求、队列或者日志来参数化其他对象，同时支持可撤消的操作。将发出命令的责任和执行命令的责任分割开。

命令就是对象化（实例化）的方法调用。

命令模式是一种回调的面向对象实现。

**优点**

命令模式的优点有：对类间解耦、可扩展性强、易于命令的组合维护、易于与其他模式结合，而缺点是会导致类的膨胀

**使用场合**

命令模式很适合实现诸如撤消，重做，回放，时间倒流之类的功能。
基于命令模式实现录像与回放等功能，也就是执行并解析一系列经过预录制的序列化后的各玩家操作的有序命令集合。


## 2.1 配置输入


将一组行为抽象为对象，这个对象和其他对象一样可以被存储和传递，从而实现行为请求者与行为实现者之间的松耦合，这就是命令模式。




下面是一种简单实现
```cpp
void InputHandler::handleInput()
{
  if (isPressed(BUTTON_X)) jump();
  else if (isPressed(BUTTON_Y)) fireGun();
  else if (isPressed(BUTTON_A)) swapWeapon();
  else if (isPressed(BUTTON_B)) lurchIneffectively();
}
```

我们定义了一个基类代表可触发的游戏行为：

```cpp
class Command
{
public:
  virtual ~Command() {}
  virtual void execute() = 0;
};
```
然后我们为不同的游戏行为定义相应的子类：
```cpp
class JumpCommand : public Command
{
public:
  virtual void execute() { jump(); }
};

class FireCommand : public Command
{
public:
  virtual void execute() { fireGun(); }
};
// 你知道思路了吧
```

在代码的输入处理部分，为每个按键存储一个指向命令的指针。

```cpp
class InputHandler
{
public:
  void handleInput();

  // 绑定命令的方法……

private:
  Command* buttonX_;
  Command* buttonY_;
  Command* buttonA_;
  Command* buttonB_;
};


void InputHandler::handleInput()
{
  if (isPressed(BUTTON_X)) buttonX_->execute();
  else if (isPressed(BUTTON_Y)) buttonY_->execute();
  else if (isPressed(BUTTON_A)) buttonA_->execute();
  else if (isPressed(BUTTON_B)) buttonB_->execute();
}
```

## 2.2 关于角色的说明

我们不让函数去找它们控制的角色，我们将函数控制的角色对象传进去：

```cpp
class Command
{
public:
  virtual ~Command() {}
  virtual void execute(GameActor& actor) = 0;
};
```

通过在命令和角色间增加了一层重定向， 我们获得了一个灵巧的功能：我们可以让玩家控制游戏中的任何角色，只需向命令传入不同的角色。
我们可以在AI和角色之间使用相同的命令模式，甚至在演示时为玩家角色加上AI

```cpp
class JumpCommand : public Command
{
public:
  virtual void execute(GameActor& actor)
  {
    actor.jump();
  }
};
```

```cpp
Command* InputHandler::handleInput()
{
  if (isPressed(BUTTON_X)) return buttonX_;
  if (isPressed(BUTTON_Y)) return buttonY_;
  if (isPressed(BUTTON_A)) return buttonA_;
  if (isPressed(BUTTON_B)) return buttonB_;

  // 没有按下任何按键，就什么也不做
  return NULL;
}
```

这里不能立即执行，因为还不知道哪个角色会传进来。 这里我们享受了命令是具体调用的好处——延迟到调用执行时再知道。

```cpp
Command* command = inputHandler.handleInput();
if (command)
{
  command->execute(actor);
}
```

## 2.3 撤销和重做

假设我们在制作单人回合制游戏，想让玩家能撤销移动，这样他们就可以集中注意力在策略上而不是猜测上。

```cpp
class MoveUnitCommand : public Command
{
public:
  MoveUnitCommand(Unit* unit, int x, int y)
  : unit_(unit),
    x_(x),
    y_(y)
  {}

  virtual void execute()
  {
    unit_->moveTo(x_, y_);
  }

private:
  Unit* unit_;
  int x_, y_;
};

```

```cpp
Command* handleInput()
{
  Unit* unit = getSelectedUnit();

  if (isPressed(BUTTON_UP)) {
    // 向上移动单位
    int destY = unit->y() - 1;
    return new MoveUnitCommand(unit, unit->x(), destY);
  }

  if (isPressed(BUTTON_DOWN)) {
    // 向下移动单位
    int destY = unit->y() + 1;
    return new MoveUnitCommand(unit, unit->x(), destY);
  }

  // 其他的移动……

  return NULL;
}
```
命令的一次性为我们很快地赢得了一个优点。 为了让指令可被取消，我们为每个类定义另一个需要实现的方法：

```cpp
class Command
{
public:
  virtual ~Command() {}
  virtual void execute() = 0;
  virtual void undo() = 0;
};
```

这里是添加了撤销功能后的移动命令：
```cpp
class MoveUnitCommand : public Command
{
public:
  MoveUnitCommand(Unit* unit, int x, int y)
  : unit_(unit),
    xBefore_(0),
    yBefore_(0),
    x_(x),
    y_(y)
  {}

  virtual void execute()
  {
    // 保存移动之前的位置
    // 这样之后可以复原。

    xBefore_ = unit_->x();
    yBefore_ = unit_->y();

    unit_->moveTo(x_, y_);
  }

  virtual void undo()
  {
    unit_->moveTo(xBefore_, yBefore_);
  }

private:
  Unit* unit_;
  int xBefore_, yBefore_;
  int x_, y_;
};
```


## 2.4 类风格化还是函数风格化

在某种程度上说，命令模式是为一些没有闭包的语言模拟闭包。

举个例子，如果我们使用javascript来写游戏，那么我们可以用这种方式来写让单位移动的命令：
```js
function makeMoveUnitCommand(unit, x, y) {
  // 这个函数就是命令对象:
  return function() {
    unit.moveTo(x, y);
  }
}
```

我们可以通过一对闭包来为撤销提供支持：
```js
function makeMoveUnitCommand(unit, x, y) {
  var xBefore, yBefore;
  return {
    execute: function() {
      xBefore = unit.x();
      yBefore = unit.y();
      unit.moveTo(x, y);
    },
    undo: function() {
      unit.moveTo(xBefore, yBefore);
    }
  };
}
```

## 2.5 参考

对象可以响应命令，或者将命令交给它的从属对象。如果我们这样实现了，就完成了一个职责链模式。

有些命令是无状态的纯粹行为，比如第一个例子中的JumpCommand。 在这种情况下，有多个实例是在浪费内存，因为所有的实例是等价的。 可以用享元模式解决这个问题。

