// ============================================================
// 第一轮作业：向量定义与基本运算
// 教材：《3D Math Primer》Ch 2（向量）
// 完成时间：5-10 分钟
// 目标：实现下列 TODO，让 Vector3Tests 全部 PASS
// 注意：Normalized 时若模长为 0（零向量）怎么办？思考但不要求实现防御
// ============================================================
using System;

namespace StudyNotes.Homework.Math.VectorBasics;

public struct Vector3
{
    public float X, Y, Z;

    public Vector3(float x, float y, float z)
    {
        X = x; Y = y; Z = z;
    }

    // TODO 1：模长 |v| = sqrt(X² + Y² + Z²)（勾股定理推广到 3D）
    public float Magnitude()
    {
        return MathF.Sqrt(SqrMagnitude());
    }

    // TODO 2：平方模长 X² + Y² + Z²（免开根号，常用于距离比较）
    public float SqrMagnitude()
    {
        return X * X + Y * Y + Z * Z;
        throw new NotImplementedException("TODO 2: 实现平方模长");
    }

    // TODO 3：归一化 v̂ = v / |v|（返回单位向量）
    public Vector3 Normalized()
    {
        var n = Magnitude();
        return new Vector3(X / n, Y / n, Z / n);
        throw new NotImplementedException("TODO 3: 各分量除以模长");
    }

    // TODO 4：向量加法（首尾相连，分量相加）
    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        throw new NotImplementedException("TODO 4: 实现加法");
    }

    // TODO 5：向量减法（分量相减）
    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        throw new NotImplementedException("TODO 5: 实现减法");
    }

    // TODO 6：标量乘法（缩放：每个分量乘 s）
    public static Vector3 operator *(Vector3 a, float s)
    {
        return new Vector3(a.X * s, a.Y * s, a.Z * s);
        throw new NotImplementedException("TODO 6: 实现标量乘法");
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
}

// ============ 单元测试骨架（不要改动） ============
public static class Vector3Tests
{
    public static void Run()
    {
        var a = new Vector3(3f, 4f, 0f);                    // 3-4-5 直角三角形
        Assert("Magnitude(3,4,0) == 5", a.Magnitude(), 5f);
        Assert("SqrMagnitude(3,4,0) == 25", a.SqrMagnitude(), 25f);
        Assert("Normalized(3,4,0) 模长为 1", a.Normalized().Magnitude(), 1f);
        Assert("Normalized(3,4,0).X == 0.6", a.Normalized().X, 0.6f);
        Assert("(3,4,0)+(1,0,0) == (4,4,0)", (a + new Vector3(1f, 0f, 0f)).X, 4f);
        Assert("(3,4,0)-(1,0,0) == (2,4,0)", (a - new Vector3(1f, 0f, 0f)).X, 2f);
        Assert("(3,4,0)*2 == (6,8,0)", (a * 2f).X, 6f);
    }

    static void Assert(string name, float actual, float expected)
    {
        bool pass = System.Math.Abs(actual - expected) < 0.0001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
