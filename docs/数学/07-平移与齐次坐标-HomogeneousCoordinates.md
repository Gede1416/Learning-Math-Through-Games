# 07-平移与齐次坐标（Translation & Homogeneous Coordinates）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) Ch 6.1–6.3；《Essential Mathematics for Games》(3rd) Ch 4.6  
> 阶段二 Day 2｜开课：2026-08-31｜完成：2026-09-02

---

## 教材核心约定

普通 `3×3` 线性变换保持原点，不能表达平移。把三维量写成四维齐次形式后，点与方向使用不同的 `w`：

```text
点：   (x, y, z) → (x, y, z, 1)
方向： (x, y, z) → (x, y, z, 0)
```

于是平移可以统一为 `4×4` 矩阵乘法：

```text
T = |1 0 0 tx|
    |0 1 0 ty|
    |0 0 1 tz|
    |0 0 0  1|
```

---

## 坏代码场景（苏格拉底）

武器位于世界坐标 `(10,2,5)`，没有旋转。枪口局部位置和枪口朝向都乘同一个 `localToWorld` 矩阵。以下是本课当时故意写坏的旧接口，后文会说明统一后的 API：

```csharp
public static Vector3 TransformPoint(Matrix4x4 m, Vector3 point)
{
    return m.Transform(point, w: 1f);
}

public static Vector3 TransformDirection(Matrix4x4 m, Vector3 direction)
{
    // 错误：平移武器不应改变射击方向
    return m.Transform(direction, w: 1f);
}
```

矩阵只有平移，没有旋转：

```text
|1 0 0 10|
|0 1 0  2|
|0 0 1  5|
|0 0 0  1|
```

问题：局部枪口点和方向都取 `(0,0,1)`。

1. `TransformPoint` 的世界坐标结果是多少？
2. 当前 `TransformDirection` 实际得到什么？没有旋转时，期望方向是什么？
3. 为什么点和方向的第四个齐次分量不能相同？
4. 为什么错误会让同一把枪随角色的世界位置改变射击方向？

---

## 你的第一次回答

> 1. `TransformPoint` 后结果 `(10,2,6)`，正确。
> 2. `TransformDirection` 实际 `(10,2,6)`，应为 `(0,0,1)`；方向被当作位置进行平移。
> 3. 理解 `w=1` 可让矩阵最右一列成为平移值，但尚未明白点与方向的 `w` 为什么不同。
> 4. 给方向向量加入了平移，导致方向错误。

数值和 bug 表现判断正确，但齐次 `w` 的语义尚未完成。

## 第一次实现与反馈

第一次代码虽然 **3/3 PASS**，却新增了忽略参数、把平移项硬编码乘零的方法：

```csharp
public Vector3 TransformDirection(Vector3 v, float w) => new(
    ... + M03 * 0,
    ... + M13 * 0,
    ... + M23 * 0);

// 调用方传 1，但上面的函数完全忽略它
m.TransformDirection(direction, 1f);
```

这使 `w` 成为假参数，调用方仍错误表达“方向的 `w=1`”。引导题是计算两个齐次点之差：

```text
A = (1,2,3,1)
B = (1,2,4,1)
B - A = (0,0,1, ?)
```

在本轮练习当时，要求只保留通用的 `Matrix4x4.Transform(v,w)`，由调用方表达点与方向的语义。后续合并矩阵类型时发现，公开裸 `w` 仍容易被调用方误用，因此最终 API 将它收为私有实现细节。

## 第二次实现

学生删除了矩阵内部重复的方法，当时保留通用 `Transform(v,w)`：

```csharp
TransformPoint     => m.Transform(point, 1f);
TransformDirection => m.Transform(direction, 0f);
```

本轮测试：**3/3 PASS**，实现与齐次语义均合格。

---

## 标准推导

平移矩阵对点和方向分别得到：

```text
T(x,y,z,1) = (x+tx, y+ty, z+tz, 1)  // 点被平移
T(x,y,z,0) = (x,    y,    z,    0)  // 方向不被平移
```

`w=0` 不是人为补丁，而是“方向是两点之差”的必然结果：

```text
B - A
= (Bx,By,Bz,1) - (Ax,Ay,Az,1)
= (Bx-Ax, By-Ay, Bz-Az, 0)
```

两个点都加上同一个平移 `t` 后：

```text
(B+t) - (A+t) = B-A
```

因此移动角色或武器只改变枪口世界位置，不能改变枪口朝向。错误地给方向 `w=1`，会把世界位置混入射击方向；归一化只能缩短错误向量，不能恢复正确方向。

三维平移 `p↦p+t` 在普通三维坐标中不是线性变换，因为它不保持原点；引入第四维后，它可以由 `4×4` 矩阵乘法统一表达。这就是齐次坐标的工程价值。

---

## 游戏场景翻译

- **枪口/射线**：枪口位置用 `w=1`，射击方向用 `w=0`。
- **挂点/骨骼**：武器、背包和帽子的局部挂点是点，要跟随父对象平移。
- **法线/速度**：它们表达方向或差值，不能直接吃到位置平移。
- **层级变换**：父子矩阵的组合顺序留到阶段二 Day 3 单独学习。

扩展练习现位于 `Homework/数学/07-平移齐次坐标-扩展/Matrix4x4ExtendedTests.cs`，包含矩阵乘法与 `T·R` 组合回归；它复用项目唯一的 `Matrix4x4`，默认测试入口不启用。

### 扩展练习验收（2026-09-02）

第一次单独运行旧 `Mat4x4` 的六项测试为 **5/6 PASS**。挂点案例期望 `(10,1,-1.3)`、实际 `(1.3,1,10)`；当时 TODO 6 旋转了角色的世界位置，却没有旋转局部剑尖偏移。正确关系是：

```text
world = translation + rot · localOffset
      = T · R · localOffset
```

排查时还发现旧测试骨架把两个相反的 Y 轴旋转方向混在了一起。现已统一采用项目约定：

- `+X` 向右、`+Y` 向上、`+Z` 向前，使用列向量。
- `CreateRotationYDegrees(+90°)` 满足 `+Z→+X`、`+X→-Z`。
- 基向量测试为 `CreateRotationYDegrees(+90°)·(0,0,5)=(5,0,0)`。

学生最终实现的旋转与挂点组合都符合该约定；迁移到统一类型后独立复验仍为 **6/6 PASS**，剑尖世界位置为 `(10,1,-1.3)`。

### `Matrix4x4` API 合并（2026-09-02）

原项目同时存在不可变 `Matrix4x4` 与可变 `Mat4x4`，相同能力却使用 `Translation/Translate`、`RotationY/RotateY`、`Transform/MultiplyPoint/TransformPoint` 等不同名称。现已删除 `Mat4x4`，唯一实现为 `StudyNotes.Homework.Math.LinearAlgebra.Matrix4x4`。

统一后的公开 API：

```csharp
Matrix4x4.Identity
Matrix4x4.CreateTranslation(offset)
Matrix4x4.CreateRotationXDegrees(angleDegrees) // 第十轮由学生补齐实现
Matrix4x4.CreateRotationYDegrees(angleDegrees)
Matrix4x4.Multiply(left, right)

matrix.TransformPoint(point)
matrix.TransformDirection(direction)
matrix.Transpose()
```

命名规则：

- `Create...` 明确表示创建新矩阵，不暗示修改原矩阵。
- `Degrees` 明确旋转参数的角度单位。
- `TransformPoint` 与 `TransformDirection` 固定 `w=1/0`，业务代码不再裸传 `w`。
- `Multiply(left,right)` 严格表示 `left·right`；列向量下先应用 `right`。
- `Transpose()` 只表示数学转置；仅当矩阵是纯旋转时，才能把它解释为旋转的逆。

---

## 跨书关联

- 《Essential Mathematics for Games》(3rd) Ch 4.6：仿射变换等于线性部分加平移，齐次坐标用于统一表示。
- 《Foundations of Game Engine Development, Vol 1》：引擎中的 `4×4` 布局要继续区分“内存行/列主序”和“行/列向量乘法约定”。

---

## 作业验收

修复点/方向的齐次 `w` 约定，保持同一个通用矩阵乘法入口。  
结果：**3/3 PASS**。

---

`[数学/几何变换-平移与齐次坐标：完成，3/3 PASS]`
