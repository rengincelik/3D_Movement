using System;
using UnityEngine;

namespace Movement.Assets._PlayerMovement.Scripts
{

    public enum EnvironmentType { Land, Water, Ice, Grass, Road, Air }

    public class EnvironmentHandler : MonoBehaviour
    {
        [Header("Raycast Settings")]
        public float rayDistance = 0.2f;
        public LayerMask groundLayers;

        public EnvironmentType CurrentEnvironment { get; private set; } = EnvironmentType.Air;
        public bool IsGrounded { get; private set; } = false;

        private void FixedUpdate()
        {
            CheckGround();
        }

        private void CheckGround()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayers))
            {
                IsGrounded = true;

                // Ground layer veya tag'a göre environment belirleme
                switch (hit.collider.tag)
                {
                    case "Land":
                        CurrentEnvironment = EnvironmentType.Land;
                        break;
                    case "Water":
                        CurrentEnvironment = EnvironmentType.Water;
                        break;
                    case "Ice":
                        CurrentEnvironment = EnvironmentType.Ice;
                        break;
                    case "Grass":
                        CurrentEnvironment = EnvironmentType.Grass;
                        break;
                    case "Road":
                        CurrentEnvironment = EnvironmentType.Road;
                        break;
                    default:
                        CurrentEnvironment = EnvironmentType.Land;
                        break;
                }
            }
            else
            {
                IsGrounded = false;
                CurrentEnvironment = EnvironmentType.Air;
            }
        }

        // Opsiyonel event sistemi
        public event Action<EnvironmentType> OnEnvironmentChanged;

        private EnvironmentType lastEnvironment;
        private void LateUpdate()
        {
            if (CurrentEnvironment != lastEnvironment)
            {
                OnEnvironmentChanged?.Invoke(CurrentEnvironment);
                lastEnvironment = CurrentEnvironment;
            }
        }
    }



}

