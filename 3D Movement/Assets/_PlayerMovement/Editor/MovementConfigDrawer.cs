

namespace Movement.Assets._PlayerMovement.Scripts.Editor
{
    using UnityEngine;
    using UnityEditor;
	using Movement.Assets._PlayerMovement.Scripts;

    [CustomPropertyDrawer(typeof(MovementConfig))]
    public class MovementConfigDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var categoryProp = property.FindPropertyRelative("movementCategory");
            var linearTypeProp = property.FindPropertyRelative("LinearMovementType");
            var rotationTypeProp = property.FindPropertyRelative("RotationMovementType");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

            // Movement Category
            EditorGUI.PropertyField(rect, categoryProp);
            rect.y += lineHeight + spacing;

            // Conditional display based on category
            MovementCategory category = (MovementCategory)categoryProp.enumValueIndex;

            if (category == MovementCategory.Linear)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, linearTypeProp, new GUIContent("Linear Type"));
                EditorGUI.indentLevel--;
            }
            else if (category == MovementCategory.Rotation)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, rotationTypeProp, new GUIContent("Rotation Type"));
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            return 2 * (lineHeight + spacing); // Category + Type
        }
    }



}


