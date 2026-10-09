using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.LinearAlgebra;

// 分量顺序 (X,Y,Z,W)，Hamilton 乘法；旋转方向与 Matrix4x4 一致。
public readonly record struct Quaternion(float X, float Y, float Z, float W)
{
    public static Quaternion Identity => new(0, 0, 0, 1);

    // TODO 12.1：this 为单位四元数。输出同方向的纯旋转矩阵，无平移/缩放。
    public Matrix4x4 ToRotationMatrix()
    {
        float
        M00 = 1 - 2 * (Y * Y + Z * Z), M01 = 2 * (X * Y - W * Z), M02 = 2 * (X * Z + W * Y),
        M10 = 2 * (X * Y + W * Z), M11 = 1 - 2 * (X * X + Z * Z), M12 = 2 * (Y * Z - W * X),
        M20 = 2 * (X * Z - W * Y), M21 = 2 * (Y * Z + W * X), M22 = 1 - 2 * (X * X + Y * Y);

        return new Matrix4x4(
            M00, M01, M02, 0,
            M10, M11, M12, 0,
            M20, M21, M22, 0,
            0, 0, 0, 1
        );
    }

    // TODO 12.2：输入为纯旋转4×4矩阵（正交、det=+1，无平移/缩放/错切）。
    // 返回表示同一旋转的单位四元数；q 或 -q 都有效，必须支持180°。
    public static Quaternion CreateFromRotationMatrix(Matrix4x4 rotationMatrix)
    {
        var m11 = rotationMatrix.M00;
        var m22 = rotationMatrix.M11;
        var m33 = rotationMatrix.M22;
        var four_power_x = m11 - m22 - m33;
        var four_power_y = m22 - m11 - m33;
        var four_power_z = m33 - m11 - m22;
        var four_power_w = m11 + m22 + m33;
        var max_four_power = MathF.Max(MathF.Max(four_power_x, four_power_y), MathF.Max(four_power_z, four_power_w));
        float def = 0.5f * MathF.Sqrt(1.0f + max_four_power);
        float mult = 0.25f / def;


        if (max_four_power == four_power_w)
        {
            float w = def;
            float x = (rotationMatrix.M21 - rotationMatrix.M12) * mult;
            float y = (rotationMatrix.M02 - rotationMatrix.M20) * mult;
            float z = (rotationMatrix.M10 - rotationMatrix.M01) * mult;
            return new Quaternion(x, y, z, w);
        }
        else if (max_four_power == four_power_x)
        {
            float x = def;
            float w = -(rotationMatrix.M12 - rotationMatrix.M21) * mult;
            float y = (rotationMatrix.M01 + rotationMatrix.M10) * mult;
            float z = (rotationMatrix.M20 + rotationMatrix.M02) * mult;
            return new Quaternion(x, y, z, w);
        }
        else if (max_four_power == four_power_y)
        {
            float y = def;
            float w = -(rotationMatrix.M20 - rotationMatrix.M02) * mult;
            float x = (rotationMatrix.M01 + rotationMatrix.M10) * mult;
            float z = (rotationMatrix.M12 + rotationMatrix.M21) * mult;
            return new Quaternion(x, y, z, w);
        }
        else if (max_four_power == four_power_z)
        {
            float z = def;
            float w = -(rotationMatrix.M01 - rotationMatrix.M10) * mult;
            float x = (rotationMatrix.M20 + rotationMatrix.M02) * mult;
            float y = (rotationMatrix.M12 + rotationMatrix.M21) * mult;
            return new Quaternion(x, y, z, w);
        }
        return Identity;
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
