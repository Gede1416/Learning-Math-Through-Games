# 09-坐标系转换（Coordinate Spaces）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) Ch 6  
> 阶段二 Day 4｜日期：2026-09-02

---

## 坏代码场景（苏格拉底）

沿用项目统一约定：`+X` 向右、`+Y` 向上、`+Z` 向前，并使用列向量 `v'=M·v`。相机位于 `(10,0,0)`，绕 Y 轴旋转 `+90°` 后面向世界 `+X`。世界目标位于 `(15,0,0)`，显然在相机正前方 5 米。

```csharp
public static Vector3 WorldToCamera(
    Vector3 worldPoint,
    Vector3 cameraPosition,
    Matrix4x4 cameraRotation)
{
    var relative = worldPoint - cameraPosition;

    // 错误：cameraRotation 是相机局部→世界，
    // 这里却直接拿它做世界→相机
    return cameraRotation.Transform(relative, 0f);
}
```

Y 轴 `+90°` 的规则：

```text
(x,0,z) → (z,0,-x)
```

## 问题

1. 先计算相对向量：`worldPoint-cameraPosition = ?`
2. 错误代码再应用 `+90°` 后得到什么？目标会被判断在相机前方还是后方？
3. 目标实际在正前方，所以相机局部坐标应是什么？
4. `cameraRotation` 把相机局部轴转到世界轴；反向把世界量表达进相机坐标时，应继续用它本身，还是用它的逆变换？纯旋转矩阵的逆可以怎样得到？

请先写出具体坐标，不要修改测试。

---

## 你的回答

第一次代码尝试把相对向量反了过来：

```csharp
var relative = cameraPosition - worldPoint;
return cameraRotation.Transform(relative, 0f);
```

当前测试为 **2/3 PASS**。在相机没有旋转时，世界点 `(12,0,0)` 相对位于相机 `(10,0,0)` 的右方 2 米，但代码算成了 `(-2,0,0)`；另外两个旋转案例通过，是两个方向错误偶然抵消，并不能证明变换方向正确。

下一次纠错先只看零旋转案例：从相机位置指向世界点的向量，应该是“终点减起点”的哪一种相减顺序？

---

`[数学/几何变换-坐标系转换：苏格拉底问题待回答]`
