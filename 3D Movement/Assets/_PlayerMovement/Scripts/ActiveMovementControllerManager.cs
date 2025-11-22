using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement.Assets._PlayerMovement.Scripts
{
    /// <summary>
    /// Singleton pattern ile ActiveMovementControllerManager
    /// </summary>

    public class ActiveMovementControllerManager : MonoBehaviour
    {
        public static ActiveMovementControllerManager Instance { get; private set; }
        public GameObject Player;
        public MovementController playerController;
        public MovementController currentController;

        public InputActionReference interactInput;

        private VehicleInteraction currentVehicle;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            if (interactInput != null) interactInput.action.Enable();

            SetActiveController(playerController);
        }
        private void Start()
        {
            if (interactInput != null)
            {
                interactInput.action.Enable();
                Debug.Log($"Input enabled in Start: {interactInput.action.enabled}"); // ← EKLE
            }
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
            {
                Debug.Log("E tuşuna basıldı!");

                currentController?.CustomUpdate();
                CheckInteraction();
            }

            if (keyboard != null && keyboard.eKey.wasReleasedThisFrame)
            {
                Debug.Log("E tuşu bırakıldı!");
            }

            if (interactInput != null && interactInput.action.triggered)
            {
                Debug.Log("E tuşuna basıldı! input system detected");
                // Mount / Dismount işlemi
            }


        }

        private void FixedUpdate()
        {
            currentController?.CustomFixedUpdate();
        }


        private void CheckInteraction()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
            {
                Debug.Log("E tuşuna basıldı!");


            if (currentVehicle != null && currentVehicle.playerInRange && !IsVehicleActive())
            {
                MountVehicle(currentVehicle);
            }
            else if (IsVehicleActive())
            {
                DismountVehicle();
            }
            }
        }

        public void RegisterVehicle(VehicleInteraction vehicle)
        {
            currentVehicle = vehicle;
        }

        public void UnregisterVehicle(VehicleInteraction vehicle)
        {
            if (currentVehicle == vehicle)
                currentVehicle = null;
        }

        private bool IsVehicleActive() => currentController != null && currentController != playerController;

        public void SetActiveController(MovementController controller)
        {
            if (currentController != null) currentController.SetActive(false);
            currentController = controller;
            if (currentController != null) currentController.SetActive(true);
        }

        public void MountVehicle(VehicleInteraction vehicle)
        {
            if (vehicle.playerController == null) return;

            Transform playerTransform = vehicle.playerController.transform;
            Rigidbody playerRb = vehicle.playerController.rb;

            // Player parent
            playerTransform.SetParent(vehicle.mountPoint != null ? vehicle.mountPoint : vehicle.transform);

            // Pozisyon (null check)
            if (vehicle.mountPoint != null)
            {
                playerTransform.localPosition = Vector3.zero; // ← localPosition kullan
                playerTransform.localRotation = Quaternion.identity; // ← localRotation kullan
            }

            // Player physics dondur
            if (playerRb != null)
            {
                playerRb.isKinematic = true; // ← EKLE
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }

            SetActiveController(vehicle.vehicleController);
            Debug.Log("[ActiveManager] Mounted vehicle");
        }

        public void DismountVehicle()
        {
            if (currentController == null || currentController == playerController) return;

            VehicleInteraction vehicle = currentController.GetComponent<VehicleInteraction>();
            if (vehicle == null || vehicle.playerController == null) return;

            Transform playerTransform = vehicle.playerController.transform;
            Rigidbody playerRb = vehicle.playerController.rb;

            // Parent kaldır
            playerTransform.SetParent(null);

            // Exit pozisyon (null check)
            if (vehicle.exitPoint != null)
            {
                playerTransform.position = vehicle.exitPoint.position; // ← position (world space)
                playerTransform.rotation = vehicle.exitPoint.rotation; // ← rotation (world space)
            }
            else
            {
                // Fallback: vehicle'ın sağına
                playerTransform.position = vehicle.transform.position + vehicle.transform.right * 2f;
            }

            // Player physics aktif et
            if (playerRb != null)
            {
                playerRb.isKinematic = false; // ← EKLE
            }

            SetActiveController(playerController);
            Debug.Log("[ActiveManager] Dismounted vehicle");
        }

    }


}

// using UnityEngine;

// namespace Movement.Assets._PlayerMovement.Scripts
// {
//     public class ActiveMovementControllerManager: MonoBehaviour
//     {
//         public MovementController playerController;
//         public MovementController currentController;

//         private void Awake()
//         {
//             // Başlangıçta player aktif
//             SetActiveController(playerController);
//         }

//         public void SetActiveController(MovementController controller)
//         {
//             if (currentController != null)
//                 currentController.SetActive(false); // pasif yap

//             currentController = controller;

//             if (currentController != null)
//                 currentController.SetActive(true);  // aktif yap
//         }

//         private void Update()
//         {
//             currentController?.CustomUpdate();   // MovementController içinde Update yerine CustomUpdate
//         }

//         private void FixedUpdate()
//         {
//             currentController?.CustomFixedUpdate(); // MovementController içinde FixedUpdate yerine
//         }

//         // Araç binildiğinde çağrılır
//         public void MountVehicle(MovementController vehicleController)
//         {
//             SetActiveController(vehicleController);
//         }

//         // Araçtan inildiğinde çağrılır
//         public void DismountVehicle()
//         {
//             SetActiveController(playerController);
//         }
//     }

// }
