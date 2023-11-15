// 子类可以赋值给父类对象，父类对象可以强制转化为子类对象
// 里氏替换规则直观理解：“男人是人”，但是“人是男人”不对
// 当派生类对象赋于基类类型，派生类数据结构一次对应于基类数据结构，而派生类拥有的自己的数据将不可见
// 当基类的对象试图转换为派生类型时，将出现基类对象的数据无法填充完派生类的所有数据结构，这就造成了它将无法完成派生类定义的功能，编译器甚至会报错
// 强制类型转换属于基类到派生类的过程

// 1
Student stu = new Student(); // 创建一个子类对象
Person per = stu; // 子类对象赋给父类对象
per.show();  // 调用父类方法

// 2
Person per = new Student();  
Student stu = (Student) per; // 强制转换成子类

// 3
// - is: 返回bool类型，指示是否可以做这个转换
// - as: 如果转换成功则返回对象否则返回null
Person per = new Student();
if (per is Teacher){
    ((Teacher)per).TeacherHello();
}
else{
    Console.WriteLine("转换失败");
}

Student s = per as Student;

if(s != null){
    s.StudentHello();
}
else {
    Console.WriteLine("转换失败");
}
