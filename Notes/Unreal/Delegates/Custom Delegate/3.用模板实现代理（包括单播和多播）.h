#pragma once
#include <memory>

namespace DelegateSpace
{
	//这是一个模板类函数指针 Class表示类  FuncType表示函数指针的类型
	template < typename Class, typename FuncType>
	struct TMemFunPtrType;

	//这是一个名为Type类型的隶属于Class类的成员函数指针，它的参数是ArgTypes...
	template<typename Class, typename RetType, typename... ArgTypes>
	struct TMemFunPtrType<Class, RetType(ArgTypes...)>
	{
		typedef RetType(Class::*Type)(ArgTypes...);
	};

	template <typename FuncType>
	struct TCommonMemFunPtrType;
	//这是一个名为Type类型的普通函数指针，它的参数是ArgTypes...
	template<typename RetType, typename... ArgTypes>
	struct TCommonMemFunPtrType<RetType(ArgTypes...)>
	{
		typedef RetType(*Type)(ArgTypes...);
	};

	//这是一个单播代理的模板类RetValType表示返回值，ParamTypes表示可变参数
	template <typename RetValType, typename... ParamTypes>
	class TBaseDelegate
	{
	public:
		template<typename UserClass>
		void BindRaw(UserClass* MyUserClass,
			typename TMemFunPtrType<UserClass, RetValType(ParamTypes...)>::Type FunPtr,
			ParamTypes...Vars)
		{
			fun = std::bind(FunPtr, MyUserClass, Vars...);
		}
		template<typename UserClass>
		void BindSP(std::shared_ptr<UserClass> MyUserClass,
			typename TMemFunPtrType<UserClass, RetValType(ParamTypes...)>::Type FunPtr,
			ParamTypes...Vars)
		{
			fun = std::bind(FunPtr, MyUserClass, Vars...);
		}
		//绑定全局函数
		void BindCommon(
			typename TCommonMemFunPtrType<RetValType(ParamTypes...)>::Type FunPtr,
			ParamTypes...Vars)
		{
			fun = std::bind(FunPtr, Vars...);
		}
		template<typename FunctorType>
		void BindLambda(FunctorType&& FunPtr,ParamTypes...Vars)
		{
			fun = std::bind(FunPtr, Vars...);
		}

		//这两个方法没法写了。因为这需要跟UE4的源码进行结合才行
		//void BindUObject();
		//void BindUFuntion()

		bool IsBound()const
		{
			return fun ? true : false;
		}
		void Execute()
		{
			fun();
		}
		bool ExecuteIfBound()
		{
			if (IsBound())
			{
				Execute();
				return true;
			}
			return false;
		}		
		//清除绑定
		void Clear()
		{
			fun = nullptr;
		}
	private:
		std::function<RetValType()> fun;
	};

//分别是带有0 1 2 3 4个参数的单播代理宏定义 且都无返回值  
#define DECLARE_DELEGATE(DelegateName) class DelegateName: public TBaseDelegate<void>{};
#define DECLARE_DELEGATE_OneParam(DelegateName,ParamType1) class DelegateName: public TBaseDelegate<void,ParamType1>{};
#define DECLARE_DELEGATE_TwoParams(DelegateName,ParamType1,ParamType2) class DelegateName: public TBaseDelegate<void, ParamType1, ParamType2>{};
#define DECLARE_DELEGATE_ThreeParams(DelegateName,ParamType1,ParamType2,ParamType3) class DelegateName: public TBaseDelegate<void, ParamType1, ParamType2, ParamType3>{};
#define DECLARE_DELEGATE_FourParams(DelegateName,ParamType1,ParamType2,ParamType3,ParamType4) class DelegateName: public TBaseDelegate<void, ParamType1, ParamType2, ParamType3,ParamType4>{};
//...后面可以接着写 5 6 7....个参数
//分别是带有0 1 2 3 4个参数的单播代理宏定义 且都有返回值  
#define DECLARE_DELEGATE_RetVal(RetType,DelegateName) class DelegateName: public TBaseDelegate<RetType>{};
#define DECLARE_DELEGATE_RetVal_OneParam(RetType,DelegateName,ParamType1) class DelegateName: public TBaseDelegate<RetType,ParamType1>{};
#define DECLARE_DELEGATE_RetVal_TwoParams(RetType,DelegateName,ParamType1,ParamType2) class DelegateName: public TBaseDelegate<RetType, ParamType1, ParamType2>{};
#define DECLARE_DELEGATE_RetVal_ThreeParams(RetType,DelegateName,ParamType1,ParamType2,ParamType3) class DelegateName: public TBaseDelegate<RetType, ParamType1, ParamType2, ParamType3>{};
#define DECLARE_DELEGATE_RetVal_FourParams(RetType,DelegateName,ParamType1,ParamType2,ParamType3,ParamType4) class DelegateName: public TBaseDelegate<RetType, ParamType1, ParamType2, ParamType3,ParamType4>{};
//...后面可以接着写 5 6 7....个参数



/************************************************************************/
/* 下面是多播的模板基类 ，再次声明UE4不是这样写的，我这里只是仿着UE4写一个提供给大家理解的 */
/************************************************************************/

	template <typename RetValType, typename... ParamTypes>
	class TBaseMutilCastDelegate
	{
	public:
		template<typename UserClass>
		void AddRaw(UserClass* MyUserClass,
			typename TMemFunPtrType<UserClass, RetValType(ParamTypes...)>::Type FunPtr,
			ParamTypes...Vars)
		{
			funArray.push_back(std::bind(FunPtr, MyUserClass, Vars...));
		}
		template<typename UserClass>
		void AddSP(std::shared_ptr<UserClass> MyUserClass,
			typename TMemFunPtrType<UserClass, RetValType(ParamTypes...)>::Type FunPtr,
			ParamTypes...Vars)
		{
			funArray.push_back(std::bind(FunPtr, MyUserClass, Vars...));
		}
		void AddCommon(
			typename TCommonMemFunPtrType<RetValType(ParamTypes...)>::Type FunPtr,
			ParamTypes...Vars)
		{
			funArray.push_back(std::bind(FunPtr, Vars...));
		}
		template<typename FunctorType>
		void AddLambda(FunctorType&& FunPtr, ParamTypes...Vars)
		{
			funArray.push_back(std::bind(FunPtr, Vars...));
		}

		//这两个方法没法写了。因为这需要跟UE4的源码进行结合才行
		//void AddUObject();
		//void AddUFuntion()
		void Broadcast()
		{
			typename std::vector<std::function<RetValType()>>::iterator it = funArray.begin();
			for (; it != funArray.end(); ++it)
			{
				(*it)();
			}
		}
		//UE4还有一个Remove方法，我这里就不写了
		//因为他存了一个代理的句柄 我这里没有这个句柄，
		//就没法移除单个绑定。虽然可以通过别的方法实现，但是意义不大，这里只是提示一下。
		//void Remove();

		//清除所有的绑定
		void RemoveAll()
		{
			typename std::vector<std::function<RetValType()>>::iterator it = funArray.begin();
			for (;it!=funArray.end();++it)
			{
				//解除每个函数的绑定
				(*it)=nullptr;
			}
			//清空vector
			funArray.clear();
			funArray.swap(std::vector<std::function<RetValType()>>());
		}
	private:
		std::vector<std::function<RetValType()>> funArray;
	};

	//分别是带有0 1 2 3 4个参数的多播代理宏定义
	//注意：
	//我这里的多播代理，只能多播返回值一致的函数，因为我没有对不同返回值进行处理
	//我们看std::vector<std::function<RetValType()>> funArray;这个定义就知道 
	//返回值只有RetValType这一种，所以在绑定的时候也只能绑定一种
	//你可以自己对返回值这块做一个动态的变换,当然对应的就不能使用std::vector容器去装这些仿函数（std::function<xxxx>）了。

#define DECLARE_MULTICAST_DELEGATE(DelegateName) class DelegateName: public TBaseMutilCastDelegate<void>{};
#define DECLARE_MULTICAST_DELEGATE_OneParam(DelegateName,ParamType1) class DelegateName: public TBaseMutilCastDelegate<void,ParamType1>{};
#define DECLARE_MULTICAST_DELEGATE_TwoParams(DelegateName,ParamType1,ParamType2) class DelegateName: public TBaseMutilCastDelegate<void, ParamType1, ParamType2>{};
#define DECLARE_MULTICAST_DELEGATE_ThreeParams(DelegateName,ParamType1,ParamType2,ParamType3) class DelegateName: public TBaseMutilCastDelegate<void, ParamType1, ParamType2, ParamType3>{};
#define DECLARE_MULTICAST_DELEGATE_FourParams(DelegateName,ParamType1,ParamType2,ParamType3,ParamType4) class DelegateName: public TBaseMutilCastDelegate<void, ParamType1, ParamType2, ParamType3,ParamType4>{};
//...后面可以接着写 5 6 7....个参数

//UE4的动态代理涉及的跟蓝图的反射，这里就没法去举例了 
};

