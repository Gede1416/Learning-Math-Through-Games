// ============================================================
// 第二轮作业：点积 Dot Product
// 教材：《3D Math Primer》Ch 2.11
// 完成时间：5-10 分钟
// 目标：实现下列 TODO，让 DotProductTests 全部 PASS
// 背景：Day 2 坏代码场景——敌人警戒锥形判定因 toPlayer 未归一化而随距离失真
// 注意：TODO 4 请先用"归一化"思路思考，不要直接抄坏代码
// ============================================================
using System;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.DotProduct;

public static class VectorMath
{
    // TODO 1：点积 a·b = ax*bx + ay*by + az*bz
    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        throw new NotImplementedException("TODO 1: 实现点积");
    }

    // TODO 2：投影长度 proj(a→b) = a · b̂（a 在 b 方向上的有符号投影）
    // 提示：b̂ = b / |b|，用第一轮的 Magnitude/Normalized
    public static float ProjectScalar(Vector3 a, Vector3 b)
    {
        var bn = b.Normalized();
        return Dot(a, bn);
        throw new NotImplementedException("TODO 2: 实现投影长度");
    }

    // TODO 3：夹角角度（度）θ = acos(a·b / (|a||b|))
    // 提示：先求 cosθ，再反余弦转角度；MathF.Acos 返回弧度
    public static float AngleDegrees(Vector3 a, Vector3 b)
    {
        var dot = Dot(a, b);
        var am = a.Magnitude();
        var bm = b.Magnitude();
        var cos = dot / am / bm;
        return MathF.Acos(cos) / MathF.PI * 180;
        throw new NotImplementedException("TODO 3: 实现夹角计算");
    }

    // TODO 4：修复 Day 2 坏代码——判断 to 是否在 from 方向的 coneAngle 度锥形内
    // 要求：结果与 to 的长度无关（距离 1 米和 10 米判定必须一致）
    public static bool IsInCone(Vector3 from, Vector3 to, float coneAngleDeg)
    {
        var isInCone = false;
        var fdir = from - new Vector3(1, 0, 0);
        var todir = from - to;
        var angle = AngleDegrees(fdir, todir);
        isInCone = angle < coneAngleDeg;
        return isInCone;
        throw new NotImplementedException("TODO 4: 实现锥形判定（先归一化！）");
    }
}

// ============ 单元测试骨架（不要改动） ============
public static class DotProductTests
{
    public static void Run()
    {
        // TODO 1：点积
        AssertF("Dot((1,0,0),(0,1,0)) == 0", VectorMath.Dot(new Vector3(1, 0, 0), new Vector3(0, 1, 0)), 0f);
        AssertF("Dot((1,0,0),(1,0,0)) == 1", VectorMath.Dot(new Vector3(1, 0, 0), new Vector3(1, 0, 0)), 1f);
        AssertF("Dot((3,4,0),(1,0,0)) == 3", VectorMath.Dot(new Vector3(3, 4, 0), new Vector3(1, 0, 0)), 3f);
        // TODO 2：投影长度
        AssertF("ProjectScalar((3,4,0)→(1,0,0)) == 3", VectorMath.ProjectScalar(new Vector3(3, 4, 0), new Vector3(1, 0, 0)), 3f);
        AssertF("ProjectScalar((1,0,0)→(3,4,0)) == 0.6", VectorMath.ProjectScalar(new Vector3(1, 0, 0), new Vector3(3, 4, 0)), 0.6f);
        // TODO 3：夹角
        AssertF("Angle((1,0,0),(0,1,0)) == 90", VectorMath.AngleDegrees(new Vector3(1, 0, 0), new Vector3(0, 1, 0)), 90f);
        AssertF("Angle((1,0,0),(-1,0,0)) == 180", VectorMath.AngleDegrees(new Vector3(1, 0, 0), new Vector3(-1, 0, 0)), 180f);
        // TODO 4：警戒锥形（关键案例：距离与判定无关）
        AssertB("IsInCone 正面 0° 距离 1 米 → true",
            VectorMath.IsInCone(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 60f), true);
        AssertB("IsInCone 背面 180° → false",
            VectorMath.IsInCone(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), 60f), false);
        AssertB("IsInCone 夹角 80° 距离 5 米 → false（坏代码会误报！）",
            VectorMath.IsInCone(new Vector3(0, 0, 0), new Vector3(5 * 0.17365f, 0, 5 * 0.98481f), 60f), false);
        AssertB("IsInCone 夹角 80° 距离 0.5 米 → false（距离无关）",
            VectorMath.IsInCone(new Vector3(0, 0, 0), new Vector3(0.5f * 0.17365f, 0, 0.5f * 0.98481f), 60f), false);
    }

    static void AssertF(string name, float actual, float expected)
    {
        bool pass = MathF.Abs(actual - expected) < 0.001f;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }

    static void AssertB(string name, bool actual, bool expected)
    {
        bool pass = actual == expected;
        Console.WriteLine($"[{(pass ? "PASS" : "FAIL")}] {name}：期望 {expected}，实际 {actual}");
    }
}
