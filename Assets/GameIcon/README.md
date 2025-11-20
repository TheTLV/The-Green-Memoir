# Game Icon Folder

## 📁 Thư mục này dùng để chứa:
- **Game Icon** (.ico, .png) - Icon hiển thị khi build game thành .exe
- **Shortcut Icon** - Icon cho shortcut trên desktop

## 🎨 Cách tạo Game Icon:

### 1. Tạo Icon trong Unity:
1. Project → Right-click → Create → Texture2D
2. Import hình ảnh icon (khuyến nghị: 512x512 hoặc 1024x1024, PNG với alpha)
3. Inspector → Texture Type: `Editor GUI and Legacy GUI`
4. Inspector → Max Size: `1024` (hoặc lớn hơn)
5. Inspector → Compression: `None` (để chất lượng tốt nhất)

### 2. Set Icon cho Build:
1. Edit → Project Settings → Player
2. Tab `Icon` → kéo icon vào `Default Icon`
3. Unity tự động tạo các size khác nhau cho các platform

### 3. Tạo Shortcut Icon (.ico):
- Dùng tool online: https://convertio.co/png-ico/
- Hoặc dùng Photoshop/GIMP để export .ico
- Đặt file .ico vào thư mục này

## 📝 Lưu ý:
- Icon nên có kích thước vuông (1:1)
- Nền trong suốt (alpha channel) sẽ hiển thị đẹp hơn
- Test icon trên desktop để đảm bảo nhìn rõ ở size nhỏ

