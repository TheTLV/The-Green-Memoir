using System;
using System.Collections.Generic;
using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Core.Infrastructure.Services
{
    /// <summary>
    /// Implementation của EventBus
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public void Publish<T>(T eventData) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (_handlers.ContainsKey(eventType))
            {
                // Tạo bản sao để tránh lỗi khi handler modify collection
                var handlers = new List<Delegate>(_handlers[eventType]);

                foreach (var handler in handlers)
                {
                    try
                    {
                        ((Action<T>)handler)(eventData);
                    }
                    catch (Exception ex)
                    {
                        // Log error nhưng không throw để không ảnh hưởng các handler khác
                        UnityEngine.Debug.LogError($"Error in event handler: {ex.Message}");
                    }
                }
            }
        }

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<Delegate>();
            }

            _handlers[eventType].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(handler);
            }
        }
    }
}

