using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.MathLibrary.Geometry;

namespace StudyNotes.Homework.Math.RayIntersection;

public static class HitscanShot
{
    // aimDirection非零，可以不是单位长度；range是世界距离，输入均在世界空间。
    public static bool TryHitSphereWithinRange(Vector3 muzzle, Vector3 aimDirection,
        Vector3 center, float radius, float range, out Vector3 hitPoint)
    {
        hitPoint = default;
        // TODO 14.4：故意把参数t当作距离。自行分析后修正。
        if (!RayIntersections.TryIntersectSphere(muzzle, aimDirection, center, radius, out float t))
            return false;
        if (t * aimDirection.Magnitude() > range) return false;
        hitPoint = RayIntersections.PointOnRay(muzzle, aimDirection, t);
        return true;
    }
}

public static class RayIntersectionLessonTests
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
        var origin = new Vector3(0, 0, 0);
        var forward = new Vector3(0, 0, 1);
        var up = new Vector3(0, 1, 0);
        Console.WriteLine("=== 第14课：射线数学工具 ===");
        Check("射线点：t=0保留起点", Near(RayIntersections.PointOnRay(new Vector3(1, 2, 3), forward, 0), new Vector3(1, 2, 3)));
        Check("射线点：非单位方向与非零起点", Near(RayIntersections.PointOnRay(new Vector3(1, 2, 3), new Vector3(0, 0, 2), 2), new Vector3(1, 2, 7)));
        bool hit = RayIntersections.TryIntersectPlane(new Vector3(0, 2, 0), new Vector3(0, -1, 0), origin, up, out float t);
        Check("平面：前方交点t=2", hit && Near(t, 2));
        hit = RayIntersections.TryIntersectPlane(new Vector3(0, 2, 0), new Vector3(0, -2, 0), origin, up, out t);
        Check("平面：方向倍长参数减半", hit && Near(t, 1));
        hit = RayIntersections.TryIntersectPlane(new Vector3(0, 2, 0), up, origin, up, out t);
        Check("平面：负根拒绝且t清零", !hit && Near(t, 0));
        hit = RayIntersections.TryIntersectPlane(new Vector3(0, 2, 0), forward, origin, up, out t);
        Check("平面：平行不命中", !hit && Near(t, 0));
        hit = RayIntersections.TryIntersectPlane(origin, forward, origin, up, out t);
        Check("平面：共面无唯一交点", !hit && Near(t, 0));
        hit = RayIntersections.TryIntersectPlane(origin, up, origin, up, out t);
        Check("平面：双面且起点在面上t=0", hit && Near(t, 0));
        hit = RayIntersections.TryIntersectPlane(new Vector3(0, 2, 0), new Vector3(1, -0.0000001f, 0), origin, up, out t);
        Check("平面：近似平行", !hit && Near(t, 0));
        var center = new Vector3(0, 0, 5);
        hit = RayIntersections.TryIntersectSphere(origin, forward, center, 1, out t);
        Check("球面：取入口4而非出口6", hit && Near(t, 4));
        hit = RayIntersections.TryIntersectSphere(origin, new Vector3(0, 0, 2), center, 1, out t);
        Check("球面：非单位方向t=2", hit && Near(t, 2));
        hit = RayIntersections.TryIntersectSphere(origin, forward, new Vector3(1, 0, 5), 1, out t);
        Check("球面：切线delta=0也命中", hit && Near(t, 5));
        hit = RayIntersections.TryIntersectSphere(origin, forward, new Vector3(2, 0, 5), 1, out t);
        Check("球面：delta负不命中", !hit && Near(t, 0));
        hit = RayIntersections.TryIntersectSphere(origin, forward, new Vector3(0, 0, -5), 1, out t);
        Check("球面：两个负根都拒绝", !hit && Near(t, 0));
        hit = RayIntersections.TryIntersectSphere(origin, forward, origin, 2, out t);
        Check("球面：内部起点取出口2", hit && Near(t, 2));
        hit = RayIntersections.TryIntersectSphere(new Vector3(0, 0, 4), new Vector3(0, 0, -1), center, 1, out t);
        Check("球面：表面起点向外仍命中t=0", hit && Near(t, 0));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；工具通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }

        passed = total = 0;
        Console.WriteLine("=== 第14课：枪线射程 ===");
        var aim = new Vector3(0, 0, 10);
        hit = HitscanShot.TryHitSphereWithinRange(origin, aim, new Vector3(0, 0, 3), 1, 4, out var point);
        Check("近目标：命中点z=2", hit && Near(point, new Vector3(0, 0, 2)));
        hit = HitscanShot.TryHitSphereWithinRange(origin, aim, new Vector3(0, 0, 10), 1, 4, out point);
        Check("远目标：9米超出4米射程", !hit && Near(point, origin));
        hit = HitscanShot.TryHitSphereWithinRange(origin, aim, new Vector3(0, 0, 10), 1, 9, out point);
        Check("边界：9米射程包含9米交点", hit && Near(point, new Vector3(0, 0, 9)));
        hit = HitscanShot.TryHitSphereWithinRange(new Vector3(2, 0, 0), aim, new Vector3(2, 0, 10), 1, 12, out point);
        Check("非零枪口：世界命中点(2,0,9)", hit && Near(point, new Vector3(2, 0, 9)));
        hit = HitscanShot.TryHitSphereWithinRange(origin, aim, new Vector3(3, 0, 10), 1, 20, out point);
        Check("偏离枪线：射程再长也不命中", !hit && Near(point, origin));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
}
