using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorSensorTechnic))]
public class ColorSensorTechnicInspector : Editor
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
        ColorSensorTechnic color = serializedObject.targetObject as ColorSensorTechnic;

        var newMode = (ColorSensorTechnic.ColorSensorTechnicMode)EditorGUILayout.EnumPopup("Mode", color.Mode);
        if (newMode != color.Mode)
        {
            Undo.RegisterCompleteObjectUndo(color, "Changed color sensor mode");
            color.Mode = newMode;
        }

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", color.IsConnected);
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.Color)
        {
            EditorGUILayout.IntField("Id", color.Id);
            EditorGUILayout.ColorField("Color", color.Color);
        }
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.Reflection)
        {
            EditorGUILayout.IntField("Reflection", color.Reflection);
        }
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.Ambient)
        {
            EditorGUILayout.IntField("Ambient", color.Ambient);
        }
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.RGBI)
        {
            EditorGUILayout.Vector3Field("RGB", color.RGB);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.Color)
        {
            EditorGUILayout.PropertyField(IdChanged);
            EditorGUILayout.PropertyField(ColorChanged);
        }
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.Reflection)
        {
            EditorGUILayout.PropertyField(ReflectionChanged);
        }
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.Ambient)
        {
            EditorGUILayout.PropertyField(AmbientChanged);
        }
        if (color.Mode == ColorSensorTechnic.ColorSensorTechnicMode.RGBI)
        {
            EditorGUILayout.PropertyField(RGBChanged);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
