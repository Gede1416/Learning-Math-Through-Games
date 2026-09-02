---
name: game-math-study
description: 用户正在学习游戏程序员的数学课，以 Milo Yip 书单"数学"板块为体系，五阶段推进
metadata:
  type: project
---

用户当前学习目标：基于 Milo Yip 书单"游戏程序员的数学课"板块，系统学习游戏开发数学，核心是**把数学概念转化为可运行的游戏代码**。

核心教材（按优先级）：
1. 《3D Math Primer for Graphics and Game Development》(2nd) — Dunn & Parberry（基石）
2. 《Essential Mathematics for Games and Interactive Applications》(3rd) — Van Verth & Bishop
3. 《Foundations of Game Engine Development, Vol 1》— Lengyel
4. 《Mathematics for 3D Game Programming and Computer Graphics》(3rd) — Lengyel（补充）
5. 《Game Physics》— Eberly / 《实时碰撞检测算法技术》— Ericson

## 已完成

- 2026-08-25：课程初始化 —— 项目结构（docs/数学、Homework/数学、MathLibrary 约定、memory、00-我的回答.md）、学习计划-数学.md、游戏数学导师-prompt.md、git 仓库（remote = Learning-Math-Through-Games）、首次推送。

## 进行中

### 阶段一：线性代数基础（2026-08-25 开始）
教材：《3D Math Primer》Ch 1-6 + 《Essential Math》Ch 2-3
- Day 1 ✅ 向量定义与基本运算（2026-08-26 完成）— 斜向移动加速 bug：input 模长 √2 → 速度 5√2 ≈ 7.07 m/s；标准解=归一化 v̂=v/|v|。作业 Vector3.cs 7/7 PASS。
- Day 2 ✅ 点积 Dot Product（2026-08-27 完成）— 警戒锥形 bug：dot = |toPlayer|·cosθ 混入距离，5 米处 80° 误报、0.5 米处漏报；标准解=方向归一化后 cosθ 与距离无关。学生两次回答偏（"方向反了"误判、"0-1 过早/1-∞ 过晚"结论反），代码练习 IsInCone 自悟归一化；作业 DotProduct.cs 10/10 PASS（累计 17/17）。
- Day 3 ✅ 叉积 Cross Product（2026-08-28 完成）— 左右转向 bug：Unity 左手系下 forward×toPlayer 的 y>0 ⟺ 玩家在右，分支映射反了；标准解=交换操作数或翻转分支。学生回答偏（"共线没判断"边缘情况有效但非主因、"顺序从右到左"表述乱），通过实现 Side（交换操作数）自悟符号映射；概念追问"参考系/手性"已澄清（手性是坐标系属性，差异来自绕序约定与轴摆放映射）；作业 CrossProduct.cs 7/7 PASS（累计 24/24）。

- Day 4 ✅ 矩阵与向量乘法（2026-08-28 完成）— 学生正确定位原坏代码把 `Mv` 写成等价的 `Mᵀv`；掌握“矩阵每行与列向量点积”及“矩阵列是变换后的基轴”。作业第一次本轮 2/5 PASS，角度转弧度比例写反；经 `h/2π = degrees/360` 引导后自行修正，第二次本轮 5/5 PASS。

- Day 5 ✅ 阶段一综合小考（2026-08-29 完成）— 2D 自动炮塔场景综合使用旋转矩阵、向量模长、点积余弦和二维叉积。学生首次文字回答指出目标向量未单位化，随后通过代码实现 `Vector2.Cos` 与 `Vector2.LRF` 完成两个修复；本轮 6/6 PASS。工程提醒：零向量需显式处理，辅助函数命名和调试输出可清理。

**阶段一完成**：向量、点积、叉积、矩阵与向量乘法、综合小考全部通过。

- 阶段二 Day 1 ✅ 线性变换与错切（2026-08-29 完成）— 风吹草叶场景中，学生正确手算顶部 `(0,2)→期望(1,2)/实际(0,2)`、根部 `(1,0)→期望(1,0)/实际(1,0.5)`，并修复为 `x'=x+k·y, y'=y`；本轮 4/4 PASS。已讲解矩阵列是变换后的基向量，以及原错误代码实际是纵向错切。

- 阶段二 Day 2 ✅ 平移与齐次坐标（2026-09-02 完成）— 学生正确算出点 `(10,2,6)`、错误方向实际 `(10,2,6)`、期望 `(0,0,1)`。首次代码虽 3/3 PASS，但通过忽略参数、硬编码零绕过 w 语义；经“两点之差的 w”引导后，用 `w=1/0` 正确区分点和方向，第二次实现 3/3 PASS。后续统一矩阵 API 时将裸 `Transform(v,w)` 收为私有实现，公开调用固定使用 `TransformPoint/TransformDirection`。已讲明平移在 3D 中是仿射变换，在齐次 4D 中可统一为矩阵乘法。

- 阶段二 Day 3 ✅ 变换组合与顺序（2026-09-02 完成）— 学生正确判断列向量约定下先旋转后平移为 `T·R`，代码数值首次达到 3/3 PASS；随后发现其把 localPoint 写成 w=0，因纯旋转无平移而被测试掩盖。经提醒后修为点 w=1，顺序与齐次语义均合格。标准推导：`R·T·p=(0,0,-11)` 为绕世界原点公转，`T·R·p=(10,0,-1)` 为角色位置自转。

**阶段二 Day 4 ✅ 坐标系转换（2026-09-02 完成）**：第一次尝试把相对向量写反，2/3 PASS。第二次恢复 `worldPoint-cameraPosition`，但通过翻转共享 `RotationY` 得到原测试表面 3/3；新增统一约定保护后为 3/4。学生随后将相对向量修为 `w=0`，在矩阵 API 合并中恢复唯一正向旋转，并用纯旋转 `R⁻¹=Rᵀ` 实现 `TransformWorldPointToCamera`。当前课程 **4/4 PASS**，阶段二完成。

**第七轮扩展练习插曲（2026-09-02）**：学生完善旧 `Mat4x4` 后首次单独测试 5/6 PASS。TODO 6 最初旋转 translation 后再加未旋转的 localOffset，同时旧测试骨架混用了相反的 Y 轴旋转方向。统一坐标约定并修正为 `translation + rot·localOffset` 后独立复验 6/6。随后按用户要求将 `Mat4x4` 与 `Matrix4x4` 合并为唯一不可变类型；扩展测试迁到 `07-平移齐次坐标-扩展/Matrix4x4ExtendedTests.cs`，迁移后仍 **6/6 PASS**。

**矩阵 API 统一决策（2026-09-02）**：唯一类型为 `StudyNotes.Homework.Math.LinearAlgebra.Matrix4x4`。创建方法固定为 `Identity`、`CreateTranslation`、`CreateRotationYDegrees`；变换固定为实例 `TransformPoint/TransformDirection`；矩阵积使用 `Multiply(left,right)`；逆旋转显式写 `Transpose()`。删除重复 `Mat4x4`、各课程自建 RotationY、公开裸 `Transform(v,w)` 和含三个不同语义参数的静态 TransformPoint。

**教学类型决策（2026-08-29）**：阶段二 Day 1 使用 `Vector2 + Matrix2x2` 只是为了把错切降维到 X/Y 平面，突出基向量列的几何意义，不代表后续课程改走二维路线。自 Day 2 起恢复项目已有 `Vector3`，引入 `Matrix4x4` 教平移与齐次坐标；避免继续扩展重复的 Vector2 数学库，并逐步对接 Unity 式三维变换。

## 整体路线（五阶段）

- 阶段一：线性代数基础（向量/点积/叉积/矩阵）✅
- 阶段二：几何变换（平移旋转缩放/齐次坐标/变换组合）✅
- 阶段三：三维旋转（欧拉角/四元数/插值）
- 阶段四：几何与碰撞（射线平面/AABB-OBB-球/分离轴）
- 阶段五：物理数学初步（刚体/碰撞响应/积分器）

## 教学方式与约定

- 苏格拉底式：坏代码（≤30 行）→ 回答 → 标准解；每次聚焦一个概念
- 游戏场景绑定；引用教材原文（标注章节）；每节以追问落地结尾
- 作业：`Homework/数学/{两位数字序号}-{概念}/{类名}.cs`，序号补零为 `01`、`02`……以便排序；5-10 分钟
- 笔记：docs/数学/{编号}-{中文}-{英文}.md
- 三维坐标：`+X` 右、`+Y` 上、`+Z` 前，列向量 `v'=M·v`；Y 轴 `+90°` 映射 `+Z→+X`、`+X→-Z`。代码、测试、手算和笔记统一遵守，例外必须显式标注并用基向量校验
- 矩阵 API：只使用 `StudyNotes.Homework.Math.LinearAlgebra.Matrix4x4`；创建、点、方向、矩阵积和转置分别使用语义明确的统一方法，不再新增缩写类型或公开裸 `w` 接口
- git：`feat: 数学/{小章节} {日期}`，提交后 git push（代理报错 → 方案 A 直连；超时 → Clash 7897）

**Why:** 用户希望按 Milo Yip 书单体系系统补齐游戏开发数学，与软件工程学习（[[software-engineering-study]]）同一套教学法。

**How to apply:** 按用户节奏推进，每次聚焦一个概念。阶段二已完成，下一教学起点为阶段三 Day 1 欧拉角与旋转顺序；开始新课前仍保持默认入口只运行第九轮 4 项测试，除非用户明确要求回归测试。
