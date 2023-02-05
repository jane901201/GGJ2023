using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuSceneView : MonoBehaviour
{
    [SerializeField] private GameObject _startMenuUi;
    [SerializeField] private GameObject _levelMenuUi;

    public void GoToLevelMenu()
    {
        _startMenuUi.SetActive(false);
        _levelMenuUi.SetActive(true);
    }

    public void GoToStartMenu()
    {
        _startMenuUi.SetActive(true);
        _levelMenuUi.SetActive(false);
    }
}
