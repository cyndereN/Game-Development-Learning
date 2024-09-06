#pragma once

typedef int(*FunMethod)(int, int);//定义这个 int(*)(int, int) 类型的函数指针名称为FunMethod
using FunMethod1 = int(*)(int, int);//定义这个 int(*)(int, int) 类型的函数指针名称为FunMethod1

int AddNum(int a, int b)
{
	return a+b;
}
