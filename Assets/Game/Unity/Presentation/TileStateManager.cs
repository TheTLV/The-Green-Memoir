using UnityEngine;
using System.Collections.Generic;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Tile State Manager - Quản lý TileStateSO và lookup
    /// Thay thế hardcode tile states bằng SO system
    /// </summary>
    public class TileStateManager : MonoBehaviour
    {
        [Header("Tile State Database")]
        [Tooltip("Danh sách tất cả TileStateSO trong game")]
        [SerializeField] private List<TileStateSO> tileStates = new List<TileStateSO>();

        private Dictionary<string, TileStateSO> _stateById = new Dictionary<string, TileStateSO>();
        private Dictionary<TileStateSO.TileStateType, TileStateSO> _stateByType = new Dictionary<TileStateSO.TileStateType, TileStateSO>();

        private static TileStateManager _instance;
        public static TileStateManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeStates();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeStates()
        {
            _stateById.Clear();
            _stateByType.Clear();

            foreach (var state in tileStates)
            {
                if (state == null) continue;

                if (!string.IsNullOrWhiteSpace(state.stateId))
                {
                    _stateById[state.stateId] = state;
                }

                _stateByType[state.stateType] = state;
            }

            Debug.Log($"TileStateManager initialized: {_stateById.Count} states");
        }

        /// <summary>
        /// Lấy TileStateSO từ state ID
        /// </summary>
        public TileStateSO GetState(string stateId)
        {
            if (string.IsNullOrWhiteSpace(stateId)) return null;
            _stateById.TryGetValue(stateId, out var state);
            return state;
        }

        /// <summary>
        /// Lấy TileStateSO từ state type
        /// </summary>
        public TileStateSO GetStateByType(TileStateSO.TileStateType stateType)
        {
            _stateByType.TryGetValue(stateType, out var state);
            return state;
        }

        /// <summary>
        /// Lấy state tiếp theo dựa trên action
        /// </summary>
        public TileStateSO GetNextStateForAction(TileStateSO currentState, string action)
        {
            if (currentState == null) return null;
            return currentState.GetNextStateForAction(action);
        }

        /// <summary>
        /// Kiểm tra có thể thực hiện action không
        /// </summary>
        public bool CanPerformAction(TileStateSO state, string action)
        {
            if (state == null) return false;

            switch (action.ToLower())
            {
                case "plow": return state.canPlow;
                case "water": return state.canWater;
                case "plant": return state.canPlant;
                case "harvest": return state.canHarvest;
                default: return false;
            }
        }
    }
}

