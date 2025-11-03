// Đặt file này ở nơi dễ truy cập (ví dụ: trong thư mục Scripts/Enums)
public enum PlayerState
{
    // Trạng thái Di chuyển (đã có trong hình)
    IdleDown,
    IdleLeft,
    IdleRight,
    IdleUp,
    RunDown,
    RunLeft,
    RunRight,
    RunUp,
    WalkDown,
    WalkLeft,
    WalkRight,
    WalkUp,

    // Trạng thái Hành động (sẽ thêm sau)
    // Các hành động này thường là trạng thái riêng biệt,
    // khi thực hiện sẽ tạm dừng di chuyển/chạy/đứng yên.
    Hoe,        // Cuốc đất
    Water,      // Tưới cây
    Harvest,    // Thu hoạch
    Chop,       // Chặt cây
    // ... và các hành động khác
}