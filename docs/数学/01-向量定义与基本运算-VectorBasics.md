# 01-向量定义与基本运算（Vector Basics）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) 第 1-3 章
> 教学日期：2026-08-25

---

## 教材原文（引用）

1. **向量定义**（Ch 2 开篇）：
   > "A vector is a quantity with both magnitude and direction."
   向量 = 既有大小又有方向的量。用箭头表示；**向量没有位置**——有位置的量叫点（point），向量只描述"从哪到哪"的位移（displacement）。
   （Ch 2.3-2.4：向量作为位移；向量 vs 点）

2. **向量加法**（Ch 2.7）：
   首尾相连（head-to-tail）：v + w = 把 w 的头接到 v 的尾，结果向量从 v 的尾指向 w 的头。分量形式：v + w = (vx+wx, vy+wy, vz+wz)。

3. **向量模长**（Ch 2.8）：
   > |v| = √(vx² + vy² + vz²)
   勾股定理从 2D 推广到 3D。

4. **向量归一化**（Ch 2.9）：
   单位向量（unit vector）是长度为 1 的向量，记作 v̂。归一化 = 各分量除以模长：
   > v̂ = v / |v|

5. **多坐标系**（Ch 3）：同一个向量在不同坐标系（世界/模型/相机）中的分量不同；位移/方向在任何坐标系下客观存在，只是"读数"随坐标系变化。

---

## 游戏场景翻译

- **位置 = 点，位移 = 向量**：`transform.position += velocity * dt` —— position 是点，velocity×dt 是位移向量。
- **速度 = 方向 × 速率**：`velocity = dir * speed` —— 这个公式的数学前提是 **dir 必须为单位向量**，否则 speed 就不再是"速率"而是被 dir 的模长放大/缩小。
- 单位向量就是"只描述朝向、不掺长度"的向量，是方向与速率解耦的关键。

---

## 坏代码场景（苏格拉底）

```csharp
// 第一人称移动（伪 Unity 代码）
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;          // 直线速度 5 m/s
    public Transform cam;                 // 相机

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");   // 键盘 A/D：-1 / 0 / 1
        float v = Input.GetAxisRaw("Vertical");     // 键盘 W/S：-1 / 0 / 1
        var input = new Vector3(h, 0f, v);

        // 期望：按 W+D 斜着走时，速度也是 5 m/s
        var dir = cam.right * input.x + cam.forward * input.z;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
```

**问题**：这段代码在什么具体游戏输入或帧率变化下会出错？数学上哪里不合理？

---

## 你的回答

- 正确部分：`input` 的模长在 1(直线)到 √2(斜向)之间变化，斜向最大速度 = 5 × √2 ≈ 7.07 m/s，比直线快 41%（"斜向移动加速 bug"）。
- 纠错过程①：sin/cos 方案绕路——需先求角度 atan2 再重建向量，应直接让手头向量变单位向量。
- 纠错过程②："使用单位向量的模长"表述有误——单位向量模长恒为 1，正确操作是**除以自身模长**。

## 标准解

**问题根源**：`dir = cam.right·h + cam.forward·v` 不是单位向量。
- Unity 中 `cam.right`/`cam.forward` 是单位向量且互相垂直，所以 `|dir| = √(h² + v²)`。
- 输入组合 (h, v) 有：直线 (1,0) → `|dir|=1`；斜向 (1,1) → `|dir|=√2`。
- 实际速率 = `|dir| × moveSpeed`，斜向 = 5√2 ≈ 7.07 m/s。**与帧率无关**（`Time.deltaTime` 已保证帧率无关，这是输入组合的错，不是帧率的错）。

**修复**：归一化——`v̂ = v / |v|`（Primer Ch 2.9）：

```csharp
var dir = (cam.right * input.x + cam.forward * input.z).normalized;
// 等价：dir /= dir.magnitude;
transform.position += dir * moveSpeed * Time.deltaTime;
```

**推广**：凡"方向 × 标量速率"的写法，前提都是方向已单位化。这是游戏代码里最常见的一类隐藏 bug（射击方向、物理弹道、AI 追踪都中过招）。

---

## 作业（5-10 分钟）

实现 `Homework/数学/第一轮-向量基本运算/Vector3.cs` 中的 TODO：
模长 `Magnitude` / 平方模长 `SqrMagnitude` / 归一化 `Normalized` / 加法 / 减法 / 标量乘法，
并通过 `Vector3Tests` 的断言。

---

## 跨书关联

- 《Essential Mathematics for Games》(3rd) **Ch 2 (Vectors and Points)**：同样以 points vs vectors 的区分开场，强调向量无位置；与 Primer Ch 2.4 一致。
- 《Foundations of Game Engine Development, Vol 1》**Ch 1 (Vectors and Basic Vector Operations)**：引擎级向量实现视角（SIMD、对齐、齐次坐标留到 Vol 1 Ch 7），后续阶段四可对照。

---

`[数学/线性代数基础-向量]`
