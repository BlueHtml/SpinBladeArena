﻿# 旋刃竞技场 [![main](https://github.com/sdcb/blade/actions/workflows/build-container.yml/badge.svg)](https://github.com/sdcb/blade/actions/workflows/build-container.yml) [![QQ](https://img.shields.io/badge/QQ_Group-495782587-52B6EF?style=social&logo=tencent-qq&logoColor=000&logoWidth=20)](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=mma4msRKd372Z6dWpmBp4JZ9RL4Jrf8X&authKey=gccTx0h0RaH5b8B8jtuPJocU7MgFRUznqbV%2FLgsKdsK8RqZE%2BOhnETQ7nYVTp1W0&noverify=0&group_code=495782587)

**旋刃竞技场**是一个多人在线对战游戏，玩家可以在游戏中选择不同的成长路线，与其他玩家（或者机器人）进行对战，体验地址：[https://blade.starworks.cc:88](https://blade.starworks.cc:88)。

旋刃竞技场是一个基于`ASP.NET Core`和`SignalR`实现了多人在线对战的游戏，游戏的画面使用`HTML Canvas`实现。

游戏的前端使用`TypeScript`编写，后端使用`C#`编写，游戏的逻辑全部在后端实现，前端只负责显示游戏画面和接收用户输入。

## 游戏部署

### `Docker`部署

使用`Docker`部署游戏非常简单，只需要在服务器上安装`Docker`，然后执行以下命令即可：

```bash
docker run --name blade -d --restart unless-stopped -p 8080:8080 -e ServerFPS=30 sdflysha/blade
```

其中：

* `ServerFPS`是游戏服务器的帧率，默认为60，可以根据服务器性能和宽带调整。

### 手动编译部署

如果你想手动编译部署，最简单的方式是使用`Visual Studio 2022`打开项目，然后直接运行项目即可。

运行前可能发现`typescript`文件没有编译，这时候你需要在`wwwroot`目录下执行以下命令：

```powershell
tsc
```

然后刷新页面即可看到游戏的画面。

## 游戏玩法

**旋刃竞技场**是一款动作策略大型多人在线游戏，玩家的目标是通过巧妙的移动和战略部署，利用旋刃击败对手并生存下来。

### 开始游戏
游戏开始前，玩家需要输入一个昵称，这将是其他玩家识别你的唯一标识。输入昵称后，玩家将被随机放置在一个多人在线的竞技场地图中。

### 控制方式
玩家通过鼠标或触屏操作控制角色的移动。点击或触摸屏幕的任何位置，游戏中的角色将会向这个点移动。玩家需要灵活控制角色的移动来追逐或逃避对手，或是触碰散落在地图上的奖励。

### 吃奖励
地图上随机散布着多种奖励，这些奖励可以增强玩家的属性，如生命值、移动速度、旋刃数量等。玩家需要移动自己的角色以碰撞这些奖励，每种奖励的具体效果和可能的负面效果请参考游戏规则中的“奖励机制”。

### 战斗
每个玩家开始时至少拥有一把旋刃。通过控制旋刃与其他玩家的旋刃相撞或直接攻击其他玩家，可以削减对方的生命值。当玩家的生命值降到0时，玩家死亡，游戏结束。玩家可以通过吃奖励增加旋刃数量和伤害，提高战斗力。

### 策略选择
玩家可以根据自己的喜好选择不同的成长路线，如专注于增加旋刃数量进行大范围攻击，或增加旋刃速度与伤害进行快速精确打击，又或是增加生命值和移动速度以增强生存能力。

每个玩家需要结合实时的战场情况调整自己的策略，利用地图环境和奖励机制，智取对手，最终成为场上的赢家。

#### 快速反应与精确操作
旋刃竞技场的胜负往往在一瞬之间，敏捷的操作和对战场的准确判断是成为优胜者不可或缺的能力。每一次操作都可能成为转折点，选择在何时战斗、何时撤退，会直接影响游戏的最终结果。

现在，让我们开始游戏，见证谁能在旋刃竞技场中笑到最后！

## 游戏规则

### 玩家属性表

| 属性名   | 简介                                             | 初始值     |
| -------- | ------------------------------------------------ | ---------- |
| 生命值   | 玩家的生命值，当生命值为0时，玩家死亡。          | 1          |
| 旋刃数量 | 玩家可以操控的旋刃数量，为0时无法攻击。          | 1          |
| 旋刃速度 | 玩家旋刃的移动速度，速度快易攻击但可能错失机会。 | 每秒10度   |
| 移动速度 | 玩家的移动速度，速度快便于走位或躲避攻击。       | 每秒75单位 |
| 旋刃伤害 | 玩家旋刃的伤害值，伤害越高攻击力越强。           | 1          |
| 玩家大小 | 玩家的大小，大小越大越容易被攻击。               | 30         |

### 奖励机制

游戏运行时，会随机生成奖励，散落在地图随机位置（但不会出现在玩家控制位置），玩家可以通过碰撞奖励获得额外的属性加成。

| 奖励名称 | 加成属性                 | 负面效果                      |
|----------|------------------------|-------------------------------|
| 增重     | 大小+10                | 移动速度-5，刀速-5           |
| 变瘦     | 大小-10                | 移动速度+3                   |
| 移速+5   | 移动速度 +5            | 大小-2，刀速-2               |
| 移速+20  | 移动速度 +20           | 大小-4，刀速-4               |
| 刀数     | 旋刃数量 +1            | 生命+1，刀速-1，移速-1       |
| 刀数+3   | 旋刃数量 +3            | 生命+3，刀速-4，移速-4       |
| 刀长+5   | 旋刃长度 +5            | 生命+1，刀速-1，移速-1       |
| 刀长+20  | 旋刃长度 +20           | 生命+3，刀速-4，移速-4       |
| 刀伤     | 旋刃伤害 +1            | 生命+1，刀速-1，移速-1       |
| 刀速+5   | 旋刃旋转速度 +5        | 生命+1，移速-1               |
| 刀速+20  | 旋刃旋转速度 +20       | 生命+1，移速-1               |
| 随机     | 随机一个奖励           | 取决于随机奖励的负面效果     |

#### 注意事项

- 任何时候玩家获得的**刀数奖励时，若玩家无刀，则会先获得一把刀**后再应用相应的加成。
- 某些奖励具有条件性的负面效果，仅在玩家拥有“强大”状态时触发，这些负面效果可能包括大小增加、移动速度和旋刃速度的减少等。

#### 金刀
如果玩家旋刃数<=2，且伤害>=2，那么玩家的旋刃会变成金刃，金刃与普通红刃对刀时，金刃会吃掉红刃，红刃的伤害不会对金刃造成伤害。

金刃的设计是为了增加游戏的趣味性，让玩家在游戏中有更多的策略选择。

### 得分机制
玩家初始分数为1，当玩家击败其他玩家时，会获得被击败玩家的得分的1/3，当玩家被击败时，得分会重置为初始值1。

在战场页面的左上角会按从高到低的顺序显示所有玩家的得分。

### 掉落机制

如果玩家死亡，那么玩家得分的1/3会随机掉落成奖励，会散落在玩家死亡的位置附近，其他玩家可以通过碰撞掉落的奖励获得额外的属性加成。

如果死亡玩家得分为1分以内，则会有1/3的概率掉落奖励，得分为2分以内，则会有2/3的概率掉落奖励。

## AI机器人

游戏中会有AI机器人，AI机器人的属性与玩家相同，但有3种不同的AI风格，我从《三国演义》中选取了一些人物作为AI机器人的昵称，这3种AI风格分别是：

| AI类型     | 风格描述                                                                                       | 对应玩家昵称                                          |
| ---------- | ---------------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| 种田养生型 | 尽量避免与其他玩家发生战斗，会尽量吃奖励，增加自己的属性，提高生存能力。                       | 小乔 刘禅 袁绍 刘表 貂蝉 刘备 糜竺 鲁肃 陶谦 孙尚香   |
| 无脑进攻型 | 尽量追击其他玩家，不顾自身安全，只要有机会就会发动攻击。                                       | 吕布 曹操 张飞 马超 孟获 邢道荣 许褚 张郃 魏延 关羽   |
| 智能进攻型 | 根据自己的属性和其他玩家的属性，智能选择进攻或撤退，以保证自己的生存和提高击败其他玩家的概率。 | 孙权 赵云 诸葛亮 曹仁 司马懿 周瑜 陆逊 姜维 邓艾 钟会 |
