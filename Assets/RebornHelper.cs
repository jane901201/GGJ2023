using System;

using UnityEngine;

public interface IRebornMechanism
{
    (Vector3 newDir, LineNode lineNode) GetDest();
}

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

public class RebornHelper : MonoBehaviour
{
    public event Action<(Vector3 newDir, LineNode lineNode)> OnRebornDestinationMade;

    private IRebornMechanism _rebornMechanism;

    private Func<bool> _isOn;
    
    public void Initialize(IRebornMechanism rebornMechanism, Func<bool> isOn)
    {
        _rebornMechanism = rebornMechanism;
        _isOn = isOn;
    }

    private void Update()
    {
        if (_isOn == null || !_isOn.Invoke())
            return;
        if(Input.GetKeyDown(KeyCode.H))
            OnRebornDestinationMade?.Invoke(_rebornMechanism.GetDest());
    }
}
