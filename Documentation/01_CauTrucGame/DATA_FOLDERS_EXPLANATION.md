# 📁 GIẢI THÍCH VỀ 3 THƯ MỤC DATA

## 🔍 PHÂN TÍCH

### 1. **Assets/Data** ✅ GIỮ LẠI
- **Mục đích**: Chứa **DATA ASSETS** (sprites, prefabs, rule tiles, etc.)
- **Nội dung**:
  - Background sprites
  - Crop sprites
  - Grass sprites
  - GroundTiles
  - ItemAsset (item sprites)
  - Map assets
  - Plant sprites
  - Prefab (game prefabs)
  - ToolAsset (tool sprites)
- **Kết luận**: ✅ **KHÔNG XÓA** - Đây là assets thực tế của game

---

### 2. **Assets/Game/Unity/Data** ⚠️ CẦN DỌN DẸP
- **Mục đích**: Chứa **CODE (.cs)** và **ASSET INSTANCES (.asset)**
- **Nội dung hiện tại**:
  - Các file .cs đã được di chuyển sang `Assets/GameData/`
  - Còn lại các .asset instances (Corn.asset, Glove.asset, etc.)
  - GameDatabase.asset
  - Các folder: Crops/, DirtState/, Items/, Tools/, Intro/
- **Kết luận**: ⚠️ **CẦN DỌN DẸP** - Di chuyển .asset files sang GameData hoặc giữ lại nếu cần

---

### 3. **Assets/GameData** ✅ MỚI TẠO
- **Mục đích**: Chứa **CODE (.cs)** cho ScriptableObjects
- **Nội dung**:
  - Core/ (MasterDatabaseSO, GameSettingsSO, CheatConfigSO)
  - World/ (Items, Farming, Buildings, Tools)
  - Narrative/ (NPCs, Dialogue, Quests, Events)
  - Interaction/ (Actions, States, Transitions, Graphs)
  - Input/ (Actions)
  - UI/ (Menu, Style, Settings)
  - Audio/ (Music, SFX, Voice)
- **Kết luận**: ✅ **GIỮ LẠI** - Đây là cấu trúc mới, chỉ chứa code

---

## 🎯 KẾT LUẬN

### **KHÔNG BỊ LẶP:**
- `Assets/Data` = **Assets thực tế** (sprites, prefabs) - KHÁC với code
- `Assets/GameData` = **Code ScriptableObjects** - KHÁC với assets

### **CẦN DỌN DẸP:**
- `Assets/Game/Unity/Data` còn các .asset instances
- Có thể:
  1. **Giữ lại** nếu cần (để chứa asset instances)
  2. **Di chuyển** .asset files sang `Assets/GameData/` tương ứng
  3. **Xóa** nếu không cần nữa

---

## 💡 ĐỀ XUẤT

### **Option 1: Giữ Assets/Game/Unity/Data cho asset instances**
- Giữ lại folder này để chứa các .asset instances
- Đổi tên thành `Assets/Game/Unity/DataInstances/` để rõ ràng hơn

### **Option 2: Di chuyển asset instances sang GameData**
- Di chuyển các .asset files sang `Assets/GameData/` tương ứng
- Ví dụ: `Items/Corn.asset` → `Assets/GameData/World/Items/Instances/Corn.asset`

### **Option 3: Tạo folder riêng cho asset instances**
- Tạo `Assets/GameDataInstances/` để chứa tất cả .asset instances
- Giữ `Assets/GameData/` chỉ cho code

---

## ✅ KHUYẾN NGHỊ

**Giữ nguyên:**
- ✅ `Assets/Data` - Assets thực tế
- ✅ `Assets/GameData` - Code ScriptableObjects

**Dọn dẹp:**
- ⚠️ `Assets/Game/Unity/Data` - Chỉ giữ lại nếu cần chứa asset instances, hoặc di chuyển/xóa

