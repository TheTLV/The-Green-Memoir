# 🎨 HƯỚNG DẪN SỬ DỤNG SPRITE SHEET CHO SETTINGS UI

## 📋 MỤC TIÊU

Sử dụng sprite sheet để tạo Settings UI với:
- **Speaker Icon** (icon loa) cho Audio volume
- **Music Note Icon** (icon nốt nhạc) cho BGM volume
- **Volume Blocks** (các block nâu) để hiển thị volume level

---

## 🔧 BƯỚC 1: IMPORT SPRITE SHEET VÀO UNITY

1. **Import sprite sheet:**
   - Kéo file sprite sheet vào `Assets/Game/Unity/Sprites/` (hoặc folder bạn muốn)
   - Unity sẽ tự động import

2. **Setup Import Settings:**
   - Chọn sprite sheet trong Project
   - Inspector → Texture Type: **Sprite (2D and UI)**
   - Inspector → Sprite Mode: **Multiple** (quan trọng!)
   - Inspector → Pixels Per Unit: 16 (hoặc tùy chỉnh)
   - Inspector → Filter Mode: **Point (no filter)** (để giữ pixel art sắc nét)
   - Click **Apply**

---

## ✂️ BƯỚC 2: SLICE SPRITE SHEET

1. **Mở Sprite Editor:**
   - Chọn sprite sheet → Inspector → Click **Sprite Editor**

2. **Slice sprites:**
   - Trong Sprite Editor → Click **Slice** (góc trên bên phải)
   - Method: **Grid By Cell Count** hoặc **Automatic**
   - Nếu dùng Grid By Cell Count:
     - Column & Row: Đếm số cột và hàng trong sprite sheet của bạn
     - Pixel Size: Kích thước mỗi sprite (ví dụ: 16x16)
   - Click **Slice**

3. **Đặt tên và lưu:**
   - Trong Sprite Editor, đặt tên cho từng sprite:
     - `SpeakerIcon` (icon loa)
     - `MusicNoteIcon` (icon nốt nhạc)
     - `VolumeBlock` (block nâu cho volume slider)
   - Click **Apply** (góc trên bên phải)
   - Đóng Sprite Editor

---

## 🎯 BƯỚC 3: TẠO SPRITE ASSETS

1. **Tạo Speaker Icon:**
   - Trong Project, tìm sprite `SpeakerIcon` (đã slice)
   - Right-click → Create → Sprite (hoặc giữ nguyên)

2. **Tạo Music Note Icon:**
   - Tìm sprite `MusicNoteIcon`
   - Right-click → Create → Sprite

3. **Tạo Volume Block:**
   - Tìm sprite `VolumeBlock` (block nâu)
   - Right-click → Create → Sprite

---

## 🏗️ BƯỚC 4: TẠO UI TRONG SCENE

### 4.1. Tạo Settings Panel (nếu chưa có)

1. **Trong scene `MainMenu`:**
   - Chọn `Canvas` → Right-click → UI → Panel → đặt tên `SettingsPanel`
   - Inspector → GameObject → Active: **BỎ TICK** (ẩn mặc định)

### 4.2. Tạo Audio Volume UI

1. **Tạo Container cho Audio Volume:**
   - Chọn `SettingsPanel` → Right-click → Create Empty → đặt tên `AudioVolumeContainer`
   - Inspector → RectTransform: Set anchor và position

2. **Tạo Speaker Icon:**
   - Chọn `AudioVolumeContainer` → Right-click → UI → Image → đặt tên `AudioIcon`
   - Inspector → Image:
     - Source Image: Kéo `SpeakerIcon` sprite vào
     - Set Native Size (để giữ kích thước gốc)

3. **Tạo Volume Slider UI:**
   - Chọn `AudioVolumeContainer` → Right-click → Create Empty → đặt tên `AudioVolumeSliderUI`
   - Inspector → Add Component → `VolumeSliderUI`
   - Inspector → VolumeSliderUI:
     - `Block Prefab`: (để trống, sẽ tạo sau)
     - `Max Blocks`: 10
     - `Spacing`: 2
     - `Horizontal`: ✓ (tick)

4. **Tạo Block Prefab:**
   - Hierarchy → Right-click → Create Empty → đặt tên `VolumeBlock`
   - Inspector → Add Component → Image
   - Inspector → Image:
     - Source Image: Kéo `VolumeBlock` sprite vào
     - Set Native Size
   - Kéo `VolumeBlock` vào Project để tạo Prefab
   - Xóa `VolumeBlock` trong Hierarchy (giữ prefab)

5. **Link Block Prefab vào VolumeSliderUI:**
   - Chọn `AudioVolumeSliderUI` → Inspector → VolumeSliderUI → `Block Prefab`: Kéo prefab `VolumeBlock` vào

### 4.3. Tạo BGM Volume UI (tương tự)

1. **Tạo Container cho BGM Volume:**
   - Chọn `SettingsPanel` → Right-click → Create Empty → đặt tên `BGMVolumeContainer`

2. **Tạo Music Note Icon:**
   - Chọn `BGMVolumeContainer` → Right-click → UI → Image → đặt tên `BGMIcon`
   - Inspector → Image → Source Image: Kéo `MusicNoteIcon` sprite vào

3. **Tạo Volume Slider UI:**
   - Chọn `BGMVolumeContainer` → Right-click → Create Empty → đặt tên `BGMVolumeSliderUI`
   - Inspector → Add Component → `VolumeSliderUI`
   - Inspector → VolumeSliderUI:
     - `Block Prefab`: Kéo prefab `VolumeBlock` vào
     - `Max Blocks`: 10
     - `Spacing`: 2
     - `Horizontal`: ✓ (tick)

### 4.4. Tạo Buttons (Optional - nếu muốn dùng button thay vì slider)

1. **Tạo Increase/Decrease Buttons:**
   - Chọn `AudioVolumeContainer` → Right-click → UI → Button → đặt tên `AudioIncreaseButton`
   - Chọn `AudioVolumeContainer` → Right-click → UI → Button → đặt tên `AudioDecreaseButton`
   - Tương tự cho BGM

---

## 🔗 BƯỚC 5: LINK VÀO SETTINGS CONTROLLER

1. **Gắn SettingsController:**
   - Chọn `SettingsPanel` (hoặc `Canvas`) → Inspector → Add Component → `SettingsController`

2. **Link References:**
   - Inspector → SettingsController:
     - `Audio Volume Slider UI`: Kéo `AudioVolumeSliderUI` vào
     - `Audio Icon`: Kéo `AudioIcon` vào
     - `BGM Volume Slider UI`: Kéo `BGMVolumeSliderUI` vào
     - `BGM Icon`: Kéo `BGMIcon` vào
     - `Settings Panel`: Kéo `SettingsPanel` vào
     - `Back Button`: Kéo `BackButton` vào (nếu có)

3. **Link Buttons (nếu có):**
   - `AudioIncreaseButton` → OnClick → Kéo `SettingsController` → `IncreaseAudioVolume()`
   - `AudioDecreaseButton` → OnClick → Kéo `SettingsController` → `DecreaseAudioVolume()`
   - Tương tự cho BGM buttons

---

## 🎮 BƯỚC 6: TEST

1. **Play game:**
   - Chạy scene `MainMenu`
   - Click "Cài Đặt" → SettingsPanel hiện ra

2. **Test volume:**
   - Nếu dùng buttons: Click Increase/Decrease → Blocks hiện/ẩn
   - Nếu dùng slider: Kéo slider → Blocks tự động update

3. **Kiểm tra:**
   - Audio volume thay đổi → Blocks update
   - BGM volume thay đổi → Blocks update
   - Volume được lưu (restart game → volume vẫn giữ nguyên)

---

## 💡 MẸO

### Nếu không có sprite sheet:
- Dùng Image → Color để tạo màu nâu cho blocks
- Dùng TextMeshPro để viết "🔊" và "🎵" làm icon tạm

### Nếu muốn dùng Slider thông thường:
- Thay `VolumeSliderUI` bằng Unity Slider
- Link vào `SettingsController` → `Audio Volume Slider` hoặc `BGM Volume Slider`

### Nếu muốn dùng cả Slider và Blocks:
- Có thể dùng cả 2 cùng lúc!
- Slider để điều chỉnh, Blocks để hiển thị

---

## 📝 TÓM TẮT

1. ✅ Import sprite sheet → Setup Multiple sprites
2. ✅ Slice sprite sheet → Lấy SpeakerIcon, MusicNoteIcon, VolumeBlock
3. ✅ Tạo VolumeBlock prefab
4. ✅ Tạo UI: AudioIcon, AudioVolumeSliderUI, BGMIcon, BGMVolumeSliderUI
5. ✅ Link vào SettingsController
6. ✅ Test!

---

**CHÚC BẠN THÀNH CÔNG! 🎨🎮**

