using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Core.Application.States
{
    /// <summary>
    /// Game State Machine - State Pattern
    /// Quản lý chuyển đổi giữa các game states
    /// </summary>
    public class GameStateMachine
    {
        private IGameState _currentState;
        private Dictionary<string, IGameState> _states = new Dictionary<string, IGameState>();

        public IGameState CurrentState => _currentState;

        /// <summary>
        /// Đăng ký state
        /// </summary>
        public void RegisterState(IGameState state)
        {
            if (state == null)
            {
                Debug.LogError("Cannot register null state");
                return;
            }

            _states[state.StateName] = state;
        }

        /// <summary>
        /// Chuyển sang state mới
        /// </summary>
        public bool ChangeState(string stateName)
        {
            if (!_states.ContainsKey(stateName))
            {
                Debug.LogError($"State '{stateName}' not found");
                return false;
            }

            var newState = _states[stateName];

            if (_currentState != null)
            {
                if (!_currentState.CanTransitionTo(newState))
                {
                    Debug.LogWarning($"Cannot transition from {_currentState.StateName} to {newState.StateName}");
                    return false;
                }

                _currentState.Exit();
            }

            _currentState = newState;
            _currentState.Enter();

            Debug.Log($"GameState changed to: {stateName}");
            return true;
        }

        /// <summary>
        /// Update current state
        /// </summary>
        public void Update()
        {
            _currentState?.Update();
        }

        /// <summary>
        /// Lấy state theo tên
        /// </summary>
        public IGameState GetState(string stateName)
        {
            return _states.ContainsKey(stateName) ? _states[stateName] : null;
        }
    }
}

