using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AccelerationSensor))]
public class AccelerationSensorInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty AccelerationChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        AccelerationChanged = serializedObject.FindProperty("AccelerationChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Status", EditorStyles.boldLabel);
        AccelerationSensor acc = serializedObject.targetObject as AccelerationSensor;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", acc.IsConnected);
        EditorGUILayout.Vector3Field("Acceleration", acc.Acceleration);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        EditorGUILayout.PropertyField(AccelerationChanged);

        serializedObject.ApplyModifiedProperties();
    }
}
