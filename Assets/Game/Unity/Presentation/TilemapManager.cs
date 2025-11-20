using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.Presentation
{
	/// <summary>
	/// Quản lý nhiều layer tilemap và kiểm tra interaction
	/// Hỗ trợ thêm/xóa layer động qua Inspector
	/// </summary>
	public class TilemapManager : MonoBehaviour
	{
		[System.Serializable]
		public enum TilemapLayerType
		{
			Background,
			Ground,
			Water,
			Decoration,
			Custom
		}

		[System.Serializable]
		public class TilemapLayerEntry
		{
			public string key = "Ground";
			public Tilemap tilemap;
			public TilemapLayerType type = TilemapLayerType.Ground;
			public bool interactable = true;
		}

		[Header("Tilemap Layers (Dynamic)")]
		[SerializeField] private List<TilemapLayerEntry> layers = new List<TilemapLayerEntry>();
		[Tooltip("Key của Ground layer mặc định để set tile")] 
		[SerializeField] private string groundLayerKey = "Ground";

		private readonly Dictionary<string, TilemapLayerEntry> _layerByKey = new Dictionary<string, TilemapLayerEntry>();

		[Header("Tile State System (SO-based)")]
		[Tooltip("TileStateManager để lookup states (tự động tìm nếu để trống)")]
		[SerializeField] private TileStateManager tileStateManager;
		
        [Header("Tile Sprites (Optional - không bắt buộc)")]
        [Tooltip("Sprite mapping từ state ID sang TileBase (optional)")]
        [SerializeField] private Dictionary<string, TileBase> stateToTileMap = new Dictionary<string, TileBase>();

        [Header("Optimization")]
        [Tooltip("Chỉ update tiles đã thay đổi (tối ưu performance)")]
        [SerializeField] private bool useDirtyFlag = true;
        
        private HashSet<Vector3Int> _dirtyTiles = new HashSet<Vector3Int>();

		[Header("Settings")]
		[Tooltip("Grid để convert world position sang tile position")]
		[SerializeField] private Grid grid;

        private void Awake()
        {
            RefreshCache();
            
            // Tìm TileStateManager nếu chưa gán
            if (tileStateManager == null)
                tileStateManager = FindFirstObjectByType<TileStateManager>();
        }

        private void LateUpdate()
        {
            // Batch update dirty tiles mỗi frame
            if (useDirtyFlag && _dirtyTiles.Count > 0)
            {
                UpdateDirtyTiles();
            }
        }

		private void OnValidate()
		{
			RefreshCache();
		}

		private void RefreshCache()
		{
			_layerByKey.Clear();
			for (int i = 0; i < layers.Count; i++)
			{
				var e = layers[i];
				if (e == null || string.IsNullOrWhiteSpace(e.key)) continue;
				_layerByKey[e.key] = e;
			}
		}

		/// <summary>
		/// Kiểm tra xem tool có thể tương tác với tile không (dùng TileStateSO)
		/// </summary>
		public bool CanInteractWithTile(Vector3Int tilePos, ToolActionType tool, string currentStateId = null)
		{
			var ground = GetGroundLayer();
			if (ground == null || !ground.interactable || ground.tilemap == null || !ground.tilemap.HasTile(tilePos))
				return false;

			// Dùng TileStateSO để kiểm tra
			if (tileStateManager != null && !string.IsNullOrWhiteSpace(currentStateId))
			{
				var state = tileStateManager.GetState(currentStateId);
				if (state != null)
				{
					switch (tool)
					{
						case ToolActionType.Plow:
							return state.canPlow;
						case ToolActionType.Plant:
							return state.canPlant;
						case ToolActionType.Water:
							return state.canWater;
						case ToolActionType.Harvest:
							return state.canHarvest;
					}
				}
			}

			// Fallback: Kiểm tra bằng tile sprite (nếu không có TileStateSO)
			// Chỉ dùng khi không có TileStateManager
			return false;
		}

		/// <summary>
		/// Lấy TileStateSO từ tile position (từ FarmTile hoặc cache)
		/// </summary>
		public TileStateSO GetTileState(Vector3Int tilePos, string stateId = null)
		{
			if (tileStateManager == null) return null;
			
			if (!string.IsNullOrWhiteSpace(stateId))
				return tileStateManager.GetState(stateId);
			
			// TODO: Lấy từ FarmTile nếu có
			return null;
		}

        /// <summary>
        /// Đổi sprite tile dựa trên state ID (từ TileStateSO)
        /// Optional - game vẫn chạy được không có sprite
        /// Tối ưu: Dùng dirty flag để batch update
        /// </summary>
        public void SetTileState(Vector3Int tilePos, string stateId, bool immediate = false)
        {
            if (useDirtyFlag && !immediate)
            {
                // Đánh dấu dirty, sẽ update sau
                _dirtyTiles.Add(tilePos);
                return;
            }

            // Update ngay lập tức
            UpdateTileState(tilePos, stateId);
        }

        /// <summary>
        /// Update tile state (internal)
        /// </summary>
        private void UpdateTileState(Vector3Int tilePos, string stateId)
        {
            var ground = GetGroundLayer();
            if (ground == null || ground.tilemap == null)
                return;

            // Lấy TileBase từ state ID (nếu có mapping)
            if (stateToTileMap != null && stateToTileMap.ContainsKey(stateId))
            {
                ground.tilemap.SetTile(tilePos, stateToTileMap[stateId]);
            }
            // Hoặc lấy từ TileStateSO
            else if (tileStateManager != null)
            {
                var state = tileStateManager.GetState(stateId);
                if (state != null && state.tileBase != null)
                {
                    ground.tilemap.SetTile(tilePos, state.tileBase);
                }
            }
            // Nếu không có sprite, không làm gì (game vẫn chạy được)
        }

        /// <summary>
        /// Batch update tất cả dirty tiles (gọi trong LateUpdate hoặc coroutine)
        /// </summary>
        public void UpdateDirtyTiles()
        {
            if (_dirtyTiles.Count == 0)
                return;

            // Lấy state từ FarmTile
            if (GameManager.FarmRepository != null)
            {
                foreach (var tilePos in _dirtyTiles)
                {
                    var position = new TilePosition(tilePos.x, tilePos.y);
                    var farmTile = GameManager.FarmRepository.GetTile(position);
                    if (farmTile != null)
                    {
                        UpdateTileState(tilePos, farmTile.CurrentStateId);
                    }
                }
            }

            _dirtyTiles.Clear();
        }

		/// <summary>
		/// Đổi sprite tile sang "đã cuốc" (Legacy - dùng SetTileState thay thế)
		/// </summary>
		[System.Obsolete("Use SetTileState instead")]
		public void SetTilePlowed(Vector3Int tilePos)
		{
			SetTileState(tilePos, "plowed");
		}

		/// <summary>
		/// Đổi sprite tile sang "đã tưới" (Legacy - dùng SetTileState thay thế)
		/// </summary>
		[System.Obsolete("Use SetTileState instead")]
		public void SetTileWatered(Vector3Int tilePos)
		{
			SetTileState(tilePos, "watered");
		}

		/// <summary>
		/// Convert world position sang tile position
		/// </summary>
		public Vector3Int WorldToCell(Vector3 worldPos)
		{
			if (grid != null)
			{
				return grid.WorldToCell(worldPos);
			}
			return Vector3Int.FloorToInt(worldPos);
		}

		/// <summary>
		/// Convert tile position sang world position
		/// </summary>
		public Vector3 CellToWorld(Vector3Int tilePos)
		{
			if (grid != null)
			{
				return grid.CellToWorld(tilePos);
			}
			return tilePos;
		}

		/// <summary>
		/// Lấy tile position từ world position
		/// </summary>
		public TilePosition GetTilePosition(Vector3 worldPos)
		{
			var cellPos = WorldToCell(worldPos);
			return new TilePosition(cellPos.x, cellPos.y);
		}

		/// <summary>
		/// Lấy tile position từ mouse position
		/// </summary>
		public TilePosition GetTilePositionFromMouse()
		{
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			mouseWorldPos.z = 0;
			return GetTilePosition(mouseWorldPos);
		}

		// Quản lý layer động
		public void AddLayer(TilemapLayerEntry entry)
		{
			if (entry == null || string.IsNullOrWhiteSpace(entry.key)) return;
			layers.RemoveAll(l => l != null && l.key == entry.key);
			layers.Add(entry);
			RefreshCache();
		}

		public void RemoveLayer(string key)
		{
			layers.RemoveAll(l => l != null && l.key == key);
			RefreshCache();
		}

		public TilemapLayerEntry GetLayer(string key)
		{
			if (string.IsNullOrWhiteSpace(key)) return null;
			_layerByKey.TryGetValue(key, out var e);
			return e;
		}

		public TilemapLayerEntry GetLayerByType(TilemapLayerType type)
		{
			for (int i = 0; i < layers.Count; i++)
			{
				var e = layers[i];
				if (e != null && e.type == type) return e;
			}
			return null;
		}

		private TilemapLayerEntry GetGroundLayer()
		{
			if (!string.IsNullOrWhiteSpace(groundLayerKey))
			{
				var byKey = GetLayer(groundLayerKey);
				if (byKey != null) return byKey;
			}
			return GetLayerByType(TilemapLayerType.Ground);
		}
	}
}

