// CustomDelegate.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <functional> //std::bind
#include <vector>
#include "3.用模板实现代理（包括单播和多播）.h"
using namespace DelegateSpace;
class EEE
{
public:
	void AAA()
	{
		std::cout<< 123 << std::endl;
	}
	void AAw(int a,int b)
	{
		std::cout << 963 << std::endl;
	}
	void AAwee(float a, int b)
	{
		std::cout << 963 << std::endl;
	}
	void AAfff(float a, int b)
	{
		std::cout << 963 << std::endl;
	}
	float rrr(float a, int b)
	{
		std::cout << "ssff" << std::endl;
		return 0.2f;
	}
};
DECLARE_DELEGATE_TwoParams(TestDelegate, int, int);
DECLARE_MULTICAST_DELEGATE_TwoParams(TestMuDelegate, float, int);
int main()
{
	//测试BindRaw
	TestDelegate s;
	EEE e;
	s.BindRaw(&e, &EEE::AAw, 2, 3);
	s.Execute();

	//测试BindSP
	TestDelegate XX;
	std::shared_ptr<EEE> RE = std::make_shared<EEE>();
	XX.BindSP(RE, &EEE::AAw, 2, 3);
	XX.Execute();

	//测试BindLambda
	TestDelegate XX2;
	XX2.BindLambda([](int, int) {std::cout << "ssff" << std::endl; }, 2, 3);
	XX2.Execute();

	//测试多播
	TestMuDelegate MuDelegate;
	EEE ggg;
	MuDelegate.AddRaw(&ggg,&EEE::AAwee,3.f,5);
	MuDelegate.AddRaw(&ggg, &EEE::AAfff, 3.f, 5);
	MuDelegate.Broadcast();
	system("pause");
}
