
using UnityEngine;

namespace Movement.Assets._PlayerMovement.Scripts
{
    public class ActiveMovementControllerManager: MonoBehaviour
    {
        public MovementController playerController;
        public MovementController currentController;

        private void Awake()
        {
            // Başlangıçta player aktif
            SetActiveController(playerController);
        }

        public void SetActiveController(MovementController controller)
        {
            if (currentController != null)
                currentController.SetActive(false); // pasif yap

            currentController = controller;

            if (currentController != null)
                currentController.SetActive(true);  // aktif yap
        }

        private void Update()
        {
            currentController?.CustomUpdate();   // MovementController içinde Update yerine CustomUpdate
        }

        private void FixedUpdate()
        {
            currentController?.CustomFixedUpdate(); // MovementController içinde FixedUpdate yerine
        }

        // Araç binildiğinde çağrılır
        public void MountVehicle(MovementController vehicleController)
        {
            SetActiveController(vehicleController);
        }

        // Araçtan inildiğinde çağrılır
        public void DismountVehicle()
        {
            SetActiveController(playerController);
        }
    }

}
