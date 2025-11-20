using System;
using UnityEngine;
using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Unity implementation của ITimeService
    /// Có thể pause/control time (không phải thời gian thực)
    /// </summary>
    public class TimeManager : MonoBehaviour, ITimeService
    {
        [Header("Time Settings")]
        [SerializeField] private float _minutesPerDay = 20f; // 20 phút thực = 1 ngày game
        [SerializeField] private int _startDay = 1;
        [SerializeField] private int _startHour = 6;
        [SerializeField] private int _startMinute = 0;

        [Header("Time Control")]
        [SerializeField] private bool _isPaused = false; // Pause time
        [SerializeField] private float _timeScale = 1f; // Time scale (1 = normal, 2 = 2x speed, 0.5 = 0.5x speed)

        public int CurrentDay { get; private set; } = 1;
        public int CurrentHour { get; private set; } = 6;
        public int CurrentMinute { get; private set; } = 0;

        public event Action OnDayChanged;
        public event Action OnHourChanged;
        public event Action OnMinuteChanged;

        private float _currentTimeInMinutes = 0f;
        private int _lastHour = -1;
        private int _lastMinute = -1;
        private int _lastDay = -1;

        private void Start()
        {
            CurrentDay = _startDay;
            CurrentHour = _startHour;
            CurrentMinute = _startMinute;
            _currentTimeInMinutes = _startHour * 60f + _startMinute;
            _lastHour = CurrentHour;
            _lastMinute = CurrentMinute;
            _lastDay = CurrentDay;
        }

        private void Update()
        {
            if (!_isPaused)
            {
                UpdateTime(Time.deltaTime * _timeScale);
            }
        }

        private void UpdateTime(float deltaTime)
        {
            // Tính thời gian trong ngày (tính bằng phút)
            float minutesPerSecond = (24f * 60f) / (_minutesPerDay * 60f);
            _currentTimeInMinutes += deltaTime * minutesPerSecond;

            // Chuyển đổi sang giờ và phút
            int totalMinutes = Mathf.FloorToInt(_currentTimeInMinutes);
            CurrentHour = (totalMinutes / 60) % 24;
            CurrentMinute = totalMinutes % 60;

            // Kiểm tra chuyển ngày
            if (totalMinutes >= 24 * 60)
            {
                _currentTimeInMinutes = 0f;
                CurrentDay++;
                CurrentHour = 0;
                CurrentMinute = 0;
            }

            // Fire events
            if (CurrentDay != _lastDay)
            {
                _lastDay = CurrentDay;
                OnDayChanged?.Invoke();
            }

            if (CurrentHour != _lastHour)
            {
                _lastHour = CurrentHour;
                OnHourChanged?.Invoke();
            }

            if (CurrentMinute != _lastMinute)
            {
                _lastMinute = CurrentMinute;
                OnMinuteChanged?.Invoke();
            }
        }

        /// <summary>
        /// Set thời gian (dùng cho testing hoặc load game)
        /// </summary>
        public void SetTime(int day, int hour, int minute)
        {
            CurrentDay = day;
            CurrentHour = hour;
            CurrentMinute = minute;
            _currentTimeInMinutes = hour * 60f + minute;
            _lastHour = hour;
            _lastMinute = minute;
            _lastDay = day;
        }

        /// <summary>
        /// Pause time (dừng thời gian game)
        /// </summary>
        public void PauseTime()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resume time (tiếp tục thời gian game)
        /// </summary>
        public void ResumeTime()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Toggle pause
        /// </summary>
        public void TogglePause()
        {
            _isPaused = !_isPaused;
        }

        /// <summary>
        /// Set time scale (1 = normal, 2 = 2x speed, 0.5 = 0.5x speed)
        /// </summary>
        public void SetTimeScale(float scale)
        {
            _timeScale = Mathf.Clamp(scale, 0f, 10f); // Giới hạn từ 0 đến 10x
        }

        /// <summary>
        /// Get time scale
        /// </summary>
        public float GetTimeScale()
        {
            return _timeScale;
        }

        /// <summary>
        /// Check if time is paused
        /// </summary>
        public bool IsPaused()
        {
            return _isPaused;
        }
    }
}

