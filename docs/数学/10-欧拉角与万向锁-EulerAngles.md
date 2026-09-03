# 10-欧拉角与万向锁（Euler Angles & Gimbal Lock）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) Ch 8–9；《Essential Mathematics for Games》(3rd) Ch 5
> 阶段三 Day 1｜第一小节：旋转顺序｜开课：2026-09-03

---

## 教材核心公式

欧拉角把一个三维朝向写成若干基本轴旋转的**有序组合**。矩阵乘法通常不可交换：

```text
Rx(α)·Ry(β) ≠ Ry(β)·Rx(α)
```

所以仅有相同的 yaw、pitch 数值还不够；还必须说明绕世界轴还是局部轴，以及组合顺序。本小节先解决顺序，修正后再进入万向锁。

项目继续使用统一约定：`+X` 右、`+Y` 上、`+Z` 前，列向量，最右侧矩阵先作用。

```text
Rx(θ) = |1   0      0 |
        |0  cosθ  -sinθ|
        |0  sinθ   cosθ|

Ry(θ) = | cosθ  0  sinθ|
        |  0    1   0  |
        |-sinθ  0  cosθ|
```

---

## 坏代码场景（苏格拉底）

FPS 相机的 yaw 绕世界 `+Y`，pitch 绕相机自己的局部 `+X`。单独左右转、单独抬头都正常；角色向右转 `90°` 后再抬头 `45°`，相机却只朝右，完全抬不起来。

```csharp
public static Matrix4x4 CreateCameraLocalToWorldRotationDegrees(
    float yawDegrees,
    float pitchDegrees)
{
    var yawAroundWorldY =
        Matrix4x4.CreateRotationYDegrees(yawDegrees);
    var pitchAroundLocalX =
        Matrix4x4.CreateRotationXDegrees(pitchDegrees);

    // 故意错误
    return Matrix4x4.Multiply(pitchAroundLocalX, yawAroundWorldY);
}
```

本题取局部前方 `forward=(0,0,1)`、`yaw=+90°`、`pitch=-45°`。负 pitch 表示向上抬头。

当前基线测试为 **4/5 PASS**。X 轴约定、零角度、仅 yaw、仅 pitch 都通过；只有 yaw 与 pitch 同时非零的组合案例失败，期望约为 `(0.7071,0.7071,0)`，实际约为 `(1,0,0)`。

## 问题

1. 对 `pitch·yaw·forward`，列向量约定下哪个矩阵先作用？
2. `forward` 经过 yaw `+90°` 后是什么方向？再经过绕世界 X 的 pitch `-45°` 后是什么方向？这是坏代码的实际结果。
3. yaw 后，相机自己的局部 `+X` 轴在世界坐标中指向哪里？真正的 pitch 应绕世界中的哪根轴？
4. 为什么仅 yaw、仅 pitch 的测试都能通过，但两个角同时非零就暴露错误？
5. 在不修改统一 `CreateRotationXDegrees/CreateRotationYDegrees` 的前提下，矩阵组合顺序应怎样调整？

请先写出第 2、3 题的具体向量，再修改 TODO。暂时不要讨论四元数。

---

## 你的回答

（等待回答）

---

## 小作业

文件：`Homework/数学/10-欧拉角与万向锁/EulerAngles.cs`
目标：只修正 `CreateCameraLocalToWorldRotationDegrees` 的组合顺序，不修改统一轴旋转工厂。
预计：5–10 分钟。

---

`[数学/三维旋转-欧拉角旋转顺序：苏格拉底问题待回答]`
