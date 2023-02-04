using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private Text humidityText;
    [SerializeField] private Text rootText;
    [SerializeField] private Text resistText;

    public void SetHumidityText(int humidity)
    {
        humidityText.text = humidity.ToString() + "%";
    }

    public void SetRootText(int depth)
    {
        rootText.text = depth.ToString() + "m";
    }

    public void SetResistText(int resist)
    {
        resistText.text = resistText.ToString();
    }

}
