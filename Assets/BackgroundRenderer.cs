#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[ExecuteAlways]
public class BackgroundRenderer : MonoBehaviour
{
    [SerializeField]
    private Material _backgroundMat;
    [SerializeField]
    private Camera _cam;
    [SerializeField]
    private float _texMagnifier = 10.0f;

    private Mesh _quadMesh;
    private static readonly int TEX_MAG = Shader.PropertyToID("_TexMagnifier");

    private void Awake()
    {
        _quadMesh = new Mesh
        {
            vertices = _GetQuadVertexPosition(0),
            triangles = new[]
            {
                0,
                1,
                2,
                0,
                2,
                3
            }
        };
    }

    private void LateUpdate()
    {
        Vector3 camPos = _cam.transform.position;
        _backgroundMat.SetFloat(TEX_MAG, _texMagnifier);
        float camZ = camPos.z;
#if UNITY_EDITOR
        var sceneView = SceneView.lastActiveSceneView; 
        if (sceneView != null)
        {
            camZ = Mathf.Max(sceneView.camera.transform.position.z, camZ);
        }
#endif
        Graphics.DrawMesh(_quadMesh, new Vector3(camPos.x, camPos.y, camZ + 1), Quaternion.identity, _backgroundMat, 0);
    }

    private static Vector3[] _GetQuadVertexPosition(float z /*= UNITY_NEAR_CLIP_VALUE*/)
    {
        var r = new Vector3[4];
        for (uint i = 0; i < 4; i++)
        {
            uint topBit = i >> 1;
            uint botBit = i & 1;
            float x = topBit;
            float y = (1 - (topBit + botBit)) & 1; // produces 1 for indices 0,3 and 0 for 1,2
            r[i] = new Vector3(x, y, z);
        }
        return r;
    }

    private void OnDestroy()
    {
        Destroy(_quadMesh);
    }
}