using LEGODeviceUnitySDK;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LEGOMaterials;

public class VisionSensor : ServiceBase // ILEGOColorSensorDelegate?
{
    public VisionSensorMode Mode
    {
        get => mode;
        set
        {
            if (mode != value)
            {
                mode = value;
                UpdateInputFormat();
            }
        }
    }

    public int Id
    {
        get => id; // MouldingColour.Id
        private set
        {
            id = value;
            if (id >= 0)
            {
                color = MouldingColour.GetColour(id);
            }
            else
            {
                color = Color.clear;
            }
            IdChanged.Invoke(id);
            ColorChanged.Invoke(color);
        }
    }

    public Color Color
    {
        get => color;
    }

    public int Reflection
    {
        get => reflection; // 0 to 100.
        private set
        {
            reflection = value;
            ReflectionChanged.Invoke(reflection);
        }
    }

    public int Ambient
    {
        get => ambient; // 0 to 100.
        private set
        {
            ambient = value;
            AmbientChanged.Invoke(ambient);
        }
    }

    public Vector3 RGB
    {
        get => rgb; // Raw RGB response - 0 to 1.
        private set
        {
            rgb = value;
            RGBChanged.Invoke(rgb);
        }
    }

    public UnityEvent<int> IdChanged;
    public UnityEvent<Color> ColorChanged;
    public UnityEvent<int> ReflectionChanged;
    public UnityEvent<int> AmbientChanged;
    public UnityEvent<Vector3> RGBChanged;

    static int ColorIndexToId(int index)
    {
        switch (index)
        {
            case 0: return 26; // MouldingColour.Id.Black
            //case 1: return 124; // MouldingColour.Id.BrightReddishViolet. Cannot be seen by this sensor.
            //case 2: return -1; // Unknown
            case 3: return 23; // MouldingColour.Id.BrightBlue
            //case 4: return 102; // MouldingColour.Id.MediumBlue. Cannot be seen by this sensor.
            case 5: return 28; // MouldingColour.Id.DarkGreen
            //case 6: return -1; // Unknown
            case 7: return 24; // MouldingColour.Id.BrightYellow
            //case 8: return -1; // Unknown
            case 9: return 21; // MouldingColour.Id.BrightRed
            case 10: return 1; // MouldingColour.Id.White
            default: return -1; // Unknown
        }
    }

    #region internals
    public enum VisionSensorMode
    {
        Color = 0,
        //Detect = 1,
        //Count = 2,
        Reflection = 3,
        Ambient = 4,
        //ColorIndexOut = 5,
        [InspectorName("RGB")]
        RGBRaw = 6,
        //IRTx = 7,
        //Spec1 = 8,
        //Debug = 9,
        //Calib = 10
    }

    private LEGOVisionSensor sensor;
    [SerializeField] private VisionSensorMode mode = VisionSensorMode.Color;
    private int id;
    private Color color;
    private int reflection;
    private int ambient;
    private Vector3 rgb;

    public override bool Setup(ICollection<ILEGOService> services)
    {
        if (IsConnected)
        {
            return true;
        }
        sensor = services.FirstOrDefault(s => (port == -1 || port == s.ConnectInfo.PortID) && s.ioType == IOType.LEIOTypeVisionSensor) as LEGOVisionSensor;
        if (sensor == null)
        {
            Debug.LogWarning(name + " service not found");
            return false;
        }
        services.Remove(sensor);
        UpdateInputFormat();
        sensor.RegisterDelegate(this);
        IsConnected = true;
        Debug.Log(name + " connected");
        return true;
    }

    private void UpdateInputFormat()
    {
        sensor?.UpdateInputFormat(new LEGOInputFormat(sensor.ConnectInfo.PortID, sensor.ioType, (int)mode, 1, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw, true));
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
        if (newValue.Mode == (int)VisionSensorMode.Color)
        {
            Id = ColorIndexToId((int)newValue.RawValues[0]);
        }
        else if (newValue.Mode == (int)VisionSensorMode.Reflection)
        {
            Reflection = Mathf.RoundToInt(newValue.PctValues[0]);
        }
        else if (newValue.Mode == (int)VisionSensorMode.Ambient)
        {
            Ambient = Mathf.RoundToInt(newValue.PctValues[0]);
        }
        else if (newValue.Mode == (int)VisionSensorMode.RGBRaw)
        {
            RGB = new Vector3(newValue.RawValues[0] / 1023.0f, newValue.RawValues[1] / 1023.0f, newValue.RawValues[2] / 1023.0f);
        }
        // TODO Other modes.
    }
    #endregion
}
