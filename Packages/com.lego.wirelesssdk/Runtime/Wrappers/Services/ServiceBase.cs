using LEGODeviceUnitySDK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ServiceBase : MonoBehaviour, ILEGOGeneralServiceDelegate
{
    public bool IsConnected
    {
        get => isConnected;
        protected set
        {
            if (value != isConnected)
            {
                isConnected = value;
                IsConnectedChanged.Invoke(isConnected);
            }
        }
    }
    private bool isConnected;
    public UnityEvent<bool> IsConnectedChanged;

    public bool isInternal = false; // todo make private, serialized. add public get property and assign it via reflection
    public int port = -1;

    public abstract bool Setup(ICollection<ILEGOService> services);

    public abstract void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState);
    public virtual void DidUpdateInputFormat(ILEGOService service, LEGOInputFormat oldFormat, LEGOInputFormat newFormat) { }
    public virtual void DidUpdateInputFormatCombined(ILEGOService service, LEGOInputFormatCombined oldFormat, LEGOInputFormatCombined newFormat) { }
    public virtual void DidUpdateValueData(ILEGOService service, LEGOValue oldValue, LEGOValue newValue) { }
}
