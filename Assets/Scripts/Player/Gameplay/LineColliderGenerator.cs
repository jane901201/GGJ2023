using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class LineColliderGenerator : MonoBehaviour
{

    [SerializeField]
    private LineParameters _lineParameters;
    [SerializeField]
    private Transform _lineRoot;
    [SerializeField]
    [Min(0.001f)]
    private float _outerAabbSize;

    private readonly List<BoxCollider2D> _colliderPool = new List<BoxCollider2D>();
    private readonly List<LineSegment> _validNodes = new List<LineSegment>();

    private LineDrawerManager _lineDrawerManager;
    
    public void Initialize(LineDrawerManager lineDrawerManager)
    {
        _lineDrawerManager = lineDrawerManager;
    }

    private void OnDrawGizmos()
    {
        if (_lineRoot == null)
            return;
        Vector3 rootPos = _lineRoot.transform.position;
        Bounds outerAabb = _GetAabb(rootPos, _outerAabbSize);
        Gizmos.color = Color.yellow;
        _DrawBoundsGizmos(outerAabb);
        Gizmos.color = Color.cyan;
        foreach (LineSegment node in _validNodes)
        {
            _DrawNodeGizmos(node);
        }
        Gizmos.color = Color.blue;
        if (_lineDrawerManager == null)
            return;
        foreach (Bounds bounds in _lineDrawerManager.LineBounds)
        {
            _DrawBoundsGizmos(bounds);
        }
    }

    private void OnValidate()
    {
        Assert.IsTrue(_outerAabbSize > _lineParameters.CircleRadius);
    }

    private static void _DrawBoundsGizmos(Bounds aabb)
    {
        Vector3 extents = aabb.extents;
        Gizmos.DrawLine(aabb.center + new Vector3(-extents.x, extents.y, 0), aabb.center + new Vector3(-extents.x, -extents.y, 0));
        Gizmos.DrawLine(aabb.center + new Vector3(-extents.x, extents.y, 0), aabb.center + new Vector3(extents.x, extents.y, 0));
        Gizmos.DrawLine(aabb.center + new Vector3(extents.x, extents.y, 0), aabb.center + new Vector3(extents.x, -extents.y, 0));
        Gizmos.DrawLine(aabb.center + new Vector3(extents.x, -extents.y, 0), aabb.center + new Vector3(-extents.x, -extents.y, 0)); 
    }

    private static void _DrawNodeGizmos(LineSegment node)
    {
        Gizmos.DrawLine(node.Pos, node.To.Value);
    }

    // Update is called once per frame
    private void Update()
    {
        _validNodes.Clear();

        _BoundPoints();

        _BindColliders();
    }

    private void _BindColliders()
    {
        var index = 0;
        foreach (LineSegment node in _validNodes)
        {
            _BindCollider(node.Pos, node.To.Value, index++);
        }

        for (; index < _colliderPool.Count; index++)
        {
            _colliderPool[index].enabled = false;
        }
    }

    private void _BindCollider(Vector3 from ,Vector3 to, int index)
    {
        if (_colliderPool.Count == index)
        {
            var newGo = new GameObject();
            var newCollider = newGo.AddComponent<BoxCollider2D>();
            _colliderPool.Add(newCollider);
        }
        
        BoxCollider2D coll = _colliderPool[index];
        coll.enabled = true;
        Vector3 center = Vector3.Lerp(from, to, 0.5f);
        coll.size = new Vector2(Vector3.Distance(from, to), _lineParameters.LineColliderWidth);
        coll.transform.position = center;
        float angle = Vector3.Angle(to - from, Vector3.right);
        coll.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void _BoundPoints()
    {
        Vector3 rootPos = _lineRoot.transform.position;
        Bounds outerAabb = _GetAabb(rootPos, _outerAabbSize);
        List<LineRenderer> renderers = _lineDrawerManager.GetLineRenderers(outerAabb);
        foreach (LineRenderer lineRenderer in renderers)
        {
            _BoundPointsInRenderer(lineRenderer, outerAabb, rootPos);
        }
    }

    private void _BoundPointsInRenderer(LineRenderer lineRenderer, Bounds outerAabb, Vector3 rootPos)
    {
        int positionCount = lineRenderer.positionCount;
        for (var nodeIndex = 0; nodeIndex < positionCount - 1; nodeIndex++)
        {
            Vector3 nodePos = _GetLineNodePos(lineRenderer, nodeIndex);
            Vector3 nextPos = _GetLineNodePos(lineRenderer, nodeIndex + 1);
            if (!outerAabb.Contains(nodePos) || Vector3.Distance(rootPos, nextPos) <= _lineParameters.CircleRadius)
                continue;
            _validNodes.Add
            (
                new LineSegment
                (
                    nodePos,
                    nextPos
                )
            );
        }  
    }

    private static Bounds _GetAabb(Vector3 pos, float size)
        => new Bounds(pos, new Vector3(size, size, size));

    private Vector3 _GetLineNodePos(LineRenderer lineRenderer, int index)
    {
        Vector3 pos = lineRenderer.GetPosition(index);
        return new Vector3(pos.x, pos.y, _lineRoot.position.z);
    }
}