# 15 游戏引擎的Gameplay玩法系统基础

Gameplay（玩法系统）没有清晰的边界，和不同游戏的需求有强关系。

## 游戏性课程框架

## 事件机制

本质是信息的发送和接受

Object Lifespan and Callback Safety

- Strong Ref: 解决需要响应callback时，引用已经被销毁了。表达内存锁定关系。Make sure to unregister callback function before deleting objs.

- Weak Ref: 检验是否已经被销毁，避免内存长时间被占用造成浪费


Event Dispatch

- Send event to appropriate destination

## 游戏逻辑与脚本系统

热更新的需求。

Script is converted to bytecode by a compiler first then run on a virtual machine