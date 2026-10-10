using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.RotationInterpolation;

public static class CameraTurn
{
    // 固定起终点，durationSeconds>0；elapsedSeconds为开始以来的累计秒数。
    // 需求：沿最短路径，在指定时长内近似恒定角速度转到目标，结束后保持目标。
    public static Quaternion SampleOrientation(Quaternion start, Quaternion target,
        float elapsedSeconds, float durationSeconds)
    {
        // TODO 13.5：两处故意错误：进度的单位与插值工具选择。
        float t = elapsedSeconds / durationSeconds;
        return Quaternion.SlerpShortestPath(start, target, t);
    }
}

public static class RotationInterpolationTests
{
    public static void Run()
    {
        int passed = 0, total = 0;
        void Check(string name, bool ok)
        {
            total++;
            if (ok) passed++;
            Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name}");
        }
        var start = Quaternion.Identity;
        var y120 = Yaw(120);
        var minusY120 = new Quaternion(-y120.X, -y120.Y, -y120.Z, -y120.W);
        Console.WriteLine("=== 第13课：插值工具 ===");
        Check("Dot：四分量一般值", Near(Quaternion.Dot(new Quaternion(1, 2, 3, 4), new Quaternion(5, 6, 7, 8)), 70));
        Check("Dot：q与-q为-1", Near(Quaternion.Dot(y120, minusY120), -1));
        var normalized = new Quaternion(0, 3, 0, 4).Normalized();
        Check("Normalized：四维长度", Near(normalized.X, 0) && Near(normalized.Y, 0.6f) && Near(normalized.Z, 0) && Near(normalized.W, 0.8f));
        Check("Nlerp：t<0取起点", SameRotation(Quaternion.NlerpShortestPath(start, y120, -0.2f), start));
        Check("Nlerp：t>1取终点", SameRotation(Quaternion.NlerpShortestPath(start, y120, 1.2f), y120));
        Check("Nlerp：中点且长度为1", SameRotation(Quaternion.NlerpShortestPath(start, y120, 0.5f), Yaw(60)));
        Check("Nlerp：q与-q不产生零四元数", SameRotation(Quaternion.NlerpShortestPath(y120, minusY120, 0.5f), y120));
        // 线性混合的独立数值参照：atan2(y,w)*2 = 27.795772°，不是30°。
        Check("Nlerp：四分之一处不是匀速", SameRotation(Quaternion.NlerpShortestPath(start, y120, 0.25f), Yaw(27.795772f)));
        Check("Slerp：四分之一处为30°", SameRotation(Quaternion.SlerpShortestPath(start, y120, 0.25f), Yaw(30)));
        Check("Slerp：四分之三处为90°", SameRotation(Quaternion.SlerpShortestPath(start, y120, 0.75f), Yaw(90)));
        Check("Slerp：跨±180°取短路", SameRotation(Quaternion.SlerpShortestPath(Yaw(170), Yaw(-170), 0.5f), Yaw(180)));
        Check("Slerp：q与-q", SameRotation(Quaternion.SlerpShortestPath(y120, minusY120, 0.5f), y120));
        Check("Slerp：极近角退化分支", SameRotation(Quaternion.SlerpShortestPath(start, Yaw(0.01f), 0.5f), Yaw(0.005f)));
        Check("Slerp：边界限制", SameRotation(Quaternion.SlerpShortestPath(start, y120, -1), start) && SameRotation(Quaternion.SlerpShortestPath(start, y120, 2), y120));
        // 非Y轴且起点非Identity，防止只插值yaw或只看forward的错误实现。
        var rotatedStart = Quaternion.CreateFromAxisAngleDegrees(new Vector3(1, 0, 0), 40);
        var delta = Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 0, 1), 120);
        var rotatedTarget = Quaternion.Multiply(rotatedStart, delta);
        var expected = Quaternion.Multiply(rotatedStart, Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 0, 1), 30));
        Check("Slerp：完整朝向混合", SameRotation(Quaternion.SlerpShortestPath(rotatedStart, rotatedTarget, 0.25f), expected));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；工具通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }
        passed = total = 0;
        Console.WriteLine("=== 第13课：两秒镜头转向 ===");
        foreach (var seconds in new[] { 0f, 0.5f, 1f, 1.5f, 2f, 3f })
        {
            float degrees = System.Math.Min(seconds / 2f, 1f) * 120f;
            Check($"累计{seconds}秒→{degrees}°", SameRotation(CameraTurn.SampleOrientation(start, y120, seconds, 2f), Yaw(degrees)));
        }
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    private static Quaternion Yaw(float degrees) => Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 1, 0), degrees);
    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.00001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
    // 不调用本课Dot/Normalized作为测试判据，避免实现错误与测试互相掩盖。
    private static bool SameRotation(Quaternion a, Quaternion b) =>
        Near(a.X * a.X + a.Y * a.Y + a.Z * a.Z + a.W * a.W, 1f) &&
        Near(a.TransformDirection(new Vector3(1, 0, 0)), b.TransformDirection(new Vector3(1, 0, 0))) &&
        Near(a.TransformDirection(new Vector3(0, 1, 0)), b.TransformDirection(new Vector3(0, 1, 0))) &&
        Near(a.TransformDirection(new Vector3(0, 0, 1)), b.TransformDirection(new Vector3(0, 0, 1)));
}
