// 按值传递引用类型

class PassingRefByVal
{
    static void Change(int[] pArray)
    {
        pArray[0] = 888;  // This change affects the original element.
        pArray = new int[5] {-3, -1, -2, -3, -4};   // This change is local.
        System.Console.WriteLine("Inside the method, the first element is: {0}", pArray[0]);
    }

    static void Main()
    {
        int[] arr = {1, 4, 5};
        System.Console.WriteLine("Inside Main, before calling the method, the first element is: {0}", arr [0]);

        Change(arr);
        System.Console.WriteLine("Inside Main, after calling the method, the first element is: {0}", arr [0]);
    }
}
/* Output:
    Inside Main, before calling the method, the first element is: 1
    Inside the method, the first element is: -3
    Inside Main, after calling the method, the first element is: 888
*/

// 数组 arr 属于引用类型，传递给不含 ref 参数的方法。 
// 在这种情况下，将向该方法传递一个指向 arr 的引用副本。 输出表明，此方法可以更改数组元素的内容，在本示例中将 1 更改为了 888。 
// 但是，通过在 Change 方法内使用 new 运算符来分配一份新的内存可使变量 pArray 引用新的数组。 
// 因此，此后的任意更改都不会影响创建于 Main 内的原始数组 arr。
// 实际上，本示例创建了两个数组，一个在 Main 方法内，另一个在 Change 方法内

//----------------------------------------------------------------------------------------

// 按引用传递引用类型
class PassingRefByRef
{
    static void Change(ref int[] pArray)
    {
        // Both of the following changes will affect the original variables:
        pArray[0] = 888;
        pArray = new int[5] {-3, -1, -2, -3, -4};
        System.Console.WriteLine("Inside the method, the first element is: {0}", pArray[0]);
    }

    static void Main()
    {
        int[] arr = {1, 4, 5};
        System.Console.WriteLine("Inside Main, before calling the method, the first element is: {0}", arr[0]);

        Change(ref arr);
        System.Console.WriteLine("Inside Main, after calling the method, the first element is: {0}", arr[0]);
    }
}
/* Output:
    Inside Main, before calling the method, the first element is: 1
    Inside the method, the first element is: -3
    Inside Main, after calling the method, the first element is: -3
*/

// 方法内所作的所有更改都将影响 Main 中的原始数组。 
// 事实上，将使用 new 运算符重新分配原始数组。 
// 因此，调用 Change 方法后，对 arr 的任何引用都将指向 Change 方法中创建的五元素数组。


//----------------------------------------------------------------------------------------

class SwappingStrings
 {
     static void SwapStrings(ref string s1, ref string s2)
     // The string parameter is passed by reference.
     // Any changes on parameters will affect the original variables.
     {
         string temp = s1;
         s1 = s2;
         s2 = temp;
         System.Console.WriteLine("Inside the method: {0} {1}", s1, s2);
     }

     static void Main()
     {
         string str1 = "John";
         string str2 = "Smith";
         System.Console.WriteLine("Inside Main, before swapping: {0} {1}", str1, str2);

         SwapStrings(ref str1, ref str2);   // Passing strings by reference
         System.Console.WriteLine("Inside Main, after swapping: {0} {1}", str1, str2);
     }
 }
 /* Output:
     Inside Main, before swapping: John Smith
     Inside the method: Smith John
     Inside Main, after swapping: Smith John
*/