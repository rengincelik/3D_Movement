using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movement.Assets._PlayerMovement.Editor
{
    // MovementConfigDrawer.cs
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(MovementConfig))]
    public class MovementConfigDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float padding = 2f;

            Rect categoryRect = new Rect(position.x, position.y, position.width, lineHeight);
            SerializedProperty categoryProp = property.FindPropertyRelative("movementCategory");
            EditorGUI.PropertyField(categoryRect, categoryProp);

            if ((MovementCategory)categoryProp.enumValueIndex == MovementCategory.Linear)
            {
                Rect linearRect = new Rect(position.x, position.y + lineHeight + padding, position.width, lineHeight);
                EditorGUI.PropertyField(linearRect, property.FindPropertyRelative("linearMovementType"));
            }
            else
            {
                Rect rotRect = new Rect(position.x, position.y + lineHeight + padding, position.width, lineHeight);
                EditorGUI.PropertyField(rotRect, property.FindPropertyRelative("rotationMovementType"));
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty categoryProp = property.FindPropertyRelative("movementCategory");
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float padding = 2f;

            return (categoryProp.enumValueIndex == (int)MovementCategory.Linear ? 2 : 2) * lineHeight + padding;
        }
    }

}
