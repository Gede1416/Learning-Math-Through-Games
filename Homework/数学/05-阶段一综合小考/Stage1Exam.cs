// 第五轮：阶段一综合小考（故意写坏的炮塔瞄准代码）
using System;
using StudyNotes.Homework.Math.MatrixVectorMultiplication;

namespace StudyNotes.Homework.Math.Stage1Exam;

public static class TurretMath
{
    public static bool CanFire(Vector2 turret, Vector2 target, float facingDeg, float halfFovDeg)
    {
        var forward = Matrix2x2.RotationDegrees(facingDeg).Multiply(new Vector2(1, 0));
        var toTarget = new Vector2(target.X - turret.X, target.Y - turret.Y);
        float dot = Vector2.Cos(forward, toTarget);
        halfFovDeg = MathF.Cos(halfFovDeg * MathF.PI / 180f);
        Console.WriteLine("dot：" + dot + " halfFovDeg:" + halfFovDeg);
        return dot >= halfFovDeg;
    }

    public static int TurnDirection(Vector2 forward, Vector2 toTarget)
    {
        return Vector2.LRF(forward, toTarget);
    }
}

public static class Stage1ExamTests
{
    public static void Run()
    {
        var origin = new Vector2(0, 0);
        AssertB("视野内30°、距离0.5 → 开火", TurretMath.CanFire(origin, Polar(30, 0.5f), 0, 60), true);
        AssertB("视野外80°、距离5 → 不开火", TurretMath.CanFire(origin, Polar(80, 5), 0, 60), false);
        AssertB("背后180° → 不开火", TurretMath.CanFire(origin, Polar(180, 1), 0, 60), false);
        AssertI("目标在左 → +1", TurretMath.TurnDirection(new(1, 0), new(0, 1)), 1);
        AssertI("目标在右 → -1", TurretMath.TurnDirection(new(1, 0), new(0, -1)), -1);
        AssertI("目标正前方 → 0", TurretMath.TurnDirection(new(1, 0), new(5, 0)), 0);
    }

    static Vector2 Polar(float degrees, float length)
    {
        float r = degrees * MathF.PI / 180f;
        return new Vector2(MathF.Cos(r) * length, MathF.Sin(r) * length);
    }

    static void AssertB(string name, bool actual, bool expected)
        => Console.WriteLine($"[{(actual == expected ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");

    static void AssertI(string name, int actual, int expected)
        => Console.WriteLine($"[{(actual == expected ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
}
