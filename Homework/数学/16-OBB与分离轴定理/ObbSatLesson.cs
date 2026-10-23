using StudyNotes.Homework.Math.CrossProduct;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.MathLibrary.Geometry;

namespace StudyNotes.Homework.Math.ObbSat;

public static class RotatedBoxCollision
{
    // 需求：判断两个旋转盒本身是否相交，不只是世界轴包围范围是否重叠。
    public static bool ShouldCollide(Obb a, Obb b)
    {
        return a.Intersects(b);
    }
}

public static class ObbSatLessonTests
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
        var zero = new Vector3(0, 0, 0);
        var x = new Vector3(1, 0, 0);
        var y = new Vector3(0, 1, 0);
        var z = new Vector3(0, 0, 1);
        var half = new Vector3(2, 1, 0.5f);
        var y90 = Quaternion.CreateFromAxisAngleDegrees(y, 90);
        var z45 = Quaternion.CreateFromAxisAngleDegrees(z, 45);
        var a = Box(zero, new Vector3(1, 1, 1), Quaternion.Identity);
        var farX = Box(new Vector3(3, 0, 0), new Vector3(1, 1, 1), Quaternion.Identity);
        var touching = Box(new Vector3(2, 0, 0), new Vector3(1, 1, 1), Quaternion.Identity);
        var edgeA = Box(zero, new Vector3(1.5f, 0.25f, 0.25f), Quaternion.Identity);
        var edgeB = Box(new Vector3(1.3f, 0.9f, 0.6f), new Vector3(1.5f, 0.25f, 0.25f), Quaternion.CreateFromAxisAngleDegrees(new Vector3(1, 1, 1), 45));
        var thinA = Box(zero, new Vector3(2, 0.2f, 0.2f), z45);
        var thinB = Box(new Vector3(-0.6f, 0.6f, 0), new Vector3(2, 0.2f, 0.2f), z45);
        Console.WriteLine("=== 第16课：OBB与SAT工具 ===");

        // ===== API：Obb.CreateFromLocalToWorldRotation =====
        var made = Obb.CreateFromLocalToWorldRotation(new Vector3(5, -2, 3), half, Quaternion.Identity);
        Check("创建：中心半尺寸及Identity轴", Near(made.Center, new Vector3(5, -2, 3)) && Near(made.HalfExtents, half) && Near(made.AxisX, x) && Near(made.AxisY, y) && Near(made.AxisZ, z));
        made = Obb.CreateFromLocalToWorldRotation(zero, half, y90);
        Check("创建：Y+90局部轴映射", Near(made.AxisX, new Vector3(0, 0, -1)) && Near(made.AxisY, y) && Near(made.AxisZ, x));

        // ===== API：Obb.ProjectionRadiusOnAxis =====
        var wide = Box(zero, half, Quaternion.Identity);
        Check("投影：任意非单位轴", Near(wide.ProjectionRadiusOnAxis(new Vector3(1, 2, 3)), 5.5f));
        Check("投影：轴翻向半径不变", Near(wide.ProjectionRadiusOnAxis(new Vector3(-1, -2, -3)), 5.5f));
        Check("投影：Z+45后的世界X半径", Near(Box(zero, half, z45).ProjectionRadiusOnAxis(x), 3 * MathF.Sqrt(0.5f)));

        // ===== API：Obb.IsSeparatedOnAxis =====
        Check("单轴：明显分离", a.IsSeparatedOnAxis(farX, x));
        Check("单轴：接触不分离", !a.IsSeparatedOnAxis(touching, x));
        Check("单轴：非单位轴同结论", a.IsSeparatedOnAxis(farX, new Vector3(-2, 0, 0)));
        Check("单轴：零轴跳过", !a.IsSeparatedOnAxis(farX, zero));
        Check("单轴：近零轴按平方长度跳过", !a.IsSeparatedOnAxis(farX, new Vector3(1e-7f, 0, 0)));
        Check("单轴：极小间距在容差内", !a.IsSeparatedOnAxis(Box(new Vector3(2.000005f, 0, 0), new Vector3(1, 1, 1), Quaternion.Identity), x));
        Check("单轴：边边叉积能发现分离", edgeA.IsSeparatedOnAxis(edgeB, CrossMath.Cross(edgeA.AxisX, edgeB.AxisX)));

        // ===== API：Obb.Intersects =====
        Check("相交：未旋转盒重叠", a.Intersects(Box(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 1, 1), Quaternion.Identity)));
        Check("相交：X分离", !a.Intersects(farX));
        Check("相交：Y分离", !a.Intersects(Box(new Vector3(0, 3, 0), new Vector3(1, 1, 1), Quaternion.Identity)));
        Check("相交：Z分离", !a.Intersects(Box(new Vector3(0, 0, 3), new Vector3(1, 1, 1), Quaternion.Identity)));
        Check("相交：面接触", a.Intersects(touching));
        var small = Box(zero, new Vector3(0.2f, 0.2f, 0.2f), z45);
        Check("相交：包含且交换对称", a.Intersects(small) && small.Intersects(a));
        Check("相交：同中心旋转盒", a.Intersects(Box(zero, new Vector3(1, 1, 1), z45)));
        Check("相交：世界XYZ重叠但旋转薄盒分离", !thinA.Intersects(thinB));
        Check("相交：6面法线不足，需叉积轴", !edgeA.Intersects(edgeB) && !edgeB.Intersects(edgeA));
        var nearRotation = Quaternion.CreateFromAxisAngleDegrees(y, 0.00001f);
        Check("相交：近乎平行轴且非零负坐标", Box(new Vector3(-5, -3, -2), new Vector3(1, 1, 1), Quaternion.Identity).Intersects(Box(new Vector3(-5, -3, -2), new Vector3(1, 1, 1), nearRotation)));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；工具通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }

        passed = total = 0;
        Console.WriteLine("=== 第16课：旋转盒碰撞 ===");
        // ===== API：RotatedBoxCollision.ShouldCollide =====
        Check("普通重叠", RotatedBoxCollision.ShouldCollide(a, a));
        Check("明显分离", !RotatedBoxCollision.ShouldCollide(a, farX));
        Check("同中心旋转仍重叠", RotatedBoxCollision.ShouldCollide(a, small));
        Check("世界XYZ不足：平行薄盒分离", !RotatedBoxCollision.ShouldCollide(thinA, thinB));
        Check("叉积轴不可漏：斜放薄盒分离", !RotatedBoxCollision.ShouldCollide(edgeA, edgeB));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    // 测试输入独立构造，不调用待实现OBB工厂；只复用上一阶段已验收的方向旋转。
    private static Obb Box(Vector3 center, Vector3 half, Quaternion rotation) => new(center, half,
        rotation.TransformDirection(new Vector3(1, 0, 0)), rotation.TransformDirection(new Vector3(0, 1, 0)), rotation.TransformDirection(new Vector3(0, 0, 1)));
    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
}
