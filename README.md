# 🌱 The Green Memoir - Farming Game

Game nông trại 2D top-down được phát triển với Unity, tuân thủ nguyên tắc SOLID và hướng đối tượng.

---

## 📋 Mục Lục

- [Giới Thiệu](#giới-thiệu)
- [Tính Năng](#tính-năng)
- [Cài Đặt](#cài-đặt)
- [Cách Chơi](#cách-chơi)
- [Cấu Trúc Code](#cấu-trúc-code)
- [Yêu Cầu Hệ Thống](#yêu-cầu-hệ-thống)
- [Công Nghệ Sử Dụng](#công-nghệ-sử-dụng)
- [Đóng Góp](#đóng-góp)
- [License](#license)

---

## 🎮 Giới Thiệu

**The Green Memoir** là game nông trại 2D top-down, nơi người chơi vào vai một học sinh thành phố trở về quê, thừa kế mảnh đất của ông nội và từng bước khôi phục nông trại.

Game được xây dựng với kiến trúc mở rộng, hỗ trợ:
- ✅ Nhiều map (hiện tại 1 map demo)
- ✅ Tilemap nhiều layer
- ✅ Hệ thống inventory với tags và filter
- ✅ NPC shop system
- ✅ Tool interaction system
- ✅ Seed selection UI
- ✅ Action menu khi chọn item

---

## ✨ Tính Năng

### **Hiện Tại (Demo)**

- ✅ **Trồng Cây**: Cuốc đất → Gieo hạt → Tưới nước → Thu hoạch
- ✅ **Cuốc Đất**: Sử dụng cuốc để chuẩn bị đất trồng
- ✅ **Thu Hoạch**: Thu hoạch cây trưởng thành
- ✅ **Bán Lấy Tiền**: Bán vật phẩm cho NPC để lấy tiền

### **Hệ Thống**

- ✅ **Inventory System**: 3 phần (mô tả, tag filter, danh sách)
- ✅ **Item Tags**: Phân loại vật phẩm (Seed, Tool, Quest, Food, v.v.)
- ✅ **Tool System**: Tương tác với tilemap theo layer
- ✅ **Crop System**: Nhiều giai đoạn phát triển, sprite khô/ướt
- ✅ **NPC Shop**: Mua/bán vật phẩm
- ✅ **Event System**: UI tự động cập nhật qua events

### **Sắp Tới**

- 🔜 Nhiều map
- 🔜 Hệ thống nhiệm vụ
- 🔜 Chăn nuôi
- 🔜 Chế tạo
- 🔜 Multiplayer

---

## 🚀 Cài Đặt

### **Yêu Cầu**

- Unity 2021.3 LTS trở lên
- .NET Framework 4.8 hoặc .NET Standard 2.1
- Windows 10/11 (hoặc macOS/Linux)

### **Cách Cài Đặt**

1. **Clone repository:**
   ```bash
   git clone https://github.com/yourusername/the-green-memoir.git
   cd the-green-memoir
   ```

2. **Mở project trong Unity:**
   - Mở Unity Hub
   - Click `Add` → Chọn thư mục project
   - Chọn Unity version 2021.3 LTS trở lên
   - Click `Open`

3. **Cài đặt packages:**
   - Unity sẽ tự động import packages
   - Đảm bảo có **Input System** package

4. **Setup scene:**
   - Mở scene trong `Assets/Scenes/`
   - Xem `UNITY_COMPLETE_SETUP_GUIDE.md` để setup chi tiết

---

## 🎮 Cách Chơi

### **Điều Khiển**

- **WASD** hoặc **Arrow Keys**: Di chuyển
- **E** hoặc **Click chuột**: Tương tác (cuốc đất, trồng cây, thu hoạch)
- **I**: Mở/đóng Inventory

### **Cách Chơi Cơ Bản**

1. **Cuốc Đất:**
   - Chọn tool "Cuốc" (hoặc mặc định)
   - Click vào ô đất trên Ground layer
   - Đất sẽ đổi sprite sang "đã cuốc"

2. **Trồng Cây:**
   - Chọn tool "Găng tay" (Plant tool)
   - Click vào ô đã cuốc
   - Chọn hạt giống từ danh sách
   - Cây sẽ được trồng

3. **Tưới Nước:**
   - Chọn tool "Bình tưới" (Water tool)
   - Click vào ô đã trồng
   - Cây sẽ được tưới (đổi sprite ướt)

4. **Thu Hoạch:**
   - Chọn tool "Thu hoạch" (Harvest tool)
   - Click vào cây trưởng thành
   - Vật phẩm sẽ được thêm vào inventory

5. **Bán Hàng:**
   - Tương tác với NPC
   - Chọn tab "Sell"
   - Chọn vật phẩm muốn bán
   - Nhận tiền

### **Inventory**

- **Mở Inventory**: Nhấn `I`
- **Filter**: Click các nút filter (All, Seeds, Tools, v.v.)
- **Xem Chi Tiết**: Click vào item → Hiện mô tả và action menu
- **Actions**: Sử dụng, Trồng, Bán, Vứt (tùy theo item)

---

## 📁 Cấu Trúc Code

```
Assets/Game/
├── Core/                    # Core logic (không phụ thuộc Unity)
│   ├── Domain/             # Entities, Value Objects, Interfaces
│   ├── Application/        # Services, Commands, Events
│   └── Infrastructure/     # Repositories, EventBus
│
└── Unity/                   # Unity-specific code
    ├── Data/               # ScriptableObjects (ItemDataSO, CropDataSO, ToolDataSO)
    ├── Managers/           # GameManager, TimeManager
    ├── Presentation/       # UI Controllers, TilemapManager
    └── Input/              # InputHandler
```

### **Kiến Trúc**

- **Domain Layer**: Business logic thuần túy
- **Application Layer**: Use cases, services
- **Infrastructure Layer**: Data persistence, events
- **Unity Layer**: Presentation, UI, Unity-specific code

Xem chi tiết: `GAME_ARCHITECTURE.md`

---

## 💻 Yêu Cầu Hệ Thống

### **Tối Thiểu**

- **OS**: Windows 10, macOS 10.14, hoặc Linux
- **CPU**: Intel Core i3 hoặc tương đương
- **RAM**: 4 GB
- **GPU**: DirectX 11 compatible
- **Storage**: 2 GB trống

### **Khuyến Nghị**

- **OS**: Windows 11, macOS 12, hoặc Linux mới nhất
- **CPU**: Intel Core i5 hoặc tương đương
- **RAM**: 8 GB
- **GPU**: DirectX 12 compatible
- **Storage**: 5 GB trống

---

## 🛠️ Công Nghệ Sử Dụng

- **Unity 2021.3 LTS**: Game engine
- **C#**: Ngôn ngữ lập trình
- **Unity Input System**: Xử lý input
- **Tilemap System**: Quản lý map
- **ScriptableObjects**: Quản lý dữ liệu
- **SOLID Principles**: Kiến trúc code
- **OOP Design**: Hướng đối tượng
- **Event-Driven Architecture**: Hệ thống sự kiện

---

## 📚 Tài Liệu

- `GAME_ARCHITECTURE.md` - Kiến trúc tổng thể
- `UNITY_COMPLETE_SETUP_GUIDE.md` - Hướng dẫn setup Unity
- `UNITY_DATA_GUIDE.md` - Hướng dẫn tạo dữ liệu
- `ITEM_TAGS_GUIDE.md` - Hướng dẫn Item Tags
- `EXPANSION_GUIDE.md` - Hướng dẫn mở rộng game

---

## 🎯 Roadmap

### **Phase 1: Core Systems** ✅
- [x] Domain Layer
- [x] Application Services
- [x] ScriptableObjects System
- [x] Inventory System
- [x] Farming System
- [x] Tool System

### **Phase 2: UI & Interaction** ✅
- [x] Inventory UI (3 phần)
- [x] Seed Selection UI
- [x] Action Menu UI
- [x] NPC Shop UI

### **Phase 3: Content** 🔄
- [ ] Nhiều loại cây trồng
- [ ] Nhiều công cụ
- [ ] NPC và dialogue
- [ ] Quest system

### **Phase 4: Polish** 📅
- [ ] Animation
- [ ] Sound effects
- [ ] Music
- [ ] Visual effects

---

## 🤝 Đóng Góp

Mọi đóng góp đều được hoan nghênh! Vui lòng:

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

### **Guidelines**

- Tuân thủ code style hiện tại
- Viết comment rõ ràng
- Test trước khi commit
- Update documentation nếu cần

---

## 📝 License

Dự án này được phát hành dưới giấy phép MIT. Xem `LICENSE` để biết thêm chi tiết.

---

## 👥 Tác Giả

- **Developer**: [Your Name]
- **Art**: [Artist Name]
- **Music**: [Composer Name]

---

## 🙏 Lời Cảm Ơn

- Unity Technologies
- Cộng đồng game development
- Tất cả contributors

---

## 📞 Liên Hệ

- **Email**: your.email@example.com
- **GitHub**: [@yourusername](https://github.com/yourusername)
- **Discord**: [Server Link]

---

## ⚠️ Lưu Ý

- Game đang trong giai đoạn phát triển
- Một số tính năng có thể chưa hoàn thiện
- Báo lỗi qua Issues trên GitHub

---

**Chúc bạn chơi game vui vẻ! 🌱**

---

*Last updated: 2024*
