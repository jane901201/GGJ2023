using System;

using UnityEngine;

public class CollisionSender : MonoBehaviour
{
    public event Action OnCollideToDeath;

    private void OnTriggerEnter2D(Collider2D col)
    { 
        OnCollideToDeath?.Invoke(); 
    }
}
