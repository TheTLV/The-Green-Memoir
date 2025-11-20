using UnityEngine;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Camera Controller - Giới hạn tầm nhìn và delay chuyển cam
    /// Flexible: Không lỗi nếu thiếu components
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform target; // Player transform
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

        [Header("Camera Limits")]
        [SerializeField] private bool useLimits = true;
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 10f;
        [SerializeField] private float minY = -10f;
        [SerializeField] private float maxY = 10f;

        [Header("Follow Settings")]
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float delayTime = 0.2f; // Delay khi chuyển map
        [SerializeField] private bool smoothFollow = true;

        [Header("Map Bounds (Optional)")]
        [SerializeField] private BoxCollider2D mapBounds;

        private Vector3 targetPosition;
        private float delayTimer = 0f;
        private bool isDelaying = false;

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    target = playerObj.transform;
            }

            // Calculate bounds from mapBounds nếu có
            if (mapBounds != null && useLimits)
            {
                CalculateBoundsFromCollider();
            }
        }

        private void LateUpdate()
        {
            if (target == null || mainCamera == null) return;

            // Calculate target position
            targetPosition = target.position + offset;

            // Apply limits
            if (useLimits)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            }

            // Delay logic
            if (isDelaying)
            {
                delayTimer -= Time.deltaTime;
                if (delayTimer <= 0f)
                {
                    isDelaying = false;
                }
            }

            // Move camera
            if (smoothFollow && !isDelaying)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            }
            else if (!isDelaying)
            {
                transform.position = targetPosition;
            }
        }

        /// <summary>
        /// Set camera limits từ map bounds
        /// </summary>
        public void SetMapBounds(BoxCollider2D bounds)
        {
            mapBounds = bounds;
            if (bounds != null && useLimits)
            {
                CalculateBoundsFromCollider();
            }
        }

        private void CalculateBoundsFromCollider()
        {
            if (mapBounds == null || mainCamera == null) return;

            Bounds bounds = mapBounds.bounds;
            float cameraHeight = mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            minX = bounds.min.x + cameraWidth / 2f;
            maxX = bounds.max.x - cameraWidth / 2f;
            minY = bounds.min.y + cameraHeight / 2f;
            maxY = bounds.max.y - cameraHeight / 2f;
        }

        /// <summary>
        /// Trigger delay khi chuyển map
        /// </summary>
        public void TriggerMapTransitionDelay()
        {
            isDelaying = true;
            delayTimer = delayTime;
        }

        /// <summary>
        /// Set camera limits manually
        /// </summary>
        public void SetLimits(float minX, float maxX, float minY, float maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            useLimits = true;
        }

        /// <summary>
        /// Disable limits
        /// </summary>
        public void DisableLimits()
        {
            useLimits = false;
        }

        /// <summary>
        /// Set target (player)
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}

