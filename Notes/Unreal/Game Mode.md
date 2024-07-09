## AGameModeBase

## AGameMode

AGameMode 是 AGameModeBase 的子类，拥有一些额外的功能支持多人游戏和旧行为。
所有新建项目默认使用 AGameModeBase。
如果需要此额外行为，可切换到从 AGameMode 进行继承。如从 AGameMode 进行继承，也可从 AGameState 继承游戏状态（其支持比赛状态机）。

##Game Mode 蓝图

可创建派生自 Game Mode 类的蓝图，并将它们用作项目或关卡的默认 Game Mode。
派生自 Game Mode 的蓝图可进行以下默认设置：

- 默认 Pawn 类
- HUD 类
- 玩家控制器 类
- Spectator 类
- Game State 类
- Player State 类

此外，Game Mode 的蓝图十分实用，因为它们无需调整代码即可启用变量调整。因此可用于使单一 Game Mode 适用到多个不同关卡，无需使用硬编码资源引用或为每次调整请求工程支持和代码修改。
