# 09-坐标系转换（Coordinate Spaces）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) Ch 6  
> 阶段二 Day 4｜日期：2026-09-02

---

## 坏代码场景（苏格拉底）

沿用项目统一约定：`+X` 向右、`+Y` 向上、`+Z` 向前，并使用列向量 `v'=M·v`。相机位于 `(10,0,0)`，绕 Y 轴旋转 `+90°` 后面向世界 `+X`。世界目标位于 `(15,0,0)`，显然在相机正前方 5 米。以下是开课时使用旧 API 的故意错误代码：

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

## 第二次实现与反馈

学生恢复了正确的相对向量：

```csharp
var relative = worldPoint - cameraPosition;
```

但随后把 `Composition3D.RotationY` 的正负号整体翻转，并仍直接使用这个共享函数做 world→camera；第二次提交最初把相对向量作为 `w=1` 传入，之后已自行改回 `w=0`。原有三项数值测试曾显示 **3/3 PASS**，但仍不能验收：

- 被修改后的 `RotationY(+90°)` 变成 `+Z→-X、+X→+Z`，违反项目统一约定，也破坏了第 08 课的 local→world 语义。
- `worldPoint-cameraPosition` 是两个点之差，所以它是方向，齐次分量必须是 `w=0`。纯旋转矩阵没有平移列，因此此前的 `w=1` 只是数值上碰巧没有影响。
- world→camera 需要在当前函数里使用 camera local→world 旋转的逆，不能通过反转共享函数的定义来实现。

为防止“改坏共享旋转但当前测试全绿”，本课新增一项约定保护：`RotationY(+90°)·+Z=+X`。加入后当前结果为 **3/4 PASS**。

这是第二次尝试，标准修正方向如下：先恢复 `Composition3D.RotationY` 的统一矩阵，再对纯旋转矩阵取转置得到逆旋转：

```text
R⁻¹ = Rᵀ
cameraLocal = Rᵀ · (worldPoint - cameraPosition), w=0
```

其中只需转置左上角 `3×3` 旋转部分；不要改变项目全局的 `RotationY` 定义。

## 统一 API 后的标准实现

合并 `Matrix4x4/Mat4x4` 时，旋转工厂被收敛为唯一的 `CreateRotationYDegrees`，并为矩阵增加了语义明确的 `Transpose` 与 `TransformDirection`。最终代码是：

```csharp
public static Vector3 TransformWorldPointToCamera(
    Vector3 worldPoint,
    Vector3 cameraWorldPosition,
    Matrix4x4 cameraLocalToWorldRotation)
{
    var worldOffsetFromCamera = worldPoint - cameraWorldPosition;
    var worldToCameraRotation = cameraLocalToWorldRotation.Transpose();
    return worldToCameraRotation.TransformDirection(worldOffsetFromCamera);
}
```

方法和参数名同时表达了变换方向：输入矩阵是 `cameraLocalToWorldRotation`，函数内部得到 `worldToCameraRotation`。本轮当前测试 **4/4 PASS**：统一旋转约定、无旋转相机、正前方目标、局部右侧目标全部通过。

注意：`R⁻¹=Rᵀ` 只对正交纯旋转成立；如果矩阵包含非均匀缩放或错切，必须计算真正的逆矩阵，不能直接转置。

---

`[数学/几何变换-坐标系转换：完成，4/4 PASS]`
