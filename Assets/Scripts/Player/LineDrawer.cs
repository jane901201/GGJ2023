using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField]
    private float _interval = 0.001f;

    private Vector3 _previousPos;
    
    [SerializeField]
    private LineRenderer _lineRenderer;

    void Start()
    {
        _previousPos = transform.position;
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, _previousPos);
    }
    
    // Update is called once per frame
    void Update()
    {
        var position = transform.position;
        if (Vector3.Distance(_previousPos, position) > _interval)
        {
            _lineRenderer.positionCount++;
            _previousPos = position;
        }
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1, position);
    }
}
