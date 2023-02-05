using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSceneObject : MonoBehaviour
{
    public enum SceneObjectType
    {
        Self,
        Effect,
        Obstacle,
    }

    public abstract SceneObjectType ObjectType { get; }

    [SerializeField] private AudioClip _audio;

    public void PlayAudio(AudioSource audioSource)
    {
        if (_audio != null)
        {
            audioSource.PlayOneShot(_audio);
        }
    }
}
