using System;

using UnityEngine;

public class RebornHelper : MonoBehaviour
{
    public event Action<(Vector3 newDir, LineNode lineNode)> OnRebornDestinationMade;

    private IRebornMechanism _rebornMechanism;

    private Func<bool> _isOn;
    
    public void Initialize(IRebornMechanism rebornMechanism, Func<bool> isOn)
    {
        _rebornMechanism = rebornMechanism;
        _isOn = isOn;
    }

    private void Update()
    {
        if (_isOn == null || !_isOn.Invoke())
            return;
        if(Input.GetKeyDown(KeyCode.H))
            OnRebornDestinationMade?.Invoke(_rebornMechanism.GetDest());
        //TODO:倒數3秒重生的UI顯示
    }
}
