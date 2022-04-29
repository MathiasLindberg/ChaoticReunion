using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisionSensor))]
public class VisionSensorInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty IdChanged;
    SerializedProperty ColorChanged;
    SerializedProperty ReflectionChanged;
    SerializedProperty AmbientChanged;
    SerializedProperty RGBChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        IdChanged = serializedObject.FindProperty("IdChanged");
        ColorChanged = serializedObject.FindProperty("ColorChanged");
        ReflectionChanged = serializedObject.FindProperty("ReflectionChanged");
        AmbientChanged = serializedObject.FindProperty("AmbientChanged");
        RGBChanged = serializedObject.FindProperty("RGBChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        VisionSensor color = serializedObject.targetObject as VisionSensor;

        var newMode = (VisionSensor.VisionSensorMode)EditorGUILayout.EnumPopup("Mode", color.Mode);
        if (newMode != color.Mode)
        {
            Undo.RegisterCompleteObjectUndo(color, "Changed color sensor mode");
            color.Mode = newMode;
        }

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", color.IsConnected);
        if (color.Mode == VisionSensor.VisionSensorMode.Color)
        {
            EditorGUILayout.IntField("Id", color.Id);
            EditorGUILayout.ColorField("Color", color.Color);
        }
        if (color.Mode == VisionSensor.VisionSensorMode.Reflection)
        {
            EditorGUILayout.IntField("Reflection", color.Reflection);
        }
        if (color.Mode == VisionSensor.VisionSensorMode.Ambient)
        {
            EditorGUILayout.IntField("Ambient", color.Ambient);
        }
        if (color.Mode == VisionSensor.VisionSensorMode.RGBRaw)
        {
            EditorGUILayout.Vector3Field("RGB", color.RGB);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        if (color.Mode == VisionSensor.VisionSensorMode.Color)
        {
            EditorGUILayout.PropertyField(IdChanged);
            EditorGUILayout.PropertyField(ColorChanged);
        }
        if (color.Mode == VisionSensor.VisionSensorMode.Reflection)
        {
            EditorGUILayout.PropertyField(ReflectionChanged);
        }
        if (color.Mode == VisionSensor.VisionSensorMode.Ambient)
        {
            EditorGUILayout.PropertyField(AmbientChanged);
        }
        if (color.Mode == VisionSensor.VisionSensorMode.RGBRaw)
        {
            EditorGUILayout.PropertyField(RGBChanged);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
