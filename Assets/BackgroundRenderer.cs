#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class BackgroundRenderer : MonoBehaviour
{
    [SerializeField]
    private Material _backgroundMat;
    [SerializeField]
    private Camera _cam;
    [SerializeField]
    private float _texMagnifier = 10.0f;
    [SerializeField]
    private float _groundOffset = 0.0f;

    private Mesh _quadMesh;
    private static readonly int TEX_PARAM = Shader.PropertyToID("_TexParam");

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
        var proj = _cam.cameraToWorldMatrix;
        Vector3 camPos = new Vector3(proj.m03, proj.m13, proj.m23);
        _backgroundMat.SetVector(TEX_PARAM, new Vector4(_texMagnifier, _groundOffset, 0, 0));
        var pos = new Vector3(camPos.x, camPos.y, camPos.z + _cam.farClipPlane);
#if UNITY_EDITOR
        var sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            Matrix4x4 sceneViewCamProj = sceneView.camera.cameraToWorldMatrix;
            var sceneViewCamPos = new Vector3(sceneViewCamProj.m03, sceneViewCamProj.m13, sceneViewCamProj.m23);
            var sceneViewPos = new Vector3(sceneViewCamPos.x, sceneViewCamPos.y, sceneViewCamPos.z + sceneView.camera.farClipPlane);
            if (sceneViewPos.z < pos.z)
                pos = sceneViewCamPos;
        }
#endif
        Graphics.DrawMesh(_quadMesh, new Vector3(pos.x, pos.y, pos.z - 0.001f), Quaternion.identity, _backgroundMat, 0);
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
        CoreUtils.Destroy(_quadMesh);
    }
}