# Lambda

## C++ Lambda 基础知识

Lambda ，就是希腊字母“λ”，据说是代表着“λ演算（lambda calculus）”。C++11开始支持Lambda，可以说它只是一个便利机制。Lambda能做的事情，本质上都可以手写代码完成，但是它确实太方便了！怎么说呢，还好以前没有认真学std::bind各种绕法，现在用lambda方便多了。

我们可以通过简单的例子初步认识一下：

```cpp
int var1 = 100;
std::string var2 = "hello";

auto myLambda = [var1, &var2](int param) -> std::string {
    var2.append(std::to_string(var1));
    var2.append(std::to_string(param));
    return var2;
 };

std::cout << "fistLambda typeid = " << typeid(myLambda).name() << std::endl;
```

上面代码中由[]开头的那一串就是lambda了。在大多数情况下我们就使用“lambda”这个名词就够了，但其实仔细想想，其中代码涉及到三个概念：

- lambda表达式（lambda expression）
- 闭包（closure）
- 闭包类（closure class）

例如，在上面这段代码中：

- 定义了一个变量：myLambda，它就是“闭包”
- myLambda 的类型是一个编译器生成的匿名的类，也就是“闭包类”；
- 这个闭包类是由等号右边的"lambda表达式"生成的，这个lambda表达式：
    - 按值捕获了var1；按引用捕获了var2;
    - 并且接受一个int型参数；
    - 返回一个std::string对象
  
我们可以尝试把编译器自动生成的"闭包类"写出来，把“闭包”对象的构造也写出来，就应该能说明问题了。下面这段代码大体上和上面的代码等效：

```cpp
int var1 = 100;
std::string var2 = "hello";

class MyClosureClass {
  int var1;
  std::string& var2;

public:
    MyClosureClass(int inVar1, std::string& inVar2)
        : var1(inVar1), var2(inVar2) {}

    // not default constructible
    MyClosureClass() = delete;

    MyClosureClass(const MyClosureClass&) = default;
    MyClosureClass(MyClosureClass&&) = default;
    ~MyClosureClass() = default;

    // not copy assignable
    MyClosureClass& operator=(const MyClosureClass&) = delete;

    // function-call operator
    std::string operator()(int param) {
      var2.append(std::to_string(var1));
      var2.append(std::to_string(param));
      return var2;
    }
};

auto myLambda = MyClosureClass(var1, var2);
std::cout << "myLambda: " << myLambda(2233) << std::endl;

// class MyClosureClass 还可能包含一个自定义的类型转换操作符，用来把闭包对象转换成函数指针。
```

## 捕获列表“有坑”

lambda表达式的常用语法格式如下：

```cpp
[ captures ] ( params ) -> return_type { body }
```

其中比较值得一说的就是[captures]：捕获列表了！

[captures]支持多种写法，首先就是个人不推荐使用的两种默认捕获模式（default capture modes）：

```
[=]: 按值捕获当前作用域所有变量
[&]: 按引用捕获当前作用域所有变量
```

从性能、代码可维护性等方面都不建议使用这两种方式。比较常用的写法就是明确列出需要捕获的变量，例如：[var1, &var2], 其中var1使用了“按值捕获”模式，var2前面加了一个&代表着它使用“按引用捕获”的模式。下面就分别讨论一下“按值捕获”和“按引用捕获”有什么坑。

## 按值捕获 & 捕获时机

按值捕获就是在创建闭包的时候，将当前作用域内的变量赋值到闭包类的成员变量中，这个比较好理解，但是也有一个小小的坑。请看下面代码：

```cpp
FString LocalStr = TEXT("First string");

auto TestLambda = [LocalStr]()  {
    UE_LOG(LogTemp, Error, TEXT("String = %s ."), *LocalStr);
};

LocalStr = TEXT("Second string");
TestLambda();
```

当调用TestLambda()的时候，也许会觉得意外，输出的还是：String = First string。这就是要注意的地方，当闭包生成的那一刻，被捕获的变量已经按值赋值的方式进行了捕获，后面那个LocalStr对象再怎么变化，已经和闭包对象里面的值没有关系了。

如果按引用捕获，则可以跟踪LocalStr的更新了，但是按引用捕获的坑更深。

## 按引用捕获 & 悬空引用

如果是在C#中使用 lambda 就简单很多了，它有自动垃圾回收、class对象全部是引用类型这些特性，而对于C++来说，对象的生命周期、内存管理这根弦始终要绷紧。在C++编程中，程序员有责任保证Lambda调用的时候，保证被捕获的变量仍然有效~！是的，责任在你，而不在编译器。如果不能很好理解这点，就会遇到悬空引用的问题！

悬空引用（ dangling references ）就是说我们创建了一个对象的引用类型的变量，但是被引用的对象被析构了、无效了。一般情况下，引用类型的变量必须在初始化的时候赋值，很少遇到这种情况，但是如果lambda被延迟调用，在调用时，已经脱离了当前的作用域，那么按引用捕获的对象就是悬空引用。

我们先来看一段代码：

```cpp
FString LocalStr = TEXT("Local string");

auto TestLambda = [&LocalStr]() {
    UE_LOG(LogTemp, Error, TEXT("String = %s ."), *LocalStr);
    LocalStr = TEXT("Lambda string");
};

// 在这里直接调用是没问题的
TestLambda();

// 在Timer中调用，妥妥的Crash！
FTimerDelegate Delegate;
Delegate.BindLambda(TestLambda);

FTimerHandle TestTimer;
GetWorldTimerManager().SetTimer(TestTimer, Delegate, 1.0f, true);
```

上面这段的代码，在定义lambda之后立即调用则可以运行，同样一个labmda放入timer则会crash！这是为什么呢？

前面基本概念那一部分讲到了TestLambda是一个闭包对象，它的类型是编译器生成的一个匿名的class。对于这个例子，我尝试把这个闭包类的核心部分写出来：

```cpp
class MyLambdaClass {
    FString& LocalStr;
public:
    MyLambdaClass(FString& InLocalStr) :LocalStr(InLocalStr) {}

    void operator()() const 
    {
        UE_LOG(LogTemp, Error, TEXT("String = %s ."), *LocalStr);
        LocalStr = TEXT("Lambda string");
    }
};
```

看到上面这个class，应该就很清晰了：

- TestLambda()直接调用那一句，FString LocalStr这个对象还在作用域内，所以可以执行；
- 而在Timer执行的时候，LocalStr这个对象已经出了作用域，被析构了，这个时候Lambda中捕获的那个引用就变成了悬空引用啦，所以会导致Crash！

总之，使用各种 Delegate 的 “BindLambda” 的时候，要格外小心悬空引用的风险。

## 捕获UObject指针

虚幻的UObject具备自动垃圾回收机制，但这个机制是基于对象之间的引用关系的，也就是说一个 UObject 指针被捕获之后，还是可能被垃圾回收的。所以，对于延迟调用的lambda是不建议捕获UObject的；如果实在需要的话建议使用 FWeakObjectPtr ，例如这样：

```cpp
AActor* TargetActor = FindMyTargetActor();

auto ObjectLambda = [ActorPtr = TWeakObjectPtr<AActor>(TargetActor)](const FVector& Offset) {
    if (ActorPtr.IsValid()) {
        AActor* TargetActor = ActorPtr.Get();
        TargetActor->AddActorWorldOffset(Offset);
    }
};
```

通过 FWeakObjectPtr 引用 UObject 指针不会影响对象的生命周期，在 FWeakObjectPtr::IsValid() 方法中默认会判断当前对象是不是 “Pending Kill” 状态。

如果希望持有某个UObject的强引用，保证它不被垃圾回收，那么建议不要用lambda，建议使用其他写法：

1. 使用 Delegate 的 BindUObject 或者 BindUFunction 来处理；
2. 如果是很复杂的代码，也可以用 UObject 或者 FGCObject 的派生类来处理。


## C++14的初始化捕获（init capture）

在上面UObject指针的例子中，捕获列表是这样写的：ActorPtr = TWeakObjectPtr<AActor>(TargetActor)，这种写法就是C++14引入的新特性“初始化捕获”，也被称为广义捕获（generalized capture）。这个的语法是这样的：

- 等号左边的变量是声明在“闭包类” 里面的，它的类型由编译器自动推导；
- 等号右边的表达式，其作用域就是当前定义lambda的作用域，可以引用局部变量或者实参。

这个语法更有用的地方是：它可以把使用“移动语义”把局部变量移动到闭包中，类似这样：

```cpp
FString SomeBigString;
// ...
auto MyLambda = [MyStr = MoveTemp(SomeBigString)] {
//....
};
```

ref: https://zhuanlan.zhihu.com/p/110077218