using UnityEngine;

public sealed class RandomPickOnLatestLine : IRebornMechanism
{
    private readonly LineRendererManager _manager;
    
    public RandomPickOnLatestLine(LineRendererManager manager)
        => _manager = manager;

    (Vector3 newDir, LineNode lineNode) IRebornMechanism.GetDest()
    {
        float selected = Random.value;
        LineRenderer renderer = _manager.GetLatestRenderer();
        int index = Mathf.FloorToInt(selected * (renderer.positionCount - 2));
        float angle = Random.value * 360;
        Vector3 newDir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        return (newDir, new LineNode(renderer, index));
    }
}