using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement.Assets._PlayerMovement.Scripts
{
    public enum DirectionType {Up, Down, Left, Right, Forward, Back}
    public enum InputType
    {
        Button,   // Space, E, Shift → bool/float
        Axis2D,   // WASD, Joystick → Vector2
        Axis1D,   // Mouse scroll → float
        Mouse     // Mouse delta → Vector2
    }
    public enum CameraType { FirstPerson, ThirdPerson, TopDown}
    public enum MovementCategory { Linear, Rotation }
    public enum LinearMovementType
    {
        Velocity,
        AddForce,
        AddRelativeForce,
        MovePosition,
        VelocityChange,
        Transform
    }

    public enum RotationMovementType
    {
        AngularVelocity,
        AddTorque,
        LookRotation
    }

    [Serializable]
    public class CameraConfig
    {
        public bool useMainCamera = true;
        public Camera camera;
        public CameraType cameraType;
    }
    [Serializable]
    public class MovementConfig
	{
		public MovementCategory movementCategory;
        public LinearMovementType LinearMovementType;
        public RotationMovementType RotationMovementType;
	}

    [Serializable]
    public class ForceConfig
    {
        public MovementConfig movementConfig;
        public DirectionType directionType;
        public float forceMagnatiute;
        public ForceMode ForceMode;
        Vector3 CalculateForceDirection()
        {
            switch (directionType)
            {
                case DirectionType.Forward:
                    return Vector3.forward; // (0,0,1)
                case DirectionType.Back:
                    return Vector3.back;    // (0,0,-1)
                case DirectionType.Right:
                    return Vector3.right;   // (1,0,0)
                case DirectionType.Left:
                    return Vector3.left;    // (-1,0,0)
                case DirectionType.Up:
                    return Vector3.up;      // (0,1,0)
                case DirectionType.Down:
                    return Vector3.down;    // (0,-1,0)
                default:
                    return Vector3.zero;
            }
        }
        public Vector3 ForceDirection => CalculateForceDirection()*forceMagnatiute;


    }

    [Serializable]
    public class UserInput
    {
        public InputActionReference action;

        private InputType? cachedType;

        public InputType GetInputType()
        {
            if (cachedType.HasValue)
                return cachedType.Value;

            if (action?.action == null)
                return InputType.Button;

            cachedType = action.action.expectedControlType switch
            {
                "Vector2" => InputType.Axis2D,
                "Button" => InputType.Button,
                "Axis" => InputType.Axis1D,
                _ => InputType.Button
            };

            return cachedType.Value;
        }

        public Vector2 GetVector2Value()
            => action?.action.ReadValue<Vector2>() ?? Vector2.zero;

        public float GetFloatValue()
            => action?.action.ReadValue<float>() ?? 0f;

    }


    [Serializable]
    public class InputForceBridge
    {
        public UserInput playerInput;
        public ForceConfig forceConfig;
        public bool IsValid => playerInput != null && forceConfig != null;
        public CameraConfig cameraConfig;
        public Vector3 GetDirection()
        {
            if (!IsValid)
                return Vector3.zero;

            return CalculateFinalDirection(cameraConfig.cameraType, CalculateInputDirection());

        }

        Vector3 CalculateInputDirection()
        {
            float magnatiute=forceConfig.forceMagnatiute;
            switch (playerInput.GetInputType())
            {
                case InputType.Axis2D:
                    Vector2 v2 = playerInput.GetVector2Value();
                    return new Vector3(v2.x*magnatiute, 0, v2.y*magnatiute);

                case InputType.Button:
                    float b = playerInput.GetFloatValue();
                    return forceConfig.ForceDirection;


                case InputType.Axis1D:
                    float a = playerInput.GetFloatValue();

                    return forceConfig.ForceDirection;

                case InputType.Mouse:
                    Vector2 m = playerInput.GetVector2Value();
                    return new Vector3(m.x*magnatiute, 0, m.y*magnatiute);

                default:
                    return Vector3.zero;
            }
        }

        Vector3 CalculateFinalDirection(CameraType t, Vector3 dir)
		{
			switch (t)
            {
                case CameraType.FirstPerson:
                    return cameraConfig.camera.transform.TransformDirection(dir);

                case CameraType.ThirdPerson:
                    return cameraConfig.camera.transform.TransformDirection(dir);


                case CameraType.TopDown:
                    return cameraConfig.camera.transform.TransformDirection(dir);


                default:
                    return dir;
            }
		}


    }




}


