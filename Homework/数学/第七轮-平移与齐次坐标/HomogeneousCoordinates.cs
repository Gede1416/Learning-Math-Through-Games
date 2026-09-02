// 第七轮：平移与齐次坐标（故意写坏的武器方向变换）
using System;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.HomogeneousCoordinates;

public readonly record struct Matrix4x4(
    float M00, float M01, float M02, float M03,
    float M10, float M11, float M12, float M13,
    float M20, float M21, float M22, float M23,
    float M30, float M31, float M32, float M33)
{
    public static Matrix4x4 Translation(Vector3 t) => new(
        1, 0, 0, t.X,
        0, 1, 0, t.Y,
        0, 0, 1, t.Z,
        0, 0, 0, 1);

    public Vector3 Transform(Vector3 v, float w) => new(
        M00 * v.X + M01 * v.Y + M02 * v.Z + M03 * w,
        M10 * v.X + M11 * v.Y + M12 * v.Z + M13 * w,
        M20 * v.X + M21 * v.Y + M22 * v.Z + M23 * w);
}

public static class Transform3D
{
    public static Vector3 TransformPoint(Matrix4x4 m, Vector3 point)
        => m.Transform(point, 1f);

    // 方向使用 w=0，因此不受矩阵平移列影响
    public static Vector3 TransformDirection(Matrix4x4 m, Vector3 direction)
        => m.Transform(direction, 0f);
}

public static class HomogeneousCoordinatesTests
{
    public static void Run()
    {
        var m = Matrix4x4.Translation(new Vector3(10, 2, 5));
        AssertV("枪口点随武器平移", Transform3D.TransformPoint(m, new Vector3(0, 0, 1)), new Vector3(10, 2, 6));
        AssertV("枪口方向不受平移影响", Transform3D.TransformDirection(m, new Vector3(0, 0, 1)), new Vector3(0, 0, 1));
        AssertV("零方向仍是零方向", Transform3D.TransformDirection(m, new Vector3(0, 0, 0)), new Vector3(0, 0, 0));
    }

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
