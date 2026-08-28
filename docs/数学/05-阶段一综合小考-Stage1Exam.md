# 05-阶段一综合小考（Stage 1 Exam）

> 范围：向量、点积、叉积、矩阵与向量乘法  
> 游戏场景：2D 自动炮塔瞄准  
> 日期：2026-08-28

---

## 坏代码场景（苏格拉底）

约定：角度逆时针为正，`TurnDirection` 返回 `+1=左转`、`-1=右转`、`0=不转`。

```csharp
public static bool CanFire(Vector2 turret, Vector2 target,
                           float facingDeg, float halfFovDeg)
{
    var forward = Matrix2x2.RotationDegrees(facingDeg)
                           .Multiply(new Vector2(1, 0));
    var toTarget = new Vector2(target.X - turret.X,
                               target.Y - turret.Y);
    float dot = forward.X * toTarget.X + forward.Y * toTarget.Y;
    return dot >= MathF.Cos(halfFovDeg * MathF.PI / 180f);
}

public static int TurnDirection(Vector2 forward, Vector2 toTarget)
{
    float cross = forward.X * toTarget.Y - forward.Y * toTarget.X;
    if (cross > 0) return -1;
    if (cross < 0) return 1;
    return 0;
}
```

## 问题

代码中有两个独立的数学错误。请分别说明：

1. 炮塔面向 `0°`，半视野角 `60°`：为什么 **30°、距离 0.5** 的目标会漏判，而 **80°、距离 5** 的目标会误判？
2. `forward=(1,0)`、`toTarget=(0,1)` 时，手算二维叉积标量是多少？目标在左还是右？函数实际返回什么？
3. 每个错误应该修正哪一个数学约定？先说明思路，不要直接改测试。

---

## 你的回答

> 1. 角度计算没有使用单位向量进行计算。
> 2. （未回答）

## 第一次反馈

第一点的根因正确：`toTarget` 的长度混入了点积，当前比较的不是纯粹的 `cosθ`。但需要补齐数值验证：

```text
30°、距离0.5：dot = 0.5 × cos30° = ?，与 cos60° = 0.5 比较
80°、距离5：  dot = 5 × cos80°   = ?，与 cos60° = 0.5 比较
```

第二点尚未回答。请直接代入：

```text
cross = 1×1 - 0×0 = ?
```

再判断：从 `(1,0)` 转向 `(0,1)` 是逆时针还是顺时针；按题目约定它是左还是右；当前 `cross>0` 分支返回 `-1`，是否匹配。

---

## 代码修正与验收

学生直接在代码中完成第二次回答：

- `Vector2.Cos(a,b)` 使用 `Dot(a,b) / (|a||b|)`，使视野判断只取决于夹角。
- `Vector2.Cross(a,b)` 使用 `ax·by - ay·bx`。
- `Vector2.LRF(a,b)` 将叉积正值映射为 `+1`（左），负值映射为 `-1`（右）。
- `CanFire` 使用旋转矩阵将局部 `(1,0)` 变为炮塔世界朝向，矩阵部分正确。

本轮测试：**6/6 PASS**。

## 标准数值推导

错误代码直接计算 `forward·toTarget = |toTarget|cosθ`：

```text
30°、距离0.5：0.5cos30° ≈ 0.433 < cos60° = 0.5 → 错误地不开火
80°、距离5：  5cos80°   ≈ 0.868 > cos60° = 0.5 → 错误地开火
```

将方向单位化后：

```text
30°：cos30° ≈ 0.866 > 0.5 → 开火
80°：cos80° ≈ 0.174 < 0.5 → 不开火
```

二维叉积判断：

```text
(1,0) × (0,1) = 1×1 - 0×0 = +1
```

从向右转向向上是逆时针，即目标在左。因此题目约定下应返回 `+1`；原代码返回 `-1`，映射相反。

## 工程备注

- `Cos` 遇到零向量时分母为零；真实项目应明确决定“目标与炮塔重合”时的行为。
- 可将 `Super` / `MSuper` 命名为 `SqrMagnitude` / `Magnitude`，并移除临时 `Console.WriteLine`，提高可读性；这些不影响本轮数学验收。

---

`[数学/阶段一综合小考：完成，6/6 PASS；阶段一完成]`
