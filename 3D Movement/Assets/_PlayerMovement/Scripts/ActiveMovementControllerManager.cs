using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement.Assets._PlayerMovement.Scripts
{
    public class ActiveMovementControllerManager : MonoBehaviour
    {
        public static ActiveMovementControllerManager Instance { get; private set; }
        public GameObject Player;
        public MovementController playerController;
        public MovementController currentController;

        public InputActionReference interactInput;

        private VehicleInteraction currentVehicle;
        private bool canInteract = false;

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
            currentController?.CustomUpdate();

            // SADECE interaction mümkünse kontrol et
            if (canInteract)
            {
                CheckInteractionInput();
            }
        }
        private void CheckInteractionInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
            {
                // Vehicle yanında mı?
                if (currentVehicle != null && currentVehicle.playerInRange && !IsVehicleActive())
                {
                    MountVehicle(currentVehicle);
                }
                // Vehicle'da mı?
                else if (IsVehicleActive())
                {
                    Debug.Log("dismount");
                    DismountVehicle();
                }
            }
        }
        public void RegisterVehicle(VehicleInteraction vehicle)
        {
            currentVehicle = vehicle;
            canInteract = true; // ← Interaction aktif
            Debug.Log("[ActiveManager] Vehicle registered - E to enter enabled");
        }

        public void UnregisterVehicle(VehicleInteraction vehicle)
        {
            if (currentVehicle == vehicle)
            {
                currentVehicle = null;

                // Eğer vehicle'da değilse interaction kapat
                if (!IsVehicleActive())
                {
                    canInteract = false; // ← Interaction pasif
                    Debug.Log("[ActiveManager] Vehicle unregistered - E disabled");
                }
            }
        }


        private void FixedUpdate()
        {
            currentController?.CustomFixedUpdate();
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
            canInteract = true; // ← Vehicle'dayken de E aktif (inme için)
            Debug.Log("[ActiveManager] Mounted vehicle - E to exit enabled");
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
            canInteract = false; // ← İndikten sonra pasif
            Debug.Log("[ActiveManager] Dismounted vehicle - E disabled");
        }

    }


}

