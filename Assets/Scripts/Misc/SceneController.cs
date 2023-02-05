using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public Animator TitleAnimator;

    public void GoLevelScene()
    {
        DontDestroy.dontDestroy.Inject();
        SceneManager.LoadScene("MainGameScene");
    }

    public void GoLevelScene(SceneObjectSetting sceneObjectSetting)
    {
        StartCoroutine(Init(sceneObjectSetting));
    }

    public IEnumerator Init(SceneObjectSetting sceneObjectSetting)
    {
        TitleAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        DontDestroy.dontDestroy.Inject(sceneObjectSetting);
        SceneManager.LoadScene("MainGameScene");
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
