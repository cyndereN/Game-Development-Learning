# 为什么你必须使用 Godot 引擎

分享会把 Godot 和 Unity 对着讲。结论不是「Godot 功能更多」，而是：引擎更轻、资源和节点更顺手，写游戏时的小阻力少，开发方式会更健康。后半段也列了现在还不好用的地方。

## Godot 还是 Unity


### Unity 十败

1. 收费，商业政策不稳定，国内国外区别对待，缺乏信誉。唐奕新：因为封号，买过的插件全没了。爱给网万岁。
2. 太重。做小游戏时，引擎本身反而加重成本。
3. Unity 引擎工程师自己也不会用 Unity 写游戏。
4. Unity 三败，此乃四败。
5. ……

### Godot 十胜

1. 免费，不用注册账号，也不会被封号。做游戏不需要登录。
2. 轻。安装方便、启动方便，各种等待都很短。
3. 引擎贡献者真的用它做游戏，拿出来的功能基本都能用。
4. Godot 三胜，此乃四胜。
5. ……

## 对生活质量的改善

### 资源管理

不为只用一次的资源付管理成本。资源系统少了很多没必要单独存在的 asset，文件好管。

自定义资源和 `.tscn` 用同一套格式存，子资源可以嵌进去，还可以互相引用。

例子 `EdittableTexture`：一个 tscn 里嵌材质，材质里再嵌 Shader 和程序化噪声。Unity 里这是 3 个资源文件加 1 个 prefab；Godot 里 1 个 tscn 就够。

带动画的 prefab 同理。Unity 常见拆法是：1 个 prefab、1 个 Animator Controller、N 个 Animation Clip，再加 1 个 C# 脚本。

**builtin script** 被称作资源系统的最佳用法。几行逻辑不用再配一个 prefab 加一个只有几行的 C# 文件，直接在同一个 tscn 里写匿名脚本，一个场景把功能做完。

唐奕新：三行就能让一个东西转起来，而且不额外产生文件。Unity 里同样的事，至少要新建一个 `.cs`，或者一个 `animation_controller` 再加一个 `animation_clip`。

```gdscript
extends TextureRect
func _process(delta):
    rotation -= delta * 0.3
```

### UI

编辑器和游戏用的是同一套 UI，所以主题、布局和高级控件都能直接用。布局性能好，组件也多。

对照：Unity UI 性能很差；项目中期换 UI 美术，工作量很大。

细节：Container 下面的控件，编辑器里不允许缩放。Ethan：好。

### 节点管理

`UniqueName`（`%`）很大程度替代了 `SerializeField`。脚本里按名字引用节点，不用拖引用。

```gdscript
var blur_panel := %BlurPanel
```

对策划更轻松。策划能一眼看出哪些节点不能乱动、哪些可以调。Inspector 也更干净：露出来的值是用来调效果的，不是动一下就会改逻辑。

也适合做工具软件。UI 好用，引擎还做了软件向的优化，比如低 CPU 占用模式、更适合界面的字体渲染。Dove 的业余项目：tabro。

**内部节点**可以做黑盒。插入子节点时选成内部节点后，节点树里不显示，`GetChildren` 也会跳过。适合用一串节点包一个复杂功能，编辑器自带的高级控件（拾色器、代码编辑器）就是这么做的。

```gdscript
AddChild(new Button(), @internal=InternalMode.Front)
```

### 写代码

GDScript 可以边跑边改，不用等编译，也能和 C# 混用。

Unity 的 editor reload 要额外做很多支持；C# 版本低，用不上新语法糖。Godot 的 C# 版本高。

幻灯片上的对照：Godot 用 `field ??=` 缓存子节点；Unity 要自己写一个可空字段，再在 getter 里 `Find`。

```csharp
// godot
public Node Child => field ??= GetNode("Child");

// unity
Transform cachedChild;
public Transform Child {
    get {
        if (cachedChild is null)
            cachedChild = transform.Find("Child");
        return cachedChild;
    }
}
```

**节点比 GameObject 轻。** 节点甚至可以不带 Transform。纯数据、纯逻辑的类也能丢进场景树，走统一生命周期，不用自己管创建和销毁。

对象池的做法：建一个不挂到主树的游离节点当池子的父节点。对象回池时挂到它下面，少维护一套状态。

引擎开源，极端情况可以自己改。Unity 不开源，有些 bug 在论坛里放很久，只能绕。

### Shader 和小体量 3D

写 Shader 的体验好。Unity 的 ShaderLab 很难用。Godot 用接近 GLSL 的语言，定义清楚，文档清楚。

gdshader 适合风格化渲染。小体量 3D 不比 Unity 差。幻灯片配了方块村庄和热气球线稿两张风格化画面。

## 开发方式会更健康

以前用 Unity 时的习惯：

1. 动画喜欢用代码 tween 或手写函数，不用引擎的动画系统。
2. UI 喜欢代码里建，不爱做 prefab 再挂脚本。
3. 逻辑不愿意用 `SerializeField` 绑对象，不喜欢 C# 和 prefab 一起做功能。

原因是编辑器和代码配合得不好。想按「更健康」的方式做，阻力是一点一点加上去的：等编译、建好几个文件、想文件名、想路径。最后就想扔掉这些步骤，全用代码解决。对工程是有害的。

Godot 的编辑器功能是在减这些阻力。用一段时间后，上面三个习惯都改掉了。

就算继续用「错误」方式，阻力也比 Unity 小：

- 自带 tween，和节点生命周期绑在一起。
- 代码里搭复杂节点树更轻松。
- GDScript 里 `GetNode()` 按路径取节点，路径还能自动补全。

## Godot 与 AI

场景（`.tscn`）、资源、脚本大多是纯文本。diff 清楚，模型容易整份读懂，也容易打小补丁。Unity 也有 YAML，但工程里还有大量 `.meta`、二进制资源和 Package 组合，上下文更碎；asset 一多，噪声也更多。

Godot CLI 启动方便。Agent 可以直接拿它编译、打包、跑自动测试，介入成本比 Unity 低。例子：超写实战斗是 Agent 自己开发、自己测的。

## 使用 Godot 的不便

- 视频不好方便地播。
- UI 的鼠标事件有细节问题：按下按钮之后，指针被另一个控件挡住，再松开，按钮仍然会触发。
- 3D 物理还没打磨好。飞刀大师里，飞刀停止飞行发生在真正碰撞的后一帧。这是新接入的 3D 物理系统的问题。
- 编辑器扩展不方便：gizmo、Inspector 这些。
- 插件还不够多（2800+?）。
- 登录主机平台的难度还未知。
