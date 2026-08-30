# 07-平移与齐次坐标（Translation & Homogeneous Coordinates）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) Ch 6.1-6.3 + 《Essential Mathematics for Games》(3rd) Ch 4.6
> 阶段二 Day 2｜日期：2026-08-31

---

## 教材原文（引用）

1. **线性变换的局限**（Ch 5 回顾 / Ch 6 引子）：
   > 线性变换（矩阵 × 向量）**不包含平移**：原点永远被映射到原点。
   3×3 矩阵可以旋转、缩放、错切，但无法表达"把物体挪到别处"。

2. **齐次坐标**（Ch 6.1）：
   > 把三维向量 (x, y, z) 写成齐次形式 (x, y, z, **w**)，点用 w=1，方向向量用 w=0。
   用 4×4 矩阵作用在 (x, y, z, 1) 上，平移就变成了线性变换：

   ```text
   |1 0 0 tx| |x|   |x + tx|
   |0 1 0 ty| |y|   |y + ty|
   |0 0 1 tz| |z| = |z + tz|
   |0 0 0 1 | |1|   |  1   |
   ```

3. **w 的语义**：点 (w=1) 参与平移，方向向量 (w=0) 不受平移影响——这正是"向量没有位置"的坐标化表达。

4. **世界变换**（Ch 6.3）：层级变换 = 父链矩阵按序相乘。列向量约定下，先应用的矩阵在最右：**world = T·R**（先旋转后平移）。

---

## 游戏场景翻译

- **挂点/骨骼**：武器、背包、帽子挂在角色局部坐标的某个偏移上；世界位置 = T(角色) · R(角色) · localOffset。
- **层级父子**：月亮绕行星、行星绕太阳——每个物体记录相对父级的变换，世界变换 = 逐层相乘。
- **相机/物体绕任意点旋转**：先平移到旋转中心，旋转，再平移回来（3×3 做不到，必须齐次）。
- **投影**：w 还是透视投影的入口（阶段四用），现在先记住 w=1 是点、w=0 是方向。

---

## 坏代码场景（苏格拉底）

```csharp
// 角色挥剑：剑尖应围绕"手部挂点"旋转（随角色移动 + 转向）
public class Sword : MonoBehaviour
{
    public Transform player;
    public Vector3 handOffset = new Vector3(0.5f, 1f, 0f);   // 手部局部偏移
    public Vector3 bladeOffset = new Vector3(0.8f, 0f, 0f);  // 剑尖相对手的偏移

    void Update()
    {
        var rot = Matrix3x3.RotateY(player.eulerAngles.y);   // 3×3 旋转矩阵

        // 期望：剑尖 = 手部世界位置 + 旋转后的剑尖偏移
        var hand = player.position + handOffset;
        var tip  = rot * (hand + bladeOffset);    // ← 整个手部世界位置被塞进旋转矩阵
        transform.position = tip;
    }
}
```

## 问题

代入具体数值手算：角色在 `(10, 0, 0)`、朝向绕 Y 轴 90°：

1. 代码给出的剑尖世界位置是什么？
2. 期望的剑尖位置是什么（手部世界位置 + 旋转后的剑尖偏移）？
3. 数学上：3×3 旋转矩阵对"原点"做了什么？为什么它无法把剑尖"搬"到手部？—— 这正是"为什么需要第 4 维"。

---

## 你的回答

（待填写，同步到 `00-我的回答.md`）

## 标准解

（待学生回答后补充：3×3 原点→原点 → 平移不是线性变换 → 齐次坐标 w=1 → world = T·R·local）

---

## 作业（5-10 分钟）

实现 `Homework/数学/第七轮-平移齐次坐标/Mat4x4.cs` 中的 TODO：
平移矩阵 `Translate` / 4×4×点 `MultiplyPoint` / 组合变换 `TransformPoint`，
并通过 `Mat4x4Tests` 的断言。

---

## 跨书关联

- 《Essential Mathematics for Games》(3rd) **Ch 4.6 (Affine Transformations)**：仿射变换 = 线性变换 + 平移，齐次坐标的标准动机。
- 《Foundations of Game Engine Development, Vol 1》**Ch 7**：引擎级 4×4 存储布局（行主序 vs 列主序的 C 结构差异），阶段二 Day 4 对照。
- 平移矩阵与旋转矩阵的乘积顺序再次强调不可交换性（Day 4 已学）。

---

`[数学/几何变换-平移与齐次坐标]`
