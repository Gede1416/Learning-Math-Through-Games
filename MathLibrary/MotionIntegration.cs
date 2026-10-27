using System.Drawing;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Motion;

// 某一物理时刻的世界位置(m)和速度(m/s)，不是初态或累计位移。
public readonly record struct MotionState(Vector3 Position, Vector3 Velocity);

public static class MotionIntegration
{
    // TODO 19.1：显式欧拉一步；位置用本步开始的旧速度，速度用本步加速度。
    // 输入有限、deltaSeconds>=0，acceleration为世界加速度(m/s²)。返回新状态，不改变输入。
    public static MotionState StepExplicitEuler(MotionState state, Vector3 acceleration, float deltaSeconds)
    {
        Vector3 p = state.Position + state.Velocity * deltaSeconds;
        Vector3 v = state.Velocity + acceleration * deltaSeconds;
        return new MotionState(p, v);
    }

    // TODO 19.2：半隐式欧拉一步；先得到新速度，再用新速度更新位置。
    // 同样只前进一步，不累计总时间；deltaSeconds=0应保留状态。
    public static MotionState StepSemiImplicitEuler(MotionState state, Vector3 acceleration, float deltaSeconds)
    {
        Vector3 v = state.Velocity + acceleration * deltaSeconds;
        Vector3 p = state.Position + v * deltaSeconds;
        return new MotionState(p, v);
    }
}
