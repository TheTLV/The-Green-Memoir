using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Farm Area Renderer - Chỉ render tiles trong khu ruộng
    /// 
    /// GIẢI PHÁP TỐI ƯU:
    /// - Giữ nguyên background lớn (ground + pair) - KHÔNG CẮT
    /// - Chỉ tạo Tilemap overlay cho KHU RUỘNG
    /// - Chỉ render/update tiles trong khu ruộng
    /// - Tự động detect khu ruộng từ bounds
    /// </summary>
    public class FarmAreaRenderer : MonoBehaviour
    {
        [Header("Background (Giữ Nguyên - Không Cắt)")]
        [Tooltip("SpriteRenderer cho background ground (bức ảnh lớn)")]
        [SerializeField] private SpriteRenderer backgroundGround;

        [Tooltip("SpriteRenderer cho background pair (bức ảnh lớn)")]
        [SerializeField] private SpriteRenderer backgroundPair;

        [Header("Farm Area (Khu Ruộng)")]
        [Tooltip("Tilemap overlay cho khu ruộng (chỉ vẽ tiles ở đây)")]
        [SerializeField] private Tilemap farmTilemap;

        [Tooltip("Bounds của khu ruộng (tự động detect nếu để trống)")]
        [SerializeField] private Bounds farmAreaBounds;

        [Tooltip("Tự động detect bounds từ Tilemap")]
        [SerializeField] private bool autoDetectBounds = true;

        [Header("Crop Rendering")]
        [Tooltip("Parent để chứa crop sprites")]
        [SerializeField] private Transform cropParent;

        [Tooltip("Prefab cho crop sprite")]
        [SerializeField] private GameObject cropSpritePrefab;

        [Header("Settings")]
        [Tooltip("Grid để convert position")]
        [SerializeField] private Grid grid;

        [Tooltip("Kích thước tile (ví dụ: 288x288 pixels)")]
        [SerializeField] private Vector2 tileSize = new Vector2(288, 288);

        [Tooltip("Pixels per unit")]
        [SerializeField] private float pixelsPerUnit = 100f;

        // Cache
        private Dictionary<TilePosition, GameObject> _cropSprites = new Dictionary<TilePosition, GameObject>();
        private HashSet<TilePosition> _dirtyTiles = new HashSet<TilePosition>();
        private TileStateManager _tileStateManager;

        private void Awake()
        {
            // Tự động tìm references
            if (farmTilemap == null)
                farmTilemap = FindFirstObjectByType<Tilemap>();

            if (grid == null)
                grid = FindFirstObjectByType<Grid>();

            if (cropParent == null)
            {
                var cropParentObj = new GameObject("CropSprites");
                cropParentObj.transform.SetParent(transform);
                cropParent = cropParentObj.transform;
            }

            _tileStateManager = FindFirstObjectByType<TileStateManager>();

            // Auto detect bounds
            if (autoDetectBounds && farmTilemap != null)
            {
                DetectFarmAreaBounds();
            }

            // Subscribe events
            if (GameManager.EventBus != null)
            {
                GameManager.EventBus.Subscribe<Core.Application.Events.CropPlantedEvent>(OnCropPlanted);
                GameManager.EventBus.Subscribe<Core.Application.Events.CropHarvestedEvent>(OnCropHarvested);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.EventBus != null)
            {
                GameManager.EventBus.Unsubscribe<Core.Application.Events.CropPlantedEvent>(OnCropPlanted);
                GameManager.EventBus.Unsubscribe<Core.Application.Events.CropHarvestedEvent>(OnCropHarvested);
            }
        }

        /// <summary>
        /// Tự động detect bounds của khu ruộng từ Tilemap
        /// </summary>
        private void DetectFarmAreaBounds()
        {
            if (farmTilemap == null)
                return;

            // Lấy bounds từ tiles có trong Tilemap
            farmTilemap.CompressBounds();
            var bounds = farmTilemap.localBounds;
            farmAreaBounds = bounds;

            Debug.Log($"Farm area detected: {bounds.min} to {bounds.max}");
        }

        /// <summary>
        /// Kiểm tra vị trí có trong khu ruộng không
        /// </summary>
        public bool IsInFarmArea(TilePosition position)
        {
            var worldPos = GetWorldPosition(position);
            return farmAreaBounds.Contains(worldPos);
        }

        /// <summary>
        /// Kiểm tra vị trí có trong khu ruộng không (world position)
        /// </summary>
        public bool IsInFarmArea(Vector3 worldPosition)
        {
            return farmAreaBounds.Contains(worldPosition);
        }

        /// <summary>
        /// Đánh dấu tile cần update
        /// </summary>
        public void MarkTileDirty(TilePosition position)
        {
            // Chỉ mark nếu trong khu ruộng
            if (IsInFarmArea(position))
            {
                _dirtyTiles.Add(position);
            }
        }

        private void Update()
        {
            // Batch update dirty tiles
            if (_dirtyTiles.Count > 0)
            {
                UpdateDirtyTiles();
                _dirtyTiles.Clear();
            }
        }

        private void UpdateDirtyTiles()
        {
            foreach (var position in _dirtyTiles)
            {
                UpdateTile(position);
            }
        }

        /// <summary>
        /// Update tile (ground + crop)
        /// </summary>
        private void UpdateTile(TilePosition position)
        {
            if (!IsInFarmArea(position))
                return;

            var farmTile = GameManager.FarmRepository?.GetTile(position);
            if (farmTile == null)
                return;

            // Update ground tile
            UpdateGroundTile(position, farmTile);

            // Update crop sprite
            UpdateCropSprite(position, farmTile);
        }

        /// <summary>
        /// Update ground tile trong Tilemap
        /// </summary>
        private void UpdateGroundTile(TilePosition position, FarmTile farmTile)
        {
            if (farmTilemap == null)
                return;

            var tilePos = new Vector3Int(position.X, position.Y, 0);
            var state = _tileStateManager?.GetState(farmTile.CurrentStateId);

            if (state != null && state.tileBase != null)
            {
                farmTilemap.SetTile(tilePos, state.tileBase);
            }
        }

        /// <summary>
        /// Update crop sprite
        /// </summary>
        private void UpdateCropSprite(TilePosition position, FarmTile farmTile)
        {
            if (farmTile.Crop == null)
            {
                RemoveCropSprite(position);
                return;
            }

            var crop = farmTile.Crop;
            var cropData = GetCropData(crop.Id);
            if (cropData == null)
                return;

            var sprite = cropData.GetSpriteForCrop(crop);
            if (sprite == null)
                return;

            var cropSprite = GetOrCreateCropSprite(position);
            if (cropSprite != null)
            {
                var spriteRenderer = cropSprite.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = sprite;
                }

                cropSprite.transform.position = GetWorldPosition(position);
            }
        }

        private GameObject GetOrCreateCropSprite(TilePosition position)
        {
            if (_cropSprites.TryGetValue(position, out var existing) && existing != null)
                return existing;

            if (cropSpritePrefab == null)
            {
                var prefabObj = new GameObject("CropSprite");
                prefabObj.AddComponent<SpriteRenderer>();
                cropSpritePrefab = prefabObj;
            }

            var cropSprite = Instantiate(cropSpritePrefab, cropParent);
            cropSprite.name = $"Crop_{position.X}_{position.Y}";
            _cropSprites[position] = cropSprite;
            return cropSprite;
        }

        private void RemoveCropSprite(TilePosition position)
        {
            if (_cropSprites.TryGetValue(position, out var sprite))
            {
                if (sprite != null)
                    Destroy(sprite);
                _cropSprites.Remove(position);
            }
        }

        private CropDataSO GetCropData(CropId cropId)
        {
            var database = GameDatabaseManager.GetDatabase();
            return database?.GetCropData(cropId);
        }

        private Vector3 GetWorldPosition(TilePosition position)
        {
            if (grid != null)
            {
                return grid.CellToWorld(new Vector3Int(position.X, position.Y, 0));
            }

            float unitSize = tileSize.x / pixelsPerUnit;
            return new Vector3(position.X * unitSize, position.Y * unitSize, 0);
        }

        private void OnCropPlanted(Core.Application.Events.CropPlantedEvent evt)
        {
            MarkTileDirty(evt.Position);
        }

        private void OnCropHarvested(Core.Application.Events.CropHarvestedEvent evt)
        {
            MarkTileDirty(evt.Position);
        }

        /// <summary>
        /// Render tất cả tiles trong khu ruộng
        /// </summary>
        public void RenderAllFarmTiles()
        {
            if (GameManager.FarmRepository == null)
                return;

            var allTiles = GameManager.FarmRepository.GetAllTiles();
            foreach (var tile in allTiles)
            {
                if (IsInFarmArea(tile.Position))
                {
                    MarkTileDirty(tile.Position);
                }
            }

            UpdateDirtyTiles();
        }
    }
}

