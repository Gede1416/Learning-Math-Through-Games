using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Geometry;

// 同一坐标空间内的轴对齐包围盒；输入保证有限且每轴Min<=Max。
// 本课使用闭区间：边界点、面/边/角接触都算相交；允许零尺寸。
public readonly record struct Aabb(Vector3 Min, Vector3 Max)
{
    // TODO 15.1：halfExtents各分量>=0，是半尺寸，非完整宽高深。
    public static Aabb CreateFromCenterAndHalfExtents(Vector3 center, Vector3 halfExtents)
    {
        Vector3 Min = center - halfExtents;
        Vector3 Max = center + halfExtents;
        return new Aabb(Min, Max);
    }

    // TODO 15.2：X/Y/Z三轴都处于闭区间[Min,Max]，才算在盒内。
    public bool ContainsPoint(Vector3 point)
    {
        bool moreMin = point >= Min;
        bool smallMax = point <= Max;
        return moreMin && smallMax;
    }

    // TODO 15.3：三轴投影都重叠才相交；任意轴分离即可返回false。
    public bool Intersects(Aabb other)
    {
        //左右区分
        bool res = false;
        res = res ? res : this.Min <= other.Max && this.Max >= other.Min;
        res = res ? res : other.Min <= this.Max && other.Max > this.Min;
        return res;
    }

    // TODO 17.1：返回实体闭盒中离point最近的点；内部点原样返回。
    // 每轴限制到[Min,Max]，可复用Math.Clamp；输入在同一坐标空间。
    public Vector3 ClosestPoint(Vector3 point)
    {
        float x = Math.Clamp(point.X, Min.X, Max.X);
        float y = Math.Clamp(point.Y, Min.Y, Max.Y);
        float z = Math.Clamp(point.Z, Min.Z, Max.Z);
        Vector3 res = new Vector3(x, y, z);
        return res; // 未实现占位。
    }
}
