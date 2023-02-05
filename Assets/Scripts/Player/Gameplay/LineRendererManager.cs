using System.Collections.Generic;

using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _template;
    
    private readonly List<LineRenderer> _pool = new List<LineRenderer>();

    private bool _taken;

    public List<LineRenderer> Renderers
        => _pool;
    
    private void Awake()
    {
        _template.enabled = false;
        _template.positionCount = 0;
        _template.useWorldSpace = false;
        _pool.Add(_template);
    }

    public LineRenderer GetNewRenderer()
    {
        if (!_taken)
        {
            _taken = true;
            _template.enabled = true;
            return _template;
        }
        LineRenderer lineRenderer = Instantiate(_template);
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false;
        _pool.Add(lineRenderer);
        return lineRenderer;
    }
    
    public LineRenderer GetLatestRenderer()
        => _pool[^1];
}
