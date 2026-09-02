// 第七轮：平移与齐次坐标（故意写坏的武器方向变换）
using System;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.HomogeneousCoordinates;

public static class HomogeneousCoordinatesTests
{
    public static void Run()
    {
        var translation = Matrix4x4.CreateTranslation(new Vector3(10, 2, 5));
        AssertV("枪口点随武器平移", translation.TransformPoint(new Vector3(0, 0, 1)), new Vector3(10, 2, 6));
        AssertV("枪口方向不受平移影响", translation.TransformDirection(new Vector3(0, 0, 1)), new Vector3(0, 0, 1));
        AssertV("零方向仍是零方向", translation.TransformDirection(new Vector3(0, 0, 0)), new Vector3(0, 0, 0));
    }

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
