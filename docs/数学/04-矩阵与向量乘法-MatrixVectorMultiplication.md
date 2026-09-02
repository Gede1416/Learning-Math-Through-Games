# 04-矩阵与向量乘法（Matrix-Vector Multiplication）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) Ch 4  
> 教学日期：2026-08-28

---

## 坏代码场景（苏格拉底）

一款 2D 游戏用下面的函数把小地图标记从局部方向旋转到世界方向。约定：正角度表示逆时针旋转。

```csharp
static Vector2 RotateMarker(Vector2 local, float degrees)
{
    float r = degrees * MathF.PI / 180f;
    float c = MathF.Cos(r);
    float s = MathF.Sin(r);

    // 逆时针旋转矩阵的四个元素
    float m00 = c,  m01 = -s;
    float m10 = s,  m11 =  c;

    // 把局部方向变换到世界方向
    return new Vector2(
        local.X * m00 + local.Y * m10,
        local.X * m01 + local.Y * m11);
}
```

**问题**：这段代码在什么具体游戏输入下会明显出错？请至少代入 `local = (1, 0)`、`degrees = 90` 手算实际结果和期望结果；数学上，代码把矩阵的什么弄反了？

---

## 你的回答

> 计算顺序错误，导致旋转方向错误。修改为：

```csharp
return new Vector2(
    local.X * m00 + local.Y * m01,
    local.X * m10 + local.Y * m11);
```

## 第一次反馈

- **方向正确**：修改后的第一个输出分量使用矩阵第 0 行，第二个输出分量使用第 1 行；这符合本题采用的“矩阵乘列向量”约定。
- **回答尚不完整**：还没有代入 `local=(1,0)`、`degrees=90°`，分别写出原代码的实际结果与修改后代码的期望结果，因此暂不展示标准解。
- **追问**：此时 `m00=0, m01=-1, m10=1, m11=0`。将它们代入原代码，两个输出分量各是多少？再代入修改后的代码，各是多少？

## 你的第二次回答

> 原期望 `(0,1)`，实际 `(0,-1)`。

判断：**正确**。数值结果和旋转方向均已验证。

---

## 教材定义与核心公式

《3D Math Primer》(2nd) Ch 4 对矩阵的基本定义是：

> “A matrix is a rectangular array of numbers.”

矩阵与列向量相乘时，结果的第 `i` 个分量，是矩阵第 `i` 行与输入向量的点积：

```text
| m00  m01 | | x |   | m00·x + m01·y |
| m10  m11 | | y | = | m10·x + m11·y |
```

也可以从“列”的角度理解：

```text
M(x, y) = x × M的第0列 + y × M的第1列
```

因此，矩阵的每一列表示一个局部基轴经过变换后落在世界空间中的方向。

---

## 标准解

90° 逆时针旋转矩阵为：

```text
M = | 0  -1 |
    | 1   0 |
```

用列向量约定计算：

```text
M |1| = |0×1 + (-1)×0| = |0|
  |0|   |1×1 +    0×0|   |1|
```

所以 `(1,0)` 变为 `(0,1)`。

原代码却计算：

```text
(x, y)M
```

若仍把结果写成列向量，这等价于 `Mᵀv`。旋转矩阵的转置恰好是它的逆矩阵，所以 `+90°` 被变成了 `-90°`，结果为 `(0,-1)`。

正确实现：

```csharp
return new Vector2(
    m00 * local.X + m01 * local.Y,
    m10 * local.X + m11 * local.Y);
```

### 一个必须分清的工程概念

**行主序/列主序存储**与**行向量/列向量乘法约定**不是同一件事：前者描述数字在内存中的排列，后者描述数学表达式写成 `vM` 还是 `Mv`。接入 Unity、System.Numerics 或着色器 API 时，应分别核对这两项约定。

---

## 游戏场景翻译

- `M × localDirection`：把小地图标记、角色朝向或顶点从局部空间映射到目标空间。
- 矩阵第一列：局部 `+X` 轴变换后的方向。
- 矩阵第二列：局部 `+Y` 轴变换后的方向。
- 把乘法侧写反：对旋转矩阵通常表现为旋转方向反了；对一般矩阵则不只是“方向反转”。

---

## 作业（5–10 分钟）

实现 `Homework/数学/04-矩阵向量乘法/Matrix2x2.cs` 中的两个 TODO：

1. `Multiply`：计算 `M × v`。
2. `RotationDegrees`：构造 2D 逆时针旋转矩阵。

实现部分不超过 20 行。运行 `dotnet run`，让本轮 5 项测试全部通过，同时保持前三轮回归测试通过。

---

## 跨书关联

- 《Essential Mathematics for Games and Interactive Applications》(3rd) Ch 3：同样用矩阵表示线性变换，并强调向量与矩阵的放置约定必须一致。
- 《Foundations of Game Engine Development, Vol 1》Ch 1：把矩阵的列解释为变换后的基向量，这种理解比死记分量公式更适合引擎代码。

---

## 作业第一次验收

- `Multiply`：正确；单位矩阵和一般矩阵两项测试通过。
- `RotationDegrees`：三项旋转测试失败。
- 本轮结果：`2 PASS / 3 FAIL`。（此前显示的历史课程结果不再计入本轮。）

问题集中在：

```csharp
var h = 360 / degrees * 2 * MathF.PI;
```

当 `degrees=90` 时，这行得到 `h=8π`，因此 `cos(h)≈1`、`sin(h)≈0`，矩阵几乎没有旋转。

引导：已知 `360° = 2π` 弧度，请先填写比例式：

```text
h / 2π = degrees / 360
```

再单独解出 `h`，并确认 `degrees=90` 时应得到 `π/2`。暂不展示代码答案。

## 作业第二次验收

学生将弧度换算修正为：

```csharp
var h = degrees / 360 * 2 * MathF.PI;
```

验收结果：**本轮 5/5 PASS**。

- 单位矩阵保持向量：PASS
- 一般矩阵乘列向量：PASS
- `+90°` 右→上：PASS
- `+90°` 上→左：PASS
- `-90°` 上→右：PASS

第四轮完成。下一步为阶段一综合小考。

---

`[数学/线性代数基础-矩阵与向量乘法：完成，5/5 PASS]`
