using System;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Hệ thống sự kiện (Event Bus)
    /// </summary>
    public interface IEventBus
    {
        void Publish<T>(T eventData) where T : IGameEvent;
        void Subscribe<T>(Action<T> handler) where T : IGameEvent;
        void Unsubscribe<T>(Action<T> handler) where T : IGameEvent;
    }

    /// <summary>
    /// Marker interface cho tất cả game events
    /// </summary>
    public interface IGameEvent
    {
    }
}

