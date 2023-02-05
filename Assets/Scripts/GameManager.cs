using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] MainGameUI mainGameUI;
    [SerializeField] Transform playerTransform;

    private int currentScore;



    private void Update()
    {
        currentScore = (int)Mathf.Abs(playerTransform.position.y);
        mainGameUI.SetRootText(currentScore);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainGameScene");
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainGameScene2");
        }
    }
}
