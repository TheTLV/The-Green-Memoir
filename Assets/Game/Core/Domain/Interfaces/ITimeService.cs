using System;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Dịch vụ quản lý thời gian game
    /// </summary>
    public interface ITimeService
    {
        int CurrentDay { get; }
        int CurrentHour { get; }
        int CurrentMinute { get; }

        event Action OnDayChanged;
        event Action OnHourChanged;
        event Action OnMinuteChanged;
    }
}

