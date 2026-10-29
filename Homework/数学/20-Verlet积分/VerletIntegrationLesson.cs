using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.MathLibrary.Motion;

namespace StudyNotes.Homework.Math.VerletMotion;

// 游戏特效粒子的自由运动，也是绳索/布料粒子施加约束之前的一步。
public sealed class VerletParticle
{
    public PositionVerletState State { get; private set; }
    public Vector3 Acceleration { get; }
    public float FixedStepSeconds { get; }

    public VerletParticle(Vector3 initialPosition, Vector3 initialVelocity,
        Vector3 acceleration, float fixedStepSeconds)
    {
        Acceleration = acceleration;
        FixedStepSeconds = fixedStepSeconds;
        State = VerletIntegration.InitializePositionVerlet(initialPosition, initialVelocity,
            acceleration, fixedStepSeconds);
    }

    // 调用一次就是一个固定物理步；本课不重复实现第19课的渲染帧累加器。
    public void Step()
    {
        State = VerletIntegration.StepPositionVerlet(State, Acceleration, FixedStepSeconds);
    }

    // TODO 20.4a：瞬移到目标点但保留运动，不是施加冲量。
    public void TeleportPreservingMotion(Vector3 targetPosition)
    {
        Vector3 v = (State.Position - State.PreviousPosition) / FixedStepSeconds + 0.5f * Acceleration * FixedStepSeconds;
        // 故意错误：只搬当前点，历史点仍在原地。
        State = VerletIntegration.InitializePositionVerlet(targetPosition, v, Acceleration, FixedStepSeconds);
    }

    // TODO 20.4b：从指定位置/速度重新发射，继续使用原来的加速度和固定步长。
    public void Restart(Vector3 initialPosition, Vector3 initialVelocity)
    {
        // 故意错误：两个点都填起点，丢失指定初速度及启动所需的加速度项。
        State = VerletIntegration.InitializePositionVerlet(initialPosition, initialVelocity, Acceleration, FixedStepSeconds);
    }
}

public static class VerletIntegrationLessonTests
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
        var launchVelocity = new Vector3(2, 4, 0);
        var shifted = new Vector3(3, -2, 5);
        var mixedVelocity = new Vector3(4, -2, 1);
        var mixedAcceleration = new Vector3(-8, 4, 2);
        var launchHistory = new PositionVerletState(new Vector3(-1, -3.25f, 0), zero);
        Console.WriteLine("=== 第20课：位置Verlet工具 ===");

        // ===== API：VerletIntegration.InitializePositionVerlet =====
        Check("初始化：静止无加速度", Near(VerletIntegration.InitializePositionVerlet(shifted, zero, zero, 0.5f), new PositionVerletState(shifted, shifted)));
        Check("初始化：匀速历史倒退一个h", Near(VerletIntegration.InitializePositionVerlet(shifted, mixedVelocity, zero, 0.5f), new PositionVerletState(new Vector3(1, -1, 4.5f), shifted)));
        Check("初始化：静止重力也需要0.5*a*h²", Near(VerletIntegration.InitializePositionVerlet(zero, zero, gravity, 0.5f), new PositionVerletState(new Vector3(0, -1.25f, 0), zero)));
        Check("初始化：发射历史不是简单p0-v0*h", Near(VerletIntegration.InitializePositionVerlet(zero, launchVelocity, gravity, 0.5f), launchHistory));
        Check("初始化：非原点与三轴加速度", Near(VerletIntegration.InitializePositionVerlet(shifted, mixedVelocity, mixedAcceleration, 0.5f), new PositionVerletState(new Vector3(0, -0.5f, 4.75f), shifted)));
        Check("初始化：h改变时重新生成历史", Near(VerletIntegration.InitializePositionVerlet(zero, launchVelocity, gravity, 0.25f), new PositionVerletState(new Vector3(-0.5f, -1.3125f, 0), zero)));

        // ===== API：VerletIntegration.StepPositionVerlet =====
        Check("步进：静止无加速度", Near(VerletIntegration.StepPositionVerlet(new PositionVerletState(shifted, shifted), zero, 0.5f), new PositionVerletState(shifted, shifted)));
        Check("步进：匀速保留每步位移", Near(VerletIntegration.StepPositionVerlet(new PositionVerletState(new Vector3(1, -1, 4.5f), shifted), zero, 0.5f), new PositionVerletState(shifted, new Vector3(5, -3, 5.5f))));
        Check("步进：从静止下落首步1.25米", Near(VerletIntegration.StepPositionVerlet(new PositionVerletState(new Vector3(0, -1.25f, 0), zero), gravity, 0.5f), new PositionVerletState(zero, new Vector3(0, -1.25f, 0))));
        Check("步进：加速度项为a*h²且历史正确移交", Near(VerletIntegration.StepPositionVerlet(launchHistory, gravity, 0.5f), new PositionVerletState(zero, new Vector3(1, 0.75f, 0))));
        Check("步进：非原点与三轴加速度", Near(VerletIntegration.StepPositionVerlet(new PositionVerletState(new Vector3(0, -0.5f, 4.75f), shifted), mixedAcceleration, 0.5f), new PositionVerletState(shifted, new Vector3(4, -2.5f, 5.75f))));
        var twoSteps = VerletIntegration.StepPositionVerlet(VerletIntegration.StepPositionVerlet(launchHistory, gravity, 0.5f), gravity, 0.5f);
        Check("步进：连续两步保存的是相邻两点", Near(twoSteps, new PositionVerletState(new Vector3(1, 0.75f, 0), new Vector3(2, -1, 0))));

        // ===== API：VerletIntegration.EstimateCenteredVelocity =====
        Check("居中速度：静止", Near(VerletIntegration.EstimateCenteredVelocity(shifted, shifted, 0.5f), zero));
        Check("居中速度：两点相隔2h而非h", Near(VerletIntegration.EstimateCenteredVelocity(zero, new Vector3(2, 0, 0), 0.5f), new Vector3(2, 0, 0)));
        Check("居中速度：非原点三轴与不同h", Near(VerletIntegration.EstimateCenteredVelocity(shifted, new Vector3(7, -4, 6), 0.25f), new Vector3(8, -4, 2)));
        Check("居中速度：虚拟上一点与首步终点给出初速度", Near(VerletIntegration.EstimateCenteredVelocity(launchHistory.PreviousPosition, new Vector3(1, 0.75f, 0), 0.5f), launchVelocity));
        Check("居中速度：0秒与1秒两点估计0.5秒而非1秒", Near(VerletIntegration.EstimateCenteredVelocity(zero, new Vector3(2, -1, 0), 0.5f), new Vector3(2, -1, 0)));
        Check("居中速度：h=1仍须除以2h", Near(VerletIntegration.EstimateCenteredVelocity(new Vector3(-2, -9, 0), new Vector3(2, -1, 0), 1), launchVelocity));

        // ===== 对照：位置Verlet与Kinematics恒加速度解析基准 =====
        var exactAtOne = Kinematics.PositionAtConstantAcceleration(zero, launchVelocity, gravity, 1);
        var halfSteps = Repeat(VerletIntegration.InitializePositionVerlet(zero, launchVelocity, gravity, 0.5f), gravity, 0.5f, 2);
        var quarterSteps = Repeat(VerletIntegration.InitializePositionVerlet(zero, launchVelocity, gravity, 0.25f), gravity, 0.25f, 4);
        Check("对照：h=0.5累计到1秒匹配解析位置", Near(exactAtOne, new Vector3(2, -1, 0)) && Near(halfSteps, new PositionVerletState(new Vector3(1, 0.75f, 0), exactAtOne)));
        Check("对照：h=0.25同样到1秒，历史点属于0.75秒", Near(quarterSteps, new PositionVerletState(new Vector3(1.5f, 0.1875f, 0), exactAtOne)));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；20项工具与对照通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }

        passed = total = 0;
        Console.WriteLine("=== 第20课：瞬移与重新发射 ===");

        // ===== API：VerletParticle.TeleportPreservingMotion =====
        var particle = new VerletParticle(zero, new Vector3(2, 0, 0), zero, 0.5f);
        particle.TeleportPreservingMotion(new Vector3(10, 3, -2));
        Check("瞬移：两个历史位置一起平移", Near(particle.State, new PositionVerletState(new Vector3(9, 3, -2), new Vector3(10, 3, -2))));
        particle.Step();
        Check("瞬移：后续仍每步前进1米而非突然弹飞", Near(particle.State, new PositionVerletState(new Vector3(10, 3, -2), new Vector3(11, 3, -2))));
        particle = new VerletParticle(zero, launchVelocity, gravity, 0.5f);
        particle.Step();
        particle.TeleportPreservingMotion(new Vector3(10, 5, 0));
        particle.Step();
        Check("瞬移：重力轨迹只平移而不凭空改变运动", Near(particle.State, new PositionVerletState(new Vector3(10, 5, 0), new Vector3(11, 3.25f, 0))));

        // ===== API：VerletParticle.Restart =====
        particle.Restart(shifted, mixedVelocity);
        bool resetHistory = Near(particle.State, new PositionVerletState(new Vector3(1, -2.25f, 4.5f), shifted));
        particle.Step();
        Check("重启：用指定初速度和原加速度重建历史", resetHistory && Near(particle.State, new PositionVerletState(shifted, new Vector3(5, -4.25f, 5.5f))));
        var restPosition = new Vector3(7, 5, -1);
        particle.Restart(restPosition, zero);
        bool restHistory = Near(particle.State, new PositionVerletState(new Vector3(7, 3.75f, -1), restPosition));
        particle.Step();
        Check("重启：静止发射也正确下落且清除旧运动", restHistory && Near(particle.State, new PositionVerletState(restPosition, new Vector3(7, 3.75f, -1))));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    private static PositionVerletState Repeat(PositionVerletState state, Vector3 acceleration, float h, int count)
    {
        for (int i = 0; i < count; i++) state = VerletIntegration.StepPositionVerlet(state, acceleration, h);
        return state;
    }
    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
    private static bool Near(PositionVerletState a, PositionVerletState b) => Near(a.PreviousPosition, b.PreviousPosition) && Near(a.Position, b.Position);
}
