// 第八轮：变换组合与顺序（故意写坏的自转/公转代码）
using System;
using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.Homework.Math.HomogeneousCoordinates;

namespace StudyNotes.Homework.Math.TransformComposition;

public static class Composition3D
{
    public static Matrix4x4 RotationY(float degrees)
    {
        float r = degrees * MathF.PI / 180f;
        float c = MathF.Cos(r), s = MathF.Sin(r);
        return new Matrix4x4(c, 0, s, 0, 0, 1, 0, 0, -s, 0, c, 0, 0, 0, 0, 1);
    }

    // 正确顺序：局部点先自转，再移动到世界位置
    public static Vector3 LocalToWorld(Vector3 localPoint, Matrix4x4 rotation, Matrix4x4 translation)
    {
        var rotationed = rotation.Transform(localPoint, 1f);
        var translated = translation.Transform(rotationed, 1f);
        return translated;
    }
}

public static class TransformCompositionTests
{
    public static void Run()
    {
        var t = Matrix4x4.Translation(new Vector3(10, 0, 0));
        AssertV("零旋转时正常平移", Composition3D.LocalToWorld(new(1, 0, 0), Composition3D.RotationY(0), t), new(11, 0, 0));
        AssertV("角色原点应保持在世界位置", Composition3D.LocalToWorld(new(0, 0, 0), Composition3D.RotationY(90), t), new(10, 0, 0));
        AssertV("武器尖端先自转再平移", Composition3D.LocalToWorld(new(1, 0, 0), Composition3D.RotationY(90), t), new(10, 0, -1));
    }

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
