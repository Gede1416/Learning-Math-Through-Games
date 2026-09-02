# 02-点积（Dot Product）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) 第 2.11 节
> 教学日期：2026-08-27

---

## 教材原文（引用）

1. **点积定义**（Ch 2.11）：
   > a · b = |a||b| cos θ
   分量形式：a · b = aₓbₓ + a_yb_y + a_zb_z。点积接受两个向量，返回**一个标量**。

2. **几何意义——投影**（Ch 2.11）：
   a · b̂（b̂ 为单位向量）= a 在 b 方向上的**有符号投影长度**。这是"一个数能告诉我们什么"的答案：投影。

3. **垂直判定**：a ⊥ b ⟺ a · b = 0。正交基的三个单位向量两两垂直，互相点积为 0（后面矩阵章节的基础）。

4. **单位向量下的特例**：若 |a| = |b| = 1，则 a · b = cos θ，点积直接就是夹角余弦。

---

## 游戏场景翻译

- **漫反射光照**：diffuse = max(0, N·L)——法线 N 与光方向 L 的点积，决定表面亮暗。N、L 必须单位化，否则光照强度随距离失真。
- **前后/左右判定**：角色朝向 f 与目标方向 t 的点积符号——单位向量时，正=在前方、负=在后方（90° 为界）。
- **投影**：影子长度、滑动面速度分解、UI 朝向箭头。
- **警戒锥形范围**：dot > cos(alertAngle) 判 60° 锥形——**前提是方向已归一化**（见坏代码场景）。

---

## 坏代码场景（苏格拉底）

```csharp
// 敌人 AI：玩家在正面 60° 内才进入警戒
public class Enemy : MonoBehaviour
{
    public Transform player;
    public float alertAngle = 60f;

    void Update()
    {
        var toPlayer = player.position - transform.position;
        var facing = transform.forward;   // 单位向量

        float dot = Vector3.Dot(facing, toPlayer);
        if (dot > Mathf.Cos(alertAngle * Mathf.Deg2Rad))   // cos60° = 0.5
            Alert();
        else
            Idle();
    }
}
```

**问题**：这段代码在什么具体游戏状态下会误报警戒？数学上哪里不合理？

---

## 你的回答

- ① "toPlayer 方向反了，应改为 transform.position - player.position" → **误判**：`player.position - transform.position` = 敌人指向玩家，方向本来正确；改反会让点积恒为负，背面误报。
- ② "dot 导致夹角值有问题，0-1 过早触发、1-∞ 过晚触发" → **直觉方向对、结论反了**：dot = 距离 × cosθ，距离混入了角度判定；距离 > 1 越远越容易误报（阈值对应角宽），距离 < 1 越近越容易漏报（甚至永不触发）。
- 修正过程中在 `IsInCone` 作业里自行实现"方向 ÷ 模长"（归一化），关键案例（80°×5m 与 80°×0.5m 判定一致）全部通过。

## 标准解

**问题根源**：`toPlayer` 不是单位向量，`|toPlayer|` = 玩家距离。于是
> dot = |toPlayer| · cosθ

距离被乘进了点积。阈值 `0.5` 只有在距离恰为 1 米时才对应 60°：

| 距离 d | 实际触发条件 | 后果 |
|--------|-------------|------|
| d = 1 m | cosθ > 0.5 → θ < 60° | 正常 |
| d = 5 m | cosθ > 0.1 → θ < 84° | **误报**（80° 也报警） |
| d = 0.5 m | cosθ > 1 → 无解 | **漏报**（永不报警） |

**数学关键**：点积 = 投影长度（含长度信息）；要"纯角度"必须**除掉长度**——cosθ = (a·b)/(|a||b|)。这正是归一化：两个方向都归一化后，â·b̂ = cosθ（Ch 2.11 单位向量特例）。

**修复**：

```csharp
float dot = Vector3.Dot(facing.normalized, toPlayer.normalized);
if (dot > Mathf.Cos(alertAngle * Mathf.Deg2Rad)) Alert();
```

或直接比夹角：`Vector3.Angle(facing, toPlayer) < alertAngle`（引擎内部同样走归一化点积）。

**推广**：光照（N·L）、视线判定、扇形攻击范围——凡是"点积与阈值比大小"的代码，先问自己：**向量归一化了吗？**

---

## 作业（5-10 分钟）

实现 `Homework/数学/02-点积/DotProduct.cs` 中的 TODO：
点积 `Dot` / 投影长度 `ProjectScalar` / 夹角 `AngleDegrees` / 修复警戒判定 `IsInCone`，
并通过 `DotProductTests` 的断言。

---

## 跨书关联

- 《Essential Mathematics for Games》(3rd) **Ch 2.4 (Dot Products and Projections)**：同样的定义与投影解释，强调 dot 是"最便宜的几何信息量"。
- 《Foundations of Game Engine Development, Vol 1》**Ch 1.3**：点积在引擎中的 SIMD 实现视角；注意引擎库（Unity `Vector3.Dot` / glm `dot`）均按分量乘积之和实现。

---

`[数学/线性代数基础-点积]`
