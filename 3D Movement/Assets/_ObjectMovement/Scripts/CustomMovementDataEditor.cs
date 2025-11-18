using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movement.Assets._ObjectMovement.Scripts
{
#if UNITY_EDITOR
using DG.Tweening;
using UnityEditor;

[CustomEditor(typeof(MovementDataSO))]
public class MovementDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MovementDataSO data = (MovementDataSO)target;

        data.doType = (DOType)EditorGUILayout.EnumPopup("Movement Type", data.doType);
        if(data.doType==DOType.Move)
        {
            data.targetValue = EditorGUILayout.Vector2Field("Target Value", data.targetValue);
        }
        if(data.doType==DOType.MoveX)
        {
            data.toEnd = EditorGUILayout.FloatField("toEnd", data.toEnd);
        }
        if(data.doType==DOType.MoveY)
        {
            data.toEnd = EditorGUILayout.FloatField("toEnd", data.toEnd);
        }
        if(data.doType==DOType.Jump)
        {
            data.jumpPower = EditorGUILayout.FloatField("Jump Power", data.jumpPower);
            data.jumpCount = EditorGUILayout.IntField("Jump Count", data.jumpCount);
        }
        if (data.doType == DOType.Rotate)
        {
            data.toEnd = EditorGUILayout.FloatField("toEnd", data.toEnd);
        }
        if (data.doType == DOType.Path)
        {
            SerializedProperty pathPointsProp = serializedObject.FindProperty("pathPoints");
            EditorGUILayout.PropertyField(pathPointsProp, true);
            data.pathType = (PathType)EditorGUILayout.EnumPopup("Path Type", data.pathType);
            data.pathMode = (PathMode)EditorGUILayout.EnumPopup("Path Mode", data.pathMode);
        }

        if (data.doType == DOType.LocalPath)
        {
            SerializedProperty pathPointsProp = serializedObject.FindProperty("pathPoints");
            EditorGUILayout.PropertyField(pathPointsProp, true);
            data.pathType = (PathType)EditorGUILayout.EnumPopup("Path Type", data.pathType);
            data.pathMode = (PathMode)EditorGUILayout.EnumPopup("Path Mode", data.pathMode);
        }




        data.duration = EditorGUILayout.FloatField("Duration", data.duration);
        data.ease = (Ease)EditorGUILayout.EnumPopup("Ease", data.ease);
        data.delay = EditorGUILayout.FloatField("Delay", data.delay);

        data.loopType = (LoopTypeCustom)EditorGUILayout.EnumPopup("Loop Type", data.loopType);
        data.loopCount = EditorGUILayout.IntField("Loop Count", data.loopCount);

        data.useRelativeValues = EditorGUILayout.Toggle("Use Relative Values", data.useRelativeValues);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
#endif

}
