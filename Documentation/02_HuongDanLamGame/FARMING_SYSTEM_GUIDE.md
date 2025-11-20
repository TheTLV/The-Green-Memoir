# 🌾 HỆ THỐNG FARMING - HƯỚNG DẪN ĐẦY ĐỦ

## 📋 Tổng Quan

Hướng dẫn đầy đủ về hệ thống farming: cây trồng, tiles, và rendering.

---

## 🌱 PHẦN 1: HỆ THỐNG CÂY TRỒNG

### **Logic Thực Tế:**
- ✅ **Tưới nước** → Cây lớn lên qua các ngày
- ✅ **Không tưới nước** → Cây không lớn, sau vài ngày sẽ héo
- ✅ **Đơn giản**: Chỉ cần 1 sprite là đủ!

### **Cách Setup CropDataSO:**

1. **Tạo CropDataSO:**
   - Right Click → `Create` → `Game` → `Crop Data`

2. **Thêm Growth Sprites (Bắt buộc):**
   - Mở rộng "Visual - Growth Sprites"
   - Click "+" để thêm sprite
   - Với mỗi sprite:
     - Kéo sprite vào field `Sprite`
     - Đặt `Day To Show`: số ngày để hiển thị (0, 1, 3, 5, ...)
     - Đặt `Description`: tên mô tả
     - **Tick `Is Mature`** nếu là sprite trưởng thành (có thể thu hoạch)

3. **Ví dụ:**
   ```
   Growth Sprites:
   - Sprite 1: dayToShow = 0, description = "Seed"
   - Sprite 2: dayToShow = 1, description = "Sprout"
   - Sprite 3: dayToShow = 3, description = "Growing"
   - Sprite 4: dayToShow = 5, description = "Mature", isMature = ✅
   ```

4. **Tùy chọn:**
   - Wet Sprites: Nếu có sprite riêng cho cây đã tưới
   - Wilted Sprites: Nếu có sprite riêng cho cây héo

### **Hiển Thị Icon Khi Trưởng Thành:**

1. **Tạo GameObject cho icon:**
   - Tạo GameObject: `HarvestIcon`
   - Thêm sprite/animation

2. **Attach Script:**
   - Add Component: `CropMatureIndicator`
   - Kéo icon GameObject vào "Indicator Object"
   - Đặt Offset: `(0, 1, 0)` để icon ở trên đầu cây

3. **Tự động:**
   - Script tự động hiển thị/ẩn khi cây trưởng thành

---

## 🎨 PHẦN 2: RENDERING TILES

### **Giải Pháp: Overlay Tilemap (Cho Background Lớn)**

**Tình huống:** Background lớn (ground + pair), không muốn cắt thành 2000+ tiles.

**Giải pháp:**
1. **Giữ nguyên background lớn** - Không cắt
2. **Tạo Tilemap overlay** - Chỉ cho khu ruộng
3. **Chỉ render tiles trong khu ruộng**

### **Setup:**

1. **Background (Giữ Nguyên):**
   - Kéo 2 sprite lớn (ground + pair) vào scene
   - Add SpriteRenderer
   - **KHÔNG CẮT, GIỮ NGUYÊN**

2. **Grid:**
   - Tạo GameObject: `Grid`
   - Add Component: `Grid`
   - Cell Size: `(2.88, 2.88, 0)` (cho 288px tiles)

3. **FarmArea Tilemap:**
   - Tạo GameObject: `FarmArea` (dưới Grid)
   - Add Component: `Tilemap` + `TilemapRenderer`
   - **CHỈ vẽ tiles trong khu ruộng** (ví dụ: 10x10 tiles)

4. **FarmAreaRenderer:**
   - Tạo GameObject: `FarmAreaRenderer`
   - Add Component: `FarmAreaRenderer`
   - Kéo references:
     - Background Ground
     - Background Pair
     - Farm Tilemap
     - Grid
   - Tự động detect bounds từ Tilemap

### **Tạo Tile Assets:**

1. **Cắt ảnh ground tiles** (chỉ phần khu ruộng):
   - Normal, Plowed, Watered, Seeded, Growing, Mature
   - Kích thước: 288x288 mỗi tile

2. **Tạo Tile Assets:**
   - Right Click → `Create` → `2D` → `Tiles` → `Tile`
   - Kéo sprite vào
   - Lưu: `NormalTile`, `PlowedTile`, etc.

3. **Gán vào TileStateSO:**
   - Mở TileStateSO
   - Kéo Tile asset vào `Tile Base`

---

## 💻 SỬ DỤNG TRONG CODE

### **Lấy Sprite Cây:**
```csharp
var cropData = // ... lấy từ database
var crop = // ... lấy từ FarmTile

// Cách 1: Tự động (dễ nhất)
Sprite sprite = cropData.GetSpriteForCrop(crop);

// Cách 2: Chi tiết
Sprite sprite = cropData.GetSpriteForDay(
    daysPlanted: crop.DaysPlanted,
    isWateredToday: crop.IsWateredToday,
    daysSinceWatered: crop.DaysSinceWatered,
    daysToWilt: crop.DaysToWilt
);
```

### **Kiểm Tra Cây Trưởng Thành:**
```csharp
bool isMature = cropData.IsCurrentSpriteMature(crop);
if (isMature)
{
    // Hiển thị icon/animation
}
```

### **Mark Tile Cần Update:**
```csharp
// Tự động qua events (không cần code)
// Hoặc manual:
FarmAreaRenderer.Instance?.MarkTileDirty(position);
```

---

## ✅ TÓM TẮT

### **Cây Trồng:**
- Chỉ cần thêm **Growth Sprites** (1 sprite là đủ!)
- Đánh dấu sprite trưởng thành bằng `isMature`
- Dùng `CropMatureIndicator` để hiển thị icon

### **Rendering:**
- Background lớn: **Giữ nguyên**, không cắt
- Farm Area: **Overlay Tilemap**, chỉ khu ruộng
- Tự động: `FarmAreaRenderer` xử lý tất cả

### **Performance:**
- Background: 2 draw calls (static)
- Farm Area: ~100 tiles (chỉ khu ruộng)
- Chỉ update tiles thay đổi (dirty flag)

---

**Cập nhật:** 2024

