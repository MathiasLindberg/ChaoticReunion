using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Buttons))]
public class ButtonsInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty PlusChanged;
    SerializedProperty StopChanged;
    SerializedProperty MinusChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        PlusChanged = serializedObject.FindProperty("PlusChanged");
        StopChanged = serializedObject.FindProperty("StopChanged");
        MinusChanged = serializedObject.FindProperty("MinusChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Buttons btns = serializedObject.targetObject as Buttons;

        if (btns.port == 0 || btns.port == 1)
        {
            EditorGUILayout.LabelField(btns.port == 0 ? "LEFT" : "RIGHT");
        }

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", btns.IsConnected);
        EditorGUILayout.Toggle("Plus pressed", btns.PlusPressed);
        EditorGUILayout.Toggle("Stop pressed", btns.StopPressed);
        EditorGUILayout.Toggle("Minus pressed", btns.MinusPressed);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        EditorGUILayout.PropertyField(PlusChanged);
        EditorGUILayout.PropertyField(StopChanged);
        EditorGUILayout.PropertyField(MinusChanged);

        serializedObject.ApplyModifiedProperties();
    }
}
