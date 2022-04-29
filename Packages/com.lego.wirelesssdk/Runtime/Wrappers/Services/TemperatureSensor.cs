using LEGODeviceUnitySDK;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TemperatureSensor : ServiceBase
{
    public float Temperature
    {
        get => temperature; // Degrees Celsius. Internal to the Hub.
        private set
        {
            temperature = value;
            TemperatureChanged.Invoke(temperature);
        }
    }

    public UnityEvent<float> TemperatureChanged;

    #region internals
    private LEGOTechnicTemperatureSensor sensor;
    private float temperature;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeTechnicTemperatureSensor) as LEGOTechnicTemperatureSensor;
        if (sensor == null)
        {
            Debug.LogWarning(name + " service not found");
            return false;
        }
        services.Remove(sensor);
        sensor.UpdateInputFormat(new LEGOInputFormat(sensor.ConnectInfo.PortID, sensor.ioType, 0, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
        sensor.RegisterDelegate(this);
        IsConnected = true;
        Debug.Log(name + " connected");
        return true;
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
        Temperature = Mathf.RoundToInt(newValue.RawValues[0] / 10f);
    }
    #endregion
}
