using System;

using UnityEngine;

public class CollisionSender : MonoBehaviour
{
    public event Action OnCollideToSomething;

    private void OnTriggerEnter2D(Collider2D col)
    { 
        OnCollideToSomething?.Invoke(); 
    }
}
