using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.QuaternionBasics;

public static class ShipRotation
{
    // currentLocalToWorld 表示飞船当前朝向；要求绕飞船自身的 X 轴追加旋转。
    public static Quaternion ApplyLocalPitchDegrees(Quaternion currentLocalToWorld, float deltaDegrees)
    {
        var delta = Quaternion.CreateFromAxisAngleDegrees(new Vector3(1, 0, 0), deltaDegrees);
        // 自身轴增量：先在局部空间应用增量，再由当前朝向映射到世界。
        return Quaternion.Multiply(currentLocalToWorld, delta);
    }
}

public static class QuaternionLessonTests
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
        float s = MathF.Sqrt(0.5f);
        var y90 = new Quaternion(0, s, 0, s);
        Console.WriteLine("=== 第11课：四元数工具 ===");
        Check("零角度", Near(Quaternion.CreateFromAxisAngleDegrees(new Vector3(1, 0, 0), 0), Quaternion.Identity));
        Check("非单位轴 Y+90", Near(Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 3, 0), 90), y90));
        Check("Z-90 半角与符号", Near(Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 0, 1), -90), new Quaternion(0, 0, -s, s)));
        Check("Hamilton i*j=k", Near(Quaternion.Multiply(new Quaternion(1, 0, 0, 0), new Quaternion(0, 1, 0, 0)), new Quaternion(0, 0, 1, 0)));
        Check("Hamilton j*i=-k", Near(Quaternion.Multiply(new Quaternion(0, 1, 0, 0), new Quaternion(1, 0, 0, 0)), new Quaternion(0, 0, -1, 0)));
        Check("一般分量积（不可归一化）", Near(Quaternion.Multiply(new Quaternion(1, 2, 3, 4), new Quaternion(5, 6, 7, 8)), new Quaternion(24, 48, 48, -6)));
        Check("Y+90：前→右，保留长度", Near(y90.TransformDirection(new Vector3(0, 0, 2)), new Vector3(2, 0, 0)));
        Check("q 与 -q 表示同一旋转", Near(new Quaternion(0, -s, 0, -s).TransformDirection(new Vector3(0, 0, 1)), new Vector3(1, 0, 0)));
        Check("Identity 保持方向", Near(Quaternion.Identity.TransformDirection(new Vector3(2, -3, 4)), new Vector3(2, -3, 4)));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过。完成工具后重新运行，会自动进入场景测试。");
            Environment.ExitCode = 1;
            return;
        }

        passed = total = 0;
        Console.WriteLine("=== 第11课：飞船局部旋转 ===");
        Check("零增量保持原朝向", Near(ShipRotation.ApplyLocalPitchDegrees(y90, 0).TransformDirection(new Vector3(0, 0, 1)), new Vector3(1, 0, 0)));
        Check("初始朝向追加 X+90", Near(ShipRotation.ApplyLocalPitchDegrees(Quaternion.Identity, 90).TransformDirection(new Vector3(0, 0, 1)), new Vector3(0, -1, 0)));
        var turned = ShipRotation.ApplyLocalPitchDegrees(y90, 90);
        Check("Y+90 后局部 X+90：forward", Near(turned.TransformDirection(new Vector3(0, 0, 1)), new Vector3(0, -1, 0)));
        Check("Y+90 后局部 X+90：right", Near(turned.TransformDirection(new Vector3(1, 0, 0)), new Vector3(0, 0, -1)));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
    private static bool Near(Quaternion a, Quaternion b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z) && Near(a.W, b.W);
}
