// 项目唯一的 4×4 仿射矩阵实现
// 坐标约定：+X 右、+Y 上、+Z 前；列向量 v' = M·v
// Mij 表示第 i 行第 j 列；构造参数按行书写，存储顺序与列向量数学约定是两件事。
using System;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.LinearAlgebra;

public readonly record struct Matrix4x4(
    float M00, float M01, float M02, float M03,
    float M10, float M11, float M12, float M13,
    float M20, float M21, float M22, float M23,
    float M30, float M31, float M32, float M33)
{
    public static Matrix4x4 Identity => new(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1);

    public static Matrix4x4 CreateTranslation(Vector3 offset) => new(
        1, 0, 0, offset.X,
        0, 1, 0, offset.Y,
        0, 0, 1, offset.Z,
        0, 0, 0, 1);

    // +90° 时 +Y→+Z、+Z→-Y；方法名明确角度单位为度。
    public static Matrix4x4 CreateRotationXDegrees(float angleDegrees)
    {
        // TODO（由学生完成）：
        // 1. 把角度转换成弧度；
        // 2. 计算 sin/cos；
        // 3. 按项目坐标约定填写 X 轴旋转矩阵。
        return Identity; // 临时占位：保证项目可编译，非零角测试应失败。
    }

    // +90° 时 +Z→+X、+X→-Z；方法名明确角度单位为度。
    public static Matrix4x4 CreateRotationYDegrees(float angleDegrees)
    {
        float radians = angleDegrees * MathF.PI / 180f;
        float cosine = MathF.Cos(radians);
        float sine = MathF.Sin(radians);

        return new Matrix4x4(
            cosine, 0, sine, 0,
            0, 1, 0, 0,
            -sine, 0, cosine, 0,
            0, 0, 0, 1);
    }

    // 返回 left·right；列向量约定下先应用 right，再应用 left。
    public static Matrix4x4 Multiply(Matrix4x4 left, Matrix4x4 right)
    {
        float m00 = left.M00 * right.M00 + left.M01 * right.M10 + left.M02 * right.M20 + left.M03 * right.M30;
        float m01 = left.M00 * right.M01 + left.M01 * right.M11 + left.M02 * right.M21 + left.M03 * right.M31;
        float m02 = left.M00 * right.M02 + left.M01 * right.M12 + left.M02 * right.M22 + left.M03 * right.M32;
        float m03 = left.M00 * right.M03 + left.M01 * right.M13 + left.M02 * right.M23 + left.M03 * right.M33;

        float m10 = left.M10 * right.M00 + left.M11 * right.M10 + left.M12 * right.M20 + left.M13 * right.M30;
        float m11 = left.M10 * right.M01 + left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31;
        float m12 = left.M10 * right.M02 + left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32;
        float m13 = left.M10 * right.M03 + left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33;

        float m20 = left.M20 * right.M00 + left.M21 * right.M10 + left.M22 * right.M20 + left.M23 * right.M30;
        float m21 = left.M20 * right.M01 + left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31;
        float m22 = left.M20 * right.M02 + left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32;
        float m23 = left.M20 * right.M03 + left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33;

        float m30 = left.M30 * right.M00 + left.M31 * right.M10 + left.M32 * right.M20 + left.M33 * right.M30;
        float m31 = left.M30 * right.M01 + left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31;
        float m32 = left.M30 * right.M02 + left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32;
        float m33 = left.M30 * right.M03 + left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33;

        return new Matrix4x4(
            m00, m01, m02, m03,
            m10, m11, m12, m13,
            m20, m21, m22, m23,
            m30, m31, m32, m33);
    }

    public Matrix4x4 Transpose() => new(
        M00, M10, M20, M30,
        M01, M11, M21, M31,
        M02, M12, M22, M32,
        M03, M13, M23, M33);

    public Vector3 TransformPoint(Vector3 point)
        => TransformHomogeneousXyz(point, homogeneousW: 1f);

    public Vector3 TransformDirection(Vector3 direction)
        => TransformHomogeneousXyz(direction, homogeneousW: 0f);

    private Vector3 TransformHomogeneousXyz(Vector3 value, float homogeneousW) => new(
        M00 * value.X + M01 * value.Y + M02 * value.Z + M03 * homogeneousW,
        M10 * value.X + M11 * value.Y + M12 * value.Z + M13 * homogeneousW,
        M20 * value.X + M21 * value.Y + M22 * value.Z + M23 * homogeneousW);
}
