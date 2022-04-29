using System;
using System.Collections.ObjectModel;

namespace CoreUnityBleBridge
{
    public interface IBleBridge 
    {
        event Action<AdapterScanState> AdapterScanStateChanged;
        event Action<IBleDevice> DeviceAppeared;
        event Action<IBleDevice> DeviceDisappeared;
        event Action<string> ErrorOccurred;
        event Action PacketSent;
        event Action PacketReceived;
        event Action PacketDropped;
        

        AdapterScanState AdapterScanState { get; }
        ReadOnlyCollection<IBleDevice> Devices { get; }

        void SetScanState(bool enabled);
    } 
    
}
