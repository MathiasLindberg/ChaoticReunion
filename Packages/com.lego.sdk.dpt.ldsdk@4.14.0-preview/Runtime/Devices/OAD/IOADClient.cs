/**
 *
 * -- Ported Code --
 * Ported C# version of SimpleLink BLE OAD iOS from Texas Instruments.
 *
 *  !!-- ATTENTION --!!
 * Functionality regarding RSSI and timers has been omitted from the ported code below.
 * 
 * Ported SimpleLink Version: 1.0.1
 * 
 * Source URL: http://git.ti.com/simplelink-ble-oad-ios/simplelink-ble-oad-ios/trees/master/ti_oad/Classes
 *
 */

using System;

namespace LEGODeviceUnitySDK
{

    public struct OADClientProgressValues
    {
        public UInt32 CurrentBlockSize;
        public UInt32 CurrentBlock;
        public UInt32 LastBlockSent;
        public UInt32 CurrentByte;
        public UInt32 TotalBlocks;
        public UInt32 TotalBytes;
        public float PercentProgress;
        public float CurrentBytesPerSecond;
        public double StartDownloadTime;
        public double TotalDownloadTime;        
    }
    
    public enum OADClientState
    {
        Initializing,
        PeripheralNotConnected,
        OADServiceMissingOnPeripheral,
        Ready,
        GetDeviceTypeCommandSent,
        GetDeviceTypeResponseReceived,
        BlockSizeRequestSent,
        GotBlockSizeResponse,
        HeaderSent,
        HeaderOK,
        HeaderFailed,
        OADProcessStartCommandSent,
        ImageTransfer,
        ImageTransferFailed,
        ImageTransferOK,
        EnableOADImageCommandSent,
        CompleteFeedbackOK,
        CompleteFeedbackFailed,
        DisconnectedDuringDownload,
        OADCompleteIncludingDisconnect,
        RSSIGettingLow,
    }
    
    
    public interface IOADClient
    {
        event Action<OADClient, OADClientProgressValues> OnProgressUpdated;
        event Action<OADClient, OADClientState> OnProcessStateChanged;
        event Action RequestMTUSizeForTransfer;
        
        OADTransferStats TransferStats { get; } 
        UInt32 OADDeviceID { get; }
        
        void HandleInterrogationCompleted();

        void StartOAD(IOADImageReader imgData, OADFirmwareUpgradeProcess.ImageDataType imgDataType);

        void CancelOAD();

    }
}