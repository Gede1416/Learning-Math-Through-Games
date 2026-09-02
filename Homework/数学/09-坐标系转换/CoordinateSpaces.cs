// 第九轮：世界点转换到相机局部坐标
using System;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.CoordinateSpaces;

// 项目统一约定：+X 右、+Y 上、+Z 前；列向量；Y+90° 时 +Z→+X、+X→-Z
public static class CameraSpace
{
    // cameraLocalToWorldRotation 表示相机局部→世界；纯旋转的逆等于转置。
    public static Vector3 TransformWorldPointToCamera(
        Vector3 worldPoint,
        Vector3 cameraWorldPosition,
        Matrix4x4 cameraLocalToWorldRotation)
    {
        var worldOffsetFromCamera = worldPoint - cameraWorldPosition;
        var worldToCameraRotation = cameraLocalToWorldRotation.Transpose();
        return worldToCameraRotation.TransformDirection(worldOffsetFromCamera);
    }
}

public static class CoordinateSpacesTests
{
    public static void Run()
    {
        AssertV("项目约定：Y+90° 将局部+Z转到世界+X",
            Matrix4x4.CreateRotationYDegrees(90).TransformDirection(new(0, 0, 1)), new(1, 0, 0));
        AssertV("无旋转相机：世界相对位置不变",
            CameraSpace.TransformWorldPointToCamera(new(12, 0, 0), new(10, 0, 0), Matrix4x4.CreateRotationYDegrees(0)), new(2, 0, 0));
        AssertV("相机面向+X：前方目标应在局部+Z",
            CameraSpace.TransformWorldPointToCamera(new(15, 0, 0), new(10, 0, 0), Matrix4x4.CreateRotationYDegrees(90)), new(0, 0, 5));
        AssertV("相机面向+X：局部右侧目标应在+X",
            CameraSpace.TransformWorldPointToCamera(new(10, 0, -2), new(10, 0, 0), Matrix4x4.CreateRotationYDegrees(90)), new(2, 0, 0));
    }

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
