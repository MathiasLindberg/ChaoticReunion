using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TemperatureSensor))]
public class TemperatureSensorInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty TemperatureChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        TemperatureChanged = serializedObject.FindProperty("TemperatureChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Status", EditorStyles.boldLabel);
        TemperatureSensor temp = serializedObject.targetObject as TemperatureSensor;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", temp.IsConnected);
        EditorGUILayout.FloatField("Temperature", temp.Temperature);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        EditorGUILayout.PropertyField(TemperatureChanged);

        serializedObject.ApplyModifiedProperties();
    }
}
