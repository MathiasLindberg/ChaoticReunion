using System;
using System.Collections.Generic;
using System.Linq;
using LEGO.Logger;
using CoreUnityBleBridge;
using CoreUnityBleBridge.Model;

namespace LEGODeviceUnitySDK
{
    public class DeviceListTracker
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DeviceListTracker));

        private Dictionary<string, AbstractLEGODevice> devices = new Dictionary<string, AbstractLEGODevice>();

        #region Accessors
        private int sortingKeyCounter = 0;

        public ILEGODevice[] DevicesAsArray {
            get {
                return devices.Values
                    .Where(x => x.State != DeviceState.DisconnectedNotAdvertising) // cf. doc of ILEGODeviceManager.Devices
                    .OrderBy(SortingKeyOrder)
                    .ToArray();
            }
        }

        public ILEGODevice FindLegoDevice(string deviceID)
        {
            try
            {
                return devices[deviceID];
            }
            catch(KeyNotFoundException)
            {
                logger.Warn("device not found in device list deviceId:" + deviceID);
                return null;
            }
        }

        public List<ILEGODevice> DevicesInState(DeviceState deviceState, bool includeBootLoaderDevices) {
            return devices.Values.Where(d =>
                d.State == deviceState && (!d.IsBootLoader || includeBootLoaderDevices)
            ).Cast<ILEGODevice>().OrderBy(RSSIOrder).ToList();
        }

        public List<ILEGODevice> DevicesConnected(bool connected)
        {
            return devices.Values
                   .Where(x => x.State != DeviceState.DisconnectedNotAdvertising) // cf. doc of ILEGODeviceManager.Devices
                   .Where(d => Connectedness(d) == connected)
                   .Cast<ILEGODevice>().OrderBy(RSSIOrder).ToList();
        }

        private bool Connectedness(ILEGODevice device)
        {
            switch (device.State)
            {
            case DeviceState.InterrogationFailed:
            case DeviceState.DisconnectedAdvertising:
            case DeviceState.DisconnectedNotAdvertising:
                return false;
            case DeviceState.Connecting:
            case DeviceState.Interrogating:
            case DeviceState.InterrogationFinished:
                return true;
            default:
                logger.Error("Unknown state: " + device.State);
                return false;
            }
        }

        public void OrderDevicesByRSSI()
        {
            var sorted =
                devices.Values
                    .OrderBy(RSSIOrder)
                    .ToArray();
            sortingKeyCounter = 0; // Start from beginning.
            foreach (var d in sorted)
                AssignNextSortingKey(d);
        }

        private int SortingKeyOrder(ILEGODevice x) {
            var dev = x as AbstractLEGODevice;
            return dev==null ? 0 : dev.SortingKey;
        }

        private int RSSIOrder(ILEGODevice x) {
            return x==null ? 0 : -x.RSSIValue;
        }
        #endregion

        #region Incoming updates
        public void OnDeviceAppeared(IBleDevice bleDevice)
        {
            new PendingDevice(bleDevice,
                device => devices.Add(device.DeviceID, AssignNextSortingKey(device)));
        }

        public void OnDeviceDisappeared(IBleDevice bleDevice)
        {
            var deviceID = bleDevice.ID;
            AbstractLEGODevice device;
            if (devices.TryGetValue(deviceID, out device)) {
                device.UpdateVisibilityState();
                devices.Remove(deviceID);
                device.Unsubscribe();
            }
        }

        /// <summary>
        /// A device which has appeared but not yet given sufficient data for actual ILEGODevice instantiation.
        /// </summary>
        private class PendingDevice 
        {
            private readonly IBleDevice bleDevice;
            private readonly Action<AbstractLEGODevice> onInstantiated;
            private bool instantiated = false;

            public PendingDevice(IBleDevice bleDevice, Action<AbstractLEGODevice> onInstantiated)
            {
                this.bleDevice = bleDevice;
                this.onInstantiated = onInstantiated;
                Subscribe();
            }

            private void Subscribe()
            {
                bleDevice.VisibilityStateChanged += this.OnDeviceStateChanged;
                bleDevice.ServiceGuidChanged += this.OnDeviceStateChanged;
                bleDevice.ManufacturerDataChanged += this.OnDeviceStateChanged;

                bleDevice.Disappeared += this.Unsubscribe;

                OnDeviceStateChanged(); // Check at once whether the device is already eligible for instantiation.
            }

            private void Unsubscribe()
            {
                bleDevice.VisibilityStateChanged -= this.OnDeviceStateChanged;
                bleDevice.ServiceGuidChanged -= this.OnDeviceStateChanged;
                bleDevice.ManufacturerDataChanged -= this.OnDeviceStateChanged;

                bleDevice.Disappeared -= this.Unsubscribe;
            }


            private void OnDeviceStateChanged()
            {
                if (instantiated) 
                {
                    return; // Only instantiate once.
                }

                if (bleDevice.ManufacturerData == null || bleDevice.ServiceGuid == null) 
                {
                    return;
                }

                var serviceGuid = (Guid)bleDevice.ServiceGuid;

                logger.Debug("Device appeared: '"+bleDevice.Name+"'");

                var device = CreateDeviceForGattService(bleDevice, serviceGuid);

                if (device != null) 
                {
                    instantiated = true;
                    onInstantiated(device); // perform action - which adds device to device list (see constructor 2nd parameter) - before propagating state change
                    device.Subscribe();
                    Unsubscribe();
                } 
                else 
                {
                    // Unsupported device type - ignore.
                    logger.Info("Ignoring unsupported LEGO device type: name='"+bleDevice.Name+"' id="+bleDevice.ID+" serviceID="+serviceGuid);
                }
            }
        } // end of PendingDevice subclass

        internal void OnDeviceStateChanged(BleDevice bleDevice)
        {
            var deviceID = bleDevice.ID;
            var deviceName = bleDevice.Name;

            AbstractLEGODevice device;
            if (!devices.TryGetValue(deviceID, out device)) {
                if (bleDevice.ManufacturerData == null || bleDevice.ServiceGuid == null) {
                    logger.Debug("Device appeared: '"+deviceName+"' but does not have complete data.");
                    return;
                }
                var serviceGuid = (Guid)bleDevice.ServiceGuid;

                logger.Debug("Device appeared: '"+deviceName+"'");
                device = AssignNextSortingKey(CreateDeviceForGattService(bleDevice, serviceGuid));
                if (device == null) {
                    // Unsupported device type - ignore.
                    logger.Info("Ignoring unsupported LEGO device type: name='"+deviceName+"' id="+deviceID+" serviceID="+serviceGuid);
                    return;
                }
                devices.Add(deviceID, device);
                device.Subscribe();

                // Fire first state-change;
                if (device.State != DeviceState.DisconnectedNotAdvertising)
                    LEGODeviceManager.Instance.HandleDeviceStateUpdated(device, DeviceState.DisconnectedNotAdvertising, device.State);
            }
        }

        private static AbstractLEGODevice CreateDeviceForGattService(IBleDevice bleDevice, Guid serviceGUID)
        {
            switch (serviceGUID.ToString()) {
            case V3BootloaderDevice.V3_BOOTLOADER_SERVICE:
                return new V3BootloaderDevice(bleDevice);
            case LEGODevice.V3_SERVICE:
                return new LEGODevice(bleDevice);
            //TODO Should probably add TIOAD client, when it has been refactored to new class implementing AbstractLegoDevice
            default:
                return null;
            }
        }

        private AbstractLEGODevice AssignNextSortingKey(AbstractLEGODevice device)
        {
            device.SortingKey = ++sortingKeyCounter;
            return device;
        }

        #endregion
    }
}

