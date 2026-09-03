// 第十轮：欧拉角与万向锁
// 当前步骤：先由学生补齐统一数学工具 CreateRotationXDegrees。
using System;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.EulerAngles;

public static class RotationXFactoryTests
{
    public static void Run()
    {
        AssertV("X 轴旋转 0°：+Y 保持不变",
            Matrix4x4.CreateRotationXDegrees(0).TransformDirection(new(0, 1, 0)),
            new(0, 1, 0));
        AssertV("X 轴旋转 +90°：+Y→+Z",
            Matrix4x4.CreateRotationXDegrees(90).TransformDirection(new(0, 1, 0)),
            new(0, 0, 1));
        AssertV("X 轴旋转 +90°：+Z→-Y",
            Matrix4x4.CreateRotationXDegrees(90).TransformDirection(new(0, 0, 1)),
            new(0, -1, 0));

        float diagonal = MathF.Sqrt(0.5f);
        AssertV("X 轴旋转 -45°：+Z→上前方",
            Matrix4x4.CreateRotationXDegrees(-45).TransformDirection(new(0, 0, 1)),
            new(0, diagonal, diagonal));
    }

    private static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
