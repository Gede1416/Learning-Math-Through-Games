# 03-叉积（Cross Product）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) 第 2.12 节
> 教学日期：2026-08-27

---

## 教材原文（引用）

1. **叉积定义**（Ch 2.12）：
   > a × b = |a||b| sin θ · n̂
   其中 n̂ 是**同时垂直于 a 和 b** 的单位向量。分量形式：
   > a × b = (a_yb_z − a_zb_y, a_zb_x − a_xb_z, a_xb_y − a_yb_x)

2. **方向——右手定则**：a × b 的方向由右手定则决定（a 弯向 b，拇指即结果）。**顺序敏感**：
   > a × b = −(b × a)

3. **长度**：|a × b| = |a||b| sin θ = 两向量张成的**平行四边形面积**。

4. **重要特例**：a × a = 0（零向量）；坐标轴单位向量：x̂ × ŷ = ẑ。

---

## 游戏场景翻译

- **面法线**：三角面 v0/v1/v2 的法线 = `Cross(v1−v0, v2−v0)`——顶点绕序（缠绕方向）决定法线朝内还是朝外。
- **左右/转向判定**：叉积在垂直平面上的分量符号判断目标在左还是右（注意引擎坐标系手性）。
- **三角形面积/朝向**：|叉积|÷2 = 三角形面积；符号 = 顶点顺序是顺时针还是逆时针。
- **转向轴**：摄像机/角色要"绕某轴旋转到指向目标"，旋转轴 = `Cross(当前朝向, 目标方向)`（四元数章节再展开）。

---

## 坏代码场景（苏格拉底）

```csharp
// 俯视角敌人 AI：判断玩家在左还是右，决定转向方向
public class Enemy : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        var toPlayer = player.position - transform.position;  // 指向玩家
        var forward = transform.forward;                      // 敌人朝向（XZ 平面）

        // 期望：玩家在左边 → 向左转
        var cross = Vector3.Cross(forward, toPlayer);         // 叉积垂直 XZ 平面 → 只剩 y 分量
        if (cross.y > 0f)
            TurnLeft();
        else
            TurnRight();
    }
}
```

**问题**：这段代码在什么具体游戏状态下会朝错误方向转向？数学上哪里不合理？（提示：先假设敌人面朝 +Z、玩家在敌人左侧，手算 cross 的 y 分量符号）

---

## 你的回答

- ① "等于 0 的时候没有判断" → **有效观察**：玩家在正前/正后（共线）时 cross = 0，`cross.y = 0` 会误入 else 分支；边缘情况，应单独处理（Idle），但不是转向错误的主因。
- ② "toPlayer 指向 forward，叉积顺序从右到左" → **表述混乱**：叉积无"指向"，只有操作数先后；代码为 `Cross(forward, toPlayer)`。学生通过手算/实现 `Side`（交换操作数 `toTarget × forward`）自行得出正确符号映射。
- 概念追问（"Side 是右手系、TriangleNormal 是左手系？"）→ 澄清：手性是坐标系属性，两个函数同在 Unity 左手系；差异来自**顶点绕序约定**（TriangleNormal 用教材 CCW 绕序）与**轴摆放映射**（Side 按 Unity 朝向约定）。

## 标准解

**手算**（敌人面朝 +Z，玩家在左 `toPlayer = (-1,0,0)`）：

```
forward × toPlayer = (0,0,1) × (-1,0,0) = (0·0−1·0, 1·(−1)−0·0, 0·0−0·(−1)) = (0, −1, 0)
```

`cross.y = −1 < 0` → 代码走 else → **TurnRight** ✗（玩家在左，应 TurnLeft）。即：**Unity 左手系 + `forward` 为左操作数时，`cross.y > 0 ⟺ 玩家在右**，代码的分支映射正好反了。

**数学根源**：叉积方向由**操作数顺序 + 坐标系手性**共同决定：
> a × b = −(b × a)

**修复**（二选一）：
1. 交换操作数：`Cross(toPlayer, forward)`（学生采用的方案，y>0 → 左 ✓）
2. 翻转分支：`cross.y < 0 → TurnLeft`

**边缘情况**：共线（cross.y ≈ 0）时方向无定义，应 Idle 而非转向。

**推广**：任何"叉积符号判定方向"的代码，先手算一个具体 case 验证符号映射，再写分支；引擎手性不同（Unity 左手 vs OpenGL 右手）时映射镜像。

---

## 作业（5-10 分钟）

实现 `Homework/数学/第三轮-叉积/CrossProduct.cs` 中的 TODO：
叉积 `Cross` / 面法线 `TriangleNormal` / 左右判定 `Side`，
并通过 `CrossProductTests` 的断言。

---

## 跨书关联

- 《Essential Mathematics for Games》(3rd) **Ch 2.5 (Cross Products and Normal Vectors)**：强调叉积是"2D 面积在 3D 的推广"，法线归一化后再用。
- 《Foundations of Game Engine Development, Vol 1》**Ch 1.4**：叉积分量式的引擎实现（SIMD 视角）。
- **手性注意**：Primer 的右手定则例子在左手系引擎（Unity/DirectX）中呈现为镜像——数学公式不变，但"哪个方向是上/前"的映射要按引擎惯例核对。

---

`[数学/线性代数基础-叉积]`
