using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Movement.Assets._PlayerMovement.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementController : MonoBehaviour
    {
        public Rigidbody rb;
        public List<InputForceBridge> inputMovementBridges;
        public bool EditBridge = false;

        private readonly List<InputForceBridge> holdBridges = new();
        private readonly List<InputForceBridge> triggerBridges = new();
        private readonly HashSet<InputForceBridge> activeHoldBridges = new();

        private readonly Dictionary<InputForceBridge, Action<InputAction.CallbackContext>> performedCache = new();
        private readonly Dictionary<InputForceBridge, Action<InputAction.CallbackContext>> startedCache = new();
        private readonly Dictionary<InputForceBridge, Action<InputAction.CallbackContext>> canceledCache = new();

        private void Awake()
        {
            rb = rb == null ? GetComponent<Rigidbody>() : rb;
            BuildBridges();
        }

        private void FixedUpdate()
        {
            if (EditBridge)
            {
                RebuildBridges();
                EditBridge = false;
            }

            foreach (var bridge in activeHoldBridges)
                ApplyForce(bridge);
        }
    private void ApplyForce(InputForceBridge bridge)
    {
        var force = bridge.forceConfig;
        switch (force.movementConfig.movementCategory)
        {
            case MovementCategory.Linear:
                ApplyLinearForce(bridge);
                break;
            case MovementCategory.Rotation:
                ApplyRotationForce(bridge);
                break;
        }
    }

    private void ApplyLinearForce(InputForceBridge bridge)
    {
        var force = bridge.forceConfig;
        Vector3 direction = bridge.GetDirection();

        switch (force.movementConfig.LinearMovementType)
        {
            case LinearMovementType.Velocity:
            case LinearMovementType.VelocityChange:
                rb.linearVelocity = direction;
                break;
            case LinearMovementType.AddForce:
                rb.AddForce(direction, force.ForceMode);
                break;
            case LinearMovementType.AddRelativeForce:
                rb.AddRelativeForce(direction, force.ForceMode);
                break;
            case LinearMovementType.MovePosition:
                rb.MovePosition(rb.position + direction * Time.fixedDeltaTime);
                break;
            case LinearMovementType.Transform:
                transform.position += direction * Time.deltaTime;
                break;
        }
    }

    private void ApplyRotationForce(InputForceBridge bridge)
    {
        var force = bridge.forceConfig;
        Vector3 direction = bridge.GetDirection();

        switch (force.movementConfig.RotationMovementType)
        {
            case RotationMovementType.AngularVelocity:
                rb.angularVelocity = direction;
                break;
            case RotationMovementType.AddTorque:
                rb.AddTorque(direction, force.ForceMode);
                break;
            case RotationMovementType.LookRotation:
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);
                break;
        }
    }


        private void BuildBridges()
        {
            holdBridges.Clear();
            triggerBridges.Clear();
            activeHoldBridges.Clear();
            performedCache.Clear();
            startedCache.Clear();
            canceledCache.Clear();

            foreach (var bridge in inputMovementBridges)
            {
                if (!bridge.IsValid) continue;

                var action = bridge.playerInput.action.action;
                action.Enable();

                bool requiresHold = action.interactions != null &&
                                    action.interactions.ToLower().Contains("hold");

                var performed = new Action<InputAction.CallbackContext>(_ => ApplyForce(bridge));
                var started = new Action<InputAction.CallbackContext>(_ => activeHoldBridges.Add(bridge));
                var canceled = new Action<InputAction.CallbackContext>(_ => activeHoldBridges.Remove(bridge));

                performedCache[bridge] = performed;
                startedCache[bridge] = started;
                canceledCache[bridge] = canceled;

                if (requiresHold)
                {
                    holdBridges.Add(bridge);
                    action.started += started;
                    action.canceled += canceled;
                }
                else
                {
                    triggerBridges.Add(bridge);
                    action.performed += performed;
                }
            }
        }

        private void RebuildBridges()
        {
            foreach (var bridge in inputMovementBridges)
            {
                if (!performedCache.ContainsKey(bridge)) continue;

                var action = bridge.playerInput.action.action;
                action.started -= startedCache[bridge];
                action.canceled -= canceledCache[bridge];
                action.performed -= performedCache[bridge];
            }

            BuildBridges();
        }

        private void OnDisable()
        {
            foreach (var bridge in inputMovementBridges)
            {
                if (!performedCache.ContainsKey(bridge)) continue;

                var action = bridge.playerInput.action.action;
                action.started -= startedCache[bridge];
                action.canceled -= canceledCache[bridge];
                action.performed -= performedCache[bridge];
                action.Disable();
            }
        }



    }

}



