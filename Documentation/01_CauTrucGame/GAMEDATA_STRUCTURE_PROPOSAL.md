# 📁 ĐỀ XUẤT CẤU TRÚC GAMEDATA MỚI

## ✅ ĐÁNH GIÁ CẤU TRÚC ĐỀ XUẤT

Cấu trúc bạn đề xuất **rất tốt** vì:
- ✅ **Phân loại rõ ràng** theo chức năng
- ✅ **Dễ tìm kiếm** và navigate
- ✅ **Scalable** - dễ mở rộng khi thêm tính năng mới
- ✅ **Logic** - nhóm các SOs liên quan lại với nhau
- ✅ **Professional** - giống cấu trúc của các game engine lớn

---

## 📂 CẤU TRÚC ĐỀ XUẤT (CẢI TIẾN)

```
GameData/
├── Core/
│   ├── MasterDatabaseSO          # GameDatabase (đổi tên)
│   ├── GameSettingsSO             # Feature flags
│   └── CheatConfigSO
│
├── World/
│   ├── Items/
│   │   └── ItemDataSO
│   │
│   ├── Farming/
│   │   ├── CropDataSO
│   │   └── TileStateSO            # Có thể ở đây hoặc Tiles/
│   │
│   ├── Buildings/
│   │   ├── BuildingSO
│   │   ├── FloorSO
│   │   ├── RoomSO
│   │   ├── DoorSO
│   │   └── StairSO
│   │
│   ├── Tiles/
│   │   └── TileStateSO            # Nếu không để trong Farming/
│   │
│   └── Tools/
│       └── ToolDataSO
│
├── Narrative/
│   ├── NPCs/
│   │   ├── NPCDefinitionSO
│   │   └── NPCFriendshipSO
│   │
│   ├── Dialogue/
│   │   └── DialogueSO
│   │
│   ├── Quests/
│   │   └── QuestSO
│   │
│   └── Events/
│       ├── GameEventSO
│       └── StorySO
│
├── Interaction/
│   ├── Actions/
│   │   └── InteractionActionSO
│   │
│   ├── States/
│   │   └── InteractionStateSO
│   │
│   ├── Transitions/
│   │   └── InteractionTransitionSO
│   │
│   └── Graphs/
│       └── InteractionGraphSO
│
├── Input/
│   ├── Actions/
│   │   ├── InputActionSO
│   │   ├── ToolSO
│   │   ├── SkillSO
│   │   ├── UIToggleSO
│   │   └── CheatSO
│   │
│   └── Config/
│       └── (có thể thêm InputConfigSO nếu cần)
│
├── UI/
│   ├── Menu/
│   │   ├── MenuSO
│   │   └── MenuItemSO
│   │
│   ├── Style/
│   │   └── UIStyleSO
│   │
│   └── Settings/
│       ├── GameSettingsDataSO
│       ├── BaseSettingMenuSO
│       ├── SubSettingMenuSO
│       ├── MainSettingsMenuSO
│       └── SettingMenuRegistrySO
│
└── Audio/
    ├── Music/
    │   └── AudioClipSO (Music)
    │
    ├── SFX/
    │   └── AudioClipSO (SFX)
    │
    └── Voice/
        └── AudioClipSO (Voice)
```

---

## 🔄 MAPPING TỪ CẤU TRÚC CŨ SANG MỚI

| **File Cũ** | **Thư Mục Mới** | **Ghi Chú** |
|------------|----------------|-------------|
| `GameDatabase.cs` | `Core/MasterDatabaseSO.cs` | Đổi tên class thành MasterDatabaseSO |
| `GameSettingsSO.cs` | `Core/GameSettingsSO.cs` | Giữ nguyên |
| `CheatConfigSO.cs` | `Core/CheatConfigSO.cs` | Giữ nguyên |
| `ItemDataSO.cs` | `World/Items/ItemDataSO.cs` | ✅ |
| `CropDataSO.cs` | `World/Farming/CropDataSO.cs` | ✅ |
| `TileStateSO.cs` | `World/Farming/TileStateSO.cs` hoặc `World/Tiles/TileStateSO.cs` | Tùy chọn |
| `BuildingSO.cs` | `World/Buildings/BuildingSO.cs` | ✅ |
| `FloorSO.cs` | `World/Buildings/FloorSO.cs` | ✅ |
| `RoomSO.cs` | `World/Buildings/RoomSO.cs` | ✅ |
| `DoorSO.cs` | `World/Buildings/DoorSO.cs` | ✅ |
| `StairSO.cs` | `World/Buildings/StairSO.cs` | ✅ |
| `ToolDataSO.cs` | `World/Tools/ToolDataSO.cs` | ✅ |
| `NPCDefinitionSO.cs` | `Narrative/NPCs/NPCDefinitionSO.cs` | ✅ |
| `NPCFriendshipSO.cs` | `Narrative/NPCs/NPCFriendshipSO.cs` | ✅ |
| `DialogueSO.cs` | `Narrative/Dialogue/DialogueSO.cs` | ✅ |
| `QuestSO.cs` | `Narrative/Quests/QuestSO.cs` | ✅ |
| `GameEventSO.cs` | `Narrative/Events/GameEventSO.cs` | ✅ |
| `StorySO.cs` | `Narrative/Events/StorySO.cs` | ✅ |
| `InteractionActionSO.cs` | `Interaction/Actions/InteractionActionSO.cs` | ✅ |
| `InteractionStateSO.cs` | `Interaction/States/InteractionStateSO.cs` | ✅ |
| `InteractionTransitionSO.cs` | `Interaction/Transitions/InteractionTransitionSO.cs` | ✅ |
| `InteractionGraphSO.cs` | `Interaction/Graphs/InteractionGraphSO.cs` | ✅ |
| `InputActionSO.cs` | `Input/Actions/InputActionSO.cs` | ✅ |
| `ToolSO.cs` | `Input/Actions/ToolSO.cs` | ✅ |
| `SkillSO.cs` | `Input/Actions/SkillSO.cs` | ✅ |
| `UIToggleSO.cs` | `Input/Actions/UIToggleSO.cs` | ✅ |
| `CheatSO.cs` | `Input/Actions/CheatSO.cs` | ✅ |
| `MenuSO.cs` | `UI/Menu/MenuSO.cs` | ✅ |
| `MenuItemSO.cs` | `UI/Menu/MenuItemSO.cs` | ✅ |
| `UIStyleSO.cs` | `UI/Style/UIStyleSO.cs` | ✅ |
| `GameSettingsDataSO.cs` | `UI/Settings/GameSettingsDataSO.cs` | ✅ |
| `BaseSettingMenuSO.cs` | `UI/Settings/BaseSettingMenuSO.cs` | ✅ |
| `SubSettingMenuSO.cs` | `UI/Settings/SubSettingMenuSO.cs` | ✅ |
| `MainSettingsMenuSO.cs` | `UI/Settings/MainSettingsMenuSO.cs` | ✅ |
| `SettingMenuRegistrySO.cs` | `UI/Settings/SettingMenuRegistrySO.cs` | ✅ |
| `AudioClipSO.cs` | `Audio/Music/`, `Audio/SFX/`, `Audio/Voice/` | Phân loại theo type |

---

## 💡 ĐỀ XUẤT CẢI TIẾN

### 1. **Thêm Input/ folder riêng**
- Tách Input actions ra khỏi World/Tools
- Logic hơn vì Input là hệ thống riêng

### 2. **Audio phân loại theo type**
- `Audio/Music/` - Background music
- `Audio/SFX/` - Sound effects
- `Audio/Voice/` - Voice clips
- Hoặc có thể dùng subfolder trong AudioClipSO

### 3. **Interaction/Transitions/**
- Tách Transitions ra folder riêng để rõ ràng hơn

### 4. **Core/GameSettingsSO**
- Giữ GameSettingsSO trong Core vì nó là feature flags toàn game

---

## 📝 CẬP NHẬT CreateAssetMenu Paths

Sau khi di chuyển, cần cập nhật `menuName` trong `[CreateAssetMenu]`:

```csharp
// Cũ
[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Data", order = 1)]

// Mới
[CreateAssetMenu(fileName = "NewItem", menuName = "GameData/World/Items/Item Data", order = 1)]
```

**Hoặc giữ ngắn gọn hơn:**
```csharp
[CreateAssetMenu(fileName = "NewItem", menuName = "GameData/Items", order = 1)]
```

---

## 🚀 KẾ HOẠCH MIGRATION

### Bước 1: Tạo cấu trúc thư mục mới
1. Tạo tất cả folders trong `Assets/GameData/`
2. Giữ nguyên code files (chưa di chuyển)

### Bước 2: Di chuyển files
1. Di chuyển từng file theo mapping table
2. Unity sẽ tự động cập nhật references
3. Kiểm tra lại tất cả references

### Bước 3: Cập nhật CreateAssetMenu paths
1. Cập nhật `menuName` trong tất cả SOs
2. Test tạo SO mới từ menu

### Bước 4: Cập nhật documentation
1. Cập nhật `SCRIPTABLE_OBJECTS_DOCUMENTATION.md`
2. Cập nhật các guide khác nếu có

### Bước 5: Test
1. Test tất cả tính năng
2. Kiểm tra không có broken references
3. Test tạo SO mới từ menu

---

## ⚠️ LƯU Ý

1. **Namespace không đổi** - Chỉ di chuyển files, không đổi namespace
2. **Unity tự động cập nhật references** - Nhưng nên kiểm tra lại
3. **Meta files** - Unity tự động tạo lại, không cần lo
4. **Version Control** - Commit trước khi di chuyển để dễ rollback

---

## ✅ KẾT LUẬN

Cấu trúc đề xuất **rất tốt** và nên implement! 

**Ưu điểm:**
- Dễ maintain
- Dễ tìm kiếm
- Professional
- Scalable

**Khuyến nghị:**
- Thêm `Input/` folder riêng
- Phân loại Audio theo type
- Giữ namespace không đổi
- Cập nhật CreateAssetMenu paths

---

*Tài liệu này có thể dùng làm checklist khi migration*

