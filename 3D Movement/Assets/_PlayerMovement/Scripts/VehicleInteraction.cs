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


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.InputSystem;

// namespace Movement.Assets._PlayerMovement.Scripts
// {

//     /// <summary>
//     /// Vehicle'a binme/inme logic'i
//     /// </summary>
//     [RequireComponent(typeof(MovementController))]
//     [RequireComponent(typeof(Collider))]
//     public class VehicleInteraction : MonoBehaviour
//     {
//         [Header("References")]
//         public MovementController vehicleController;

//         [Header("Settings")]
//         public float interactionRange = 3f;
//         public Vector3 exitOffset = new Vector3(2f, 0f, 0f); // Sağa 2 birim

//         [Header("Optional - Future Use")]
//         public Transform mountPoint; // Vehicle içinde player pozisyonu
//         public Transform exitPoint;  // İnme noktası

//         // Runtime
//         private MovementController playerController;
//         private bool playerInRange = false;
//         private bool isMounted = false;

//         private void Awake()
//         {
//             // Auto-assign vehicle controller
//             if (vehicleController == null)
//                 vehicleController = GetComponent<MovementController>();

//             // Collider trigger olmalı
//             var col = GetComponent<Collider>();
//             if (col != null)
//                 col.isTrigger = true;
//         }

//         private void Update()
//         {
//             // Player range'de ve E tuşuna basıldı
//             if (playerInRange && ActiveMovementControllerManager.Instance.GetInteractPressed())
//             {
//                 if (!isMounted)
//                     Mount();
//                 else
//                     Dismount();
//             }
//         }

//         private void OnTriggerEnter(Collider other)
//         {
//             // Player'ı detect et
//             if (other.CompareTag("Player"))
//             {
//                 var controller = other.GetComponent<MovementController>();
//                 if (controller != null && !isMounted)
//                 {
//                     playerController = controller;
//                     playerInRange = true;
//                     Debug.Log("[VehicleInteraction] Player in range. Press E to enter.");
//                     // Burada UI gösterebilirsin: "Press E to enter"
//                 }
//             }
//         }

//         private void OnTriggerExit(Collider other)
//         {
//             if (other.CompareTag("Player"))
//             {
//                 playerInRange = false;
//                 playerController = null;
//                 Debug.Log("[VehicleInteraction] Player left range.");
//             }
//         }

//         private void Mount()
//         {
//             if (playerController == null) return;

//             Debug.Log("[VehicleInteraction] Mounting vehicle...");

//             // Player'ı vehicle'ın child'ı yap
//             Transform playerTransform = playerController.transform;

//             if (mountPoint != null)
//             {
//                 playerTransform.SetParent(mountPoint);
//                 playerTransform.localPosition = Vector3.zero;
//                 playerTransform.localRotation = Quaternion.identity;
//             }
//             else
//             {
//                 playerTransform.SetParent(transform);
//                 playerTransform.localPosition = Vector3.zero;
//             }

//             // Player physics'ini dondur
//             Rigidbody playerRb = playerController.rb;
//             if (playerRb != null)
//             {
//                 playerRb.isKinematic = true;
//                 playerRb.linearVelocity = Vector3.zero;
//                 playerRb.angularVelocity = Vector3.zero;
//             }

//             // Controller switch
//             ActiveMovementControllerManager.Instance.MountVehicle(vehicleController);

//             isMounted = true;
//             playerInRange = false; // Artık içerde, trigger tekrar tetiklenmemeli
//         }

//         private void Dismount()
//         {
//             if (playerController == null) return;

//             Debug.Log("[VehicleInteraction] Dismounting vehicle...");

//             // Player'ı parent'tan ayır
//             Transform playerTransform = playerController.transform;
//             playerTransform.SetParent(null);

//             // Exit position
//             if (exitPoint != null)
//             {
//                 playerTransform.position = exitPoint.position;
//                 playerTransform.rotation = exitPoint.rotation;
//             }
//             else
//             {
//                 // Basit offset (vehicle'ın sağına)
//                 playerTransform.position = transform.position + transform.TransformDirection(exitOffset);
//                 playerTransform.rotation = transform.rotation;
//             }

//             // Player physics'ini aktif et
//             Rigidbody playerRb = playerController.rb;
//             if (playerRb != null)
//             {
//                 playerRb.isKinematic = false;
//             }

//             // Controller switch
//             ActiveMovementControllerManager.Instance.DismountVehicle();

//             isMounted = false;
//         }

//         // Manuel mount/dismount (test için)
//         public void ForceDismount()
//         {
//             if (isMounted)
//                 Dismount();
//         }

//         // Gizmos (Editor'da görselleştirme)
//         private void OnDrawGizmosSelected()
//         {
//             // Interaction range
//             Gizmos.color = Color.yellow;
//             Gizmos.DrawWireSphere(transform.position, interactionRange);

//             // Exit point
//             if (exitPoint != null)
//             {
//                 Gizmos.color = Color.green;
//                 Gizmos.DrawWireSphere(exitPoint.position, 0.5f);
//             }
//             else
//             {
//                 Vector3 exitPos = transform.position + transform.TransformDirection(exitOffset);
//                 Gizmos.color = Color.green;
//                 Gizmos.DrawWireSphere(exitPos, 0.5f);
//             }

//             // Mount point
//             if (mountPoint != null)
//             {
//                 Gizmos.color = Color.blue;
//                 Gizmos.DrawWireSphere(mountPoint.position, 0.3f);
//             }
//         }
//     }

// }
