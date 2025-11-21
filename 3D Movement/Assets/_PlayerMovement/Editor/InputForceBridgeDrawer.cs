

namespace Movement.Assets._PlayerMovement.Editor
{
    using UnityEngine;
    using UnityEditor;
	using Movement.Assets._PlayerMovement.Scripts;

    [CustomPropertyDrawer(typeof(InputForceBridge))]
    public class InputForceBridgeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

            // Foldout with summary
            var playerInputProp = property.FindPropertyRelative("playerInput");
            var forceConfigProp = property.FindPropertyRelative("forceConfig");
            var cameraConfigProp = property.FindPropertyRelative("cameraConfig");

            string summary = GetSummary(playerInputProp, forceConfigProp, cameraConfigProp);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, $"{label.text} {summary}", true);
            rect.y += lineHeight + spacing;


            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Validation errors
                var actionProp = playerInputProp.FindPropertyRelative("action");
                bool hasErrors = actionProp.objectReferenceValue == null;

                if (hasErrors)
                {
                    EditorGUI.HelpBox(rect, "Input Action is not assigned!", MessageType.Error);
                    rect.y += lineHeight * 2 + spacing;
                }

                // User Input
                EditorGUI.PropertyField(rect, playerInputProp, true);
                rect.y += EditorGUI.GetPropertyHeight(playerInputProp) + spacing;

                // Force Config
                EditorGUI.PropertyField(rect, forceConfigProp, true);
                rect.y += EditorGUI.GetPropertyHeight(forceConfigProp) + spacing;

                // Camera Config
                EditorGUI.PropertyField(rect, cameraConfigProp, true);
                rect.y += EditorGUI.GetPropertyHeight(cameraConfigProp) + spacing;

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private string GetSummary(SerializedProperty playerInput, SerializedProperty forceConfig, SerializedProperty cameraConfig)
        {
            var actionProp = playerInput.FindPropertyRelative("action");
            if (actionProp.objectReferenceValue == null) return "(Not Configured)";

            var movementConfigProp = forceConfig.FindPropertyRelative("movementConfig");
            var categoryProp = movementConfigProp.FindPropertyRelative("movementCategory");
            var magnitudeProp = forceConfig.FindPropertyRelative("forceMagnatiute");

            string actionName = actionProp.objectReferenceValue.name;
            string category = ((MovementCategory)categoryProp.enumValueIndex).ToString();
            float magnitude = magnitudeProp.floatValue;

            return $"[{actionName}] → {category} ({magnitude:F1})";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float height = lineHeight + spacing; // Foldout

            var playerInputProp = property.FindPropertyRelative("playerInput");
            var forceConfigProp = property.FindPropertyRelative("forceConfig");
            var cameraConfigProp = property.FindPropertyRelative("cameraConfig");

            // Error box
            var actionProp = playerInputProp.FindPropertyRelative("action");
            if (actionProp.objectReferenceValue == null)
            {
                height += lineHeight * 2 + spacing;
            }

            height += EditorGUI.GetPropertyHeight(playerInputProp) + spacing;
            height += EditorGUI.GetPropertyHeight(forceConfigProp) + spacing;
            height += EditorGUI.GetPropertyHeight(cameraConfigProp) + spacing;

            return height;
        }
    }

}
