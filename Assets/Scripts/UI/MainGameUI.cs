using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private Text humidityText;
    [SerializeField] private Text rootText;
    [SerializeField] private Text resistText;
    [SerializeField] private Text countDownText;

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
        resistText.text = resist.ToString();
    }

    public void SetCountDownText(float seconds)
    {
        if (seconds == 0)
            countDownText.text = "";
        else
            countDownText.text = seconds.ToString();
    }

}
