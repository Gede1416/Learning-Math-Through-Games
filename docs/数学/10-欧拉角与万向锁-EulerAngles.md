# 10-欧拉角与万向锁（Euler Angles & Gimbal Lock）

> 教材：《3D Math Primer for Graphics and Game Development》(2nd) Ch 8–9；《Essential Mathematics for Games》(3rd) Ch 5
> 阶段三 Day 1｜当前：万向锁准备——学生实现 Z 轴旋转工具｜开始：2026-09-03

---

## 第一小节：旋转顺序（2026-09-09 完成）

1. 学生编写数学工具：✅ `CreateRotationXDegrees`，工具测试 `4/4 PASS`
2. 导师给出错误使用代码：✅
3. 导师提出问题：✅
4. 学生回答问题：✅ 最新回答已正确区分 `-Z` 与 `+X`，乘积左右位置与代码一致
5. 学生修整代码：✅ 数值测试 5/5，两处过期注释已清理（2026-09-09 复验）

本小节五步流程已完成。以下保留错误示例和纠错过程；当前作业见文末“第二小节”。

---

## 教材核心公式

欧拉角是若干基本轴旋转的有序组合，而矩阵乘法通常不可交换：

```text
Rx(α)·Ry(β) ≠ Ry(β)·Rx(α)
```

项目继续使用统一约定：`+X` 右、`+Y` 上、`+Z` 前，列向量 `v'=M·v`。FPS 相机约定 yaw 绕世界 `+Y`，pitch 绕相机自己的局部 `+X`。

---

## 错误使用代码

下面的方法不超过 30 行。单独左右转和单独抬头都正常，但两个角同时非零时会出错：

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

原错误场景基线为 **4/5 PASS**。失败案例：

```text
局部 forward = (0,0,1)
yaw = +90°
pitch = -45°
期望 ≈ (0.7071,0.7071,0)
实际 ≈ (1,0,0)
```

## 苏格拉底问题

1. 对 `pitch · yaw · forward`，列向量约定下哪个矩阵先作用？
2. `forward=(0,0,1)` 经过 yaw `+90°` 后是什么方向？再经过当前代码中的 pitch `-45°` 后是什么方向？
3. yaw `+90°` 后，相机局部 `+X` 轴在世界坐标中指向哪里？当前 pitch 矩阵实际绕世界中的哪根轴旋转？
4. 为什么仅 yaw 或仅 pitch 时测试都通过，而两个角同时非零才暴露问题？
5. 若要表达“世界 Y yaw 后，沿相机局部 X pitch”，`yaw` 与 `pitch` 两个矩阵应分别位于乘积的哪一侧？

## 第一次回答与反馈

你给出的回答是：

```text
1. f → y → p
2. (1,0,0)，再得到 (1,0,0)
3. -Z，-Z
4. 因为旋转轴被改变而导致旋转异常
5. p × y
```

- 第 1、2 题正确。
- 第 3 题的第一个 `-Z` 正确；第二个旋转轴需要重新判断。
- 第 4 题指出了轴变化，但还没有解释为什么单轴输入会掩盖乘积顺序。
- 第 5 题的文字与已经改成的 `Multiply(yawAroundWorldY, pitchAroundLocalX)` 不一致，需要明确 left/right。

当前代码数值已经 **5/5 PASS**，但先补清楚推导：

1. yaw 后的前方是 `(1,0,0)`。标准 `Rx` 绕哪根世界轴？与该轴平行的 `(1,0,0)` 会被它改变吗？
2. 分别令 pitch 为 `0`、yaw 为 `0`；此时哪个矩阵是单位矩阵？为什么交换乘积仍看不出差别？
3. `Multiply(left,right)` 表示 `left·right`。请用完整变量名重写第 5 题，并明确谁在左、谁在右。

## 第二次回答与最小标准推导

第二次回答已经正确说明第 4 题：任一角为 `0` 时，对应旋转矩阵为 `Identity`，因此换序不会改变单轴结果。

第 3、5 题连续两次仍有混淆，按纠错规则给出最小标准推导：

```text
yaw 后的局部 X：Ry(+90°) · (1,0,0) = (0,0,-1)

旧错误代码的实际步骤：
Rx(-45°) · [Ry(+90°) · forward]
= Rx(-45°) · (1,0,0)
= (1,0,0)
```

因此，`-Z` 是 yaw 后“相机局部 X”在世界中的方向；但旧代码左侧的标准 `Rx` 仍是绕固定的世界 `+X` 轴旋转。这是两个不同问题。

对于局部向量，应先在相机局部坐标中应用 pitch，再把整个结果用 yaw 映射到世界：

```text
worldVector = Ry · (Rx · localVector)
            = (Ry · Rx) · localVector
```

这与“相机已有 yaw 朝向，再追加局部 pitch”并不矛盾：局部旋转通过右乘追加，最靠近局部向量。

请由你完成最后整理：

1. 将回答第 3 题明确写成两个不同的世界轴。
2. 将回答第 5 题改为完整变量名，并与当前代码的 left/right 一致。
3. 删除 `CreateRotationXDegrees` 返回行上的“临时占位”注释。
4. 把 `EulerCamera` 中“组合顺序故意错误”的 TODO 改为你对正确顺序的解释。

数值代码无需再改。整理好后告诉我“写好了”。

## 最终整理复核（2026-09-09）

已确认两处过期注释清理完成；新回答中 `left=yawAroundWorldY`、`right=pitchAroundLocalX` 正确，与代码一致。构建成功，本课场景测试 **5/5 PASS**。

仅剩最新回答第 3 题的第一个符号：学生写成 `+Z轴 +X轴`，应为 **`-Z轴 +X轴`**。根据项目 Y 轴旋转约定：

```text
(x,y,z) → (z,y,-x)
(1,0,0) → (0,0,-1)
```

因此 yaw 后的局部 `+X` 指向世界 `-Z`；旧错误代码实际绕世界 `+X`。学生随后已自行把符号修正为 `-Z轴 +X轴`，第一小节完成。最终组合为 `Multiply(yawAroundWorldY, pitchAroundLocalX)`，即 `Ry·Rx`。

## 第二小节：万向锁准备——Z 轴旋转工具（2026-09-09）

游戏场景：接下来让相机支持绕局部前方轴的翻滚（roll）。X/Y 旋转已有实现，现在先由你补齐 Z 轴旋转。工具通过后，导师再给出错误的相机使用代码与问题，届时研究 yaw、pitch、roll 同时参与时的问题。

当前仅进行“学生编写数学工具”步骤。作业位置：`MathLibrary/Matrix4x4.cs` 中的 **`CreateRotationZDegrees`（TODO 10.3）**。预计 5–10 分钟。

沿用项目列向量约定，绕 Z 轴只改变 X/Y 分量：

```text
Rz(θ) = |cosθ  -sinθ  0  0|
        |sinθ   cosθ  0  0|
        |  0      0   1  0|
        |  0      0   0  1|
```

实现要求：

1. 参数单位是度，复用你已有的 `GetSinCosByDegrees`（先输出 cosine，再输出 sine）。
2. 在唯一的 `Matrix4x4` 类型中补全方法体，保持纯旋转、无平移。
3. 保证 `+90°` 时 `+X→+Y`、`+Y→-X`，旋转轴 `+Z` 不变。

`Program.cs` 现在只运行 `RotationZFactoryTests.Run()` 的五项测试：零角、两个正直角基向量、Z 轴不变、负 45°。当前 `Identity` 未实现占位的预期基线为 **2/5 PASS**；完成目标为 **5/5 PASS**，不与第一小节的结果累计。

工具通过前不提供新场景代码或组合问题。完成方法后告诉我“工具写好了”。

---

`[数学/三维旋转-万向锁：Z轴旋转工具等待实现]`
