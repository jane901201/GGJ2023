using UnityEngine;
using UnityEngine.Serialization;

public class LineParameters : MonoBehaviour
{
    [Min(0.0f)]
    public float LineWidth = 0.5f;
    [Min(0.0f)]
    public float LineInterval = 0.05f;
    [Min(0.0f)]
    public float CircleRadius = 0.5f;
    [Min(0.0f)]
    public float LineColliderWidth = 0.5f;
    [Min(0.0f)]
    public float CircleColliderRadius = 0.5f;
}
