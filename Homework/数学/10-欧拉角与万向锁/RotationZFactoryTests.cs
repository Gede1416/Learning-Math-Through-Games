// 第十轮第二小节：万向锁；当前先验收学生实现的 Z 轴旋转工具。
using System;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.EulerAngles;

public static class RotationZFactoryTests
{
    public static void Run()
    {
        AssertV("Z 轴旋转 0°：+X 保持不变",
            Matrix4x4.CreateRotationZDegrees(0).TransformDirection(new(1, 0, 0)),
            new(1, 0, 0));
        AssertV("Z 轴旋转 +90°：+X→+Y",
            Matrix4x4.CreateRotationZDegrees(90).TransformDirection(new(1, 0, 0)),
            new(0, 1, 0));
        AssertV("Z 轴旋转 +90°：+Y→-X",
            Matrix4x4.CreateRotationZDegrees(90).TransformDirection(new(0, 1, 0)),
            new(-1, 0, 0));
        AssertV("Z 轴旋转 37°：+Z 保持不变",
            Matrix4x4.CreateRotationZDegrees(37).TransformDirection(new(0, 0, 1)),
            new(0, 0, 1));

        float diagonal = MathF.Sqrt(0.5f);
        AssertV("Z 轴旋转 -45°：+X→右下方",
            Matrix4x4.CreateRotationZDegrees(-45).TransformDirection(new(1, 0, 0)),
            new(diagonal, -diagonal, 0));
    }

    private static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
