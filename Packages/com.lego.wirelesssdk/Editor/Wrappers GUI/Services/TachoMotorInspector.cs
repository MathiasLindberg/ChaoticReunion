using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TachoMotor))]
public class TachoMotorInspector : Editor
{
    SerializedProperty IsConnectedChanged;
    SerializedProperty PositionChanged;
    SerializedProperty SpeedChanged;
    SerializedProperty PowerChanged;

    void OnEnable()
    {
        IsConnectedChanged = serializedObject.FindProperty("IsConnectedChanged");
        PositionChanged = serializedObject.FindProperty("PositionChanged");
        SpeedChanged = serializedObject.FindProperty("SpeedChanged");
        PowerChanged = serializedObject.FindProperty("PowerChanged");
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TachoMotor motor = serializedObject.targetObject as TachoMotor;

        EditorGUI.BeginDisabledGroup(motor.isInternal);

        // Removed in the name of simplification.
        /*Enum e;
        if (motor.typeBitMask == 0)
        {
            Rect fieldRect = EditorGUILayout.GetControlRect(hasLabel: true, height: EditorGUIUtility.singleLineHeight, style: EditorStyles.colorField);
            Rect highlightRect = new Rect(
                    x: fieldRect.x + EditorGUIUtility.labelWidth,
                    y: fieldRect.y - 2,
                    width: fieldRect.width + 2 - EditorGUIUtility.labelWidth,
                    height: fieldRect.height + 2 * 2);
            EditorGUI.DrawRect(highlightRect, Color.red);
            e = EditorGUI.EnumPopup(fieldRect, "Motor Types", (TachoMotor.TachoMotorIOTypesWithoutClear)0);
        }
        else
        {
            e = EditorGUILayout.EnumFlagsField("Motor Types", (TachoMotor.TachoMotorIOTypes)motor.typeBitMask);
        }
        if ((int)(TachoMotor.TachoMotorIOTypes)e != motor.typeBitMask)
        {
            Undo.RegisterCompleteObjectUndo(motor, "Changed tacho motor type");
            motor.typeBitMask = (int)(TachoMotor.TachoMotorIOTypes)e;
        }*/


        // Removed in the name of simplification.
        /*var newVirtual = EditorGUILayout.Toggle("Virtual", motor.virtualConn);
        if (newVirtual != motor.virtualConn)
        {
            Undo.RegisterCompleteObjectUndo(motor, "Changed tacho motor virtual");
            motor.virtualConn = newVirtual;
        }*/

        EditorGUI.EndDisabledGroup();

        var newMode = (TachoMotor.TachoMotorMode)EditorGUILayout.EnumPopup("Mode", motor.Mode);
        if (newMode != motor.Mode)
        {
            Undo.RegisterCompleteObjectUndo(motor, "Changed motor mode");
            motor.Mode = newMode;
        }

        GUILayout.Space(10);
        GUILayout.Label("Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Connected", motor.IsConnected);
        if (motor.Mode == TachoMotor.TachoMotorMode.AbsolutePosition || motor.Mode == TachoMotor.TachoMotorMode.Position || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
        {
            EditorGUILayout.IntField("Position", motor.Position);
        }
        if (motor.Mode == TachoMotor.TachoMotorMode.Speed || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
        {
            EditorGUILayout.IntField("Speed", motor.Speed);

        }
        if (motor.Mode == TachoMotor.TachoMotorMode.Power)
        {
            EditorGUILayout.IntField("Power", motor.Power);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(IsConnectedChanged);
        if (motor.Mode == TachoMotor.TachoMotorMode.AbsolutePosition || motor.Mode == TachoMotor.TachoMotorMode.Position || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
        {
            EditorGUILayout.PropertyField(PositionChanged);
        }
        if (motor.Mode == TachoMotor.TachoMotorMode.Speed || motor.Mode == TachoMotor.TachoMotorMode.SpeedAndPosition)
        {
            EditorGUILayout.PropertyField(SpeedChanged);
        }
        if (motor.Mode == TachoMotor.TachoMotorMode.Power)
        {
            EditorGUILayout.PropertyField(PowerChanged);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
