using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DistanceSensor))]
public class DistanceSensorInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty DistanceChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        DistanceChanged = serializedObject.FindProperty("DistanceChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Status", EditorStyles.boldLabel);
        DistanceSensor dist = serializedObject.targetObject as DistanceSensor;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", dist.IsConnected);
        EditorGUILayout.FloatField("Distance", dist.Distance);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        EditorGUILayout.PropertyField(DistanceChanged);

        serializedObject.ApplyModifiedProperties();
    }
}
