using System;
using System.Collections.Generic;

using UnityEngine;

using Random = UnityEngine.Random;

public class RandomPickOnWhole : IRebornMechanism
{
    private readonly LineRendererManager _lineRendererManager;
    private readonly Transform _lineRoot;
    private readonly LineParameters _lineParameters;
    private readonly float _angularOffset;

    public RandomPickOnWhole(LineRendererManager lineRendererManager, Transform lineRoot, LineParameters parameters, float angularOffset)
    {
        _lineRendererManager = lineRendererManager;
        _lineRoot = lineRoot;
        _lineParameters = parameters;
        _angularOffset = angularOffset;
    }

    (Vector3 newDir, LineNode lineNode) IRebornMechanism.GetDest()
    {
        var segments = new List<(LineRenderer, int)>();
        {
            List<LineRenderer> renderers = _lineRendererManager.Renderers;
            Vector3 linePos = _GetLineRootPos();
            foreach (LineRenderer lineRenderer in renderers)
            {
                int count = lineRenderer.positionCount;
                for (var index = 1; index < count; index++)
                {
                    Vector3 position = lineRenderer.GetPosition(index);
                    /*if (Vector3.Distance(linePos, position) > _lineParameters.CircleRadius)*/
                    segments.Add((lineRenderer, index));
                }
            }
        }
        int segmentIndex = Random.Range(0, segments.Count);
        (LineRenderer line, int ind) = segments[segmentIndex];
        Vector3 from = line.GetPosition(ind - 1);
        Vector3 to = line.GetPosition(ind);
        Vector3 vec = to - from;
        var normal = new Vector3(vec.y, -vec.x, 0);
        normal *= Mathf.Sign(Random.Range(-1, 1));
        var vecQ = Quaternion.FromToRotation(Vector3.right, vec);
        var normalQ = Quaternion.FromToRotation(Vector3.right, normal);
        var newQ = Quaternion.Lerp(normalQ, vecQ, _angularOffset);
        var newNormal = newQ * Vector3.right;
        return new ValueTuple<Vector3, LineNode>(newNormal, new LineNode(line, ind));
    }

    private Vector3 _GetLineRootPos()
    {
        Vector3 pos = _lineRoot.transform.position;
        return new Vector3(pos.x, pos.y, 0);
    }
}