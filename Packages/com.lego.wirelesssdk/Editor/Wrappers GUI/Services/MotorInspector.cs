using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Motor))]
public class MotorInspector : Editor
{
    SerializedProperty IsConnectedChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Motor motor = serializedObject.targetObject as Motor;

        // Removed in the name of simplification.
        /*Enum e;
        if (motor.typeBitMask == 0)
        {
            Rect fieldRect = MakeBg(Color.red);
            e = EditorGUI.EnumPopup(fieldRect, "Motor Types", (Motor.MotorIOTypesWithoutClear)0);
        }
        else if ((motor.typeBitMask & 7) == 0)
        {
            Rect fieldRect = MakeBg(Color.yellow);
            e = EditorGUI.EnumFlagsField(fieldRect, "Motor Types", (Motor.MotorIOTypes)motor.typeBitMask);
            EditorGUI.LabelField(fieldRect, new GUIContent("", "Only tacho motors selected. Use TachoMotor class for full functionality."));
        }
        else
        {
            e = EditorGUILayout.EnumFlagsField("Motor Types", (Motor.MotorIOTypes)motor.typeBitMask);
        }
        if ((int)(Motor.MotorIOTypes)e != motor.typeBitMask)
        {
            Undo.RegisterCompleteObjectUndo(motor, "Changed motor type");
            motor.typeBitMask = (int)(Motor.MotorIOTypes)e;
        }*/

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", motor.IsConnected);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);

        serializedObject.ApplyModifiedProperties();
    }

    private Rect MakeBg(Color color)
    {
        Rect fieldRect = EditorGUILayout.GetControlRect(hasLabel: true, height: EditorGUIUtility.singleLineHeight, style: EditorStyles.colorField);
        Rect highlightRect = new Rect(
                x: fieldRect.x + EditorGUIUtility.labelWidth,
                y: fieldRect.y - 2,
                width: fieldRect.width + 2 - EditorGUIUtility.labelWidth,
                height: fieldRect.height + 2 * 2);
        EditorGUI.DrawRect(highlightRect, color);
        return fieldRect;
    }
}
