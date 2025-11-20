using UnityEngine;
using TheGreenMemoir.Core.Application.States;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Unity.States;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Game State Manager - Quản lý Game State Machine
    /// Singleton pattern + State Pattern
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager _instance;
        public static GameStateManager Instance => _instance;

        private GameStateMachine _stateMachine;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            _stateMachine = new GameStateMachine();

            // Lấy EventBus từ GameManager
            var eventBus = GameManager.EventBus;
            if (eventBus == null)
            {
                Debug.LogError("EventBus not initialized! Make sure GameManager is initialized first.");
                return;
            }

            // Đăng ký các states
            _stateMachine.RegisterState(new MenuGameState(eventBus));
            _stateMachine.RegisterState(new PlayingGameState(eventBus));
            _stateMachine.RegisterState(new DialogueGameState());

            // Mặc định bắt đầu ở Menu
            _stateMachine.ChangeState("Menu");

            Debug.Log("GameStateManager initialized");
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        /// <summary>
        /// Chuyển sang state mới
        /// </summary>
        public bool ChangeState(string stateName)
        {
            return _stateMachine?.ChangeState(stateName) ?? false;
        }

        /// <summary>
        /// Lấy state hiện tại
        /// </summary>
        public IGameState GetCurrentState()
        {
            return _stateMachine?.CurrentState;
        }

        /// <summary>
        /// Kiểm tra xem có đang ở state nào không
        /// </summary>
        public bool IsInState(string stateName)
        {
            return _stateMachine?.CurrentState?.StateName == stateName;
        }
    }
}

