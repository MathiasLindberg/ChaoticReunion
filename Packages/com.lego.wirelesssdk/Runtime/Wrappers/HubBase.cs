using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEGODeviceUnitySDK;
using System.Linq;
using UnityEngine.Events;

public class HubBase : MonoBehaviour
{
    #region serialized stufff
    public HubType hubType;
    public string connectToSpecificHubId;
    public bool autoConnectOnStart = true;
    [SerializeField] Color _ledColor = Color.green;
    public Color LedColor
    {
        get => _ledColor;
        set
        {
            _ledColor = value;
            if (IsConnected)
            {
                rgbLight.SetColor(_ledColor);
            }
        }
    }
    #endregion
    #region non serialized state
    private bool _isConnected;
    public bool IsConnected
    {
        get => _isConnected;
        private set
        {
            if (value != _isConnected)
            {
                _isConnected = value;
                IsConnectedChanged.Invoke(_isConnected);
            }
        }
    }
    public string HubName { get { return device == null ? "" : device.DeviceName; } }
    private bool _buttonPressed;
    public bool ButtonPressed { get => _buttonPressed; private set { _buttonPressed = value; ButtonChanged.Invoke(_buttonPressed); } }
    public int BatteryLevel { get { return device == null ? 0 : device.BatteryLevel; } }
    #endregion
    #region services
    public List<ServiceBase> internalServices = new List<ServiceBase>();
    public List<ServiceBase> externalServices = new List<ServiceBase>();
    #endregion
    #region events
    public UnityEvent<bool> IsConnectedChanged;
    public UnityEvent<bool> ButtonChanged;
    #endregion

    #region internals
    public enum HubType
    {
        CityHub, TechnicHub, BoostHub, CityRemote // Large?
    }
    public ILEGODevice device { get; private set; }
    private RGBLight rgbLight = new RGBLight(); // make it one of internal services?

    void Awake()
    {
        if (LEGODeviceManager.Instance == null)
        {
            LEGODeviceManager.Initialize();
        }
    }

    void Start()
    {
        if (autoConnectOnStart)
        {
            StartCoroutine(DelayedAutoConnect());
        }
    }

    void OnDestroy()
    {
        UseDevice(null);
        RequestScan(false);
    }

    private IEnumerator DelayedAutoConnect()
    {
        yield return new WaitForSeconds(3);
        FindDeviceToUse();
    }

    private void DoSetup()
    {
        device.OnServiceConnectionChanged += ServiceConnectionChange;
        IList<ILEGOService> freeServices = device.Services.ToList();
        rgbLight.Setup(freeServices);
        Setup(freeServices);
        IsConnected = true;
        ButtonPressed = device.ButtonPressed;
        rgbLight.SetColor(LedColor);
    }

    private void Setup(ICollection<ILEGOService> freeServices)
    {
        foreach (var service in internalServices.Concat(externalServices).Where(s => s != null).OrderByDescending(s => s.port))
        {
            service.Setup(freeServices);
        }
    }

    private void ServiceConnectionChange(ILEGODevice device, ILEGOService service, bool connected)
    {
        if (connected)
        {
            var s = new List<ILEGOService>() { service };
            Setup(s);
            if (s.Count == 1)
            {
                Debug.LogWarning("unused service connected " + service.ServiceName + ", type " + service.ioType + ", on port " + service.ConnectInfo.PortID);
            }
        }
    }

    public List<int> AllPorts()
    {
        switch (hubType)
        {
            case HubType.CityHub: return new List<int>() { 0, 1 };
            case HubType.TechnicHub: return new List<int>() { 0, 1, 2, 3 };
            case HubType.BoostHub: return new List<int>() { 0, 1 };
            case HubType.CityRemote: return new List<int>();
        }
        return new List<int>();
    }

    public List<int> FreePorts()
    {
        List<int> ports = AllPorts();
        foreach (ServiceBase s in externalServices)
        {
            if (s != null && s.port != -1)
            {
                ports.Remove(s.port);
            }
        }
        ports.Insert(0, -1);
        return ports;
    }

    void OnButton(ILEGODevice device, bool pressed)
    {
        ButtonPressed = pressed;
    }

    // todo take out into some "dont destroy on load" class instead of static stuff
    private static List<ILEGODevice> usedDevices = new List<ILEGODevice>();
    private static int scanningCount = 0;
    private bool wantScan;

    private void UseDevice(ILEGODevice newDevice)
    {
        if (device != null)
        {
            IsConnected = false;
            device.OnServiceConnectionChanged -= ServiceConnectionChange;
            device.OnButtonStateUpdated -= OnButton;
            device.OnDeviceStateUpdated -= OnDeviceState;
            LEGODeviceManager.Instance.DisconnectDevice(device);
            usedDevices.Remove(device);
        }
        device = newDevice;
        if (device != null)
        {
            usedDevices.Add(device);
            device.OnButtonStateUpdated += OnButton;
            device.OnDeviceStateUpdated += OnDeviceState;
            LEGODeviceManager.Instance.ConnectDevice(device);
        }
    }

    private void RequestScan(bool scan)
    {
        if (wantScan == scan)
        {
            return;
        }
        wantScan = scan;
        if (wantScan)
        {
            scanningCount++;
            LEGODeviceManager.Instance.OnDeviceStateUpdated += CheckNewDevice;
            if (LEGODeviceManager.Instance.ScanState == DeviceManagerState.NotScanning)
            {
                LEGODeviceManager.Instance.Scan(); // todo might not be able to scan right now
            }
        }
        else
        {
            scanningCount--;
            LEGODeviceManager.Instance.OnDeviceStateUpdated -= CheckNewDevice;
            if (scanningCount == 0 && LEGODeviceManager.Instance.ScanState != DeviceManagerState.NotScanning)
            {
                LEGODeviceManager.Instance.StopScanning();
            }
        }
    }

    public void FindDeviceToUse()
    {
        if (!CheckAdvertisingDevices())
        {
            RequestScan(true);
        }
    }

    private bool CheckAdvertisingDevices()
    {
        foreach (var d in LEGODeviceManager.Instance.DevicesInState(DeviceState.DisconnectedAdvertising).OrderBy(d => new RSSIComparer()))
        {
            if (TryPotentialDevice(d))
            {
                return true;
            }
        }
        return false;
    }

    private void CheckNewDevice(ILEGODevice d, DeviceState oldState, DeviceState newState)
    {
        if (d.State == DeviceState.DisconnectedAdvertising)
        {
            if (TryPotentialDevice(d))
            {
                RequestScan(false);
            }
        }
    }

    private AbstractLEGODevice.DeviceType DeviceType()
    {
        switch (hubType)
        {
            case HubType.CityHub: return AbstractLEGODevice.DeviceType.Hub65;
            case HubType.TechnicHub: return AbstractLEGODevice.DeviceType.Hub128;
            case HubType.BoostHub: return AbstractLEGODevice.DeviceType.Hub64;
            case HubType.CityRemote: return AbstractLEGODevice.DeviceType.Hub66;
        }
        return new AbstractLEGODevice.DeviceType(DeviceSystemType.Unknown, -1);
    }

    private bool TryPotentialDevice(ILEGODevice d)
    {
        AbstractLEGODevice.DeviceType deviceType = DeviceType();
        if ((d.SystemType == deviceType.DeviceSystemType)
         && (d.SystemDeviceNumber == deviceType.SystemDeviceNumber)
         && !usedDevices.Contains(d)
         && (connectToSpecificHubId?.Length > 0 ? d.DeviceID == connectToSpecificHubId : true)
        )
        {
            UseDevice(d);
            return true;
        }
        return false;
    }

    private void OnDeviceState(ILEGODevice device, DeviceState oldState, DeviceState newState)
    {
        if (newState == DeviceState.InterrogationFinished)
        {
            DoSetup();
        }
        else if (newState == DeviceState.InterrogationFailed || newState == DeviceState.DisconnectedNotAdvertising || newState == DeviceState.DisconnectedAdvertising)
        {
            Debug.LogError("Device disconnected. State: " + device.State);
            UseDevice(null);
        }
    }

    public class RSSIComparer : Comparer<ILEGODevice>
    {
        public override int Compare(ILEGODevice x, ILEGODevice y)
        {
            return x.RSSIValue.CompareTo(y.RSSIValue);
        }
    }
    #endregion
}