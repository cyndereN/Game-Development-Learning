1. 类型转化
```csharp
public class Calculator
{
    public enum OPERATOR {Add, Minus, Multiply, Divide, Unknown};
    public static OPERATOR GetOperator(char ch)
    {
        OPERATOR op = OPERATOR.Unknown;
        switch (ch)
        {
            case '+':
                op = OPERATOR.Add;
                break;
            case '-':
                op = OPERATOR.Minus;
                break;
            case '*':
                op = OPERATOR.Multiply;
                break;
            case '/':
                op = OPERATOR.Divide;
                break;
            default:
                break;
        }
        return op;
    }
}
```

```csharp
public class Calculator
{
    public enum OPERATOR { Add, Minus, Multiply, Divide, Unknown};
    static private var OperatorMap = new Dictionary<char, OPERATOR> {
        { '+', OPERATOR.Add },
        { '-', OPERATOR.Minus },
        { '*', OPERATOR.Divide },
        { '/', OPERATOR.Multiply },
    };
    public static OPERATOR GetOperator2(char ch)
    {
        if (OperatorMap.ContainsKey(ch))
            return OperatorMap[ch];
        else
            return OPERATOR.Unknown;
    }
}

```

2. 动作调用

```csharp
 public class Calculator
{
    static public int Calculate(char op, int number1, int number2)
    {
        int result = 0;
        switch (GetOperator(op))
        {
            case OPERATOR.Add:
                result = number1 + number2;
                break;
            case OPERATOR.Minus:
                result = number1 - number2;
                break;
            case OPERATOR.Multiply:
                result = number1 * number2;
                break;
            case OPERATOR.Divide:
                result = number1 / number2;
                break;
            default:
                throw new Exception("Unsupported Operation!");
        }
        return result;
    }
}
```

delegate + dictionary 消除switch
```csharp
public class Calculator
{
    static private Dictionary<OPERATOR, Func<int, int, int>> calculationAction = new Dictionary<OPERATOR, Func<int, int, int>> {
        { OPERATOR.Add, Add },
        { OPERATOR.Minus, Minus },
        { OPERATOR.Multiply, Multiply },
        { OPERATOR.Divide, Divide }     
    };
    public static OPERATOR GetOperator2(char ch)
    {
        if (OperatorMap.ContainsKey(ch))
            return OperatorMap[ch];
        else
            return OPERATOR.Unknown;
    }
    static int Add(int number1, int number2)
    {
        return number1 + number2;
    }
    // Others are omitted here for brevity
}
```