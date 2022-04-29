using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ForceSensor))]
public class ForceSensorInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty ForceChanged;
    SerializedProperty TouchChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        ForceChanged = serializedObject.FindProperty("ForceChanged");
        TouchChanged = serializedObject.FindProperty("TouchChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ForceSensor force = serializedObject.targetObject as ForceSensor;

        var newMode = (ForceSensor.ForceSensorMode)EditorGUILayout.EnumPopup("Mode", force.Mode);
        if (newMode != force.Mode)
        {
            Undo.RegisterCompleteObjectUndo(force, "Changed force sensor mode");
            force.Mode = newMode;
        }

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", force.IsConnected);
        if (force.Mode == ForceSensor.ForceSensorMode.Force)
        {
            EditorGUILayout.IntField("Force", force.Force);
        }
        if (force.Mode == ForceSensor.ForceSensorMode.Touch)
        {
            EditorGUILayout.Toggle("Touch", force.Touch);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        if (force.Mode == ForceSensor.ForceSensorMode.Force)
        {
            EditorGUILayout.PropertyField(ForceChanged);
        }
        if (force.Mode == ForceSensor.ForceSensorMode.Touch)
        {
            EditorGUILayout.PropertyField(TouchChanged);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
