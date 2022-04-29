using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[CustomEditor(typeof(HubBase), true)]
public class HubBaseInspector : Editor
{
    [MenuItem("LEGO Tools/Create LEGO Hub", priority = 200)]
    [MenuItem("GameObject/Create LEGO Hub", priority = 0)]
    private static void CreateLEGOHubObject()
    {
        var hub = new GameObject("LEGO Hub");
        var hubBase = hub.AddComponent<HubBase>();

        // Setup as technic by default.
        hubBase.hubType = HubBase.HubType.TechnicHub;
        var t1 = hub.AddComponent<TemperatureSensor>();
        t1.port = 61;
        t1.isInternal = true;
        hubBase.internalServices.Add(t1);
        var t2 = hub.AddComponent<TemperatureSensor>();
        t2.port = 96;
        t2.isInternal = true;
        hubBase.internalServices.Add(t2);
        var a = hub.AddComponent<AccelerationSensor>();
        a.port = 97;
        a.isInternal = true;
        hubBase.internalServices.Add(a);
        var o = hub.AddComponent<OrientationSensor>();
        o.port = 99;
        o.isInternal = true;
        hubBase.internalServices.Add(o);

        Undo.RegisterCreatedObjectUndo(hub, "Create LEGO Hub");

        Selection.activeGameObject = hub;
    }

    SerializedProperty autoConnectOnStart;
    SerializedProperty connectToSpecificDeviceId;
    SerializedProperty IsConnectedChanged;
    SerializedProperty ButtonChanged;

    Texture2D banner;
    GUIStyle bannerStyle;
    HubBase.HubType bannerType;
    Rect addButtonRect;

    void OnEnable()
    {
        autoConnectOnStart = serializedObject.FindProperty("autoConnectOnStart");
        connectToSpecificDeviceId = serializedObject.FindProperty("connectToSpecificHubId");
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        ButtonChanged = serializedObject.FindProperty("ButtonChanged");
        if (bannerStyle == null)
        {
            bannerStyle = new GUIStyle
            {
                padding = new RectOffset(0, 0, 10, 0),
                alignment = TextAnchor.MiddleCenter
            };
        }
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    private void LoadBanner(HubBase.HubType type)
    {
        if (banner == null || type != bannerType)
        {
            bannerType = type;
            string path = null;
            switch (type)
            {
                case HubBase.HubType.CityHub: path = "City Hub"; break;
                case HubBase.HubType.BoostHub: path = "Boost Hub"; break;
                case HubBase.HubType.CityRemote: path = "City Remote"; break;
                case HubBase.HubType.TechnicHub: path = "Technic Hub"; break;
            }
            if (path != null)
            {
                banner = Resources.Load<Texture2D>(path);
            }
            else
            {
                banner = null;
            }
        }
    }

    private void SetHubType(HubBase hub, HubBase.HubType type)
    {
        if (type == hub.hubType)
        {
            return;
        }

        Undo.RegisterCompleteObjectUndo(hub, "Changed hub type");

        hub.hubType = type;
        foreach (ServiceBase s in hub.internalServices)
        {
            if (s != null && s.gameObject == hub.gameObject)
            {
                Undo.DestroyObjectImmediate(s);
            }
        }
        foreach (ServiceBase s in hub.externalServices)
        {
            if (s != null && s.transform.parent == hub.transform)
            {
                Undo.DestroyObjectImmediate(s.gameObject);
            }
        }
        hub.internalServices.Clear();
        hub.externalServices.Clear();

        switch (type)
        {
            case HubBase.HubType.TechnicHub:
                var t1 = Undo.AddComponent<TemperatureSensor>(hub.gameObject);
                t1.port = 61;
                t1.isInternal = true;
                hub.internalServices.Add(t1);
                var t2 = Undo.AddComponent<TemperatureSensor>(hub.gameObject);
                t2.port = 96;
                t2.isInternal = true;
                hub.internalServices.Add(t2);
                var a = Undo.AddComponent<AccelerationSensor>(hub.gameObject);
                a.port = 97;
                a.isInternal = true;
                hub.internalServices.Add(a);
                var o = Undo.AddComponent<OrientationSensor>(hub.gameObject);
                o.port = 99;
                o.isInternal = true;
                hub.internalServices.Add(o);
                break;
            case HubBase.HubType.BoostHub:
                var m1 = Undo.AddComponent<TachoMotor>(hub.gameObject);
                m1.typeBitMask = (int)TachoMotor.TachoMotorIOTypes.InternalMotorWithTacho;
                m1.isInternal = true;
                hub.internalServices.Add(m1); // todo what ports?
                var m2 = Undo.AddComponent<TachoMotor>(hub.gameObject);
                m2.typeBitMask = (int)TachoMotor.TachoMotorIOTypes.InternalMotorWithTacho;
                m2.isInternal = true;
                hub.internalServices.Add(m2); // todo what ports?
                var m3 = Undo.AddComponent<TachoMotor>(hub.gameObject);
                m3.typeBitMask = (int)TachoMotor.TachoMotorIOTypes.InternalMotorWithTacho;
                m3.virtualConn = true;
                m3.isInternal = true;
                hub.internalServices.Add(m3); // todo what ports?
                break;
            case HubBase.HubType.CityRemote:
                var b0 = Undo.AddComponent<Buttons>(hub.gameObject);
                b0.port = 0;
                b0.isInternal = true;
                hub.internalServices.Add(b0);
                var b1 = Undo.AddComponent<Buttons>(hub.gameObject);
                b1.port = 1;
                b1.isInternal = true;
                hub.internalServices.Add(b1);
                break;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        HubBase hub = target as HubBase;

        LoadBanner(hub.hubType);
        if (banner != null)
        {
            GUILayout.Box(banner, bannerStyle, GUILayout.Height(200));
        }

        EditorGUILayout.SelectableLabel("Connected Hub Id: " + (hub.device != null ? hub.device.DeviceID : "Not connected"));

        HubBase.HubType newType = (HubBase.HubType)EditorGUILayout.EnumPopup("Hub type:", hub.hubType);
        SetHubType(hub, newType);

        EditorGUILayout.PropertyField(autoConnectOnStart);

        EditorGUILayout.PropertyField(connectToSpecificDeviceId);

        Color newColor = EditorGUILayout.ColorField("Led color", hub.LedColor);
        if (newColor != hub.LedColor)
        {
            Undo.RegisterCompleteObjectUndo(hub, "Changed led color");
            hub.LedColor = newColor;
        }

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", hub.IsConnected);
        EditorGUILayout.TextField("Name", hub.HubName);
        EditorGUILayout.Toggle("Button pressed", hub.ButtonPressed);
        EditorGUILayout.IntField("Battery", hub.BatteryLevel);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        EditorGUILayout.PropertyField(ButtonChanged);

        if (hub.FreePorts().Count > 1 || hub.externalServices.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("External Services", EditorStyles.boldLabel);
            List<int> freePorts = hub.FreePorts();
            for (int i = 0; i < hub.externalServices.Count; i++)
            {
                ServiceBase service = hub.externalServices[i];
                EditorGUILayout.BeginHorizontal();
                // check duplicates?
                bool wasNull = service == null;
                service = hub.externalServices[i] = (ServiceBase)EditorGUILayout.ObjectField(service, typeof(ServiceBase), true);
                if (service != null)
                {
                    if (wasNull)
                    {
                        service.port = -1;
                    }
                    int[] ports;
                    if (service.port != -1)
                    {
                        ports = freePorts.Append(service.port).ToArray();
                        Array.Sort(ports);
                    }
                    else
                    {
                        ports = freePorts.ToArray();
                    }
                    string[] portNames = ports.Select(p => p < 0 ? "Any" : ((char)('A' + p)).ToString()).ToArray();
                    var newPort = EditorGUILayout.IntPopup(service.port, portNames, ports, GUILayout.Width(60));
                    if (newPort != service.port)
                    {
                        Undo.RegisterCompleteObjectUndo(service, "Changed port");
                        service.port = newPort;
                    }
                }

                if (GUILayout.Button(EditorGUIUtility.FindTexture("Toolbar Minus"), GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    Undo.RegisterCompleteObjectUndo(hub, "Remove external service");
                    Undo.DestroyObjectImmediate(hub.externalServices[i].gameObject);
                    hub.externalServices.RemoveAt(i--);
                }

                EditorGUILayout.EndHorizontal();
            }
            if (hub.externalServices.Count < hub.AllPorts().Count)
            {
                if (GUILayout.Button("Add External Service", GUILayout.Height(50)))
                {
                    PopupWindow.Show(addButtonRect, new AddServicePopup(hub));
                }

                if (Event.current.type == EventType.Repaint)
                {
                    addButtonRect = GUILayoutUtility.GetLastRect();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

public class AddServicePopup : PopupWindowContent
{
    private HubBase hub;
    Type serviceClassType;
    Enum mode;
    int port;
    GUIStyle bannerStyle;
    Dictionary<string, Type> types;

    const int padding = 10;

    public AddServicePopup(HubBase hub)
    {
        this.hub = hub;
        port = -1;
        bannerStyle = new GUIStyle
        {
            padding = new RectOffset(0, 0, 10, 10),
            alignment = TextAnchor.MiddleCenter
        };

        types = new Dictionary<string, Type>()
        {
            {"Motors", typeof(Motor) },
            {"Tacho Motors", typeof(TachoMotor) },
            {"Light", typeof(WhiteLight) },
            {"Vision Sensor", typeof(VisionSensor) },
            //{"Duplo Color Sensor", typeof(ColorSensorDuplo) },
            {"Technic Color Sensor", typeof(ColorSensorTechnic) },
            {"Technic Distance Sensor", typeof(DistanceSensor) },
            {"Technic Force Sensor", typeof(ForceSensor) }
        };
        SetType(typeof(TachoMotor));
    }

    private void SetType(Type newType)
    {
        serviceClassType = newType;
        mode = null;
        if (newType == typeof(TachoMotor))
        {
            mode = TachoMotor.TachoMotorMode.Position;
        }
        else if (newType == typeof(ColorSensorDuplo))
        {
            mode = ColorSensorDuplo.ColorSensorDuploMode.Color;
        }
        else if (newType == typeof(ColorSensorTechnic))
        {
            mode = ColorSensorTechnic.ColorSensorTechnicMode.Color;
        }
        else if (newType == typeof(VisionSensor))
        {
            mode = VisionSensor.VisionSensorMode.Color;
        }
        else if (newType == typeof(ForceSensor))
        {
            mode = ForceSensor.ForceSensorMode.Force;
        }
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(300, 250);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginArea(new Rect(padding, padding, editorWindow.position.width - 2 * padding, editorWindow.position.height - 2 * padding));
        GUILayout.Box(Resources.Load<Texture2D>(GetImage(serviceClassType.Name)), bannerStyle, GUILayout.Height(150));

        int index = types.Values.ToList().IndexOf(serviceClassType);
        string[] typeNames = types.Keys.ToArray();
        int newIndex = EditorGUILayout.Popup("Type", index, typeNames);
        if (newIndex != index)
        {
            SetType(types[typeNames[newIndex]]);
        }

        if (mode != null)
        {
            mode = EditorGUILayout.EnumPopup("Mode", mode);
        }

        int[] ports = hub.FreePorts().ToArray();
        string[] portNames = ports.Select(p => p < 0 ? "Any" : ((char)('A' + p)).ToString()).ToArray();
        port = EditorGUILayout.IntPopup("Port", port, portNames, ports);

        if (GUILayout.Button("Add"))
        {
            GameObject serviceGO = new GameObject(ObjectNames.NicifyVariableName(serviceClassType.Name));
            serviceGO.transform.parent = hub.transform;
            ServiceBase service = serviceGO.AddComponent(serviceClassType) as ServiceBase;
            service.port = port;
            if (service.GetType() == typeof(ColorSensorDuplo))
            {
                (service as ColorSensorDuplo).Mode = (ColorSensorDuplo.ColorSensorDuploMode)mode;
            }
            else if (service.GetType() == typeof(ColorSensorTechnic))
            {
                (service as ColorSensorTechnic).Mode = (ColorSensorTechnic.ColorSensorTechnicMode)mode;
            }
            else if (service.GetType() == typeof(VisionSensor))
            {
                (service as VisionSensor).Mode = (VisionSensor.VisionSensorMode)mode;
            }
            else if (service.GetType() == typeof(TachoMotor))
            {
                (service as TachoMotor).Mode = (TachoMotor.TachoMotorMode)mode;
            }
            else if (service.GetType() == typeof(ForceSensor))
            {
                (service as ForceSensor).Mode = (ForceSensor.ForceSensorMode)mode;
            }
            Undo.RegisterCreatedObjectUndo(serviceGO, "Added service");
            Undo.RegisterCompleteObjectUndo(hub, "Added service");
            hub.externalServices.Add(service);

            editorWindow.Close();
        }

        GUILayout.EndArea();
    }

    public static string GetImage(string type)
    {

        switch (type)
        {
            case "ColorSensorTechnic": return "Technic Color Sensor";
            case "ForceSensor": return "Technic Force Sensor";
            case "DistanceSensor": return "Technic Distance Sensor";
            case "VisionSensor": return "Boost Vision Sensor";
            case "WhiteLight": return "City Light";
            case "Motor": return "Motors";
            case "TachoMotor": return "Tacho Motors";
            // TODO Fill in remaining services.
            default: return "";
        }
    }
}
