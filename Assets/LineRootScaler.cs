using UnityEngine;

public class LineRootScaler : MonoBehaviour
{
    [SerializeField]
    private Transform _lineRoot;
    [SerializeField]
    private CircleCollider2D _collider;
    [SerializeField]
    private LineParameters _parameters;
    
    private void Update()
    {
        _lineRoot.localScale = new Vector3(_parameters.CircleScale, _parameters.CircleScale, 1);
        _collider.radius = 0.5f;
    }
}
