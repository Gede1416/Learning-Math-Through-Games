using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Motion;

// 输入有限且统一在世界空间；位置m、时间s、速度m/s、加速度m/s²。
// 只做恒速度/恒加速度解析求值，不处理碰撞、阻力或数值积分。
public static class Kinematics
{
    // TODO 18.1：区间平均速度=位移/时长；durationSeconds>0，不归一化位移。
    public static Vector3 VelocityFromDisplacement(Vector3 displacement, float durationSeconds)
    {
        Vector3 res = displacement * (1.0f / durationSeconds);
        return res; // 未实现占位。
    }

    // TODO 18.2：从固定初始位置计算匀速运动的绝对位置；elapsedSeconds>=0。
    public static Vector3
    PositionAtConstantVelocity
    (Vector3 initialPosition
    , Vector3 velocity
    , float elapsedSeconds)
    {
        if (elapsedSeconds < 0) return initialPosition;
        Vector3 res = initialPosition + velocity * elapsedSeconds;
        return res;
    }

    // TODO 18.3：从固定初速度与恒定加速度计算t时刻速度；elapsedSeconds>=0。
    public static Vector3
    VelocityAtConstantAcceleration
    (Vector3 initialVelocity
    , Vector3 acceleration
    , float elapsedSeconds)
    {
        if (elapsedSeconds < 0) return initialVelocity;
        Vector3 res
        = initialVelocity + acceleration * elapsedSeconds;
        return res;
    }

    // TODO 18.4：从固定初态计算恒加速度运动的绝对位置，包含0.5*a*t²。
    public static Vector3 PositionAtConstantAcceleration(Vector3 initialPosition, Vector3 initialVelocity,
        Vector3 acceleration, float elapsedSeconds)
    {
        Vector3 v1 = initialVelocity;
        Vector3 v2 = VelocityAtConstantAcceleration(initialVelocity, acceleration, elapsedSeconds);
        Vector3 v = (v1 + v2) * 0.5f;
        Vector3 res = PositionAtConstantVelocity(initialPosition, v, elapsedSeconds);
        return res;
    }
}
