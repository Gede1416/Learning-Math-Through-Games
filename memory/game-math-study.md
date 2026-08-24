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
- Day 1：向量定义与基本运算 🚧 — 开课：坏代码场景「斜向移动变快」（输入向量未归一化）已给出，苏格拉底问题待学生回答；作业 Vector3.cs（Magnitude/SqrMagnitude/Normalized/加减/标量乘）已布置

**下一步**：等学生回答 Day 1 苏格拉底问题 → 标准解 → 验收作业 → git commit + push。

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

**How to apply:** 按用户节奏推进，每次聚焦一个概念。当前阶段一 Day 1 向量，等学生回答后进入标准解与作业验收。
