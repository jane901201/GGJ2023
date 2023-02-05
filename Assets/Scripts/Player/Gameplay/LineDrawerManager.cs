using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class LineDrawerManager
{
    private readonly Dictionary<LineRenderer, LineDrawer> _lineDrawers;
    private readonly LineParameters _parameters;
    private readonly List<LineRenderer> _getRenderersResult;
    
    public LineDrawerManager(LineParameters parameters)
    {
        _lineDrawers = new Dictionary<LineRenderer, LineDrawer>();
        _parameters = parameters; 
        _getRenderersResult = new List<LineRenderer>();
    }

    public LineDrawer GetLineDrawer(LineRenderer lineRenderer)
    {
        if (_lineDrawers.TryGetValue(lineRenderer, out LineDrawer lineDrawer))
            return lineDrawer;

        var newDrawer = new LineDrawer(_parameters, lineRenderer);
        _lineDrawers.Add(lineRenderer, newDrawer);

        return newDrawer;
    }

    public IEnumerable<Bounds> LineBounds
        => _lineDrawers.Values.Select(d => d.Aabb);

    public List<LineRenderer> GetLineRenderers(Bounds aabb)
    {
        _getRenderersResult.Clear();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (KeyValuePair<LineRenderer, LineDrawer> kvp in _lineDrawers)
        {
            if(kvp.Value.Aabb.Intersects(aabb))
                _getRenderersResult.Add(kvp.Key);
        }
        return _getRenderersResult;
    }
}
