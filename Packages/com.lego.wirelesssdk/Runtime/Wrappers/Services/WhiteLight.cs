using LEGODeviceUnitySDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WhiteLight : ServiceBase
{
    public void SetIntensity(int percentage) // 0 to 100.
    {
        if (light == null)
        {
            Debug.LogError(name + " is not connected");
            return;
        }
        light.SendCommand(new LEGOSingleColorLight.SetPercentCommand() { Percentage = Mathf.Clamp(percentage, 0, 100) });
    }

    #region internals
    private new LEGOSingleColorLight light;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        light = services.FirstOrDefault(s => s.ioType == IOType.LEIOTypeLight) as LEGOSingleColorLight;
        if (light == null)
        {
            Debug.LogWarning(name + " service not found");
            return false;
        }
        services.Remove(light);
        light.UpdateInputFormat(new LEGOInputFormat(light.ConnectInfo.PortID, light.ioType, (int)LEGOSingleColorLight.LightMode.Percentage, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, false));
        light.RegisterDelegate(this);
        IsConnected = true;
        Debug.Log(name + " connected");
        return true;
    }

    private void OnDestroy()
    {
        light?.UnregisterDelegate(this);
    }

    public override void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState)
    {
        if (newState == ServiceState.Disconnected)
        {
            Debug.LogWarning(name + " disconnected");
            light.UnregisterDelegate(this);
            light = null;
            IsConnected = false;
        }
    }
    #endregion
}
