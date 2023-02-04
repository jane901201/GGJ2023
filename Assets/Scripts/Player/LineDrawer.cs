using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField]
    private LineParameters _lineParameters;
   
    [SerializeField]
    private Transform _lineRoot;
    private Vector3 _previousPos;
    
    [SerializeField]
    private LineRenderer _lineRenderer;

    private void Start()
    {
        _previousPos = _lineRoot.position;
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, _previousPos);
    }
    
    // Update is called once per frame
    private void Update()
    {
        Vector3 position = _lineRoot.position;
        _lineRenderer.startWidth = _lineRenderer.endWidth = _lineParameters.LineWidth;
        if (Vector3.Distance(_previousPos, position) > _lineParameters.LineInterval)
        {
            _lineRenderer.positionCount++;
            _previousPos = position;
        }
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1, position);
    }
}
