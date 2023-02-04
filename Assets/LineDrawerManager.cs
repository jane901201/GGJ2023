using System.Collections.Generic;
using UnityEngine;

public class LineDrawerManager
{
    private readonly Dictionary<LineRenderer, LineDrawer> _lineDrawers;
    private readonly LineParameters _parameters;

    public LineDrawerManager(LineParameters parameters)
    {
        _lineDrawers = new Dictionary<LineRenderer, LineDrawer>();
        _parameters = parameters;
    }

    public LineDrawer GetLineDrawer(LineRenderer lineRenderer)
    {
        if (_lineDrawers.TryGetValue(lineRenderer, out LineDrawer lineDrawer))
            return lineDrawer;

        var newDrawer = new LineDrawer(_parameters, lineRenderer);
        _lineDrawers.Add(lineRenderer, newDrawer);

        return newDrawer;
    }
}
