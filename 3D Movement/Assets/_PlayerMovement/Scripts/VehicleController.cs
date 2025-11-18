using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement.Assets._PlayerMovement.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleMovementController : MonoBehaviour
    {
        public Rigidbody rb;
        public List<InputForceBridge> inputMovementBridges;

        // public VehicleStateMachine stateMachine;
        private List<InputForceBridge> holdBridges = new();
        private List<InputForceBridge> triggerBridges = new();
        private HashSet<InputForceBridge> activeHoldBridges = new();

        public event Action<string> OnCollided;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            foreach (var bridge in inputMovementBridges)
            {
                if (!bridge.IsValid) continue;

                InputActionReference action = bridge.playerInput.action;
                action.action.Enable();


                bool requiresHold = action.action.interactions != null &&
                                    action.action.interactions.ToLower().Contains("hold");

                if (requiresHold)
                {
                    holdBridges.Add(bridge);

                    action.action.started += ctx => activeHoldBridges.Add(bridge);
                    action.action.canceled += ctx => activeHoldBridges.Remove(bridge);
                }
                else
                {
                    triggerBridges.Add(bridge);
                    action.action.performed += ctx => ApplyForce(bridge);
                }

            }

            // stateMachine = new VehicleStateMachine(this);
            // stateMachine.Enter();
        }

        private void FixedUpdate()
        {
            // basılı tuşlar için sürekli kuvvet uygula
            foreach (var bridge in activeHoldBridges)
                ApplyForce(bridge);
            // stateMachine.Update();
        }

        private void ApplyForce(InputForceBridge bridge)
        {
            ForceConfig force=bridge.forceConfig;
            switch (force.MovementType)
			{
				case MovementType.AddForce:
                rb.AddForce(force.ForceDirection, force.ForceMode);break;
                case MovementType.AddTorque:
                rb.AddTorque(force.ForceDirection, force.ForceMode);break;

			}



        }


        private void OnDisable()
        {
            foreach (var bridge in inputMovementBridges)
            {
                var action = bridge.playerInput.action.action;

                // 2. Eylemi devre dışı bırak
                action.Disable();
            }

            holdBridges.Clear();
            triggerBridges.Clear();
            activeHoldBridges.Clear();
        }


        public void OnCollisionEnter(Collision other)
        {
            OnCollided?.Invoke(other.collider.name);
            Debug.Log($"colliden {other.collider.name}");
        }


    }


}
