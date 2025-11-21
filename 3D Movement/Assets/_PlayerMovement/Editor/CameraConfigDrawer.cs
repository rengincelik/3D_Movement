namespace Movement.Assets._PlayerMovement.Editor
{
    using UnityEngine;
    using UnityEditor;
	using Movement.Assets._PlayerMovement.Scripts;

    [CustomPropertyDrawer(typeof(CameraConfig))]
    public class CameraConfigDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var useMainCameraProp = property.FindPropertyRelative("useMainCamera");
            var cameraProp = property.FindPropertyRelative("camera");
            var cameraTypeProp = property.FindPropertyRelative("cameraType");

            // Check if we need useMainCamera property (add it if not exists)
            bool hasUseMainCamera = useMainCameraProp != null;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

            // Use Main Camera checkbox (if property exists, otherwise add note)
            if (!hasUseMainCamera)
            {
                EditorGUI.LabelField(rect, label.text + " (Add 'public bool useMainCamera' to CameraConfig)", EditorStyles.boldLabel);
                rect.y += lineHeight + spacing;
            }
            else
            {
                EditorGUI.PropertyField(rect, useMainCameraProp, new GUIContent("Use Main Camera"));
                rect.y += lineHeight + spacing;

                // Auto-assign Camera.main in editor
                if (useMainCameraProp.boolValue && !Application.isPlaying)
                {
                    Camera mainCam = Camera.main;
                    if (mainCam != null && cameraProp.objectReferenceValue != mainCam)
                    {
                        cameraProp.objectReferenceValue = mainCam;
                    }
                }
            }

            // Camera field with validation
            bool cameraNull = cameraProp.objectReferenceValue == null;
            if (cameraNull && (!hasUseMainCamera || !useMainCameraProp.boolValue))
            {
                GUI.color = new Color(1f, 0.5f, 0.5f); // Red tint
            }

            EditorGUI.PropertyField(rect, cameraProp);
            GUI.color = Color.white;
            rect.y += lineHeight + spacing;

            // Warning if camera is null
            if (cameraNull && (!hasUseMainCamera || !useMainCameraProp.boolValue))
            {
                EditorGUI.HelpBox(rect, "Camera reference is missing!", MessageType.Warning);
                rect.y += lineHeight * 2 + spacing;
            }

            // Camera Type
            EditorGUI.PropertyField(rect, cameraTypeProp);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            int lines = 3; // useMainCamera + camera + cameraType

            var cameraProp = property.FindPropertyRelative("camera");
            var useMainCameraProp = property.FindPropertyRelative("useMainCamera");

            // Add space for warning
            bool cameraNull = cameraProp.objectReferenceValue == null;
            bool useMainCamera = useMainCameraProp != null && useMainCameraProp.boolValue;

            if (cameraNull && !useMainCamera)
            {
                lines += 2; // HelpBox
            }

            return lines * (lineHeight + spacing);
        }
    }

}
