using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GGJ23/SceneObjectSetting", fileName = "New SceneObjectSetting")]
public class SceneObjectSetting : ScriptableObject
{
    [System.Serializable]
    public class SceneObjectPool
    {
        public int MinDepth;
        public int MaxDepth;
        public List<SceneObject> Objects;

        public SceneObject RandomPick(Vector2Int maxSize)
        {
            Debug.Log(maxSize);
            var validObjs = Objects.Where(obj => obj.Size.x <= maxSize.x && obj.Size.y <= maxSize.y).ToList();
            if (validObjs.Count == 0)
            {
                return null;
            }
            return validObjs[Random.Range(0, validObjs.Count)];
        }
    }

    [System.Serializable]
    public class SceneObject
    {
        public Vector2Int Size;
        public GameObject Prefab;
    }

    public List<SceneObjectPool> Pools;

    public SceneObject RandomPick(int depth, Vector2Int maxSize)
    {
        foreach (var pool in Pools)
        {
            if (pool.MinDepth <= depth && depth <= pool.MaxDepth)
            {
                return pool.RandomPick(maxSize);
            }
        }
        return null;
    }
}
