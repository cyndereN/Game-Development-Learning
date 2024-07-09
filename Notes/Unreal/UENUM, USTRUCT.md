## 枚举


### 第1种方式

定义枚举

```cpp
UENUM(BlueprintType)
namespace MyEnumType
{
	enum MyCustomType
	{
		Type1, // 或者使用带 DisplayName别名 ==> Enum1 UMETA(DisplayName = "Type1"),
		Type2,
		Type3,
		Type4,
	};
}
```

调用枚举，使用TEnumAsByte模板

```cpp
/**
 * Template to store enumeration values as bytes in a type safe way.
 */
template<class TEnum>
class TEnumAsByte{
    typedef TEnumAsByte_EnumClass<TIsEnumClass<TEnum>::Value> Check;

public:
    typedef TEnum EnumType;
}
```

```cpp
// 在蓝图中声明
UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyEnumType")
TEnumAsByte<MyEnumType::MyCustomType> MyEnumType;
```

### 第2种方式

```cpp
UENUM(BlueprintType)
enum class MyEnumType2 : uint8
{
	Enum1 UMETA(DisplayName = "Type1"),
	Enum2 UMETA(DisplayName = "Type2"),
	Enum3 UMETA(DisplayName = "Type3"),
	Enum4 UMETA(DisplayName = "Type4"),
};

UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyEnumType2")
TEnumAsByte<MyEnumType2> MyEnumType2;

```

## 结构体

### 定义结构体

注意，定义结构体名称前要加F前缀，不然编译不通过。

```cpp
USTRUCT(BlueprintType)
struct FMyCustomStruct
{
	GENERATED_USTRUCT_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyStructType")
	FString ID;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyStructType")
	FString Name;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyStructType")
	int32 Age;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyStructType")
	float Height;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyStructType")
	bool IsMan;
};
```

### 声明结构体

```cpp
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyCustomStruct")
	FMyCustomStruct MyCustomStruct;

```

## 补充属性说明符（ExposeOnSoawn）


蓝图生成时暴露变量，方便赋予变量的初始值

```cpp
UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "MyExposeOnSpawn", meta = (ExposeOnSpawn = "ExposeOnSpawnValue"))
	float ExposeOnSpawnValue;

```


### 结构体创建数据表格

```cpp
#include "Engine/DataTable.h"

USTRUCT(BlueprintType)
struct FDateTableStruct : public FTableRowBase
{
	GENERATED_USTRUCT_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "DateTableStruct")
	FString ID;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "DateTableStruct")
	FString Name;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "DateTableStruct")
	int32 Age;
};
```

编译之后，创建一个关联的信息表格，保存为.csv格式。
然后拖到项目文件目录中
