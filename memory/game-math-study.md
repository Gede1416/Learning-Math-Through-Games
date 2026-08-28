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

**下一步**：阶段二 Day 2 → 平移与齐次坐标，武器挂点/层级变换场景。

## 整体路线（五阶段）

- 阶段一：线性代数基础（向量/点积/叉积/矩阵）🚧 当前
- 阶段二：几何变换（平移旋转缩放/齐次坐标/变换组合）
- 阶段三：三维旋转（欧拉角/四元数/插值）
- 阶段四：几何与碰撞（射线平面/AABB-OBB-球/分离轴）
- 阶段五：物理数学初步（刚体/碰撞响应/积分器）

## 教学方式与约定

- 苏格拉底式：坏代码（≤30 行）→ 回答 → 标准解；每次聚焦一个概念
- 游戏场景绑定；引用教材原文（标注章节）；每节以追问落地结尾
- 作业：Homework/数学/{轮次}/{类名}.cs，5-10 分钟
- 笔记：docs/数学/{编号}-{中文}-{英文}.md
- git：`feat: 数学/{小章节} {日期}`，提交后 git push（代理报错 → 方案 A 直连；超时 → Clash 7897）

**Why:** 用户希望按 Milo Yip 书单体系系统补齐游戏开发数学，与软件工程学习（[[software-engineering-study]]）同一套教学法。

**How to apply:** 按用户节奏推进，每次聚焦一个概念。阶段一 Day 4 已完成；下次从 Day 5 综合小考开始。默认测试入口只运行当前课程，除非用户明确要求回归测试。
