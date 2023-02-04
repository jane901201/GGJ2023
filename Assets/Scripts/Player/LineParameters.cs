using UnityEngine;
using UnityEngine.Serialization;

public class LineParameters : MonoBehaviour
{
    [Min(0.0f)]
    public float LineWidth = 0.5f;
    [Min(0.001f)]
    public float LineInterval = 0.05f;
    [FormerlySerializedAs("CircleWidth")]
    [Min(0.0f)]
    public float CircleScale = 0.5f;
}
