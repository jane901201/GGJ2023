using UnityEngine;

public readonly struct LineSegment
{
    public readonly Vector3 Pos;
    public readonly Vector3? To;

    public LineSegment(Vector3 pos, Vector3? to)
    {
        Pos = pos;
        To = to;
    }
}