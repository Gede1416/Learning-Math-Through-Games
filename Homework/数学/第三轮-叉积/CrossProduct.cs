// ============================================================
// 第三轮作业：叉积 Cross Product
// 教材：《3D Math Primer》Ch 2.12
// 完成时间：5-10 分钟
// 目标：实现下列 TODO，让 CrossProductTests 全部 PASS
// 背景：Day 3 坏代码场景——敌人左右转向判定方向反了
// 注意：叉积顺序敏感 a×b = -(b×a)；Unity 为左手坐标系
// ============================================================
using System;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.CrossProduct;

public static class CrossMath
{
    // TODO 1：叉积 a×b = (ay*bz-az*by, az*bx-ax*bz, ax*by-ay*bx)
    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        throw new NotImplementedException("TODO 1: 实现叉积");
    }

    // TODO 2：三角面法线 = Cross(v1-v0, v2-v0)（顶点按逆时针绕序时为外侧法线）
    public static Vector3 TriangleNormal(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        throw new NotImplementedException("TODO 2: 实现面法线");
    }

    // TODO 3：左右判定（俯视角 XZ 平面）
    // forward 为朝向，toTarget 为指向目标的向量
    // 返回：+1 = 目标在左侧，-1 = 目标在右侧，0 = 正前方（叉积 y ≈ 0）
    // 提示：先算 cross = forward × toTarget，看 cross.y 的符号（Unity 左手系！）
    public static int Side(Vector3 forward, Vector3 toTarget)
    {
        throw new NotImplementedException("TODO 3: 实现左右判定");
    }
}

// ============ 单元测试骨架（不要改动） ============
public static class CrossProductTests
{
    public static void Run()
    {
        var x = new Vector3(1, 0, 0);
        var y = new Vector3(0, 1, 0);
        var z = new Vector3(0, 0, 1);

        // TODO 1：叉积
        AssertV("x×y == z", CrossMath.Cross(x, y), z);
        AssertV("y×x == -z（交换反号）", CrossMath.Cross(y, x), new Vector3(0, 0, -1));
        AssertV("x×x == 0", CrossMath.Cross(x, x), new Vector3(0, 0, 0));
        AssertF("|(3,0,0)×(0,4,0)| == 12（面积）", CrossMath.Cross(new Vector3(3, 0, 0), new Vector3(0, 4, 0)).Magnitude(), 12f);
        AssertF("cross·a == 0（垂直）", CrossMath.DotHelper(CrossMath.Cross(new Vector3(1, 2, 3), new Vector3(4, 5, 6)), new Vector3(1, 2, 3)), 0f);
        // TODO 2：面法线（右手系 CCW 绕序）
        AssertV("TriangleNormal(0,0,0)(1,0,0)(0,1,0) == +z",
            CrossMath.TriangleNormal(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0)), z);
        // TODO 3：左右判定（Unity 左手系：forward=+Z，左侧 = -X）
        AssertI("面朝 +Z，目标在左(-X) → +1",
            CrossMath.Side(z, new Vector3(-1, 0, 0)), 1);
        AssertI("面朝 +Z，目标在右(+X) → -1",
            CrossMath.Side(z, new Vector3(1, 0, 0)), -1);
        AssertI("面朝 +Z，目标正前方 → 0",
            CrossMath.Side(z, new Vector3(0, 0, 5)), 0);
    }

    public static float DotHelper(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    static void AssertV(string name, Vector3 actual, Vector3 expected)
    {
        bool pass = (actual - expected).Magnitude() < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }

    static void AssertF(string name, float actual, float expected)
    {
        bool pass = MathF.Abs(actual - expected) < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }

    static void AssertI(string name, int actual, int expected)
    {
        bool pass = actual == expected;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
