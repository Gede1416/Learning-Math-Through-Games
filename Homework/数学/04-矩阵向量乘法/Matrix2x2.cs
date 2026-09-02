// 第四轮作业：矩阵与列向量相乘（5-10 分钟）
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace StudyNotes.Homework.Math.MatrixVectorMultiplication;

public readonly record struct Vector2(float X, float Y)
{
    // 向量平方模
    public float Super()
    {
        return X * X + Y * Y;
    }
    // 向量值
    public float MSuper()
    {
        return MathF.Sqrt(Super());
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static float Cos(Vector2 a, Vector2 b)
    {
        var d = Dot(a, b) / a.MSuper() / b.MSuper();
        return d;
    }

    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    public static int LRF(Vector2 a, Vector2 b)
    {
        int res = 0;
        var rl = Cross(a, b);
        if (rl > 0)
            res = 1;
        if (rl < 0)
            res = -1;
        return res;
    }


    // x
}

public readonly record struct Matrix2x2(float M00, float M01, float M10, float M11)
{
    // TODO 1：返回 M × v；每个结果分量 = 矩阵对应行与 v 的点积
    public Vector2 Multiply(Vector2 v)
    {
        var xt = v.X * M00 + v.Y * M01;
        var yt = v.X * M10 + v.Y * M11;
        return new Vector2(xt, yt);
    }
    //=> throw new NotImplementedException("TODO 1: 实现矩阵乘列向量");

    // TODO 2：构造逆时针旋转 degrees 的矩阵 [cos -sin; sin cos]
    public static Matrix2x2 RotationDegrees(float degrees)
    {
        var h = degrees / 360 * 2 * MathF.PI;
        var sin = MathF.Sin(h);
        var cos = MathF.Cos(h);

        return new Matrix2x2
        {
            M00 = cos,
            M01 = -sin,
            M10 = sin,
            M11 = cos
        };
    }
    //=> throw new NotImplementedException("TODO 2: 构造旋转矩阵");
}

public static class Matrix2x2Tests
{
    public static void Run()
    {
        AssertV("单位矩阵保持向量", new Matrix2x2(1, 0, 0, 1).Multiply(new Vector2(3, 4)), new(3, 4));
        AssertV("一般矩阵乘向量", new Matrix2x2(1, 2, 3, 4).Multiply(new Vector2(5, 6)), new(17, 39));
        AssertV("+90°：右→上", Matrix2x2.RotationDegrees(90).Multiply(new Vector2(1, 0)), new(0, 1));
        AssertV("+90°：上→左", Matrix2x2.RotationDegrees(90).Multiply(new Vector2(0, 1)), new(-1, 0));
        AssertV("-90°：上→右", Matrix2x2.RotationDegrees(-90).Multiply(new Vector2(0, 1)), new(1, 0));
    }

    static void AssertV(string name, Vector2 actual, Vector2 expected)
    {
        bool pass = MathF.Abs(actual.X - expected.X) < 0.001f && MathF.Abs(actual.Y - expected.Y) < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
