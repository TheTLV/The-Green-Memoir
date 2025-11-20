# 🎮 HƯỚNG DẪN TẠO MENU PIXEL ART STYLE VỚI UIStyleSO

## 📝 TỔNG QUAN

Hệ thống UI Style sử dụng **ScriptableObject (UIStyleSO)** để cấu hình style UI một cách linh hoạt:
- ✅ Tạo nhiều theme khác nhau (Pixel, Modern, Fantasy, v.v.)
- ✅ Tự động áp dụng cho tất cả UI elements
- ✅ Dễ thay đổi theme mà không cần sửa code
- ✅ Tương thích với code cũ (fallback nếu không có SO)

---

## 🎨 BƯỚC 1: TẠO UIStyleSO (5 phút)

### 1.1. Download Font Pixel Art

**Font khuyến nghị: Press Start 2P**
- Download: https://fonts.google.com/specimen/Press+Start+2P
- Hoặc: https://www.dafont.com/press-start-2p.font

### 1.2. Import Font vào Unity

1. Kéo file `.ttf` vào Unity: `Assets/Fonts/`
2. Unity sẽ tự động import font

### 1.3. Tạo Font Asset cho TextMeshPro

1. Chọn font vừa import → Inspector
2. **Window → TextMeshPro → Font Asset Creator**
3. **Source Font File:** Chọn font vừa import
4. Click **"Generate Font Atlas"**
5. Save as: `PressStart2P_SDF`

### 1.4. Tạo UIStyleSO

1. **Project → Right-click → Create → Game → UI Style** → Đặt tên `PixelArtStyle`
2. Inspector → Cấu hình:

#### **Style Info:**
- `Style Name`: "Pixel Art"
- `Description`: "Pixel art style UI với màu xanh lá"

#### **Font Settings:**
- `Font`: Kéo `PressStart2P_SDF` vào
- `Font Size`: 28
- `Character Spacing`: 3
- `Line Spacing`: 1.2

#### **Button Colors:**
- `Button Normal Color`: #66B266 (RGB: 102, 178, 102)
- `Button Highlighted Color`: #7BC47B (RGB: 123, 196, 123)
- `Button Pressed Color`: #4A8F4A (RGB: 74, 143, 74)
- `Button Selected Color`: #66B266
- `Button Disabled Color`: #808080

#### **Text Colors:**
- `Button Text Color`: #336633 (RGB: 51, 102, 51)
- `Normal Text Color`: #E6E6E6
- `Title Text Color`: #FFFFFF

#### **Background Colors:**
- `Background Color`: #0D2B0D (RGB: 13, 43, 13)
- `Panel Background Color`: #1A401A

#### **Shadow/Border Effects:**
- `Shadow Color`: #D2B48C (RGB: 210, 180, 140)
- `Shadow Distance`: (2, -2)
- `Use Shadow`: ✓

#### **Advanced Effects (Optional):**
- `Use Hover Animation`: ✓ (nếu muốn)
- `Hover Scale`: 1.05
- `Use Flash Effect`: ✓ (nếu muốn)
- `Flash Color`: #FFFFCC

---

## 🛠️ BƯỚC 2: TẠO MENU UI (10 phút)

### 2.1. Tạo Canvas

1. **Hierarchy → Create → UI → Canvas**
2. Đặt tên: `MainMenuCanvas`

### 2.2. Tạo Background

1. **Canvas → Create → UI → Image** → Đặt tên `Background`
2. Inspector:
   - `Color`: #0D2B0D (hoặc để UIStyleApplier tự set)
   - Tag: "Background" (để UIStyleApplier nhận diện)

### 2.3. Tạo Buttons

1. **Canvas → Create → UI → Button** → Đặt tên `ResumeButton`
2. Inspector → Button:
   - `Normal Color`: (sẽ tự động set từ UIStyleSO)
3. **Button → Text (hoặc TextMeshPro)** → Đặt text: "RESUME"
4. **Duplicate button** (Ctrl+D) → Tạo các nút khác:
   - `RestartButton`: "RESTART"
   - `SettingsButton`: "SETTINGS"
   - `LevelsButton`: "LEVELS"
   - `InventoryButton`: "INVENTORY"
   - `EquipmentButton`: "EQUIPMENT"
   - `ShopButton`: "SHOP"
   - `CraftButton`: "CRAFT"
   - `QuitButton`: "QUIT"
   - `PlayButton`: "PLAY"
   - `LoadButton`: "LOAD"

### 2.4. Sắp xếp Buttons

**Option A: Dùng Vertical Layout Group**
1. **Canvas → Create Empty** → Đặt tên `ButtonContainer`
2. **Add Component → Vertical Layout Group**
3. Inspector:
   - `Spacing`: 10
   - `Padding`: Left/Right/Top/Bottom = 20
   - `Child Alignment`: Middle Center
4. Kéo tất cả buttons vào `ButtonContainer`

**Option B: Sắp xếp thủ công**
- Sắp xếp buttons theo chiều dọc, cách nhau 10-15 pixels

---

## ⚙️ BƯỚC 3: GẮN UIStyleApplier (2 phút)

### 3.1. Gắn Component

1. Chọn **Canvas** (hoặc GameObject chứa UI)
2. **Add Component → UIStyleApplier**
3. Inspector:
   - `UI Style`: Kéo `PixelArtStyle` vào
   - `Apply On Start`: ✓
   - `Apply On Validate`: ✓ (chỉ trong Editor)
   - `Apply To Child Canvases`: ✓

### 3.2. Test

1. **Play** → UI sẽ tự động apply style từ UIStyleSO
2. Nếu thay đổi UIStyleSO trong Inspector → UI sẽ tự cập nhật (nếu `Apply On Validate` = ✓)

---

## 🎯 BƯỚC 4: GẮN PixelArtMenuController (3 phút)

### 4.1. Gắn Component

1. Chọn **Canvas** (hoặc GameObject chứa menu)
2. **Add Component → PixelArtMenuController**
3. Inspector:
   - `UI Style`: Kéo `PixelArtStyle` vào (optional - nếu có UIStyleApplier thì không cần)
   - Link các buttons vào:
     - `Resume Button`: Kéo `ResumeButton` vào
     - `Restart Button`: Kéo `RestartButton` vào
     - `Settings Button`: Kéo `SettingsButton` vào
     - ... (các buttons khác)

### 4.2. Test Menu

1. **Play** → Click các buttons
2. Kiểm tra console log để xem buttons hoạt động

---

## 🎨 TẠO NHIỀU THEME (5 phút)

### Tạo Theme "Modern"

1. **Project → Right-click → Create → Game → UI Style** → `ModernStyle`
2. Inspector:
   - `Style Name`: "Modern"
   - `Font`: Font sans-serif hiện đại
   - `Button Normal Color`: #2196F3 (xanh dương)
   - `Button Text Color`: #FFFFFF (trắng)
   - `Background Color`: #F5F5F5 (xám nhạt)
   - `Use Shadow`: false
   - `Use Hover Animation`: ✓
   - `Hover Scale`: 1.1

### Tạo Theme "Fantasy"

1. **Project → Right-click → Create → Game → UI Style** → `FantasyStyle`
2. Inspector:
   - `Style Name`: "Fantasy"
   - `Font`: Font fantasy (Old English, Medieval, v.v.)
   - `Button Normal Color`: #8B4513 (nâu)
   - `Button Text Color`: #FFD700 (vàng)
   - `Background Color`: #2F1B14 (nâu đậm)
   - `Use Border`: ✓
   - `Border Color`: #D4AF37 (vàng đậm)

### Chuyển đổi Theme

1. Chọn **Canvas** → Inspector → `UIStyleApplier`
2. `UI Style`: Kéo theme mới vào (ModernStyle, FantasyStyle, v.v.)
3. **Play** → UI sẽ tự động apply theme mới

---

## 🔧 CÁCH SỬ DỤNG NÂNG CAO

### 1. Apply Style cho Prefab UI

1. Tạo prefab UI (Menu, Inventory, Shop, v.v.)
2. Gắn **UIStyleApplier** vào prefab
3. Kéo UIStyleSO vào prefab
4. Mỗi prefab có thể dùng theme riêng

### 2. Runtime Change Theme

```csharp
// Trong script của bạn
using TheGreenMemoir.Unity.UI;
using TheGreenMemoir.Unity.Data;

public class ThemeSwitcher : MonoBehaviour
{
    public UIStyleSO pixelTheme;
    public UIStyleSO modernTheme;
    
    private UIStyleApplier styleApplier;
    
    void Start()
    {
        styleApplier = GetComponent<UIStyleApplier>();
    }
    
    public void SwitchToPixelTheme()
    {
        styleApplier.SetStyle(pixelTheme);
    }
    
    public void SwitchToModernTheme()
    {
        styleApplier.SetStyle(modernTheme);
    }
}
```

### 3. Tạo UI Theme Collection

1. Tạo folder: `Assets/UIThemes/`
2. Tạo các UIStyleSO:
   - `PixelArtStyle`
   - `ModernStyle`
   - `FantasyStyle`
   - `SciFiStyle`
   - v.v.
3. Tạo script quản lý collection:

```csharp
[CreateAssetMenu(fileName = "UIThemeCollection", menuName = "Game/UI Theme Collection")]
public class UIThemeCollectionSO : ScriptableObject
{
    public UIStyleSO[] themes;
    
    public UIStyleSO GetTheme(string themeName)
    {
        return System.Array.Find(themes, t => t != null && t.styleName == themeName);
    }
}
```

---

## 📋 CHECKLIST SETUP

- [ ] Đã download font pixel art (Press Start 2P)
- [ ] Đã import font vào Unity
- [ ] Đã tạo Font Asset cho TextMeshPro
- [ ] Đã tạo UIStyleSO (PixelArtStyle)
- [ ] Đã cấu hình UIStyleSO (màu sắc, font, effects)
- [ ] Đã tạo Canvas và Background
- [ ] Đã tạo các Buttons
- [ ] Đã gắn UIStyleApplier vào Canvas
- [ ] Đã kéo UIStyleSO vào UIStyleApplier
- [ ] Đã gắn PixelArtMenuController (nếu cần)
- [ ] Đã test menu hoạt động
- [ ] Đã test thay đổi theme

---

## 💡 MẸO NHỎ

### Nếu không có font:
- Dùng font mặc định của Unity
- Tăng `Character Spacing` để giống pixel art
- Giảm `Font Size` và tăng `Resolution`

### Nếu UI không apply style:
- Kiểm tra UIStyleSO đã được kéo vào UIStyleApplier chưa
- Kiểm tra `Apply On Start` = ✓
- Kiểm tra buttons có TextMeshPro hoặc Text component chưa

### Nếu muốn tùy chỉnh từng button:
- Tắt `Apply On Start` trong UIStyleApplier
- Tự set style cho từng button thủ công
- Hoặc dùng `Target Buttons` array trong UIStyleApplier

---

## 🎨 MÀU SẮC CHUẨN (Pixel Art Style)

### Màu nút:
- **Normal:** #66B266 (RGB: 102, 178, 102)
- **Highlighted:** #7BC47B (RGB: 123, 196, 123)
- **Pressed:** #4A8F4A (RGB: 74, 143, 74)

### Màu chữ:
- **Button Text:** #336633 (RGB: 51, 102, 51)
- **Normal Text:** #E6E6E6
- **Title Text:** #FFFFFF

### Màu nền:
- **Background:** #0D2B0D (RGB: 13, 43, 13)
- **Panel:** #1A401A

### Màu shadow:
- **Shadow:** #D2B48C (RGB: 210, 180, 140)

---

## 🚀 TƯƠNG THÍCH VỚI CODE CŨ

### Nếu không có UIStyleSO:
- `PixelArtMenuController` sẽ dùng giá trị mặc định (fallback)
- `UIStyleApplier` sẽ tìm UIStyleSO trong Resources hoặc AssetDatabase
- Nếu không tìm thấy, sẽ log warning nhưng không crash

### Migration từ code cũ:
1. Tạo UIStyleSO với giá trị giống code cũ
2. Gắn UIStyleApplier vào Canvas
3. Kéo UIStyleSO vào
4. **XONG!** Code cũ vẫn hoạt động, nhưng giờ có thể custom qua SO

---

**Chúc bạn tạo được menu đẹp với UIStyleSO! 🎮**
