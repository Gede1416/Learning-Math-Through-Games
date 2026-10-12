using StudyNotes.Homework.Math.DotProduct;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Geometry;

// 所有输入位于同一坐标空间。direction非零且有限，可以不是单位向量。
// t是射线参数；失败时out t=0，不可将其误认为成功命中。
public static class RayIntersections
{
    // TODO 14.1：P(t)=origin+direction*t，本方法只求点，不判断命中。
    public static Vector3 PointOnRay(Vector3 origin, Vector3 direction, float t)
    {
        return origin + direction * t; // 未实现。
    }

    // TODO 14.2：双面平面，planeNormal为单位向量；返回唯一非负交点参数。
    // abs(dot(normal,direction)) <= 1e-6*direction.Magnitude()视为平行，含共面返回false。
    public static bool TryIntersectPlane(Vector3 origin, Vector3 direction,
        Vector3 planePoint, Vector3 planeNormal, out float t)
    {
        t = 0;
        float denominator = Vector3.Dot(planeNormal, direction);
        if (MathF.Abs(denominator) <= 1e-6f * direction.Magnitude())
        {
            return false;
        }
        t = Vector3.Dot(planeNormal, (planePoint - origin)) / Vector3.Dot(planeNormal, direction);
        if (t < 0)
        {
            t = 0;
            return false;
        }
        return true;
    }

    // TODO 14.3：radius>0；返回球面最近的非负根，内部起点取出口、表面起点允许t=0。
    // 使用一般二次方程，不可假设direction单位长度；delta=0切线也算命中。
    public static bool TryIntersectSphere(Vector3 origin, Vector3 direction,
        Vector3 center, float radius, out float t)
    {
        t = 0;
        Vector3 m = origin - center;

        float a = Vector3.Dot(direction, direction);
        float b = 2 * Vector3.Dot(m, direction);
        float c0 = Vector3.Dot(m, m) - radius * radius;
        float delta = b * b - 4 * a * c0;
        float tNear = (-b - MathF.Sqrt(delta)) / (2 * a);
        float tFar = (-b + MathF.Sqrt(delta)) / (2 * a);
        if (delta < 0)
        {
            return false;
        }
        if (tNear >= 0)
        {
            t = tNear;
            return true;
        }
        if (tFar >= 0)
        {
            t = tFar;
            return true;
        }
        return false;
    }
}
