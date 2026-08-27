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

（待填写，同步到 `00-我的回答.md`）

## 标准解

（待学生回答后补充：叉积顺序敏感 a×b=−(b×a) → 手算符号 → 与 Unity 左手系对照）

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
