// 第九轮：世界坐标转相机坐标（故意错误地使用正向旋转）
using System;
using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.Homework.Math.HomogeneousCoordinates;
using StudyNotes.Homework.Math.TransformComposition;

namespace StudyNotes.Homework.Math.CoordinateSpaces;

public static class CameraSpace
{
    // cameraRotation 表示相机局部→世界；当前实现故意错误
    public static Vector3 WorldToCamera(Vector3 worldPoint, Vector3 cameraPosition, Matrix4x4 cameraRotation)
    {
        var relative = worldPoint - cameraPosition;
        return cameraRotation.Transform(relative, 0f);
    }
}

public static class CoordinateSpacesTests
{
    public static void Run()
    {
        AssertV("无旋转相机：世界相对位置不变",
            CameraSpace.WorldToCamera(new(12, 0, 0), new(10, 0, 0), Composition3D.RotationY(0)), new(2, 0, 0));
        AssertV("相机面向+X：前方目标应在局部+Z",
            CameraSpace.WorldToCamera(new(15, 0, 0), new(10, 0, 0), Composition3D.RotationY(90)), new(0, 0, 5));
        AssertV("相机面向+X：局部右侧目标应在+X",
            CameraSpace.WorldToCamera(new(10, 0, -2), new(10, 0, 0), Composition3D.RotationY(90)), new(2, 0, 0));
    }

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
