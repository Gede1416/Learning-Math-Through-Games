// 第八轮：变换组合与顺序（故意写坏的自转/公转代码）
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.TransformComposition;

public static class Composition3D
{
    // 局部点先旋转，再移动到对象的世界位置：T·R·localPoint
    public static Vector3 TransformLocalPointToWorld(
        Vector3 localPoint,
        Matrix4x4 localRotation,
        Vector3 worldPosition)
    {
        var translation = Matrix4x4.CreateTranslation(worldPosition);
        var localToWorld = Matrix4x4.Multiply(translation, localRotation);
        return localToWorld.TransformPoint(localPoint);
    }
}

public static class TransformCompositionTests
{
    public static void Run()
    {
        var worldPosition = new Vector3(10, 0, 0);
        AssertV("零旋转时正常平移",
            Composition3D.TransformLocalPointToWorld(new(1, 0, 0), Matrix4x4.CreateRotationYDegrees(0), worldPosition), new(11, 0, 0));
        AssertV("角色原点应保持在世界位置",
            Composition3D.TransformLocalPointToWorld(new(0, 0, 0), Matrix4x4.CreateRotationYDegrees(90), worldPosition), new(10, 0, 0));
        AssertV("武器尖端先自转再平移",
            Composition3D.TransformLocalPointToWorld(new(1, 0, 0), Matrix4x4.CreateRotationYDegrees(90), worldPosition), new(10, 0, -1));
    }

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
