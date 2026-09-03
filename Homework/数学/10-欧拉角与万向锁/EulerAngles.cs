// 第十轮：欧拉角与万向锁（第一小节：旋转顺序）
// 场景：FPS 相机使用世界 Y 偏航和相机局部 X 俯仰。
using System;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.EulerAngles;

public static class EulerCamera
{
    public static Matrix4x4 CreateCameraLocalToWorldRotationDegrees(
        float yawDegrees,
        float pitchDegrees)
    {
        var yawAroundWorldY = Matrix4x4.CreateRotationYDegrees(yawDegrees);
        var pitchAroundLocalX = Matrix4x4.CreateRotationXDegrees(pitchDegrees);

        // TODO：当前组合顺序故意写错。列向量下，最右侧矩阵先作用。
        return Matrix4x4.Multiply(pitchAroundLocalX, yawAroundWorldY);
    }

    public static Vector3 CalculateForwardDirectionDegrees(float yawDegrees, float pitchDegrees)
        => CreateCameraLocalToWorldRotationDegrees(yawDegrees, pitchDegrees)
            .TransformDirection(new Vector3(0, 0, 1));
}

public static class EulerAnglesTests
{
    public static void Run()
    {
        AssertV("X+90°：+Y→+Z",
            Matrix4x4.CreateRotationXDegrees(90).TransformDirection(new(0, 1, 0)), new(0, 0, 1));
        AssertV("零角度：保持面向+Z",
            EulerCamera.CalculateForwardDirectionDegrees(0, 0), new(0, 0, 1));
        AssertV("仅偏航+90°：面向+X",
            EulerCamera.CalculateForwardDirectionDegrees(90, 0), new(1, 0, 0));
        AssertV("仅俯仰-90°：面向+Y",
            EulerCamera.CalculateForwardDirectionDegrees(0, -90), new(0, 1, 0));

        float diagonal = MathF.Sqrt(0.5f);
        AssertV("偏航+90°后再沿相机局部X俯仰-45°",
            EulerCamera.CalculateForwardDirectionDegrees(90, -45), new(diagonal, diagonal, 0));
    }

    private static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
