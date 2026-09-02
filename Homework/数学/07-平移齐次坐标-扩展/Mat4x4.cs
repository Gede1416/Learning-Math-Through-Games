// ============================================================
// 第七轮作业：平移与齐次坐标（5-10 分钟）
// 教材：《3D Math Primer》Ch 6.1-6.3
// 目标：实现下列 TODO，让 Mat4x4Tests 全部 PASS
// 背景：Day 2 坏代码场景——3×3 无法表示平移，剑尖绕世界原点打转
// 三维坐标：+X 右、+Y 上、+Z 前；RotateY(+90°)：+Z→+X、+X→-Z
// 约定：列向量 v' = M·v；点 w=1，方向 w=0；先应用的矩阵在最右
// 行主序存储：M[i*4+j]，i 行 j 列
// ============================================================
using System;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.HomogeneousTransform;

public struct Mat4x4
{
    public float M00, M01, M02, M03;
    public float M10, M11, M12, M13;
    public float M20, M21, M22, M23;
    public float M30, M31, M32, M33;

    public Mat4x4(float m00, float m01, float m02, float m03,
                  float m10, float m11, float m12, float m13,
                  float m20, float m21, float m22, float m23,
                  float m30, float m31, float m32, float m33)
    {
        M00 = m00; M01 = m01; M02 = m02; M03 = m03;
        M10 = m10; M11 = m11; M12 = m12; M13 = m13;
        M20 = m20; M21 = m21; M22 = m22; M23 = m23;
        M30 = m30; M31 = m31; M32 = m32; M33 = m33;
    }

    // TODO 1：单位矩阵（对角线 1，其余 0）
    public static Mat4x4 Identity()
    {
        return new(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
        throw new NotImplementedException("TODO 1: 实现单位矩阵");
    }

    // TODO 2：平移矩阵（齐次坐标：第 4 列 = 平移量，最后一行 0 0 0 1）
    public static Mat4x4 Translate(float tx, float ty, float tz)
    {
        return new(
            1, 0, 0, tx,
            0, 1, 0, ty,
            0, 0, 1, tz,
            0, 0, 0, 1
        );
        throw new NotImplementedException("TODO 2: 实现平移矩阵");
    }

    // TODO 3：绕 Y 轴旋转矩阵（4×4 版，右下角 1），angleDeg 为度
    // RotY(θ) = | cosθ  0  sinθ  0 |
    //           |  0    1   0    0 |
    //           |-sinθ  0  cosθ  0 |
    //           |  0    0   0    1 |
    public static Mat4x4 RotateY(float angleDeg)
    {
        return new(
            MathF.Cos(angleDeg * MathF.PI / 180), 0, MathF.Sin(angleDeg * MathF.PI / 180), 0,
            0, 1, 0, 0,
            - MathF.Sin(angleDeg * MathF.PI / 180), 0, MathF.Cos(angleDeg * MathF.PI / 180), 0,
            0, 0, 0, 1
        );
        throw new NotImplementedException("TODO 3: 实现绕Y旋转矩阵");
    }

    // TODO 4：4×4 × 点（点 = (x,y,z,1)）
    // 每行与 (x,y,z,1) 做点积；返回 (X/W, Y/W, Z/W)（齐次除法，w 恒为 1）
    public static Vector3 MultiplyPoint(Mat4x4 m, Vector3 p)
    {
        return new(
            m.M00 * p.X + m.M01 * p.Y + m.M02 * p.Z + m.M03,
            m.M10 * p.X + m.M11 * p.Y + m.M12 * p.Z + m.M13,
            m.M20 * p.X + m.M21 * p.Y + m.M22 * p.Z + m.M23
        );
        throw new NotImplementedException("TODO 4: 实现矩阵×点");
    }

    // TODO 5：4×4 矩阵乘法 C = A·B（行×列点积，不可交换！）
    public static Mat4x4 Multiply(Mat4x4 a, Mat4x4 b)
    {
        float c00 = a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20 + a.M03 * b.M30;
        float c01 = a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21 + a.M03 * b.M31;
        float c02 = a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22 + a.M03 * b.M32;
        float c03 = a.M00 * b.M03 + a.M01 * b.M13 + a.M02 * b.M23 + a.M03 * b.M33;

        float c10 = a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20 + a.M13 * b.M30;
        float c11 = a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
        float c12 = a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
        float c13 = a.M10 * b.M03 + a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;

        float c20 = a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20 + a.M23 * b.M30;
        float c21 = a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
        float c22 = a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
        float c23 = a.M20 * b.M03 + a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;

        float c30 = a.M30 * b.M00 + a.M31 * b.M10 + a.M32 * b.M20 + a.M33 * b.M30;
        float c31 = a.M30 * b.M01 + a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
        float c32 = a.M30 * b.M02 + a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
        float c33 = a.M30 * b.M03 + a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;

        return new(
            c00, c01, c02, c03,
            c10, c11, c12, c13,
            c20, c21, c22, c23,
            c30, c31, c32, c33
        );
    }

    // TODO 6：修复 Day 2 坏代码——挂点世界变换
    // worldPos = T(translation) · R(rotation) · localOffset（先旋转后平移！）
    public static Vector3 TransformPoint(Mat4x4 rot, Vector3 translation, Vector3 localOffset)
    {
        //translation = translation+ localOffset;
        var red = MultiplyPoint(rot, localOffset);
        var res = translation + red;
        return res;
        throw new NotImplementedException("TODO 6: 实现挂点变换（先旋转后平移）");
    }
}

// ============ 单元测试（断言遵循项目统一坐标约定） ============
public static class Mat4x4Tests
{
    public static void Run()
    {
        // TODO 1+4：单位矩阵
        AssertV("I * (3,4,5) == (3,4,5)",
            Mat4x4.MultiplyPoint(Mat4x4.Identity(), new Vector3(3, 4, 5)), new Vector3(3, 4, 5));

        // TODO 2：平移——3×3 做不到的事
        AssertV("Translate(10,0,0) * (0,0,0) == (10,0,0)",
            Mat4x4.MultiplyPoint(Mat4x4.Translate(10, 0, 0), new Vector3(0, 0, 0)), new Vector3(10, 0, 0));
        AssertV("Translate(2,3,4) * (1,1,1) == (3,4,5)",
            Mat4x4.MultiplyPoint(Mat4x4.Translate(2, 3, 4), new Vector3(1, 1, 1)), new Vector3(3, 4, 5));

        // TODO 3+4：绕 Y 旋转
        AssertV("RotateY(90) * (0,0,5) == (5,0,0)",
            Mat4x4.MultiplyPoint(Mat4x4.RotateY(90), new Vector3(0, 0, 5)), new Vector3(5, 0, 0));

        // TODO 5：顺序不可交换（T·R ≠ R·T，作用同一局部点）
        var tr = Mat4x4.Multiply(Mat4x4.Translate(10, 0, 0), Mat4x4.RotateY(90));
        var rt = Mat4x4.Multiply(Mat4x4.RotateY(90), Mat4x4.Translate(10, 0, 0));
        AssertB("T·R ≠ R·T（先转后移 ≠ 先移后转）",
            (Mat4x4.MultiplyPoint(tr, new Vector3(1, 0, 0)) - Mat4x4.MultiplyPoint(rt, new Vector3(1, 0, 0))).Magnitude() > 0.5f, true);

        // TODO 6：挂点关键案例——角色在 (10,0,0) 面朝 Y 转 90°，手部偏移 (0.5,1,0) + 剑尖 (0.8,0,0)
        AssertV("剑尖世界位置 == (10, 1, -1.3)",
            Mat4x4.TransformPoint(Mat4x4.RotateY(90), new Vector3(10, 0, 0), new Vector3(1.3f, 1, 0)), new Vector3(10, 1, -1.3f));
    }

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }

    static void AssertB(string name, bool actual, bool expected)
    {
        bool pass = actual == expected;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
