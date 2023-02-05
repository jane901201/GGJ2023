using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inject(SceneObjectSetting sceneObjectSetting)
    {
        StartCoroutine(_Inject(sceneObjectSetting));
    }

    public IEnumerator _Inject(SceneObjectSetting sceneObjectSetting)
    {
        while (true)
        {
            var levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                FindObjectOfType<LevelManager>().Initialize(sceneObjectSetting);
                break;
            }
            yield return null;
        }
    }
}
