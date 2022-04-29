using System;
using System.Linq;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    public class OADFirmwareUpgradeProcess : IFirmwareUpgradeProcess
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OADFirmwareUpgradeProcess));

        public enum UpdateStartMode
        {
            ViaRequestEnterBootMode,
            ViaDirectOADMode
        };
        
        public enum ImageDataType
        {
            Firmware,
            Assetpack
        }

        private static UpdateStartMode updateMode = UpdateStartMode.ViaDirectOADMode;
        public static UpdateStartMode UpdateMode
        {
            get { return updateMode; }
            set
            {
                updateMode = value;
                OnUpdateModeChanged?.Invoke(updateMode);
            }
        }

        public event Action<IFirmwareUpgradeProcess, DeviceFlashState> OnDeviceFlashStateUpdated;
        public event Action<IFirmwareUpgradeProcess, FlashErrorCode, string> OnDeviceFlashFailed;
        public event Action<IFirmwareUpgradeProcess, float> OnFlashProgressPercentageUpdated;
        public static event Action<UpdateStartMode> OnUpdateModeChanged;

        private readonly Action<IFirmwareUpgradeProcess> onCompletion;

        private DeviceFlashState flashState = DeviceFlashState.Idle;

        public DeviceFlashState FlashState
        {
            get { return flashState; }
            private set
            {
                if (flashState == value)
                {
                    return;
                }
                flashState = value;
                Logger.Debug("FlashState changed: " + flashState);
                if (OnDeviceFlashStateUpdated != null)
                {
                    Logger.Debug("OnDeviceFlashStateUpdated: " + flashState);
                    OnDeviceFlashStateUpdated(this, flashState);
                }
            }
        }

        private DateTime startTime;

        private readonly IOADClient oadClient;
        private readonly byte[] firmwareData;
        private ILEGODevice device;

        public OADFirmwareUpgradeProcess(ILEGODevice device, IOADClient oadClient, byte[] firmwareData, Action<IFirmwareUpgradeProcess> onCompletion)
        {
            this.device = device;
            this.oadClient = oadClient;
            this.firmwareData = firmwareData;
            this.onCompletion = onCompletion;
        }
        
        public void StartProcess()
        {
            RegisterProgressListeners();
            //Just cycle through the first three states. Unlike "normal" LPF2 fw flashing, when flashing an OAD Device we do not reboot and start
            //up in boot-loader mode, we just flash the already connected device
            FlashState = DeviceFlashState.Idle;
            FlashState = DeviceFlashState.ConnectingToBootLoader;
            FlashState = DeviceFlashState.ConnectedToBootLoader;
            
            if(UpdateMode == UpdateStartMode.ViaRequestEnterBootMode)
            {
                device.OnDeviceWillGoIntoBootMode += HandleDeviceWillGoIntoBootMode;
                ((LEGODevice) device).RequestToGoIntoFlashLoaderMode();
            }
            else
            {
                StartOADProcess();
            }
        } 

        private void HandleDeviceWillGoIntoBootMode(ILEGODevice device)
        {
            device.OnDeviceWillGoIntoBootMode -= HandleDeviceWillGoIntoBootMode;
            StartOADProcess();
        }

        private void StartOADProcess()
        {
            startTime = DateTime.UtcNow;
            FlashState = DeviceFlashState.Flashing;
            var dataType = ImageDataTypeCheck(firmwareData);
            oadClient.RequestMTUSizeForTransfer += OnRequestMTUSizeForTransfer;
            oadClient.StartOAD(new OADImageReader(firmwareData),dataType);
            Logger.Debug("StartOADProcess");
        }

        private ImageDataType ImageDataTypeCheck(byte[] imageData)
        {
            //Check for hex sequence
            //39FB0210 39FB0210 39FB0210 39FB0210 39FB0210 39FB0210 39FB0210 39FB0210 39FB0210 39FB0210 39FB0210 11FB0210 FFFFFFFF
            //starting from address 0009C to 000D0, which is equal to byte array index 156 to 208
            var firmwareBytePattern = "39FB021039FB021039FB021039FB021039FB021039FB021039FB021039FB021039FB021039FB021039FB021011FB0210FFFFFFFF";
            var bytePattern = imageData.Skip(156).Take(52).ToArray();
            if (bytePattern.Length <= 0)
                return ImageDataType.Assetpack;
            
            var byteString = String.Concat(Array.ConvertAll(bytePattern, x => x.ToString("X2")));
            if(byteString.Equals(firmwareBytePattern))
            {
                return ImageDataType.Firmware;
            }
            
            return ImageDataType.Assetpack;
        }
        
        public void RegisterProgressListeners(Action<OADClient, OADClientState> oadClientOnProcessStateChanged, Action<OADClient, OADClientProgressValues> oadClientOnProgressUpdated)
        {
            if (oadClientOnProgressUpdated != null) 
                oadClient.OnProgressUpdated += oadClientOnProgressUpdated;
            if (oadClientOnProcessStateChanged != null)
                oadClient.OnProcessStateChanged += oadClientOnProcessStateChanged;
        }
        
        public void UnregisterProgressListeners(Action<OADClient, OADClientState> oadClientOnProcessStateChanged, Action<OADClient, OADClientProgressValues> oadClientOnProgressUpdated)
        {
            if (oadClientOnProgressUpdated != null) 
                oadClient.OnProgressUpdated -= oadClientOnProgressUpdated;
            if (oadClientOnProcessStateChanged != null)
                oadClient.OnProcessStateChanged -= oadClientOnProcessStateChanged;
        }
        
        private void RegisterProgressListeners()
        {
            RegisterProgressListeners(OadClientOnProcessStateChanged,OadClientOnProgressUpdated);
        }

        private void UnregisterProgressListeners()
        {
            UnregisterProgressListeners(OadClientOnProcessStateChanged,OadClientOnProgressUpdated);
        }

        private void OadClientOnProgressUpdated(OADClient client, OADClientProgressValues progressValues)
        {
            if (OnFlashProgressPercentageUpdated != null)
                OnFlashProgressPercentageUpdated(this, progressValues.PercentProgress);
        }

        private void OadClientOnProcessStateChanged(OADClient client, OADClientState clientState)
        {
            Logger.Debug("OadClientOnProcessStateChanged state: " + clientState);
            switch (clientState)
            {
                //Error Cases 
                case OADClientState.PeripheralNotConnected:
                case OADClientState.HeaderFailed:
                case OADClientState.OADServiceMissingOnPeripheral:
                case OADClientState.CompleteFeedbackFailed:
                case OADClientState.DisconnectedDuringDownload:
                case OADClientState.ImageTransferFailed:
                    FireError(clientState);
                    break;
                //Flashing completed with success
                case OADClientState.OADCompleteIncludingDisconnect:
                    HandleFlashingComplete(true);
                    break;
                case OADClientState.ImageTransfer:
                    break;

                case OADClientState.ImageTransferOK:
                    OadClientOnProgressUpdated((OADClient) oadClient, new OADClientProgressValues(){ PercentProgress = 100});
                    break;
                
                case OADClientState.CompleteFeedbackOK:
                    HandleFlashingComplete(false);
                    client.HandleTransferCompleted();
                    /*
                    /* Could potentially call RequestReboot when receiving CompleteFeedbackOK on assetpack transfer
                    Logger.Warn("OAD CompleteFeedbackOK, request device reboot");
                    if (device != null && device is LEGODevice)
                    { 
                        ((LEGODevice) device).RequestToGoIntoFlashLoaderMode();
                        ((LEGODevice) device).RequestReboot();
                    }
                    */
                    break;
                //Intentionally ignored (states not communicated upwards)
                case OADClientState.Initializing:
                case OADClientState.Ready:
                case OADClientState.GetDeviceTypeCommandSent:
                case OADClientState.GetDeviceTypeResponseReceived:
                case OADClientState.BlockSizeRequestSent:
                case OADClientState.GotBlockSizeResponse:
                case OADClientState.HeaderSent:
                case OADClientState.HeaderOK:
                case OADClientState.OADProcessStartCommandSent:
                case OADClientState.EnableOADImageCommandSent:
                case OADClientState.RSSIGettingLow:
                    break;

                default:
                    throw new ArgumentOutOfRangeException("clientState", clientState, null);
            }
        }

        //HandleFlashingComplete called with bool executeOnComplete that toggles whether or not to remove listener after all messages (including disconnect) from transfer are done
        private void HandleFlashingComplete(bool executeOnComplete)
        {
            FlashState = DeviceFlashState.FlashingCompleted;
            var duration = DateTime.UtcNow - startTime;
            Logger.Info("Firmware upgrade total duration: " + duration.TotalMilliseconds + "ms");

            if(executeOnComplete)
            {
                if (onCompletion != null)
                {
                    onCompletion(this);
                }
                UnregisterProgressListeners();
            }

            OnRequestMTUSizeForNonTransfers();

            FlashState = DeviceFlashState.Idle;
            Logger.Debug("OADFirmwareUpgradeProcess::HandleFlashingComplete FlashState:" + FlashState);
        }

        private void FireError(OADClientState clientState)
        {
            FlashErrorCode errorCode;
            switch (clientState)
            {
                case OADClientState.OADServiceMissingOnPeripheral:
                case OADClientState.PeripheralNotConnected:
                    errorCode = FlashErrorCode.General;
                    break;
                case OADClientState.HeaderFailed:
                case OADClientState.CompleteFeedbackFailed:
                case OADClientState.ImageTransferFailed: 
                    errorCode = FlashErrorCode.TransferFailed;
                    break;
                case OADClientState.DisconnectedDuringDownload:
                    errorCode = FlashErrorCode.DisconnectedDuringDownload;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(clientState), clientState, null);
            }
            
            if (OnDeviceFlashFailed != null)
            {
                OnDeviceFlashFailed(this, errorCode, "Flashing of OAD device failed with: " + clientState);
            }
            
            FlashState = DeviceFlashState.Idle;
            UnregisterProgressListeners();
            OnRequestMTUSizeForNonTransfers();
        }

        public void SendCancelOADMessage()
        {
            oadClient.CancelOAD();
        }

        private void OnRequestMTUSizeForTransfer()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var abstractDevice = device as AbstractLEGODevice;
                abstractDevice.RequestMTUSize(512);
#endif
        }
        
        private void OnRequestMTUSizeForNonTransfers()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                var abstractDevice = device as AbstractLEGODevice;
                abstractDevice.RequestMTUSize(23);
#endif
        }
    }
}