using LEGODeviceUnitySDK;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OrientationSensor : ServiceBase
{
    public Vector3 Orientation
    {
        get => orientation; // Euler angles.
        private set
        {
            orientation = value;
            OrientationChanged.Invoke(orientation);
        }
    }

    public UnityEvent<Vector3> OrientationChanged;

    #region internals
    private LEGOTechnic3AxisOrientationSensor sensor;
    private Vector3 orientation;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeTechnic3AxisOrientationSensor) as LEGOTechnic3AxisOrientationSensor;
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
        Orientation = new Vector3(newValue.RawValues[1], -newValue.RawValues[0], -newValue.RawValues[2]);
    }
    #endregion
}

