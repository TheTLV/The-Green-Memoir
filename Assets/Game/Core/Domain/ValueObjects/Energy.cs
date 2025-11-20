namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// Năng lượng của người chơi
    /// </summary>
    public class Energy
    {
        public int Current { get; private set; }
        public int Max { get; }

        public Energy(int max, int current = -1)
        {
            Max = max;
            Current = current == -1 ? max : current;
        }

        /// <summary>
        /// Tiêu thụ năng lượng
        /// </summary>
        public bool Consume(int amount)
        {
            if (amount < 0)
                return false;

            if (Current >= amount)
            {
                Current -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hồi phục năng lượng
        /// </summary>
        public void Restore(int amount)
        {
            if (amount < 0)
                return;

            Current = System.Math.Min(Current + amount, Max);
        }

        /// <summary>
        /// Hồi phục đầy năng lượng
        /// </summary>
        public void RestoreFull()
        {
            Current = Max;
        }

        public bool IsExhausted => Current <= 0;
        public bool IsFull => Current >= Max;
        public float Percentage => Max > 0 ? (float)Current / Max : 0f;
    }
}

