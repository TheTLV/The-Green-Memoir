using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Volume Slider UI - Hiển thị volume bằng các block (pixel art style)
    /// Dùng sprite blocks để tạo animation volume slider
    /// </summary>
    public class VolumeSliderUI : MonoBehaviour
    {
        [Header("Volume Blocks")]
        [Tooltip("Prefab hoặc GameObject cho mỗi block (sprite block từ sprite sheet)")]
        [SerializeField] private GameObject blockPrefab;
        
        [Tooltip("Số lượng blocks tối đa (ví dụ: 10 blocks)")]
        [SerializeField] private int maxBlocks = 10;
        
        [Tooltip("Container chứa các blocks (tự động tạo nếu để trống)")]
        [SerializeField] private Transform blocksContainer;

        [Header("Layout")]
        [Tooltip("Khoảng cách giữa các blocks")]
        [SerializeField] private float spacing = 2f;
        
        [Tooltip("Hướng layout (Horizontal hoặc Vertical)")]
        [SerializeField] private bool horizontal = true;

        private List<GameObject> _blocks = new List<GameObject>();
        private float _currentVolume = 1f;

        private void Awake()
        {
            if (blocksContainer == null)
            {
                // Tự động tạo container
                GameObject container = new GameObject("BlocksContainer");
                container.transform.SetParent(transform);
                container.transform.localPosition = Vector3.zero;
                blocksContainer = container.transform;
            }
        }

        private void Start()
        {
            CreateBlocks();
            UpdateVolume(_currentVolume);
        }

        /// <summary>
        /// Tạo các blocks
        /// </summary>
        private void CreateBlocks()
        {
            // Xóa blocks cũ
            foreach (var block in _blocks)
            {
                if (block != null)
                    Destroy(block);
            }
            _blocks.Clear();

            // Tạo blocks mới
            for (int i = 0; i < maxBlocks; i++)
            {
                GameObject block;
                
                if (blockPrefab != null)
                {
                    block = Instantiate(blockPrefab, blocksContainer);
                }
                else
                {
                    // Tạo block mặc định nếu không có prefab
                    block = new GameObject($"Block_{i}");
                    block.transform.SetParent(blocksContainer);
                    
                    Image img = block.AddComponent<Image>();
                    img.color = new Color(0.6f, 0.4f, 0.2f); // Màu nâu mặc định
                    
                    RectTransform rect = block.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(16, 16); // Kích thước mặc định
                }

                // Set position
                RectTransform rectTransform = block.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localPosition = GetBlockPosition(i);
                    rectTransform.localScale = Vector3.one;
                }

                // Ẩn block mặc định
                block.SetActive(false);
                _blocks.Add(block);
            }
        }

        /// <summary>
        /// Tính vị trí của block thứ i
        /// </summary>
        private Vector3 GetBlockPosition(int index)
        {
            if (horizontal)
            {
                return new Vector3(index * (16 + spacing), 0, 0);
            }
            else
            {
                return new Vector3(0, index * (16 + spacing), 0);
            }
        }

        /// <summary>
        /// Cập nhật volume (0-1)
        /// </summary>
        public void UpdateVolume(float volume)
        {
            _currentVolume = Mathf.Clamp01(volume);
            
            // Tính số blocks cần hiển thị
            int activeBlocks = Mathf.RoundToInt(_currentVolume * maxBlocks);
            
            // Hiển thị/ẩn blocks
            for (int i = 0; i < _blocks.Count; i++)
            {
                if (_blocks[i] != null)
                {
                    _blocks[i].SetActive(i < activeBlocks);
                }
            }
        }

        /// <summary>
        /// Set block prefab (gọi từ Inspector hoặc code)
        /// </summary>
        public void SetBlockPrefab(GameObject prefab)
        {
            blockPrefab = prefab;
            CreateBlocks();
            UpdateVolume(_currentVolume);
        }

        /// <summary>
        /// Set số lượng blocks tối đa
        /// </summary>
        public void SetMaxBlocks(int max)
        {
            maxBlocks = max;
            CreateBlocks();
            UpdateVolume(_currentVolume);
        }
    }
}

