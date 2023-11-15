## 需求
用UE4下的C++实现一个订阅分发模式。

要求：

- UPublisher类，有成员函数：
    - AddListener(Callback) 添加回调。注意，回调可以是一个类的成员函数。
    - Publish() 发布信息
- 订阅者类可以是任何实现了OnMessage(FString Message)的类，注意这个函数可以是成员函数

难点在于AddListener(Callback)的参数类型。

## 常识：函数指针

假设我们有一个普通函数（非成员函数），想要绑定到函数指针上，需要做两步：

1. typedef规定新类型
2. 赋值

```cpp
typedef (*OnMessageCallback)(FString Message)

void OnMessage(FString Message)
{
    // ...
}

int main()
{
    OnMessageCallback Callback = &OnMessage;
    Callback(TEXT(""));
    return 0;
}
```

但是这只是针对非成员函数或静态函数，如果是成员函数，就稍微要复杂点。

```cpp
class Subcriber
{
    void OnMessage(FString Message)
    {
        // ...
    }
}

typedef (Subcriber::*OnMessageCallback)(FString Message)

int main()
{
    Subcriber MySubcriber = new Subcriber();
    OnMessageCallback Callback = &Subcriber::OnMessage;
    Callback(TEXT(""));
    return 0;
}
```

然而这又有个问题。我们需求里面说的是任意实现了OnMessage(FString Message)成员函数的类，都能够被作为参数传递。

但是如果用typedef的方式，则必定要定义一个「包括了类名」的别名。如果把这个别名作为Publisher::AddListener(Callback)参数类型，那么就无法实现传入任意实现了OnMessage(FString Message)成员函数的类的需求。


## std::function 与 std::bind 方案

C++11出了std::function和std::bind新特性。

前者是可以定义一个函数，而后者是为了将函数指针绑定到前者上用。

使用方法如下：

```cpp
#include <functional>

class Subcriber
{
    void OnMessage(FString Message)
    {
        // ...
    }
}

int main()
{
    Subcriber MySubcriber = new Subcriber();
    std::function<void(FString)> OnMessageCallback;
    /* 第一个参数是函数地址
     * 第二个参数是类对象，也就是this
     * 第三个是placeholder
     */
    OnMessageCallback = std::bind(&Subcriber::OnMessage, MySubcriber, std::placeholders::_1);
    OnMessageCallback(TEXT(""));
}
```

由于std::function可以是与this类型无关的，所以适合当一个事件类型。

## UE4提供的TFunction方案

TFunction其实就是UE4里的std::function，作用是承载一个lambda函数。

