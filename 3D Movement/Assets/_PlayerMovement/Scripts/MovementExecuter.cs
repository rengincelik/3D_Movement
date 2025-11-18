using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement.Assets._PlayerMovement.Scripts
{
    public enum MovementType
    {
        Velocity,        // Arcade/Responsive
        AddForce,        // Fiziksel/Realistic
        AddRelativeForce,// Lokal yön
        MovePosition,    // Kinematic-like
        VelocityChange,   // Instant, mass ignore
        Transform,       // Direkt set
        AngularVelocity, // Fiziksel
        AddTorque,       // Force ile
        LookRotation     // Hareket yönüne bak
    }

    public enum CameraPosition {FirstPerson, ThirdPerson, TopDown }


    [Serializable]
    public class ForceConfig
    {
        public MovementType MovementType;
        public Vector3 ForceDirection;
        public ForceMode ForceMode;
        public bool LookMouse;



    }

    [Serializable]
    public class UserInput
    {
        [Tooltip("only button type action ")]
        public InputActionReference action;


    }

    [Serializable]
    public class InputForceBridge
    {
        public UserInput playerInput;
        public ForceConfig forceConfig;
        public bool IsValid => playerInput != null && forceConfig != null;
        public CameraPosition cameraPosition;

    }




}


