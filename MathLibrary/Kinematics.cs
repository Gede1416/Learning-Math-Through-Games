using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Motion;

// 输入有限且统一在世界空间；位置m、时间s、速度m/s、加速度m/s²。
// 只做恒速度/恒加速度解析求值，不处理碰撞、阻力或数值积分。
public static class Kinematics
{
    // TODO 18.1：区间平均速度=位移/时长；durationSeconds>0，不归一化位移。
    public static Vector3 VelocityFromDisplacement(Vector3 displacement, float durationSeconds)
    {
        return new Vector3(0, 0, 0); // 未实现占位。
    }

    // TODO 18.2：从固定初始位置计算匀速运动的绝对位置；elapsedSeconds>=0。
    public static Vector3 PositionAtConstantVelocity(Vector3 initialPosition, Vector3 velocity, float elapsedSeconds)
    {
        return initialPosition; // 未实现占位。
    }

    // TODO 18.3：从固定初速度与恒定加速度计算t时刻速度；elapsedSeconds>=0。
    public static Vector3 VelocityAtConstantAcceleration(Vector3 initialVelocity, Vector3 acceleration, float elapsedSeconds)
    {
        return initialVelocity; // 未实现占位。
    }

    // TODO 18.4：从固定初态计算恒加速度运动的绝对位置，包含0.5*a*t²。
    public static Vector3 PositionAtConstantAcceleration(Vector3 initialPosition, Vector3 initialVelocity,
        Vector3 acceleration, float elapsedSeconds)
    {
        return initialPosition; // 未实现占位。
    }
}
