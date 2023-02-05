using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public DontDestroy dontDestroy;

    public void GoLevelScene(SceneObjectSetting sceneObjectSetting)
    {
        dontDestroy.Inject(sceneObjectSetting);
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
