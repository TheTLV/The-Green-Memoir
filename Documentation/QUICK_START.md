# 🚀 QUICK START GUIDE

## 📋 Bắt Đầu Nhanh

### **1. Setup Game Database**
1. Right Click → `Create` → `Game` → `Game Database`
2. Kéo các ItemDataSO, CropDataSO, ToolDataSO vào

### **2. Setup Crop (Cây Trồng)**
1. Right Click → `Create` → `Game` → `Crop Data`
2. Thêm **Growth Sprites** (chỉ cần 1 sprite là đủ!)
3. Đánh dấu sprite trưởng thành: Tick `Is Mature`

### **3. Setup Farming Tiles (Cho Background Lớn)**
1. **Giữ nguyên background lớn** (ground + pair) - Không cắt
2. Tạo **Grid** với cell size `(2.88, 2.88)`
3. Tạo **FarmArea Tilemap** - Chỉ vẽ tiles trong khu ruộng
4. Setup **FarmAreaRenderer** - Tự động xử lý rendering

### **4. Hiển Thị Icon Khi Cây Trưởng Thành**
1. Tạo GameObject: `HarvestIcon`
2. Add Component: `CropMatureIndicator`
3. Kéo icon vào "Indicator Object"
4. Xong - Tự động hiển thị!

---

## 📚 Tài Liệu Chi Tiết

- **[FARMING_SYSTEM_GUIDE.md](./02_HuongDanLamGame/FARMING_SYSTEM_GUIDE.md)** - Hệ thống farming đầy đủ
- **[DESIGN_PATTERNS.md](./04_KienTruc/DESIGN_PATTERNS.md)** - Design patterns

---

**Cập nhật:** 2024

