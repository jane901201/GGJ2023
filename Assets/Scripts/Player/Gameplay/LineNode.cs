using UnityEngine;

public readonly struct LineNode
{
    public readonly LineRenderer LineRenderer;
    public readonly int Index;

    public LineNode(LineRenderer lineRenderer, int index) 
    { 
        LineRenderer = lineRenderer;
        Index = index;
    }

    public Vector3 Position
        => LineRenderer.GetPosition(Index);
}