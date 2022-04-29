using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Buttons : ServiceBase
{
    public bool PlusPressed
    {
        get => plusPressed;
        private set
        {
            plusPressed = value;
            PlusChanged.Invoke(plusPressed);
        }
    }

    public bool StopPressed
    {
        get => stopPressed;
        private set
        {
            stopPressed = value;
            StopChanged.Invoke(stopPressed);
        }
    }

    public bool MinusPressed
    {
        get => minusPressed;
        private set
        {
            minusPressed = value;
            MinusChanged.Invoke(minusPressed);
        }
    }

    public UnityEvent<bool> PlusChanged;
    public UnityEvent<bool> StopChanged;
    public UnityEvent<bool> MinusChanged;

    #region internals
    protected LEGOButtonSensor buttons;
    private bool plusPressed;
    private bool stopPressed;
    private bool minusPressed;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        buttons = services.FirstOrDefault(s => port == s.ConnectInfo.PortID && s.ioType == IOType.LEIOTypeRemoteControlButtonSensor) as LEGOButtonSensor;
        if (buttons == null)
        {
            Debug.LogWarning(name + " service not found");
            return false;
        }
        services.Remove(buttons);
        buttons.UpdateInputFormat(new LEGOInputFormat(buttons.ConnectInfo.PortID, buttons.ioType, (int)LEGOButtonSensor.LEButtonSensorMode.RCKEY, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
        buttons.RegisterDelegate(this);
        IsConnected = true;
        Debug.Log(name + " connected");
        return true;
    }

    public void OnDestroy()
    {
        buttons?.UnregisterDelegate(this);
    }

    public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
    {
        if (newState == ServiceState.Disconnected)
        {
            Debug.LogWarning(name + " disconnected");
            buttons.UnregisterDelegate(this);
            buttons = null;
            IsConnected = false;
        }
    }

    public override void DidUpdateValueData(ILEGOService service, LEGOValue oldValue, LEGOValue newValue)
    {
        switch (newValue.RawValues[0])
        {
            case 0:
                PlusPressed = false;
                StopPressed = false;
                MinusPressed = false;
                break;
            case 1:
                PlusPressed = true;
                break;
            case 127:
                StopPressed = true;
                break;
            case -1:
                MinusPressed = true;
                break;
        }
    }
    #endregion
}
