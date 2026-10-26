using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.MathLibrary.Motion;

namespace StudyNotes.Homework.Math.KinematicsMotion;

public sealed class ProjectileMotion
{
    private readonly Vector3 initialPosition;
    private readonly Vector3 initialVelocity;
    private readonly Vector3 acceleration;

    public float ElapsedSeconds { get; private set; }
    public Vector3 Position { get; private set; }

    public ProjectileMotion(Vector3 initialPosition, Vector3 initialVelocity, Vector3 acceleration)
    {
        this.initialPosition = initialPosition;
        this.initialVelocity = initialVelocity;
        this.acceleration = acceleration;
        Position = initialPosition;
    }

    // TODO 18.5：初态固定，按累计时间采样解析弹道；deltaSeconds>=0。
    public void Advance(float deltaSeconds)
    {
        ElapsedSeconds += deltaSeconds;
        // 故意错误：把本帧时长当作从发射起经过的总时间。
        Position = Kinematics.PositionAtConstantAcceleration(initialPosition, initialVelocity, acceleration, deltaSeconds);
    }
}

public static class KinematicsLessonTests
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
        var gravity = new Vector3(0, -10, 0);
        Console.WriteLine("=== 第18课：位移、速度与加速度工具 ===");

        // ===== API：Kinematics.VelocityFromDisplacement =====
        Check("平均速度：零位移", Near(Kinematics.VelocityFromDisplacement(zero, 2), zero));
        Check("平均速度：位移不是速度", Near(Kinematics.VelocityFromDisplacement(new Vector3(6, 0, 0), 2), new Vector3(3, 0, 0)));
        Check("平均速度：负方向", Near(Kinematics.VelocityFromDisplacement(new Vector3(0, -9, 0), 3), new Vector3(0, -3, 0)));
        Check("平均速度：三分量及小数时长", Near(Kinematics.VelocityFromDisplacement(new Vector3(3, -6, 9), 1.5f), new Vector3(2, -4, 6)));
        Check("平均速度：不足一秒不归一化", Near(Kinematics.VelocityFromDisplacement(new Vector3(1, 2, -1), 0.25f), new Vector3(4, 8, -4)));

        // ===== API：Kinematics.PositionAtConstantVelocity =====
        Check("匀速位置：t=0保留初始位置", Near(Kinematics.PositionAtConstantVelocity(new Vector3(3, -2, 5), new Vector3(4, 6, -8), 0), new Vector3(3, -2, 5)));
        Check("匀速位置：零速度", Near(Kinematics.PositionAtConstantVelocity(new Vector3(1, 2, 3), zero, 5), new Vector3(1, 2, 3)));
        Check("匀速位置：初始位置非原点", Near(Kinematics.PositionAtConstantVelocity(new Vector3(1, 2, 3), new Vector3(2, 0, -1), 3), new Vector3(7, 2, 0)));
        Check("匀速位置：负坐标与三分量", Near(Kinematics.PositionAtConstantVelocity(new Vector3(-3, 4, -5), new Vector3(-2, 1, 6), 2), new Vector3(-7, 6, 7)));
        Check("匀速位置：半秒", Near(Kinematics.PositionAtConstantVelocity(new Vector3(1, -2, 3), new Vector3(2, 4, -6), 0.5f), new Vector3(2, 0, 0)));

        // ===== API：Kinematics.VelocityAtConstantAcceleration =====
        Check("恒加速度速度：t=0保留初速度", Near(Kinematics.VelocityAtConstantAcceleration(new Vector3(3, -2, 5), new Vector3(1, 2, 3), 0), new Vector3(3, -2, 5)));
        Check("恒加速度速度：加速度为零", Near(Kinematics.VelocityAtConstantAcceleration(new Vector3(-2, 3, 4), zero, 7), new Vector3(-2, 3, 4)));
        Check("恒加速度速度：最高点只竖直分量为零", Near(Kinematics.VelocityAtConstantAcceleration(new Vector3(2, 10, -3), gravity, 1), new Vector3(2, 0, -3)));
        Check("恒加速度速度：下降阶段", Near(Kinematics.VelocityAtConstantAcceleration(new Vector3(2, 10, -3), gravity, 2), new Vector3(2, -10, -3)));
        Check("恒加速度速度：各轴独立", Near(Kinematics.VelocityAtConstantAcceleration(new Vector3(1, -2, 3), new Vector3(2, 4, -6), 0.5f), new Vector3(2, 0, 0)));

        // ===== API：Kinematics.PositionAtConstantAcceleration =====
        Check("恒加速度位置：t=0保留初始位置", Near(Kinematics.PositionAtConstantAcceleration(new Vector3(3, -2, 5), new Vector3(4, 6, -8), new Vector3(2, -10, 4), 0), new Vector3(3, -2, 5)));
        Check("恒加速度位置：零加速度退化为匀速", Near(Kinematics.PositionAtConstantAcceleration(new Vector3(1, 2, 3), new Vector3(2, 0, -1), zero, 3), new Vector3(7, 2, 0)));
        Check("恒加速度位置：静止释放须有一半系数", Near(Kinematics.PositionAtConstantAcceleration(new Vector3(1, 8, -2), zero, gravity, 1), new Vector3(1, 3, -2)));
        Check("恒加速度位置：抛体顶点", Near(Kinematics.PositionAtConstantAcceleration(zero, new Vector3(2, 10, 0), gravity, 1), new Vector3(2, 5, 0)));
        Check("恒加速度位置：小数时间与三轴加速度", Near(Kinematics.PositionAtConstantAcceleration(new Vector3(1, -2, 3), new Vector3(2, 4, -6), new Vector3(4, -8, 2), 0.5f), new Vector3(2.5f, -1, 0.25f)));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；工具通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }

        passed = total = 0;
        Console.WriteLine("=== 第18课：抛体与分帧采样 ===");
        // ===== API：ProjectileMotion.Advance =====
        var projectile = Simulate(0.5f);
        Check("单帧半秒", Near(projectile.Position, new Vector3(1, 3.75f, 0)) && Near(projectile.ElapsedSeconds, 0.5f));
        projectile = Simulate(0.5f, 0.5f);
        Check("两帧累计一秒", Near(projectile.Position, new Vector3(2, 5, 0)) && Near(projectile.ElapsedSeconds, 1));
        projectile = Simulate(0.25f, 0.25f, 0.25f, 0.25f);
        Check("四帧一秒同位置", Near(projectile.Position, new Vector3(2, 5, 0)) && Near(projectile.ElapsedSeconds, 1));
        projectile = Simulate(0.125f, 0.375f, 0.5f);
        Check("不均匀分帧仍到相同时刻", Near(projectile.Position, new Vector3(2, 5, 0)) && Near(projectile.ElapsedSeconds, 1));
        projectile = Simulate(0.5f, 0.5f, 0.5f, 0.5f);
        Check("两秒回到发射高度但仍向前", Near(projectile.Position, new Vector3(4, 0, 0)) && Near(projectile.ElapsedSeconds, 2));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    private static ProjectileMotion Simulate(params float[] steps)
    {
        var projectile = new ProjectileMotion(new Vector3(0, 0, 0), new Vector3(2, 10, 0), new Vector3(0, -10, 0));
        foreach (float step in steps) projectile.Advance(step);
        return projectile;
    }
    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
}
