using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using Debug = System.Diagnostics.Debug;

public class LineColliderGenerator : MonoBehaviour
{
    private readonly struct LineNode
    {
        public readonly Vector3 Pos;
        public readonly Vector3? To;

        public LineNode(Vector3 pos, Vector3? to)
        {
            Pos = pos;
            To = to;
        }
    }

    [SerializeField]
    private LineParameters _lineParameters;
    [SerializeField]
    private Transform _lineRoot;
    [SerializeField]
    [Min(0.001f)]
    private float _outerAabbSize;
    [SerializeField]
    [Min(0.001f)]
    private float _innerAabbSize;
    [SerializeField]
    private LineRenderer _lineRenderer;

    private readonly List<BoxCollider2D> _colliderPool = new List<BoxCollider2D>();
    private readonly List<LineNode> _validNodes = new List<LineNode>();

    private void OnDrawGizmos()
    {
        Vector3 rootPos = _lineRoot.transform.position;
        Bounds outerAabb = _GetAabb(rootPos, _outerAabbSize);
        Bounds innerAabb = _GetAabb(rootPos, _innerAabbSize);
        Gizmos.color = Color.yellow;
        _DrawBoundsGizmos(outerAabb);
        Gizmos.color = Color.red;
        _DrawBoundsGizmos(innerAabb);
        Gizmos.color = Color.cyan;
        foreach (LineNode node in _validNodes)
        {
            _DrawNodeGizmos(node);
        }
    }

    private void OnValidate()
    {
        Assert.IsTrue(_outerAabbSize > _innerAabbSize);
    }

    private static void _DrawBoundsGizmos(Bounds aabb)
    {
        Vector3 extents = aabb.extents;
        Gizmos.DrawLine(aabb.center + new Vector3(-extents.x, extents.y, 0), aabb.center + new Vector3(-extents.x, -extents.y, 0));
        Gizmos.DrawLine(aabb.center + new Vector3(-extents.x, extents.y, 0), aabb.center + new Vector3(extents.x, extents.y, 0));
        Gizmos.DrawLine(aabb.center + new Vector3(extents.x, extents.y, 0), aabb.center + new Vector3(extents.x, -extents.y, 0));
        Gizmos.DrawLine(aabb.center + new Vector3(extents.x, -extents.y, 0), aabb.center + new Vector3(-extents.x, -extents.y, 0)); 
    }

    private static void _DrawNodeGizmos(LineNode node)
    {
        if (node.To == null)
            return;
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
        foreach (LineNode node in _validNodes.Where(node => node.To != null))
        {
            Debug.Assert(node.To != null, "node.To != null");
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
        coll.size = new Vector2(Vector3.Distance(from, to), _lineParameters.LineInterval);
        coll.transform.position = center;
        float angle = Vector3.Angle(to - from, Vector3.left);
        coll.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void _BoundPoints()
    {
        Vector3 rootPos = _lineRoot.transform.position;
        Bounds outerAabb = _GetAabb(rootPos, _outerAabbSize);
        Bounds innerAabb = _GetAabb(rootPos, _innerAabbSize);
        int positionCount = _lineRenderer.positionCount;
        for (var nodeIndex = 0; nodeIndex < positionCount; nodeIndex++)
        {
            Vector3 nodePos = _lineRenderer.GetPosition(nodeIndex);
            if (!outerAabb.Contains(nodePos) || innerAabb.Contains(nodePos))
                continue;
            _validNodes.Add
            (
                new LineNode
                (
                    nodePos,
                    nodeIndex == positionCount - 1 ? null : _lineRenderer.GetPosition(nodeIndex + 1)
                )
            );
        } 
    }

    private static Bounds _GetAabb(Vector3 pos, float size)
        => new Bounds(pos, new Vector3(size, size, size));
}
