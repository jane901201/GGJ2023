using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy dontDestroy;

    public SceneObjectSetting SceneObjectSetting;

    // Start is called before the first frame update
    void Start()
    {
        if (dontDestroy != null)
        {
            Destroy(this);
        }
        else
        {
            dontDestroy = this;
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inject()
    {
        StartCoroutine(_Inject(SceneObjectSetting));
    }

    public void Inject(SceneObjectSetting sceneObjectSetting)
    {
        SceneObjectSetting = sceneObjectSetting;
        StartCoroutine(_Inject(SceneObjectSetting));
    }

    public IEnumerator _Inject(SceneObjectSetting sceneObjectSetting)
    {
        while (true)
        {
            var levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                FindObjectOfType<LevelManager>().Initialize(SceneObjectSetting);
                break;
            }
            yield return null;
        }
    }
}
