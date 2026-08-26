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

（待填写，同步到 `00-我的回答.md`）

## 标准解

（待学生回答后补充：点积混入了距离 → 0.5 阈值只在 1 米处对应 60° → 归一化修复）

---

## 作业（5-10 分钟）

实现 `Homework/数学/第二轮-点积/DotProduct.cs` 中的 TODO：
点积 `Dot` / 投影长度 `ProjectScalar` / 夹角 `AngleDegrees` / 修复警戒判定 `IsInCone`，
并通过 `DotProductTests` 的断言。

---

## 跨书关联

- 《Essential Mathematics for Games》(3rd) **Ch 2.4 (Dot Products and Projections)**：同样的定义与投影解释，强调 dot 是"最便宜的几何信息量"。
- 《Foundations of Game Engine Development, Vol 1》**Ch 1.3**：点积在引擎中的 SIMD 实现视角；注意引擎库（Unity `Vector3.Dot` / glm `dot`）均按分量乘积之和实现。

---

`[数学/线性代数基础-点积]`
