using LEGODeviceUnitySDK;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DistanceSensor : ServiceBase
{
    public float Distance
    {
        get => distance; // Centimeters.
        private set
        {
            distance = value;
            DistanceChanged.Invoke(distance);
        }
    }

    public UnityEvent<float> DistanceChanged;

    public void SetIntensity(int percentage) { SetIntensity(percentage, percentage, percentage, percentage); }  // 0 to 100.

    public void SetIntensity(int upperLeftPercentage, int upperRightPercentage, int lowerLeftPercentage, int lowerRightPercentage) // 0 to 100.
    {
        if (sensor == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        sensor.SendCommand(new LEGOTechnicDistanceSensor.SetLightPercentageCommand() { Percentage0 = upperLeftPercentage, Percentage1 = upperRightPercentage, Percentage2 = lowerLeftPercentage, Percentage3 = lowerRightPercentage });
    }

    #region internals
    private LEGOTechnicDistanceSensor sensor;
    private float distance;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeTechnicDistanceSensor) as LEGOTechnicDistanceSensor;
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
        if (newValue.RawValues[0] < 0f)
        {
            Distance = -1f;
        }
        else
        {
            Distance = newValue.RawValues[0] / 10f;
        }
    }
    #endregion
}
