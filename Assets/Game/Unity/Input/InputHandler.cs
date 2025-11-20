using UnityEngine;
using TheGreenMemoir.Unity.Presentation;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Application.Commands;

namespace TheGreenMemoir.Unity.Input
{
    /// <summary>
    /// Xử lý input từ Unity Input System
    /// Kết nối input với các controllers
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private FarmingUIController farmingController;

        [Header("Input Settings")]
#pragma warning disable CS0414 // Field is assigned but never used (may be used in future or set from Inspector)
        [SerializeField] private float interactionRange = 1.5f;
#pragma warning restore CS0414

        [Header("Keyboard Settings (fallback if no InputActionManager)")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private string horizontalAxis = "Horizontal";
        [SerializeField] private string verticalAxis = "Vertical";

        [Header("Optional")]
        [SerializeField] private TheGreenMemoir.Unity.Input.InputActionManager inputActionManager;

        private Vector2 _moveInput;

        private void Awake()
        {
            // Tự động tìm references nếu chưa gán
            if (playerController == null)
                playerController = FindFirstObjectByType<PlayerController>();
            
            if (farmingController == null)
                farmingController = FindFirstObjectByType<FarmingUIController>();

            if (inputActionManager == null)
                inputActionManager = FindFirstObjectByType<TheGreenMemoir.Unity.Input.InputActionManager>();
        }

        private void Update()
        {
            if (inputActionManager != null)
                _moveInput = inputActionManager.Move;
            else
                ReadMovementInput();
            MovePlayer();
            HandleKeyboardInteraction();
            HandleMouseClick();
        }

        /// <summary>
        /// Xử lý tương tác (cuốc đất, trồng cây, thu hoạch)
        /// </summary>
        private void HandleInteraction()
        {
            if (playerController == null || farmingController == null)
                return;

            TilePosition tilePos = playerController.GetCurrentTilePosition();
            
            // Tạo command dựa trên tool đang chọn (mặc định là cuốc đất)
            var command = new PlowTileCommand(
                GameManager.FarmingService,
                tilePos,
                PlayerId.Default
            );

            GameManager.CommandInvoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Đọc input di chuyển từ bàn phím
        /// </summary>
        private void ReadMovementInput()
        {
            float horizontal = UnityEngine.Input.GetAxisRaw(horizontalAxis);
            float vertical = UnityEngine.Input.GetAxisRaw(verticalAxis);
            _moveInput = new Vector2(horizontal, vertical);
            if (_moveInput.magnitude > 1f)
            {
                _moveInput.Normalize();
            }
        }

        private void MovePlayer()
        {
            if (playerController != null)
            {
                playerController.Move(_moveInput);
            }
        }

        private void HandleKeyboardInteraction()
        {
            if (UnityEngine.Input.GetKeyDown(interactKey))
            {
                HandleInteraction();
            }
        }

        /// <summary>
        /// Xử lý click chuột (cho farming UI)
        /// </summary>
        private void HandleMouseClick()
        {
            if (farmingController == null)
                return;

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                mouseWorldPos.z = 0;

                TilePosition tilePos = new TilePosition(
                    Mathf.RoundToInt(mouseWorldPos.x),
                    Mathf.RoundToInt(mouseWorldPos.y)
                );

                farmingController.OnTileClicked(tilePos);
            }
        }
    }
}

