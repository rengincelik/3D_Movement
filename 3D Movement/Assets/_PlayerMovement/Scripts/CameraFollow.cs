
using UnityEngine;

namespace Movement.Assets._PlayerMovement.Scripts
{

    public class CameraFollow : MonoBehaviour
    {
        public Transform target; // Player
        public Vector3 offset;
        public float smoothSpeed = 5f;

        void LateUpdate()
        {
            Vector3 targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        }
    }
}
