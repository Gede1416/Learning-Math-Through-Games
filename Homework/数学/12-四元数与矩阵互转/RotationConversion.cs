using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.Homework.Math.RotationConversion;

public static class ShipRenderBridge
{
    // renderer 接口接收 local→world 纯旋转；不包含位置。
    public static Matrix4x4 CreateShipLocalToWorldRotation(Quaternion orientation)
    {
        // TODO 12.3：故意误以为“换表示方式”需要求逆，回答问题后修整。
        return orientation.ToRotationMatrix();//.Transpose();
    }
}

public static class RotationConversionTests
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
        Console.WriteLine("=== 第12课：互转工具 ===");
        var y90 = Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 1, 0), 90);
        var composite = Quaternion.Multiply(y90,
            Quaternion.CreateFromAxisAngleDegrees(new Vector3(1, 0, 0), 30));
        var compositeMatrix = Matrix4x4.Multiply(Matrix4x4.CreateRotationYDegrees(90), Matrix4x4.CreateRotationXDegrees(30));
        Check("q→M：Identity", SameMatrix(Quaternion.Identity.ToRotationMatrix(), Matrix4x4.Identity));
        Check("q→M：Y+90，检查全部16项", SameMatrix(y90.ToRotationMatrix(), Matrix4x4.CreateRotationYDegrees(90)));
        Check("q→M：组合朝向", SameMatrix(composite.ToRotationMatrix(), compositeMatrix));
        Check("q→M：负号不改变矩阵", SameMatrix(new Quaternion(-y90.X, -y90.Y, -y90.Z, -y90.W).ToRotationMatrix(), Matrix4x4.CreateRotationYDegrees(90)));
        Check("M→q：Identity", SameRotation(Quaternion.CreateFromRotationMatrix(Matrix4x4.Identity), Matrix4x4.Identity));
        Check("M→q：正迹分支及符号", SameRotation(Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateRotationYDegrees(90)), Matrix4x4.CreateRotationYDegrees(90)));
        // 斜轴180°能同时检查最大分量分支、非对角和项与符号。
        var axes = new[] { new Vector3(3, -2, 1), new Vector3(1, 3, -2), new Vector3(-2, 1, 3) };
        foreach (var axis in axes)
        {
            var halfTurn = HalfTurnMatrix(axis);
            Check($"M→q：180°轴{axis}", SameRotation(Quaternion.CreateFromRotationMatrix(halfTurn), halfTurn));
        }
        Check("M→q：混合旋转",
            SameRotation(
                Quaternion.CreateFromRotationMatrix(compositeMatrix), compositeMatrix));

        Check("往返：不只比较forward", SameRotation(Quaternion.CreateFromRotationMatrix(composite.ToRotationMatrix()), compositeMatrix));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过。工具通过后自动运行，无需切换入口。");
            Environment.ExitCode = 1;
            return;
        }
        passed = total = 0;
        Console.WriteLine("=== 第12课：飞船渲染接口 ===");
        Check("初始朝向", SameMatrix(ShipRenderBridge.CreateShipLocalToWorldRotation(Quaternion.Identity), Matrix4x4.Identity));
        var render = ShipRenderBridge.CreateShipLocalToWorldRotation(y90);
        Check("Y+90 forward 应为+X", Near(render.TransformDirection(new Vector3(0, 0, 1)), new Vector3(1, 0, 0)));
        Check("Y+90 right 应为-Z", Near(render.TransformDirection(new Vector3(1, 0, 0)), new Vector3(0, 0, -1)));
        Check("组合朝向保持local→world", SameMatrix(ShipRenderBridge.CreateShipLocalToWorldRotation(composite), compositeMatrix));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    // 测试独立参照：180°旋转为 2*n*nᵀ-I，不调用待实现的互转方法。
    private static Matrix4x4 HalfTurnMatrix(Vector3 axis)
    {
        var n = axis.Normalized();
        return new Matrix4x4(
            2 * n.X * n.X - 1, 2 * n.X * n.Y, 2 * n.X * n.Z, 0,
            2 * n.Y * n.X, 2 * n.Y * n.Y - 1, 2 * n.Y * n.Z, 0,
            2 * n.Z * n.X, 2 * n.Z * n.Y, 2 * n.Z * n.Z - 1, 0,
            0, 0, 0, 1);
    }

    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
    private static bool SameRotation(Quaternion q, Matrix4x4 m) =>
        Near(q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W, 1) &&
        Near(q.TransformDirection(new Vector3(1, 0, 0)), m.TransformDirection(new Vector3(1, 0, 0))) &&
        Near(q.TransformDirection(new Vector3(0, 1, 0)), m.TransformDirection(new Vector3(0, 1, 0))) &&
        Near(q.TransformDirection(new Vector3(0, 0, 1)), m.TransformDirection(new Vector3(0, 0, 1)));
    private static bool SameMatrix(Matrix4x4 a, Matrix4x4 b) =>
        Near(a.M00, b.M00) && Near(a.M01, b.M01) && Near(a.M02, b.M02) && Near(a.M03, b.M03) &&
        Near(a.M10, b.M10) && Near(a.M11, b.M11) && Near(a.M12, b.M12) && Near(a.M13, b.M13) &&
        Near(a.M20, b.M20) && Near(a.M21, b.M21) && Near(a.M22, b.M22) && Near(a.M23, b.M23) &&
        Near(a.M30, b.M30) && Near(a.M31, b.M31) && Near(a.M32, b.M32) && Near(a.M33, b.M33);
}
