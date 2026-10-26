using System.Drawing;
using StudyNotes.Homework.Math.VectorBasics;

namespace StudyNotes.MathLibrary.Geometry;

// 世界空间的实体闭球；输入有限、Radius>=0，接触和包含都算相交。
// 本课直接比较距离平方，不额外添加容差，不处理非法输入或动态碰撞。
public readonly record struct Sphere(Vector3 Center, float Radius)
{
    // TODO 17.3：中心距离平方 <= 两个半径之和的平方；复用SqrMagnitude。
    public bool Intersects(Sphere other)
    {
        float rr = other.Radius + this.Radius;
        bool res = Intersects(other.Center, rr);
        return res;
    }

    // TODO 17.4：复用box.ClosestPoint(Center)，比较到最近点的距离平方与Radius平方。
    public bool Intersects(Aabb box)
    {
        Vector3 nestP = box.ClosestPoint(this.Center);
        bool res = Intersects(nestP, this.Radius);
        return res;
    }

    // TODO 17.5：复用OBB的最近点方法；三个轴必须单位正交，半尺寸不含旋转。
    public bool Intersects(Obb box)
    {
        Vector3 nestP = box.ClosestPoint(this.Center);
        bool res = Intersects(nestP, this.Radius);
        return res;
    }

    private bool Intersects(Vector3 otherCenter, float rr)
    {
        Vector3 d = otherCenter - Center;
        float dd = d.SqrMagnitude();
        bool res = dd <= rr * rr;
        return res;
    }
}
