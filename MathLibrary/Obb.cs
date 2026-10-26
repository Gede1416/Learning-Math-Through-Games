using StudyNotes.Homework.Math.CrossProduct;
using StudyNotes.Homework.Math.LinearAlgebra;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Geometry;

// Center及三个轴都在世界空间；AxisX/Y/Z是单位正交向量，HalfExtents为正的局部半尺寸。
public readonly record struct Obb(Vector3 Center, Vector3 HalfExtents,
    Vector3 AxisX, Vector3 AxisY, Vector3 AxisZ)
{

    // TODO 16.1：rotation为单位local→world旋转；复用Quaternion.TransformDirection创建三个轴。
    public static Obb CreateFromLocalToWorldRotation(Vector3 center, Vector3 halfExtents, Quaternion rotation)
    {
        return new Obb
        (
            center,
            halfExtents,
            rotation.TransformDirection(Vector3.x),
            rotation.TransformDirection(Vector3.y),
            rotation.TransformDirection(Vector3.z)
        );
    }

    // TODO 16.2：axis可非单位；返回投影区间的半径，不包含中心投影。
    public float ProjectionRadiusOnAxis(Vector3 axis)
    {
        float hx = HalfExtents.X
        , hy = HalfExtents.Y
        , hz = HalfExtents.Z;

        Vector3 u0 = AxisX
        , u1 = AxisY
        , u2 = AxisZ;

        Vector3 L = axis;

        float r
        = hx * MathF.Abs(Vector3.Dot(u0, L))
        + hy * MathF.Abs(Vector3.Dot(u1, L))
        + hz * MathF.Abs(Vector3.Dot(u2, L));
        return r;
    }

    // TODO 16.3：轴平方长度<=1e-12视为退化，返回false。
    // 分离条件：abs(dot(other.Center-Center,axis)) > rA+rB+1e-5*|axis|。
    public bool IsSeparatedOnAxis(Obb other, Vector3 axis)
    {
        if (axis.SqrMagnitude() <= 1e-12f) return false;
        var v = MathF.Abs(Vector3.Dot(other.Center - Center, axis));
        var ra = this.ProjectionRadiusOnAxis(axis);
        var rb = other.ProjectionRadiusOnAxis(axis);
        float fix = 1e-5f * axis.Magnitude();
        var res = v > (ra + rb + fix);
        return res;
    }

    // TODO 16.4：检查双方6条面法线与9条边方向叉积；存在分离轴即false，否则true。
    // 复用IsSeparatedOnAxis；叉积复用StudyNotes.Homework.Math.CrossProduct.CrossMath.Cross。
    public bool Intersects(Obb other)
    {
        bool res = false;
        List<Vector3> axis = new List<Vector3>
        { this.AxisX,this.AxisY,this.AxisZ
        , other.AxisX,other.AxisY,other.AxisZ};
        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                axis.Add(CrossMath.Cross(axis[j], axis[i]));
            }
        }
        foreach (var a in axis)
        {
            if (IsSeparatedOnAxis(other, a))
                return false;
        }
        return true;
    }

    // TODO 17.2：返回实体OBB中离世界点point最近的世界点。
    // 将point-Center投影到三个单位正交轴，限制到各自正负半尺寸，再重构。
    public Vector3 ClosestPoint(Vector3 point)
    {
        Vector3 D = point - Center;
        float d0 = Vector3.Dot(D, AxisX);
        float d1 = Vector3.Dot(D, AxisY);
        float d2 = Vector3.Dot(D, AxisZ);

        float s0 = Math.Clamp(d0, -HalfExtents.X, HalfExtents.X);
        float s1 = Math.Clamp(d1, -HalfExtents.Y, HalfExtents.Y);
        float s2 = Math.Clamp(d2, -HalfExtents.Z, HalfExtents.Z);

        Vector3 Q = Center + AxisX * s0 + AxisY * s1 + AxisZ * s2;

        return Q;
    }
}
