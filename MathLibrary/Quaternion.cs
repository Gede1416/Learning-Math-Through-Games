using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.LinearAlgebra;

// 分量顺序 (X,Y,Z,W)，Hamilton 乘法；旋转方向与 Matrix4x4 一致。
public readonly record struct Quaternion(float X, float Y, float Z, float W)
{
    public static Quaternion Identity => new(0, 0, 0, 1);

    // TODO 11.1：输入非零轴（不保证单位长度）与角度，返回单位旋转四元数。
    public static Quaternion CreateFromAxisAngleDegrees(Vector3 axis, float angleDegrees)
    {
        return Identity; // 待实现：公式见本节笔记。
    }

    // TODO 11.2：Hamilton 积 left * right，旋转组合时右侧先作用。
    // 本方法也必须支持非单位四元数，不可擅自归一化结果。
    public static Quaternion Multiply(Quaternion left, Quaternion right)
    {
        return Identity; // 待实现。
    }

    // TODO 11.3：this 必须是单位四元数；方向不必是单位向量，保留其长度。
    public Vector3 TransformDirection(Vector3 direction)
    {
        return direction; // 待实现：q * (direction, 0) * conjugate(q)。
    }
}
