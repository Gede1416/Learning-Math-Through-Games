using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.MathLibrary.Geometry;

namespace StudyNotes.Homework.Math.SphereBox;

public static class ExplosionDamage
{
    // 需求：爆炸球覆盖目标OBB的任意部分即命中，包含接触，不考虑墙体遮挡。
    public static bool ShouldAffectTarget(Sphere blast, Obb target)
    {
        // TODO 17.6：故意错误，只测目标中心；请分析长墙漏报并复用本课工具修正。
        return blast.Intersects(target);
    }
}

public static class SphereBoxLessonTests
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
        var one = new Vector3(1, 1, 1);
        var aabb = new Aabb(new Vector3(-1, -1, -1), one);
        var unitObb = Obb.CreateFromLocalToWorldRotation(zero, one, Quaternion.Identity);
        var y90 = Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 1, 0), 90);
        var y45 = Quaternion.CreateFromAxisAngleDegrees(new Vector3(0, 1, 0), 45);
        var shifted = Obb.CreateFromLocalToWorldRotation(new Vector3(3, -2, 5), new Vector3(2, 1, 0.5f), y90);
        var diagonal = Obb.CreateFromLocalToWorldRotation(zero, new Vector3(2, 1, 0.5f), y45);
        Console.WriteLine("=== 第17课：最近点与球体相交工具 ===");

        // ===== API：Aabb.ClosestPoint =====
        Check("AABB最近点：面外", Near(aabb.ClosestPoint(new Vector3(3, 0.25f, 0)), new Vector3(1, 0.25f, 0)));
        Check("AABB最近点：边外", Near(aabb.ClosestPoint(new Vector3(3, -2, 0.5f)), new Vector3(1, -1, 0.5f)));
        Check("AABB最近点：角外", Near(aabb.ClosestPoint(new Vector3(3, 2, -4)), new Vector3(1, 1, -1)));
        var inside = new Vector3(0.25f, -0.5f, 0.75f);
        Check("AABB最近点：内部原样返回", Near(aabb.ClosestPoint(inside), inside));
        var offsetAabb = new Aabb(new Vector3(-5, -2, 3), new Vector3(-1, 2, 7));
        Check("AABB最近点：非原点及负坐标", Near(offsetAabb.ClosestPoint(new Vector3(-8, 1, 9)), new Vector3(-5, 1, 7)));
        Check("AABB最近点：零尺寸盒", Near(new Aabb(one, one).ClosestPoint(new Vector3(4, 2, -3)), one));

        // ===== API：Obb.ClosestPoint =====
        Check("OBB最近点：Identity", Near(unitObb.ClosestPoint(new Vector3(3, -2, 0.5f)), new Vector3(1, -1, 0.5f)));
        inside = new Vector3(3, -2, 5.25f);
        Check("OBB最近点：旋转盒内部不推向表面", Near(shifted.ClosestPoint(inside), inside));
        Check("OBB最近点：Y+90且中心非原点", Near(shifted.ClosestPoint(new Vector3(5, 1, 1)), new Vector3(3.5f, -1, 3)));
        Check("OBB最近点：Y+45须限制局部分量", Near(diagonal.ClosestPoint(new Vector3(4, 0, 0)), new Vector3(1.767767f, 0, -1.0606602f)));

        // ===== API：Sphere.Intersects(Sphere) =====
        var sphere = new Sphere(zero, 1);
        Check("球球：重叠", sphere.Intersects(new Sphere(new Vector3(1.5f, 0, 0), 1)));
        Check("球球：半径相加且相切", new Sphere(zero, 2).Intersects(new Sphere(new Vector3(3, 0, 0), 1)));
        Check("球球：分离", !sphere.Intersects(new Sphere(new Vector3(3.25f, 0, 0), 1)));
        var small = new Sphere(new Vector3(0.25f, 0, 0), 0.25f);
        Check("球球：包含及交换对称", sphere.Intersects(small) && small.Intersects(sphere));
        Check("球球：两个零半径同点", new Sphere(zero, 0).Intersects(new Sphere(zero, 0)));
        Check("球球：两个零半径异点", !new Sphere(zero, 0).Intersects(new Sphere(one, 0)));

        // ===== API：Sphere.Intersects(Aabb) =====
        Check("球AABB：面接触", new Sphere(new Vector3(1.5f, 0, 0), 0.5f).Intersects(aabb));
        Check("球AABB：明显分离", !new Sphere(new Vector3(3, 0, 0), 0.5f).Intersects(aabb));
        Check("球AABB：球心在盒内", new Sphere(zero, 0.1f).Intersects(aabb));
        Check("球AABB：角外拒绝膨胀盒误报", !new Sphere(new Vector3(1.8f, 1.8f, 0), 1).Intersects(aabb));
        Check("球AABB：角附近相交", new Sphere(new Vector3(1.6f, 1.6f, 0), 1).Intersects(aabb));
        Check("球AABB：零半径在角上", new Sphere(one, 0).Intersects(aabb));
        Check("球AABB：大球包住盒体", new Sphere(zero, 2).Intersects(aabb));
        Check("球AABB：半径也必须平方", new Sphere(new Vector3(2.5f, 0, 0), 2).Intersects(aabb));

        // ===== API：Sphere.Intersects(Obb) =====
        Check("球OBB：Identity面接触", new Sphere(new Vector3(2, 0, 0), 1).Intersects(unitObb));
        Check("球OBB：旋转长盒末端相交", new Sphere(new Vector3(1.5f, 0, -1.5f), 0.2f).Intersects(diagonal));
        Check("球OBB：世界包围范围内仍可分离", !new Sphere(new Vector3(1.6f, 0, 1.6f), 0.3f).Intersects(diagonal));
        Check("球OBB：Y+90长边命中", new Sphere(new Vector3(3, -2, 7.5f), 0.75f).Intersects(shifted));
        Check("球OBB：Y+90短边分离", !new Sphere(new Vector3(4.5f, -2, 5), 0.75f).Intersects(shifted));
        Check("球OBB：零半径在角上", new Sphere(one, 0).Intersects(unitObb));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；工具通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }

        passed = total = 0;
        Console.WriteLine("=== 第17课：爆炸范围检测 ===");
        // ===== API：ExplosionDamage.ShouldAffectTarget =====
        Check("爆心位于目标中心", ExplosionDamage.ShouldAffectTarget(new Sphere(zero, 0.2f), unitObb));
        var longWall = Obb.CreateFromLocalToWorldRotation(new Vector3(5, 0, 0), new Vector3(4.5f, 1, 0.5f), Quaternion.Identity);
        Check("长墙中心很远但近端被覆盖", ExplosionDamage.ShouldAffectTarget(sphere, longWall));
        Check("旋转盒末端被覆盖", ExplosionDamage.ShouldAffectTarget(new Sphere(new Vector3(1.5f, 0, -1.5f), 0.2f), diagonal));
        Check("旋转盒世界范围内但实际未覆盖", !ExplosionDamage.ShouldAffectTarget(new Sphere(new Vector3(1.6f, 0, 1.6f), 0.3f), diagonal));
        Check("爆点远离目标", !ExplosionDamage.ShouldAffectTarget(new Sphere(new Vector3(10, 0, 0), 0.5f), unitObb));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    // 旋转最近点断言容忍浮点舍入；布尔切边测试只用精确轴对齐数值。
    private static bool Near(Vector3 a, Vector3 b) => MathF.Abs(a.X - b.X) < 0.0001f
        && MathF.Abs(a.Y - b.Y) < 0.0001f && MathF.Abs(a.Z - b.Z) < 0.0001f;
}
