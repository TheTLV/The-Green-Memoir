namespace TheGreenMemoir.Core.Domain.Enums
{
    /// <summary>
    /// Giai đoạn phát triển của cây trồng
    /// </summary>
    public enum GrowthStage
    {
        Seed,       // Hạt giống
        Sprout,     // Mầm
        Growing,    // Đang lớn
        Mature,     // Trưởng thành (có thể thu hoạch)
        Wilted      // Héo úa
    }
}

