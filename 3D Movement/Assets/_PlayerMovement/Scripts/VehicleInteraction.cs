using UnityEngine;

namespace Movement.Assets._PlayerMovement.Scripts
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(Collider))]

    public class VehicleInteraction : MonoBehaviour
    {
        public MovementController vehicleController;
        public Transform mountPoint;
        public Transform exitPoint;
        public Collider interactionZone;

        [HideInInspector] public bool playerInRange = false;
        [HideInInspector] public MovementController playerController;

        private void Awake()
        {
            if (interactionZone != null)
                interactionZone.isTrigger = true;
        }



    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var controller = other.GetComponent<MovementController>();
        if (controller != null)
        {
            playerInRange = true;
            playerController = controller;
            ActiveMovementControllerManager.Instance.RegisterVehicle(this); // ← EKLE
            Debug.Log("[Vehicle] Player registered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        playerController = null;
        ActiveMovementControllerManager.Instance.UnregisterVehicle(this); // ← EKLE
        Debug.Log("[Vehicle] Player unregistered");
    }

        private void OnDrawGizmosSelected()
        {
            if (mountPoint)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(mountPoint.position, 0.3f);
            }

            if (exitPoint)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(exitPoint.position, 0.3f);
            }
        }
    }

}


