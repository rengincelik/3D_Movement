
namespace Movement.Assets._PlayerMovement.Editor
{
    using UnityEngine;
    using UnityEditor;
	using Movement.Assets._PlayerMovement.Scripts;

    [CustomPropertyDrawer(typeof(UserInput))]
    public class UserInputDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var actionProp = property.FindPropertyRelative("action");

            // Validation
            bool actionNull = actionProp.objectReferenceValue == null;
            if (actionNull)
            {
                GUI.color = new Color(1f, 0.5f, 0.5f); // Red tint
            }

            EditorGUI.PropertyField(position, actionProp, label);
            GUI.color = Color.white;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

}
