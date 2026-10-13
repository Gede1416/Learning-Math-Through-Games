using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.MathLibrary.Geometry;

namespace StudyNotes.Homework.Math.AabbIntersection;

public static class PickupTrigger
{
    // 需求：玩家盒与拾取盒有任意交集（含边界接触），就触发。
    public static bool ShouldTrigger(Aabb playerBounds, Aabb pickupBounds)
    {
        // TODO 15.4：故意将盒体相交缩减成只检测玩家中心点。
        return pickupBounds.Intersects(playerBounds);
    }
}

public static class AabbLessonTests
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
        Console.WriteLine("=== 第15课：AABB数学工具 ===");

        // ===== API：Aabb.CreateFromCenterAndHalfExtents =====
        var made = Aabb.CreateFromCenterAndHalfExtents(new Vector3(5, -2, 10), new Vector3(1, 2, 3));
        Check("创建：非零中心和半尺寸", Near(made.Min, new Vector3(4, -4, 7)) && Near(made.Max, new Vector3(6, 0, 13)));
        made = Aabb.CreateFromCenterAndHalfExtents(new Vector3(-4, 1, 7), new Vector3(0, 0, 0));
        Check("创建：零尺寸允许退化点", Near(made.Min, new Vector3(-4, 1, 7)) && Near(made.Max, new Vector3(-4, 1, 7)));

        // ===== API：Aabb.ContainsPoint =====
        // 固定输入独立构造，不调用待实现工厂，避免两种错误互相掩盖。
        var box = new Aabb(new Vector3(-1, -2, -3), new Vector3(1, 2, 3));
        Check("点：内部", box.ContainsPoint(new Vector3(0, 0, 0)));
        Check("点：Min和Max边界均包含", box.ContainsPoint(box.Min) && box.ContainsPoint(box.Max));
        Check("点：仅X越界", !box.ContainsPoint(new Vector3(1.1f, 0, 0)));
        Check("点：仅Y越界", !box.ContainsPoint(new Vector3(0, 2.1f, 0)));
        Check("点：仅Z越界", !box.ContainsPoint(new Vector3(0, 0, -3.1f)));

        // ===== API：Aabb.Intersects =====
        var a = new Aabb(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
        Check("盒：部分重叠", a.Intersects(new Aabb(new Vector3(1, 1, 1), new Vector3(3, 3, 3))));
        Check("盒：仅X分离", !a.Intersects(new Aabb(new Vector3(3, 0, 0), new Vector3(4, 2, 2))));
        Check("盒：仅Y分离", !a.Intersects(new Aabb(new Vector3(0, 3, 0), new Vector3(2, 4, 2))));
        Check("盒：仅Z分离", !a.Intersects(new Aabb(new Vector3(0, 0, 3), new Vector3(2, 2, 4))));
        Check("盒：面接触算相交", a.Intersects(new Aabb(new Vector3(2, 0, 0), new Vector3(3, 2, 2))));
        Check("盒：角接触算相交", a.Intersects(new Aabb(new Vector3(2, 2, 2), new Vector3(3, 3, 3))));
        var inside = new Aabb(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 1, 1));
        Check("盒：包含关系交换顺序仍相交", a.Intersects(inside) && inside.Intersects(a));
        var horizontal = new Aabb(new Vector3(-3, -0.5f, -1), new Vector3(3, 0.5f, 1));
        var vertical = new Aabb(new Vector3(-0.5f, -3, -1), new Vector3(0.5f, 3, 1));
        Check("盒：十字相交无需角点进入", horizontal.Intersects(vertical));
        var pointBox = new Aabb(new Vector3(1, 1, 1), new Vector3(1, 1, 1));
        Check("盒：退化点盒在内部也相交", a.Intersects(pointBox) && pointBox.Intersects(a));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；工具通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }
        passed = total = 0;
        Console.WriteLine("=== 第15课：拾取触发器 ===");

        // ===== API：PickupTrigger.ShouldTrigger =====
        var pickup = new Aabb(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
        Check("中心进入：正常触发", PickupTrigger.ShouldTrigger(new Aabb(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f)), pickup));
        Check("中心在外：玩家边缘进入也触发", PickupTrigger.ShouldTrigger(new Aabb(new Vector3(0.8f, -0.5f, -0.5f), new Vector3(2.8f, 0.5f, 0.5f)), pickup));
        Check("边界接触：也触发", PickupTrigger.ShouldTrigger(new Aabb(new Vector3(1, -0.5f, -0.5f), new Vector3(2, 0.5f, 0.5f)), pickup));
        Check("完全分离：不触发", !PickupTrigger.ShouldTrigger(new Aabb(new Vector3(2, -0.5f, -0.5f), new Vector3(3, 0.5f, 0.5f)), pickup));
        Check("玩家包住小拾取区：也触发", PickupTrigger.ShouldTrigger(new Aabb(new Vector3(-2, -2, -2), new Vector3(2, 2, 2)), new Aabb(new Vector3(1.5f, -0.25f, -0.25f), new Vector3(2, 0.25f, 0.25f))));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
}
