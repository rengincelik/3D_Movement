

namespace Movement.Assets._PlayerMovement.Editor
{
    using UnityEngine;
    using UnityEditor;
	using Movement.Assets._PlayerMovement.Scripts;

    [CustomPropertyDrawer(typeof(ForceConfig))]
    public class ForceConfigDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            rect.y += lineHeight + spacing;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var movementConfigProp = property.FindPropertyRelative("movementConfig");
                var directionTypeProp = property.FindPropertyRelative("directionType");
                var magnitudeProp = property.FindPropertyRelative("forceMagnatiute");
                var forceModeProp = property.FindPropertyRelative("ForceMode");

                // Movement Config (nested drawer)
                EditorGUI.PropertyField(rect, movementConfigProp, true);
                rect.y += EditorGUI.GetPropertyHeight(movementConfigProp) + spacing;

                // Get movement type to conditionally show fields
                var categoryProp = movementConfigProp.FindPropertyRelative("movementCategory");
                var linearTypeProp = movementConfigProp.FindPropertyRelative("LinearMovementType");
                var rotationTypeProp = movementConfigProp.FindPropertyRelative("RotationMovementType");

                MovementCategory category = (MovementCategory)categoryProp.enumValueIndex;
                LinearMovementType linearType = (LinearMovementType)linearTypeProp.enumValueIndex;
                RotationMovementType rotationType = (RotationMovementType)rotationTypeProp.enumValueIndex;

                // Check if we need DirectionType (only for non-Axis2D inputs, but we don't have access here)
                // Show DirectionType always, but note it's only used for Button/Axis1D
                EditorGUI.PropertyField(rect, directionTypeProp, new GUIContent("Direction (Button/Axis1D)"));
                rect.y += lineHeight + spacing;

                // Magnitude with warning
                float magnitude = magnitudeProp.floatValue;
                if (magnitude < 0.1f && magnitude > -0.1f)
                {
                    GUI.color = new Color(1f, 1f, 0.5f); // Yellow tint
                }
                EditorGUI.PropertyField(rect, magnitudeProp);
                GUI.color = Color.white;
                rect.y += lineHeight + spacing;

                if (magnitude < 0.1f && magnitude > -0.1f)
                {
                    EditorGUI.HelpBox(rect, "Magnitude is very small, movement might be barely noticeable.", MessageType.Info);
                    rect.y += lineHeight * 2 + spacing;
                }

                // ForceMode (conditional based on movement type)
                bool needsForceMode = false;
                if (category == MovementCategory.Linear)
                {
                    needsForceMode = linearType == LinearMovementType.AddForce ||
                                   linearType == LinearMovementType.AddRelativeForce;
                }
                else if (category == MovementCategory.Rotation)
                {
                    needsForceMode = rotationType == RotationMovementType.AddTorque;
                }

                if (needsForceMode)
                {
                    EditorGUI.PropertyField(rect, forceModeProp);
                    rect.y += lineHeight + spacing;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float height = lineHeight + spacing; // Foldout

            var movementConfigProp = property.FindPropertyRelative("movementConfig");
            var magnitudeProp = property.FindPropertyRelative("forceMagnatiute");

            height += EditorGUI.GetPropertyHeight(movementConfigProp) + spacing; // MovementConfig
            height += lineHeight + spacing; // DirectionType
            height += lineHeight + spacing; // Magnitude

            // Warning for small magnitude
            float magnitude = magnitudeProp.floatValue;
            if (magnitude < 0.1f && magnitude > -0.1f)
            {
                height += lineHeight * 2 + spacing;
            }

            // ForceMode (conditional)
            var categoryProp = movementConfigProp.FindPropertyRelative("movementCategory");
            var linearTypeProp = movementConfigProp.FindPropertyRelative("LinearMovementType");
            var rotationTypeProp = movementConfigProp.FindPropertyRelative("RotationMovementType");

            MovementCategory category = (MovementCategory)categoryProp.enumValueIndex;
            bool needsForceMode = false;

            if (category == MovementCategory.Linear)
            {
                LinearMovementType linearType = (LinearMovementType)linearTypeProp.enumValueIndex;
                needsForceMode = linearType == LinearMovementType.AddForce ||
                               linearType == LinearMovementType.AddRelativeForce;
            }
            else if (category == MovementCategory.Rotation)
            {
                RotationMovementType rotationType = (RotationMovementType)rotationTypeProp.enumValueIndex;
                needsForceMode = rotationType == RotationMovementType.AddTorque;
            }

            if (needsForceMode)
            {
                height += lineHeight + spacing;
            }

            return height;
        }
    }

}
