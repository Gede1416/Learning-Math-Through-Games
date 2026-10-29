using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Motion;

// 固定步长下的相邻两个世界位置(m)：PreviousPosition在前，Position在后。
public readonly record struct PositionVerletState(Vector3 PreviousPosition, Vector3 Position);

public static class VerletIntegration
{
    // TODO 20.1：从初始位置/速度/加速度生成虚拟上一位置，当前点仍为initialPosition。
    // 输入有限；fixedStepSeconds>0；位置m、速度m/s、加速度m/s²，统一世界空间。
    public static PositionVerletState InitializePositionVerlet(Vector3 initialPosition,
        Vector3 initialVelocity, Vector3 acceleration, float fixedStepSeconds)
    {
        var befPos = initialPosition - initialVelocity * fixedStepSeconds + acceleration * 0.5f * fixedStepSeconds * fixedStepSeconds;

        return new PositionVerletState(befPos, initialPosition);
    }

    // TODO 20.2：位置Verlet前进一步；返回(旧当前位置, 新位置)，不改变输入。
    // fixedStepSeconds>0且必须与生成这份历史的步长一致；acceleration为当前时刻采样。
    public static PositionVerletState StepPositionVerlet(PositionVerletState state,
        Vector3 acceleration, float fixedStepSeconds)
    {
        Vector3 afPos = 2.0f * state.Position - state.PreviousPosition + acceleration * fixedStepSeconds * fixedStepSeconds;
        return new PositionVerletState(state.Position, afPos); // 未实现占位。
    }

    // TODO 20.3：用相隔2h的前后位置，估计中间时刻的速度(m/s)，不是nextPosition时刻速度。
    // fixedStepSeconds>0表示单个相邻物理步h，不是前后两点的总间隔2h。
    public static Vector3 EstimateCenteredVelocity(Vector3 previousPosition,
        Vector3 nextPosition, float fixedStepSeconds)
    {
        return (nextPosition - previousPosition) / (2.0f * fixedStepSeconds);
    }
}
