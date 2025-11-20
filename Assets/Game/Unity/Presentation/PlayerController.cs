using UnityEngine;
using TheGreenMemoir.Unity.Presentation;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Player Controller - Di chuyển nhân vật và tương tác với đất
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Tốc độ đi bộ")]
        [SerializeField] private float walkSpeed = 5f;
        
        [Tooltip("Tốc độ chạy (khi nhấn Run Key)")]
        [SerializeField] private float runSpeed = 8f;
        
        [Tooltip("Có thể di chuyển không")]
        [SerializeField] private bool canMove = true;

        [Header("Animation")]
        [Tooltip("Animator Controller (tự động tìm nếu để trống)")]
        [SerializeField] private Animator animator;
        
        [Tooltip("Tên parameter trong Animator cho hướng X")]
        [SerializeField] private string animParamMoveX = "CurrentMoveX";
        
        [Tooltip("Tên parameter trong Animator cho hướng Y")]
        [SerializeField] private string animParamMoveY = "CurrentMoveY";
        
        [Tooltip("Tên parameter trong Animator cho hướng X cuối cùng")]
        [SerializeField] private string animParamLastMoveX = "LastMoveX";
        
        [Tooltip("Tên parameter trong Animator cho hướng Y cuối cùng")]
        [SerializeField] private string animParamLastMoveY = "LastMoveY";
        
        [Tooltip("Tên parameter trong Animator cho trạng thái đang di chuyển")]
        [SerializeField] private string animParamIsMoving = "isMoving";
        
        [Tooltip("Tên parameter trong Animator cho trạng thái đang chạy")]
        [SerializeField] private string animParamIsRunning = "isRunning";
        
        [Tooltip("Tên Trigger parameter trong Animator để trigger animation tool (ví dụ: 'UseTool')")]
        [SerializeField] private string animParamUseTool = "UseTool";
        
        [Tooltip("Thời gian chờ animation tool chạy xong (giây) - để không cho click liên tục")]
        [SerializeField] private float toolAnimationCooldown = 0.5f;

        [Header("References")]
        [Tooltip("Tool Interaction System (tự động tìm nếu để trống)")]
        [SerializeField] private ToolInteractionSystem toolSystem;
        
        [Tooltip("Tilemap Manager (tự động tìm nếu để trống)")]
        [SerializeField] private TilemapManager tilemapManager;
        
        [Tooltip("Animator Override Controller (dùng để override animation tool tại runtime - optional)")]
        [SerializeField] private RuntimeAnimatorController animatorController;
        
        private AnimatorOverrideController animatorOverrideController;

        [Header("Input")]
        [Tooltip("Phím để tương tác với đất tại chỗ (ví dụ: Z)")]
        [SerializeField] private KeyCode interactKey = KeyCode.Z;
        
        [Tooltip("Phím để chạy (có thể đổi trong Settings)")]
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

        [Header("Click-to-Move")]
        [Tooltip("Có cho phép click-to-move không?")]
        [SerializeField] private bool enableClickToMove = true;
        
        [Tooltip("Tự động tương tác với đất khi đến đích (click vào ground tile)")]
        [SerializeField] private bool autoInteractOnArrival = true;
        
        [Tooltip("Khoảng cách tối thiểu để coi là đã đến đích (tile)")]
        [SerializeField] private float arrivalDistance = 0.1f;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private Vector2 lastMoveDirection = Vector2.down; // Mặc định hướng xuống
        private bool isInteracting = false;
        private bool isRunning = false;
        private float lastToolUseTime = 0f; // Thời gian sử dụng tool cuối cùng
        
        // Click-to-move
        private Vector3? targetPosition = null; // Vị trí đích (null = không có đích)
        private Vector3Int? targetTile = null; // Tile đích (để auto-interact)
        private bool isMovingToTarget = false; // Đang di chuyển tới đích

        void Start()
        {
            // Lấy Rigidbody2D
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; // Không rơi
                rb.freezeRotation = true; // Không xoay
            }

            // Tự động tìm Animator
            if (animator == null)
                animator = GetComponent<Animator>();

            // Lấy Animator Controller từ Animator (nếu có) để dùng cho AnimatorOverrideController
            if (animator != null && animatorController == null)
            {
                animatorController = animator.runtimeAnimatorController;
            }

            // Tự động tìm ToolSystem và TilemapManager
            if (toolSystem == null)
                toolSystem = FindFirstObjectByType<ToolInteractionSystem>();
            
            if (tilemapManager == null)
                tilemapManager = FindFirstObjectByType<TilemapManager>();
        }

        void Update()
        {
            // Xử lý click-to-move
            if (enableClickToMove)
            {
                HandleClickToMove();
            }

            // Di chuyển (keyboard hoặc click-to-move)
            if (canMove && !isInteracting)
            {
                HandleMovement();
            }

            // Tương tác với đất (phím interact)
            HandleInteraction();
        }

        void FixedUpdate()
        {
            // Di chuyển nhân vật
            if (canMove && rb != null && !isInteracting)
            {
                float currentSpeed = isRunning ? runSpeed : walkSpeed;
                rb.linearVelocity = moveInput * currentSpeed;
            }
            else if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Xử lý click-to-move: Click chuột vào tile → Di chuyển tới đó
        /// </summary>
        private void HandleClickToMove()
        {
            if (tilemapManager == null)
                return;

            // Click chuột trái để di chuyển
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                // Lấy vị trí tile từ chuột
                var tilePos = tilemapManager.GetTilePositionFromMouse();
                var cellPos = new Vector3Int(tilePos.X, tilePos.Y, 0);
                
                // Lấy world position của tile
                Vector3 worldPos = tilemapManager.CellToWorld(cellPos);
                worldPos.z = transform.position.z; // Giữ Z position
                
                // Kiểm tra tile có thể đi được không (ground tile)
                var groundLayer = tilemapManager.GetLayer("Ground");
                if (groundLayer != null && groundLayer.tilemap != null && groundLayer.tilemap.HasTile(cellPos))
                {
                    // Set đích di chuyển
                    targetPosition = worldPos;
                    targetTile = cellPos;
                    isMovingToTarget = true;
                }
            }

            // Kiểm tra đã đến đích chưa
            if (isMovingToTarget && targetPosition.HasValue)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetPosition.Value);
                
                if (distanceToTarget <= arrivalDistance)
                {
                    // Đã đến đích
                    isMovingToTarget = false;
                    targetPosition = null;
                    
                    // Dừng di chuyển
                    moveInput = Vector2.zero;
                    rb.linearVelocity = Vector2.zero;
                    
                    // Tự động tương tác với đất (nếu có tool và là ground tile)
                    if (autoInteractOnArrival && targetTile.HasValue)
                    {
                        InteractWithTile(targetTile.Value);
                    }
                    
                    targetTile = null;
                }
                else
                {
                    // Di chuyển tới đích
                    Vector3 direction = (targetPosition.Value - transform.position).normalized;
                    moveInput = new Vector2(direction.x, direction.y);
                    
                    // Kiểm tra nút chạy
                    isRunning = UnityEngine.Input.GetKey(runKey);
                }
            }
        }

        /// <summary>
        /// Xử lý di chuyển (keyboard hoặc click-to-move)
        /// </summary>
        private void HandleMovement()
        {
            // Nếu đang di chuyển tới đích (click-to-move), không đọc keyboard input
            if (isMovingToTarget && targetPosition.HasValue)
            {
                // Di chuyển đã được xử lý trong HandleClickToMove
                UpdateAnimation();
                return;
            }

            // Đọc input từ keyboard
            moveInput = new Vector2(
                UnityEngine.Input.GetAxisRaw("Horizontal"),
                UnityEngine.Input.GetAxisRaw("Vertical")
            ).normalized;

            // Kiểm tra nút chạy
            isRunning = UnityEngine.Input.GetKey(runKey) && moveInput.magnitude > 0.1f;

            // Nếu có input từ keyboard, hủy click-to-move
            if (moveInput.magnitude > 0.1f)
            {
                isMovingToTarget = false;
                targetPosition = null;
                targetTile = null;
            }

            // Cập nhật animation
            UpdateAnimation();
        }

        /// <summary>
        /// Chuyển hướng chéo thành hướng gần nhất (4 hướng chính: Up, Down, Left, Right)
        /// Dùng cho animation 4 hướng khi player đi chéo
        /// Ưu tiên Y (lên/xuống) hơn X (trái/phải) vì animation dọc thường rõ ràng hơn
        /// </summary>
        private Vector2 GetClosestCardinalDirection(Vector2 direction)
        {
            if (direction.magnitude < 0.1f)
                return Vector2.down; // Mặc định hướng xuống

            direction = direction.normalized;

            // So sánh độ lớn của X và Y
            // Ưu tiên Y (lên/xuống) hơn X (trái/phải) vì animation dọc thường rõ ràng hơn
            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                // Hướng Y (lên/xuống) - ưu tiên hơn
                return direction.y > 0 ? Vector2.up : Vector2.down;
            }
            else
            {
                // Hướng X (trái/phải)
                return direction.x > 0 ? Vector2.right : Vector2.left;
            }
        }

        /// <summary>
        /// Cập nhật animation parameters
        /// </summary>
        private void UpdateAnimation()
        {
            if (animator == null)
                return;

            // Cập nhật hướng di chuyển hiện tại (giữ nguyên để Blend Tree hoạt động đúng)
            animator.SetFloat(animParamMoveX, moveInput.x);
            animator.SetFloat(animParamMoveY, moveInput.y);

            // Cập nhật hướng cuối cùng (để idle đúng hướng và tool animation)
            if (moveInput.magnitude > 0.1f)
            {
                lastMoveDirection = moveInput.normalized;
                
                // Chuyển hướng chéo thành hướng gần nhất (4 hướng chính) cho animation
                Vector2 cardinalDirection = GetClosestCardinalDirection(lastMoveDirection);
                
                // Lưu cả hướng gốc (để di chuyển) và hướng đã normalize (để animation)
                animator.SetFloat(animParamLastMoveX, cardinalDirection.x);
                animator.SetFloat(animParamLastMoveY, cardinalDirection.y);
            }

            // Cập nhật trạng thái di chuyển
            bool isMoving = moveInput.magnitude > 0.1f;
            animator.SetBool(animParamIsMoving, isMoving);
            animator.SetBool(animParamIsRunning, isRunning);
        }

        /// <summary>
        /// Xử lý tương tác với đất (phím interact - Z)
        /// Tương tác với ô đất mà player đang đứng
        /// </summary>
        private void HandleInteraction()
        {
            if (toolSystem == null || tilemapManager == null)
                return;

            // Kiểm tra cooldown (tránh spam)
            if (Time.time - lastToolUseTime < toolAnimationCooldown)
                return;

            // Phím interact (Z) để tương tác tại chỗ
            if (UnityEngine.Input.GetKeyDown(interactKey))
            {
                // Lấy vị trí tile hiện tại của player
                var currentTilePos = GetCurrentTilePosition();
                var cellPos = new Vector3Int(currentTilePos.X, currentTilePos.Y, 0);
                
                // Tương tác với tile hiện tại
                InteractWithTile(cellPos);
            }
        }

        /// <summary>
        /// Tương tác với tile (dùng chung cho click-to-move và phím interact)
        /// </summary>
        private void InteractWithTile(Vector3Int cellPos)
        {
            if (toolSystem == null || tilemapManager == null)
                return;

            // Kiểm tra tool có thể tương tác với tile này không (chỉ ground tiles)
            var currentToolData = toolSystem.GetCurrentToolData();
            if (currentToolData != null && tilemapManager.CanInteractWithTile(cellPos, currentToolData.actionType))
            {
                // Trigger animation tool (nếu có Animator) - chỉ khi tool có thể tương tác
                TriggerToolAnimation();
                
                // Gọi tool system để xử lý
                toolSystem.OnTileClicked(cellPos);
                
                // Cập nhật thời gian sử dụng tool
                lastToolUseTime = Time.time;
            }
            // Nếu không thể tương tác (house/background), không làm gì (silent fail)
        }

        /// <summary>
        /// Trigger animation tool trong Animator
        /// Sử dụng animation từ ToolDataSO nếu có, nếu không thì dùng trigger mặc định
        /// </summary>
        private void TriggerToolAnimation()
        {
            if (animator == null)
                return;

            // Kiểm tra animator đang trong transition
            if (animator.IsInTransition(0))
                return;

            // Lấy tool data hiện tại từ ToolInteractionSystem
            var currentToolData = toolSystem?.GetCurrentToolData();
            
            // Nếu tool có animation riêng trong ToolDataSO
            if (currentToolData != null)
            {
                // Nếu tool có animation 4 hướng
                if (currentToolData.hasDirectionalAnimation)
                {
                    PlayToolAnimationDirectional(currentToolData);
                }
                // Nếu tool có 1 animation
                else if (currentToolData.useAnimation != null)
                {
                    PlayToolAnimationFromClip(currentToolData.useAnimation);
                }
            }
            
            // Trigger animation (dùng cho cả 2 trường hợp)
            if (!string.IsNullOrEmpty(animParamUseTool))
            {
                // Reset trigger trước (best practice)
                animator.ResetTrigger(animParamUseTool);
                animator.SetTrigger(animParamUseTool);
            }
        }

        /// <summary>
        /// Phát animation tool từ AnimationClip (dùng AnimatorOverrideController)
        /// Dùng cho tool có 1 animation (không phải 4 hướng)
        /// </summary>
        private void PlayToolAnimationFromClip(AnimationClip toolAnimation)
        {
            if (animator == null || toolAnimation == null)
                return;

            // Tạo AnimatorOverrideController nếu chưa có
            if (animatorOverrideController == null && animatorController != null)
            {
                animatorOverrideController = new AnimatorOverrideController(animatorController);
                animator.runtimeAnimatorController = animatorOverrideController;
            }

            // Nếu có AnimatorOverrideController, override animation "UseTool" state
            if (animatorOverrideController != null)
            {
                // Override animation "UseTool" state (có thể là single animation hoặc Blend Tree)
                // Nếu là Blend Tree, sẽ override tất cả animations trong Blend Tree bằng cùng 1 animation
                animatorOverrideController["UseTool"] = toolAnimation;
            }
        }

        /// <summary>
        /// Phát animation tool 4 hướng (dùng AnimatorOverrideController với Blend Tree)
        /// Dùng cho tool có animation 4 hướng (hasDirectionalAnimation = true)
        /// </summary>
        private void PlayToolAnimationDirectional(TheGreenMemoir.Unity.Data.ToolDataSO toolData)
        {
            if (animator == null || toolData == null)
                return;

            // Tạo AnimatorOverrideController nếu chưa có
            if (animatorOverrideController == null && animatorController != null)
            {
                animatorOverrideController = new AnimatorOverrideController(animatorController);
                animator.runtimeAnimatorController = animatorOverrideController;
            }

            // Nếu có AnimatorOverrideController, override các animation trong Blend Tree
            if (animatorOverrideController != null)
            {
                // Lưu ý: AnimatorOverrideController không thể override trực tiếp các animation trong Blend Tree
                // Cách giải quyết: Override state "UseTool" với animation dựa trên hướng hiện tại
                // Chuyển hướng chéo thành hướng gần nhất (4 hướng chính) để chọn animation phù hợp
                
                // Chuyển hướng cuối cùng thành hướng gần nhất (4 hướng chính)
                Vector2 cardinalDirection = GetClosestCardinalDirection(lastMoveDirection);
                
                // Chọn animation dựa trên hướng đã normalize
                AnimationClip animationToPlay = null;
                
                // Chọn animation dựa trên hướng đã normalize (cardinalDirection đã là một trong 4 hướng chính)
                // So sánh các thành phần X và Y (tránh lỗi floating point)
                if (Mathf.Abs(cardinalDirection.y - 1f) < 0.1f) // Up (0, 1)
                {
                    animationToPlay = toolData.useAnimationUp ?? toolData.useAnimation;
                }
                else if (Mathf.Abs(cardinalDirection.y + 1f) < 0.1f) // Down (0, -1)
                {
                    animationToPlay = toolData.useAnimationDown ?? toolData.useAnimation;
                }
                else if (Mathf.Abs(cardinalDirection.x + 1f) < 0.1f) // Left (-1, 0)
                {
                    animationToPlay = toolData.useAnimationLeft ?? toolData.useAnimation;
                }
                else if (Mathf.Abs(cardinalDirection.x - 1f) < 0.1f) // Right (1, 0)
                {
                    animationToPlay = toolData.useAnimationRight ?? toolData.useAnimation;
                }
                else
                {
                    // Fallback: Dùng animation mặc định
                    animationToPlay = toolData.useAnimation;
                }
                
                // Override animation "UseTool" state
                if (animationToPlay != null)
                {
                    animatorOverrideController["UseTool"] = animationToPlay;
                }
            }
        }

        /// <summary>
        /// Set tool system (gọi từ ToolSelectionUI)
        /// </summary>
        public void SetToolSystem(ToolInteractionSystem system)
        {
            toolSystem = system;
        }

        /// <summary>
        /// Set tilemap manager
        /// </summary>
        public void SetTilemapManager(TilemapManager manager)
        {
            tilemapManager = manager;
        }

        /// <summary>
        /// Set có thể di chuyển không
        /// </summary>
        public void SetCanMove(bool can)
        {
            canMove = can;
        }

        /// <summary>
        /// Set đang tương tác (để dừng di chuyển khi đang tương tác)
        /// </summary>
        public void SetIsInteracting(bool interacting)
        {
            isInteracting = interacting;
        }

        /// <summary>
        /// Di chuyển player (dùng bởi InputHandler)
        /// </summary>
        public void Move(Vector2 movement)
        {
            if (canMove && !isInteracting)
            {
                moveInput = movement.normalized;
            }
        }

        /// <summary>
        /// Lấy vị trí tile hiện tại của player (dùng bởi InputHandler)
        /// </summary>
        public TilePosition GetCurrentTilePosition()
        {
            if (tilemapManager != null)
            {
                // Lấy vị trí tile từ world position của player
                Vector3 worldPos = transform.position;
                return tilemapManager.GetTilePosition(worldPos);
            }
            return new TilePosition(0, 0);
        }

        /// <summary>
        /// Set run key (dùng để đổi key trong Settings)
        /// </summary>
        public void SetRunKey(KeyCode key)
        {
            runKey = key;
        }

        /// <summary>
        /// Lấy run key hiện tại
        /// </summary>
        public KeyCode GetRunKey()
        {
            return runKey;
        }

        /// <summary>
        /// Set walk speed
        /// </summary>
        public void SetWalkSpeed(float speed)
        {
            walkSpeed = speed;
        }

        /// <summary>
        /// Set run speed
        /// </summary>
        public void SetRunSpeed(float speed)
        {
            runSpeed = speed;
        }

        /// <summary>
        /// Set interact key (dùng để đổi key trong Settings)
        /// </summary>
        public void SetInteractKey(KeyCode key)
        {
            interactKey = key;
        }

        /// <summary>
        /// Lấy interact key hiện tại
        /// </summary>
        public KeyCode GetInteractKey()
        {
            return interactKey;
        }

        /// <summary>
        /// Set enable click-to-move
        /// </summary>
        public void SetEnableClickToMove(bool enable)
        {
            enableClickToMove = enable;
            if (!enable)
            {
                // Hủy di chuyển nếu đang di chuyển tới đích
                isMovingToTarget = false;
                targetPosition = null;
                targetTile = null;
            }
        }

        /// <summary>
        /// Set auto-interact on arrival
        /// </summary>
        public void SetAutoInteractOnArrival(bool enable)
        {
            autoInteractOnArrival = enable;
        }
    }
}
