using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Farm Tile Renderer - Tối ưu rendering cho farming tiles
    /// 
    /// CÁCH TỐI ƯU: Hybrid System
    /// - Ground tiles: Dùng Tilemap (static, ít thay đổi)
    /// - Crop sprites: Dùng SpriteRenderer riêng (dynamic, dễ animate)
    /// - Chỉ render tiles có thay đổi (dirty flag)
    /// - Batch update nhiều tiles cùng lúc
    /// </summary>
    public class FarmTileRenderer : MonoBehaviour
    {
        [Header("Tilemap References")]
        [Tooltip("Tilemap cho ground (đất)")]
        [SerializeField] private Tilemap groundTilemap;

        [Tooltip("Grid để convert position")]
        [SerializeField] private Grid grid;

        [Header("Crop Rendering")]
        [Tooltip("Parent để chứa crop sprites")]
        [SerializeField] private Transform cropParent;

        [Tooltip("Prefab cho crop sprite (tự động tạo nếu để trống)")]
        [SerializeField] private GameObject cropSpritePrefab;

        [Header("Settings")]
        [Tooltip("Kích thước tile (ví dụ: 288x288 pixels)")]
        [SerializeField] private Vector2 tileSize = new Vector2(288, 288);

        [Tooltip("Pixels per unit (ví dụ: 100 = 1 unit = 100 pixels)")]
        [SerializeField] private float pixelsPerUnit = 100f;

        [Tooltip("Batch update interval (giây) - update nhiều tiles cùng lúc")]
        [SerializeField] private float batchUpdateInterval = 0.1f;

        // Cache và dirty flags
        private Dictionary<TilePosition, GameObject> _cropSprites = new Dictionary<TilePosition, GameObject>();
        private HashSet<TilePosition> _dirtyGroundTiles = new HashSet<TilePosition>();
        private HashSet<TilePosition> _dirtyCropTiles = new HashSet<TilePosition>();
        private float _lastBatchUpdate = 0f;

        // References
        private TileStateManager _tileStateManager;
        private TilemapManager _tilemapManager;

        // Static instance để dễ truy cập
        private static FarmTileRenderer _instance;
        public static FarmTileRenderer Instance => _instance;

        private void Awake()
        {
            // Singleton pattern
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Tự động tìm references
            if (groundTilemap == null)
                groundTilemap = FindFirstObjectByType<Tilemap>();

            if (grid == null)
                grid = FindFirstObjectByType<Grid>();

            if (cropParent == null)
            {
                var cropParentObj = new GameObject("CropSprites");
                cropParentObj.transform.SetParent(transform);
                cropParent = cropParentObj.transform;
            }

            _tileStateManager = FindFirstObjectByType<TileStateManager>();
            _tilemapManager = FindFirstObjectByType<TilemapManager>();

            // Tạo prefab mặc định nếu chưa có
            if (cropSpritePrefab == null)
            {
                var prefabObj = new GameObject("CropSprite");
                prefabObj.AddComponent<SpriteRenderer>();
                cropSpritePrefab = prefabObj;
            }

            // Subscribe events để tự động mark dirty
            if (GameManager.EventBus != null)
            {
                GameManager.EventBus.Subscribe<Core.Application.Events.CropPlantedEvent>(OnCropPlanted);
                GameManager.EventBus.Subscribe<Core.Application.Events.CropHarvestedEvent>(OnCropHarvested);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }

            // Unsubscribe events
            if (GameManager.EventBus != null)
            {
                GameManager.EventBus.Unsubscribe<Core.Application.Events.CropPlantedEvent>(OnCropPlanted);
                GameManager.EventBus.Unsubscribe<Core.Application.Events.CropHarvestedEvent>(OnCropHarvested);
            }
        }

        private void OnCropPlanted(Core.Application.Events.CropPlantedEvent evt)
        {
            MarkGroundTileDirty(evt.Position);
            MarkCropTileDirty(evt.Position);
        }

        private void OnCropHarvested(Core.Application.Events.CropHarvestedEvent evt)
        {
            MarkGroundTileDirty(evt.Position);
            MarkCropTileDirty(evt.Position);
        }

        private void Update()
        {
            // Batch update để tối ưu performance
            if (Time.time - _lastBatchUpdate >= batchUpdateInterval)
            {
                UpdateDirtyTiles();
                _lastBatchUpdate = Time.time;
            }
        }

        /// <summary>
        /// Đánh dấu ground tile cần update
        /// </summary>
        public void MarkGroundTileDirty(TilePosition position)
        {
            _dirtyGroundTiles.Add(position);
        }

        /// <summary>
        /// Đánh dấu crop tile cần update
        /// </summary>
        public void MarkCropTileDirty(TilePosition position)
        {
            _dirtyCropTiles.Add(position);
        }

        /// <summary>
        /// Update tất cả dirty tiles (batch update)
        /// </summary>
        private void UpdateDirtyTiles()
        {
            // Update ground tiles
            if (_dirtyGroundTiles.Count > 0)
            {
                foreach (var position in _dirtyGroundTiles)
                {
                    UpdateGroundTile(position);
                }
                _dirtyGroundTiles.Clear();
            }

            // Update crop tiles
            if (_dirtyCropTiles.Count > 0)
            {
                foreach (var position in _dirtyCropTiles)
                {
                    UpdateCropTile(position);
                }
                _dirtyCropTiles.Clear();
            }
        }

        /// <summary>
        /// Update ground tile (đất)
        /// </summary>
        private void UpdateGroundTile(TilePosition position)
        {
            if (groundTilemap == null || _tilemapManager == null)
                return;

            var tilePos = new Vector3Int(position.X, position.Y, 0);
            
            // Lấy FarmTile từ repository
            var farmTile = GameManager.FarmRepository?.GetTile(position);
            if (farmTile == null)
                return;

            // Lấy TileStateSO
            var state = _tileStateManager?.GetState(farmTile.CurrentStateId);
            if (state == null)
                return;

            // Update tile trong Tilemap
            if (state.tileBase != null)
            {
                groundTilemap.SetTile(tilePos, state.tileBase);
            }
            else if (_tilemapManager != null)
            {
                _tilemapManager.SetTileState(tilePos, farmTile.CurrentStateId);
            }
        }

        /// <summary>
        /// Update crop tile (cây)
        /// </summary>
        private void UpdateCropTile(TilePosition position)
        {
            // Lấy FarmTile từ repository
            var farmTile = GameManager.FarmRepository?.GetTile(position);
            if (farmTile == null || farmTile.Crop == null)
            {
                // Không có crop → xóa sprite
                RemoveCropSprite(position);
                return;
            }

            // Có crop → hiển thị sprite
            var crop = farmTile.Crop;
            var cropData = GetCropData(crop.Id);
            if (cropData == null)
                return;

            // Lấy sprite từ CropDataSO
            var sprite = cropData.GetSpriteForCrop(crop);
            if (sprite == null)
                return;

            // Tạo hoặc update crop sprite
            var cropSprite = GetOrCreateCropSprite(position);
            if (cropSprite != null)
            {
                var spriteRenderer = cropSprite.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = sprite;
                }

                // Đặt vị trí
                var worldPos = GetWorldPosition(position);
                cropSprite.transform.position = worldPos;
            }
        }

        /// <summary>
        /// Lấy hoặc tạo crop sprite GameObject
        /// </summary>
        private GameObject GetOrCreateCropSprite(TilePosition position)
        {
            if (_cropSprites.TryGetValue(position, out var existing))
            {
                if (existing != null)
                    return existing;
                else
                    _cropSprites.Remove(position);
            }

            // Tạo mới
            var cropSprite = Instantiate(cropSpritePrefab, cropParent);
            cropSprite.name = $"Crop_{position.X}_{position.Y}";
            _cropSprites[position] = cropSprite;
            return cropSprite;
        }

        /// <summary>
        /// Xóa crop sprite
        /// </summary>
        private void RemoveCropSprite(TilePosition position)
        {
            if (_cropSprites.TryGetValue(position, out var sprite))
            {
                if (sprite != null)
                {
                    Destroy(sprite);
                }
                _cropSprites.Remove(position);
            }
        }

        /// <summary>
        /// Lấy CropDataSO từ database
        /// </summary>
        private CropDataSO GetCropData(CropId cropId)
        {
            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
                return null;

            return database.GetCropData(cropId);
        }

        /// <summary>
        /// Convert tile position sang world position
        /// </summary>
        private Vector3 GetWorldPosition(TilePosition position)
        {
            if (grid != null)
            {
                return grid.CellToWorld(new Vector3Int(position.X, position.Y, 0));
            }

            // Fallback: tính toán thủ công
            float unitSize = tileSize.x / pixelsPerUnit;
            return new Vector3(position.X * unitSize, position.Y * unitSize, 0);
        }

        /// <summary>
        /// Render tất cả tiles (gọi khi load game hoặc refresh)
        /// </summary>
        public void RenderAllTiles()
        {
            if (GameManager.FarmRepository == null)
                return;

            var allTiles = GameManager.FarmRepository.GetAllTiles();
            foreach (var tile in allTiles)
            {
                MarkGroundTileDirty(tile.Position);
                if (tile.IsPlanted)
                {
                    MarkCropTileDirty(tile.Position);
                }
            }

            // Update ngay lập tức
            UpdateDirtyTiles();
        }

        /// <summary>
        /// Clear tất cả crop sprites (khi cần reset)
        /// </summary>
        public void ClearAllCropSprites()
        {
            foreach (var sprite in _cropSprites.Values)
            {
                if (sprite != null)
                    Destroy(sprite);
            }
            _cropSprites.Clear();
        }
    }
}

