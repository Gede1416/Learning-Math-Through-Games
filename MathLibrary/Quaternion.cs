using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.LinearAlgebra;

// 分量顺序 (X,Y,Z,W)，Hamilton 乘法；旋转方向与 Matrix4x4 一致。
public readonly record struct Quaternion(float X, float Y, float Z, float W)
{
    public static Quaternion Identity => new(0, 0, 0, 1);

    // TODO 12.1：this 为单位四元数。输出同方向的纯旋转矩阵，无平移/缩放。
    public Matrix4x4 ToRotationMatrix()
    {
        return Matrix4x4.Identity; // 未实现：可旋转三个基向量后按列组装。
    }

    // TODO 12.2：输入为纯旋转4×4矩阵（正交、det=+1，无平移/缩放/错切）。
    // 返回表示同一旋转的单位四元数；q 或 -q 都有效，必须支持180°。
    public static Quaternion CreateFromRotationMatrix(Matrix4x4 rotationMatrix)
    {
        return Identity; // 未实现：四分支公式见第12课笔记。
    }

    // TODO 11.1：输入非零轴（不保证单位长度）与角度，返回单位旋转四元数。
    public static Quaternion CreateFromAxisAngleDegrees(Vector3 axis, float angleDegrees)
    {
        float length = MathF.Sqrt(axis.X * axis.X + axis.Y * axis.Y + axis.Z * axis.Z);
        float x = axis.X / length;
        float y = axis.Y / length;
        float z = axis.Z / length;

        float halfAngle = angleDegrees * MathF.PI / 180f / 2f;
        float sinHalf = MathF.Sin(halfAngle);
        float cosHalf = MathF.Cos(halfAngle);

        return new Quaternion(x * sinHalf, y * sinHalf, z * sinHalf, cosHalf);
    }

    // TODO 11.2：Hamilton 积 left * right，旋转组合时右侧先作用。
    // 本方法也必须支持非单位四元数，不可擅自归一化结果。
    public static Quaternion Multiply(Quaternion left, Quaternion right)
    {
        float ax = left.X, ay = left.Y, az = left.Z, aw = left.W;
        float bx = right.X, by = right.Y, bz = right.Z, bw = right.W;

        return new Quaternion(
            aw * bx + ax * bw + ay * bz - az * by,
            aw * by - ax * bz + ay * bw + az * bx,
            aw * bz + ax * by - ay * bx + az * bw,
            aw * bw - ax * bx - ay * by - az * bz);
    }

    // TODO 11.3：this 必须是单位四元数；方向不必是单位向量，保留其长度。
    public Vector3 TransformDirection(Vector3 direction)
    {
        var q = this;
        var qConjugate = new Quaternion(-q.X, -q.Y, -q.Z, q.W);
        var p = new Quaternion(direction.X, direction.Y, direction.Z, 0f);

        var rotated = Multiply(Multiply(q, p), qConjugate);
        return new Vector3(rotated.X, rotated.Y, rotated.Z);
    }
}
