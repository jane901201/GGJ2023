using System;
using System.Collections;
using UnityEngine;

public class RebornHelper : MonoBehaviour
{
    public float seconds = 3f;
    public event Action<(Vector3 newDir, LineNode lineNode)> OnRebornDestinationMade;

    private IRebornMechanism _rebornMechanism;

    private Func<bool> _isOn;

    public Action<float> _setCountdownView;

    private bool isCountDownComplete = true;
    
    public void Initialize(IRebornMechanism rebornMechanism, Func<bool> isOn)
    {
        _rebornMechanism = rebornMechanism;
        _isOn = isOn;
    }

    private void Update()
    {
        if (_isOn == null || !_isOn.Invoke())
            return;
        if (isCountDownComplete)
        {
            isCountDownComplete = false;
            StartCoroutine(Countdown());
        }
        //if(Input.GetKeyDown(KeyCode.H))
        //    OnRebornDestinationMade?.Invoke(_rebornMechanism.GetDest());
    }

    private IEnumerator Countdown()
    {
        float currentSeconds = seconds;
        while(currentSeconds > 0)
        {
            if (currentSeconds > 0)
            {
                _setCountdownView.Invoke(currentSeconds);
                Debug.Log("Seconds " + currentSeconds);
                yield return new WaitForSeconds(1f);
                currentSeconds--;
            }
            if (currentSeconds == 0)
                _setCountdownView.Invoke(currentSeconds);
        }
        isCountDownComplete = true;
        OnRebornDestinationMade?.Invoke(_rebornMechanism.GetDest());
        StopCoroutine(Countdown());
    }
}
