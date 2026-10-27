using StudyNotes.Homework.Math.VectorBasics;
using StudyNotes.MathLibrary.Motion;

namespace StudyNotes.Homework.Math.EulerIntegration;

public sealed class FixedStepJump
{
    private readonly Vector3 acceleration;
    public float FixedStepSeconds { get; }
    public float RemainingSeconds { get; private set; }
    public float SimulatedSeconds { get; private set; }
    public MotionState State { get; private set; }

    // 输入保证fixedStepSeconds>0；本课无地面碰撞，允许下降到发射高度以下。
    public FixedStepJump(MotionState initialState, Vector3 acceleration, float fixedStepSeconds)
    {
        State = initialState;
        this.acceleration = acceleration;
        FixedStepSeconds = fixedStepSeconds;
    }

    // TODO 19.3：一帧可消费0/1/多次固定物理步，余量保留；frameDeltaSeconds>=0。
    // ponytail: 本课只处理受控短帧；生产环境另设追帧预算，避免长帧过度追赶。
    public void Tick(float frameDeltaSeconds)
    {
        RemainingSeconds += frameDeltaSeconds;
        // 故意错误1：长帧可能需要多次物理步，但这里只处理一次。
        while (RemainingSeconds >= FixedStepSeconds)
        {
            // 故意错误2：每个物理步不应使用整个渲染帧的时长。
            State = MotionIntegration.StepSemiImplicitEuler(State, acceleration, FixedStepSeconds);
            RemainingSeconds -= FixedStepSeconds;
            SimulatedSeconds += FixedStepSeconds;
        }
    }
}

public static class EulerIntegrationLessonTests
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
        var initial = new MotionState(zero, new Vector3(2, 4, 0));
        var shifted = new MotionState(new Vector3(3, -2, 5), new Vector3(4, -2, 1));
        var mixedAcceleration = new Vector3(-8, 4, 2);
        Console.WriteLine("=== 第19课：欧拉工具与误差对照 ===");

        // ===== API：MotionIntegration.StepExplicitEuler =====
        Check("显式：零步长", Near(MotionIntegration.StepExplicitEuler(shifted, mixedAcceleration, 0), shifted));
        Check("显式：零加速度匀速", Near(MotionIntegration.StepExplicitEuler(shifted, zero, 0.5f), new MotionState(new Vector3(5, -3, 5.5f), shifted.Velocity)));
        Check("显式：静止下落本步位置先不变", Near(MotionIntegration.StepExplicitEuler(new MotionState(zero, zero), gravity, 0.5f), new MotionState(zero, new Vector3(0, -5, 0))));
        Check("显式：位置使用旧速度", Near(MotionIntegration.StepExplicitEuler(initial, gravity, 0.5f), new MotionState(new Vector3(1, 2, 0), new Vector3(2, -1, 0))));
        Check("显式：平移与三轴加速度", Near(MotionIntegration.StepExplicitEuler(shifted, mixedAcceleration, 0.5f), new MotionState(new Vector3(5, -3, 5.5f), new Vector3(0, 0, 2))));
        var explicitTwo = MotionIntegration.StepExplicitEuler(MotionIntegration.StepExplicitEuler(initial, gravity, 0.5f), gravity, 0.5f);
        Check("显式：从新状态连续两步", Near(explicitTwo, new MotionState(new Vector3(2, 1.5f, 0), new Vector3(2, -6, 0))));

        // ===== API：MotionIntegration.StepSemiImplicitEuler =====
        Check("半隐式：零步长", Near(MotionIntegration.StepSemiImplicitEuler(shifted, mixedAcceleration, 0), shifted));
        Check("半隐式：零加速度匀速", Near(MotionIntegration.StepSemiImplicitEuler(shifted, zero, 0.5f), new MotionState(new Vector3(5, -3, 5.5f), shifted.Velocity)));
        Check("半隐式：静止下落本步位置即改变", Near(MotionIntegration.StepSemiImplicitEuler(new MotionState(zero, zero), gravity, 0.5f), new MotionState(new Vector3(0, -2.5f, 0), new Vector3(0, -5, 0))));
        Check("半隐式：位置使用新速度", Near(MotionIntegration.StepSemiImplicitEuler(initial, gravity, 0.5f), new MotionState(new Vector3(1, -0.5f, 0), new Vector3(2, -1, 0))));
        Check("半隐式：平移与三轴加速度", Near(MotionIntegration.StepSemiImplicitEuler(shifted, mixedAcceleration, 0.5f), new MotionState(new Vector3(3, -2, 6), new Vector3(0, 0, 2))));
        var semiTwo = MotionIntegration.StepSemiImplicitEuler(MotionIntegration.StepSemiImplicitEuler(initial, gravity, 0.5f), gravity, 0.5f);
        Check("半隐式：从新状态连续两步", Near(semiTwo, new MotionState(new Vector3(2, -3.5f, 0), new Vector3(2, -6, 0))));

        // ===== 对照：两种Euler重复步进与Kinematics解析基准 =====
        var exactPosition = Kinematics.PositionAtConstantAcceleration(initial.Position, initial.Velocity, gravity, 1);
        var exactVelocity = Kinematics.VelocityAtConstantAcceleration(initial.Velocity, gravity, 1);
        var explicitHalf = Repeat(initial, gravity, 0.5f, 2, useSemiImplicit: false);
        var semiHalf = Repeat(initial, gravity, 0.5f, 2, useSemiImplicit: true);
        var explicitQuarter = Repeat(initial, gravity, 0.25f, 4, useSemiImplicit: false);
        var semiQuarter = Repeat(initial, gravity, 0.25f, 4, useSemiImplicit: true);
        Check("误差：显式h=0.5偏高2.5米", Near(exactPosition, new Vector3(2, -1, 0)) && Near(explicitHalf.Position, new Vector3(2, 1.5f, 0)) && Near(explicitHalf.Velocity, exactVelocity) && Near(explicitHalf.Position.Y - exactPosition.Y, 2.5f));
        Check("误差：半隐式h=0.5偏低2.5米", Near(semiHalf.Position, new Vector3(2, -3.5f, 0)) && Near(semiHalf.Velocity, exactVelocity) && Near(semiHalf.Position.Y - exactPosition.Y, -2.5f));
        Check("误差：显式步长减半误差减半", Near(explicitQuarter.Position, new Vector3(2, 0.25f, 0)) && Near(explicitQuarter.Velocity, exactVelocity) && Near(explicitQuarter.Position.Y - exactPosition.Y, 1.25f));
        Check("误差：半隐式减半后仍非解析解", Near(semiQuarter.Position, new Vector3(2, -2.25f, 0)) && Near(semiQuarter.Velocity, exactVelocity) && Near(semiQuarter.Position.Y - exactPosition.Y, -1.25f));
        Console.WriteLine($"工具：{passed}/{total} PASS");
        if (passed != total)
        {
            Console.WriteLine("场景：跳过；工具与误差对照通过后自动运行。");
            Environment.ExitCode = 1;
            return;
        }

        passed = total = 0;
        Console.WriteLine("=== 第19课：固定物理步跳跃场景 ===");
        // ===== API：FixedStepJump.Tick =====
        var jump = RunFrames(0.125f);
        Check("不足一步时不积分且保留余量", Near(jump.State, initial) && Near(jump.RemainingSeconds, 0.125f) && Near(jump.SimulatedSeconds, 0));
        jump = RunFrames(0.25f);
        Check("恰好一步也执行", Near(jump.State, new MotionState(new Vector3(0.5f, 0.375f, 0), new Vector3(2, 1.5f, 0))) && Near(jump.RemainingSeconds, 0) && Near(jump.SimulatedSeconds, 0.25f));
        jump = RunFrames(1);
        Check("长帧追赶四个固定步", Near(jump.State, new MotionState(new Vector3(2, -2.25f, 0), new Vector3(2, -6, 0))) && Near(jump.RemainingSeconds, 0) && Near(jump.SimulatedSeconds, 1));
        jump = RunFrames(0.375f);
        bool carried = Near(jump.RemainingSeconds, 0.125f) && Near(jump.SimulatedSeconds, 0.25f)
            && Near(jump.State, new MotionState(new Vector3(0.5f, 0.375f, 0), new Vector3(2, 1.5f, 0)));
        jump.Tick(0.125f);
        Check("跨帧余量与固定步长配对", carried && Near(jump.State, new MotionState(new Vector3(1, 0.125f, 0), new Vector3(2, -1, 0))) && Near(jump.RemainingSeconds, 0) && Near(jump.SimulatedSeconds, 0.5f));
        var uneven = RunFrames(0.125f, 0.125f, 0.5f, 0.25f);
        var even = RunFrames(0.25f, 0.25f, 0.25f, 0.25f);
        Check("相同物理步数不受渲染分帧影响", Near(uneven.State, even.State) && Near(even.State, new MotionState(new Vector3(2, -2.25f, 0), new Vector3(2, -6, 0)))
            && Near(uneven.RemainingSeconds, 0) && Near(even.RemainingSeconds, 0) && Near(uneven.SimulatedSeconds, 1) && Near(even.SimulatedSeconds, 1));
        Console.WriteLine($"场景：{passed}/{total} PASS");
        Environment.ExitCode = passed == total ? 0 : 1;
    }

    private static MotionState Repeat(MotionState state, Vector3 acceleration, float h, int count, bool useSemiImplicit)
    {
        for (int i = 0; i < count; i++) state = useSemiImplicit
            ? MotionIntegration.StepSemiImplicitEuler(state, acceleration, h)
            : MotionIntegration.StepExplicitEuler(state, acceleration, h);
        return state;
    }
    private static FixedStepJump RunFrames(params float[] frames)
    {
        var jump = new FixedStepJump(new MotionState(new Vector3(0, 0, 0), new Vector3(2, 4, 0)), new Vector3(0, -10, 0), 0.25f);
        foreach (float frame in frames) jump.Tick(frame);
        return jump;
    }
    private static bool Near(float a, float b) => MathF.Abs(a - b) < 0.0001f;
    private static bool Near(Vector3 a, Vector3 b) => Near(a.X, b.X) && Near(a.Y, b.Y) && Near(a.Z, b.Z);
    private static bool Near(MotionState a, MotionState b) => Near(a.Position, b.Position) && Near(a.Velocity, b.Velocity);
}
