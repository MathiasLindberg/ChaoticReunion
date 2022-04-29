using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrientationSensor))]
public class OrientationSensorInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty OrientationChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        OrientationChanged = serializedObject.FindProperty("OrientationChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Status", EditorStyles.boldLabel);
        OrientationSensor orient = serializedObject.targetObject as OrientationSensor;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", orient.IsConnected);
        EditorGUILayout.Vector3Field("Orientation", orient.Orientation);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        EditorGUILayout.PropertyField(OrientationChanged);

        serializedObject.ApplyModifiedProperties();
    }
}
