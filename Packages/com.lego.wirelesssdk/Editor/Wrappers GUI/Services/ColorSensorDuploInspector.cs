using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorSensorDuplo))]
public class ColorSensorDuploInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty IdChanged;
    SerializedProperty ColorChanged;
    SerializedProperty ReflectionChanged;
    SerializedProperty RGBChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        IdChanged = serializedObject.FindProperty("IdChanged");
        ColorChanged = serializedObject.FindProperty("ColorChanged");
        ReflectionChanged = serializedObject.FindProperty("ReflectionChanged");
        RGBChanged = serializedObject.FindProperty("RGBChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ColorSensorDuplo color = serializedObject.targetObject as ColorSensorDuplo;

        var newMode = (ColorSensorDuplo.ColorSensorDuploMode)EditorGUILayout.EnumPopup("Mode", color.Mode);
        if (newMode != color.Mode)
        {
            Undo.RegisterCompleteObjectUndo(color, "Changed color sensor mode");
            color.Mode = newMode;
        }

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", color.IsConnected);
        if (color.Mode == ColorSensorDuplo.ColorSensorDuploMode.Color)
        {
            EditorGUILayout.IntField("Id", color.Id);
            EditorGUILayout.ColorField("Color", color.Color);
        }
        if (color.Mode == ColorSensorDuplo.ColorSensorDuploMode.Reflection)
        {
            EditorGUILayout.IntField("Reflection", color.Reflection);
        }
        if (color.Mode == ColorSensorDuplo.ColorSensorDuploMode.RGBRaw)
        {
            EditorGUILayout.Vector3Field("RGB", color.RGB);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        if (color.Mode == ColorSensorDuplo.ColorSensorDuploMode.Color)
        {
            EditorGUILayout.PropertyField(IdChanged);
            EditorGUILayout.PropertyField(ColorChanged);
        }
        if (color.Mode == ColorSensorDuplo.ColorSensorDuploMode.Reflection)
        {
            EditorGUILayout.PropertyField(ReflectionChanged);
        }
        if (color.Mode == ColorSensorDuplo.ColorSensorDuploMode.RGBRaw)
        {
            EditorGUILayout.PropertyField(RGBChanged);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
