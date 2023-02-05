using System;

using UnityEngine;

public class CollisionSender : MonoBehaviour
{
    public event Action<Collider2D> OnCollideToSomething;

    private void OnTriggerEnter2D(Collider2D col)
    { 
        OnCollideToSomething?.Invoke(col); 
    }
}
