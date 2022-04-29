using System;
using dk.lego.devicesdk.bluetooth.V3bootloader.messages;

namespace LEGODeviceUnitySDK
{
    internal interface IFlashLoaderDevice : ILEGODevice
    {
        void SendMessage(DownstreamMessage msg, int? seqNr = null, bool useSoftAck = false);
        event Action<UpstreamMessage> OnReceivedMessage;
        event Action<int> OnPacketSent;
        void Disconnect();
    }
}

