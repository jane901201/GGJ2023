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
            
            int totalWeight = validObjs.Sum(obj => obj.Weight);
            var rand = Random.Range(0, totalWeight);

            var acc = 0;
            for (int i = 0 ; i < validObjs.Count ; i++)
            {
                acc = acc + validObjs[i].Weight;
                if (acc > rand)
                {
                    return validObjs[i];
                }
            }
            return validObjs[0];
        }
    }

    [System.Serializable]
    public class SceneObject
    {
        public Vector2Int Size = new Vector2Int(1, 1);
        public int Weight = 1;
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
