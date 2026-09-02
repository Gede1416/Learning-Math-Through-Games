// 第七轮扩展回归测试：复用项目唯一的 Matrix4x4，不再维护第二套矩阵类型。
using System;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.HomogeneousCoordinates;

public static class Matrix4x4ExtendedTests
{
    public static void Run()
    {
        AssertV("Identity 保持点不变",
            Matrix4x4.Identity.TransformPoint(new Vector3(3, 4, 5)), new Vector3(3, 4, 5));

        AssertV("CreateTranslation(10,0,0) 平移原点",
            Matrix4x4.CreateTranslation(new Vector3(10, 0, 0)).TransformPoint(new Vector3(0, 0, 0)), new Vector3(10, 0, 0));
        AssertV("CreateTranslation(2,3,4) 平移点",
            Matrix4x4.CreateTranslation(new Vector3(2, 3, 4)).TransformPoint(new Vector3(1, 1, 1)), new Vector3(3, 4, 5));

        AssertV("CreateRotationYDegrees(90) 将 +Z 转到 +X",
            Matrix4x4.CreateRotationYDegrees(90).TransformDirection(new Vector3(0, 0, 5)), new Vector3(5, 0, 0));

        var translation = Matrix4x4.CreateTranslation(new Vector3(10, 0, 0));
        var rotation = Matrix4x4.CreateRotationYDegrees(90);
        var translationAfterRotation = Matrix4x4.Multiply(translation, rotation);
        var rotationAfterTranslation = Matrix4x4.Multiply(rotation, translation);
        AssertB("T·R ≠ R·T",
            (translationAfterRotation.TransformPoint(new Vector3(1, 0, 0))
             - rotationAfterTranslation.TransformPoint(new Vector3(1, 0, 0))).Magnitude() > 0.5f,
            true);

        AssertV("挂点局部坐标经 T·R 得到剑尖世界位置",
            translationAfterRotation.TransformPoint(new Vector3(1.3f, 1, 0)), new Vector3(10, 1, -1.3f));
    }

    private static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }

    private static void AssertB(string name, bool actual, bool expected)
    {
        bool pass = actual == expected;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
