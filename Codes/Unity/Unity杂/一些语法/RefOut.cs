
// 使用ref参数，在方法执行完成后，对参数的改变可以反映到变量上去
 
// 传入参数的时候必须给参数赋值（start方法里边的a=0）
// 调用方法是也必须加上ref
private void Start()
{
    int a = 0, b = 0;
    test(ref a, b);
    Debug.LogError(a + "   " + b);
}
public void test(ref int a, int b)
{
    a = 10;
    b = 10;
}

// out参数
// 在方法调用前，可以不对参数赋值
// 在方法内部，必须对out修饰的参数赋值
private void Awake()
{
    int a;
    test2(out a);
    Debug.LogError(a);
}
public void test2(out int a)
{
    a = 10;
}
