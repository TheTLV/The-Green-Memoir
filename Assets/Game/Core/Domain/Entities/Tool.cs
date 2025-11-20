using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Entities
{
    /// <summary>
    /// Công cụ trong game
    /// </summary>
    public class Tool
    {
        public ToolId Id { get; }
        public string Name { get; }
        public ToolActionType ActionType { get; }
        public int MaxUses { get; }
        public int CurrentUses { get; private set; }

        public bool IsBroken => CurrentUses <= 0;
        public bool CanUse => CurrentUses > 0;
        public float DurabilityPercentage => MaxUses > 0 ? (float)CurrentUses / MaxUses : 0f;

        public Tool(ToolId id, string name, ToolActionType actionType, int maxUses)
        {
            Id = id;
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            ActionType = actionType;
            MaxUses = maxUses;
            CurrentUses = maxUses;
        }

        /// <summary>
        /// Sử dụng công cụ
        /// </summary>
        public bool Use()
        {
            if (!CanUse)
                return false;

            CurrentUses--;
            return true;
        }

        /// <summary>
        /// Sửa chữa/đổ đầy công cụ
        /// </summary>
        public void Refill()
        {
            CurrentUses = MaxUses;
        }

        /// <summary>
        /// Sửa chữa một phần
        /// </summary>
        public void Repair(int amount)
        {
            if (amount > 0)
            {
                CurrentUses = System.Math.Min(CurrentUses + amount, MaxUses);
            }
        }
    }
}

