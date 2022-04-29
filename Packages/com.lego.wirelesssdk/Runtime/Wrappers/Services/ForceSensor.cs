using LEGODeviceUnitySDK;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ForceSensor : ServiceBase
{
    public ForceSensorMode Mode
    {
        get => mode;
        set
        {
            if (mode != value)
            {
                mode = value;
                UpdateInputFormat();
            }
        }
    }

    public int Force
    {
        get => force; // 0 to 100.
        private set
        {
            force = value;
            ForceChanged.Invoke(force);
        }
    }

    public bool Touch
    {
        get => touch;
        private set
        {
            touch = value;
            TouchChanged.Invoke(touch);
        }
    }

    public UnityEvent<int> ForceChanged;
    public UnityEvent<bool> TouchChanged;

    #region internals
    public enum ForceSensorMode
    {
        Force = 0,
        Touch = 1
    }

    private LEGOTechnicForceSensor sensor;
    [SerializeField] private ForceSensorMode mode = ForceSensorMode.Force;
    private int force;
    private bool touch;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeTechnicForceSensor) as LEGOTechnicForceSensor;
        if (sensor == null)
        {
            Debug.LogWarning(name + " service not found");
            return false;
        }
        services.Remove(sensor);
        UpdateInputFormat();
        sensor.RegisterDelegate(this);
        IsConnected = true;
        Debug.Log(name + " connected");
        return true;
    }

    private void UpdateInputFormat()
    {
        sensor?.UpdateInputFormat(new LEGOInputFormat(sensor.ConnectInfo.PortID, sensor.ioType, (int)mode, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
    }

    private void OnDestroy()
    {
        sensor?.UnregisterDelegate(this);
    }

    public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
    {
        if (newState == ServiceState.Disconnected)
        {
            Debug.LogWarning(name + " disconnected");
            sensor.UnregisterDelegate(this);
            sensor = null;
            IsConnected = false;
        }
    }

    public override void DidUpdateValueData(ILEGOService service, LEGOValue oldValue, LEGOValue newValue)
    {
        if (newValue.Mode == (int)ForceSensorMode.Force)
        {
            Force = Mathf.RoundToInt(newValue.PctValues[0]);
        }
        else if (newValue.Mode == (int)ForceSensorMode.Touch)
        {
            Touch = (int)newValue.RawValues[0] == 1;
        }
    }
    #endregion
}
