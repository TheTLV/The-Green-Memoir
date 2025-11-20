using TheGreenMemoir.Core.Domain.Interfaces;

namespace TheGreenMemoir.Core.Application.Events
{
    /// <summary>
    /// Sự kiện khi ngày mới bắt đầu
    /// </summary>
    public class DayChangedEvent : IGameEvent
    {
        public int NewDay { get; }
        public int PreviousDay { get; }

        public DayChangedEvent(int newDay, int previousDay)
        {
            NewDay = newDay;
            PreviousDay = previousDay;
        }
    }
}

