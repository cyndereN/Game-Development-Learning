#pragma once
#include <iostream>
#include <functional>

//首先声明一点，UE4底层不是用std::bind写的，他自己有别的写法，这里只是模拟一下UE4的代理而已
//这个命名空间是防止 写类的时候重复定义
namespace WorkSpace1
{
	//测试类
	class TestA
	{
	public:
		int AddNum(int a, int b)
		{
			const int c = a + b;
			std::cout << "a + b=" << c << std::endl;
			return c;
		}

		int LessNum(int a, int b)
		{
			const int c = a - b;
			std::cout << "a-b=" << c << std::endl;
			return c;
		}
	};
	//TestA类成员函数指针
	using RawFunMethodNoParam = int(TestA::*)();
	//普通的函数指针
	typedef int(*FunMethodNoParam)();
	//这是一个无返回值 无参数的的代理类
	class FDelegateNoparam
	{
	public:
		//绑定全局函数
		void BindGlobalFun(FunMethodNoParam FunPtr)
		{
			Fun = std::bind(FunPtr);
		}
		//绑定一个类的成员函数
		void BindRaw(TestA* UserClass, RawFunMethodNoParam FunPtr)
		{
			Fun = std::bind(FunPtr, UserClass);
		}
		//判断单播是否被绑定了
		bool IsBound() const
		{
			return Fun ? true : false;
		}
		//执行
		void Exectue()
		{
			Fun();
		}
		//如果绑定就执行
		bool ExectueIfBound()
		{
			if (IsBound())
			{
				Exectue();
				return true;
			}
			return false;
		}
		//清除绑定
		void Clear()
		{
			Fun=nullptr;
		}
	private:
		//函数指针存储在Fun这个变量里面
		std::function<int()> Fun; 
	};

/************************************************************************/
/*          这是一个无返回值 有两个参数的的代理类（非模板类）			 */
/************************************************************************/
	//TestA类成员函数指针 两个参数都是int
	using RawFunMethodTwoParams = int(TestA::*)(int,int);
	//普通的函数指针  两个参数都是int
	typedef int(*FunMethodTwoParams)(int, int);

	class FDelegateTwoparams
	{
	public:
		//绑定全局函数
		void BindGlobalFun(FunMethodTwoParams FunPtr, int a, int b)
		{
			Fun = std::bind(FunPtr, a, b);
		}
		//绑定一个类的成员函数
		void BindRaw(TestA* UserClass, RawFunMethodTwoParams FunPtr, int a, int b)
		{
			Fun = std::bind(FunPtr, UserClass, a, b);
		}
		//判断单播是否被绑定了
		bool IsBound() const
		{
			return Fun ? true : false;
		}
		//执行
		void Exectue()
		{
			Fun();
		}
		//如果绑定就执行
		bool ExectueIfBound()
		{
			if (IsBound())
			{
				Exectue();
				return true;
			}
			return false;
		}
		//清除绑定
		void Clear()
		{
			Fun = nullptr;
		}
	private:
		//函数指针存储在Fun这个变量里面
		std::function<int()> Fun;
	};
};
