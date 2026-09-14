// 第二小节：允许局部 roll 的 FPS 相机；俯仰设计范围为 [-89°, +89°]。
using System;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.EulerAngles;

public static class GimbalCamera
{
    // yaw 绕世界 Y，pitch 绕局部 X，roll 绕随后局部 Z；所有参数单位为度。
    public static Matrix4x4 CreateCameraLocalToWorldRotationDegrees(
        float yawDegrees, float requestedPitchDegrees, float rollDegrees)
    {
        // TODO 10.4：边界故意错误。自行完成分析、修整和测试，整节完成后统一反馈。
        float pitchDegrees = System.Math.Clamp(requestedPitchDegrees, -89f, 89f);

        var yaw = Matrix4x4.CreateRotationYDegrees(yawDegrees);
        var pitch = Matrix4x4.CreateRotationXDegrees(pitchDegrees);
        var roll = Matrix4x4.CreateRotationZDegrees(rollDegrees);
        return Matrix4x4.Multiply(Matrix4x4.Multiply(yaw, pitch), roll);
    }
}

public static class GimbalLockTests
{
    public static void Run()
    {
        AssertV("零角度：前方保持+Z",
            GimbalCamera.CreateCameraLocalToWorldRotationDegrees(0, 0, 0)
                .TransformDirection(new(0, 0, 1)), new(0, 0, 1));
        AssertV("roll +90°：局部右方向转向+Y",
            GimbalCamera.CreateCameraLocalToWorldRotationDegrees(0, 0, 90)
                .TransformDirection(new(1, 0, 0)), new(0, 1, 0));
        float diagonal = MathF.Sqrt(0.5f);
        AssertV("pitch +45°：范围内输入保持不变",
            GimbalCamera.CreateCameraLocalToWorldRotationDegrees(0, 45, 0)
                .TransformDirection(new(0, 0, 1)), new(0, -diagonal, diagonal));

        float limitRadians = 89f * MathF.PI / 180f;
        float sine = MathF.Sin(limitRadians);
        float cosine = MathF.Cos(limitRadians);
        AssertV("请求 pitch +90°：按相机设计限制到+89°",
            GimbalCamera.CreateCameraLocalToWorldRotationDegrees(0, 90, 0)
                .TransformDirection(new(0, 0, 1)), new(0, -sine, cosine));
        AssertV("请求 pitch -90°：按相机设计限制到-89°",
            GimbalCamera.CreateCameraLocalToWorldRotationDegrees(0, -90, 0)
                .TransformDirection(new(0, 0, 1)), new(0, sine, cosine));
    }

    private static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
