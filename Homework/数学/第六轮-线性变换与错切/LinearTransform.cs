// 第六轮：线性变换与错切（故意写坏的风吹草叶代码）
using System;
using StudyNotes.Homework.Math.MatrixVectorMultiplication;

namespace StudyNotes.Homework.Math.LinearTransform;

public static class Transform2D
{
    // 目标：x'=x+k*y, y'=y；当前实现故意错误
    public static Vector2 WindShear(Vector2 vertex, float k)
    {
        var res = new Vector2(
            vertex.X + k * vertex.Y,
            vertex.Y);
        return res;
    }
    //=> 
}

public static class LinearTransformTests
{
    public static void Run()
    {
        AssertV("无风k=0保持顶点", Transform2D.WindShear(new(2, 3), 0), new(2, 3));
        AssertV("顶部中心随高度右移", Transform2D.WindShear(new(0, 2), 0.5f), new(1, 2));
        AssertV("根部右侧保持不动", Transform2D.WindShear(new(1, 0), 0.5f), new(1, 0));
        AssertV("一般顶点按y横向偏移", Transform2D.WindShear(new(1, 2), 0.5f), new(2, 2));
    }

    static void AssertV(string name, Vector2 actual, Vector2 expected)
    {
        bool pass = MathF.Abs(actual.X - expected.X) < 0.001f && MathF.Abs(actual.Y - expected.Y) < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
