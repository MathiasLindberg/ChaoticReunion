using LEGODeviceUnitySDK;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AccelerationSensor : ServiceBase
{
    public Vector3 Acceleration
    {
        get => acceleration; // Unsure.
        private set
        {
            acceleration = value;
            AccelerationChanged.Invoke(acceleration);
        }
    }

    public UnityEvent<Vector3> AccelerationChanged;

    #region internals
    private LEGOTechnic3AxisAccelerationSensor sensor;
    private Vector3 acceleration;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeTechnic3AxisAccelerometer) as LEGOTechnic3AxisAccelerationSensor;
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
        Acceleration = new Vector3(newValue.RawValues[1], -newValue.RawValues[2], -newValue.RawValues[0]) / 4096f; // 4096 approximately normalizes the raw values.
    }
    #endregion
}
