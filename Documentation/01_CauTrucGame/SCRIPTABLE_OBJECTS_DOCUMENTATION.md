# 📚 SCRIPTABLE OBJECTS DOCUMENTATION
## Tổng hợp tất cả ScriptableObjects trong The Green Memoir

---

## 📋 MỤC LỤC

1. [Item & Inventory](#item--inventory)
2. [Farming & Crops](#farming--crops)
3. [Tools](#tools)
4. [Map & Tiles](#map--tiles)
5. [Buildings](#buildings)
6. [NPC & Dialogue](#npc--dialogue)
7. [Quest & Events](#quest--events)
8. [Input & Actions](#input--actions)
9. [Settings & UI](#settings--ui)
10. [Audio](#audio)
11. [Interaction System](#interaction-system)
12. [Database & Configuration](#database--configuration)

---

## 🎒 ITEM & INVENTORY

### **ItemDataSO**
- **Path**: `Game/Item Data`
- **Chức năng**: Định nghĩa vật phẩm trong game
- **Thuộc tính chính**:
  - `itemId`: ID duy nhất
  - `itemName`: Tên hiển thị
  - `icon`: Icon trong inventory
  - `maxStackSize`: Số lượng tối đa trong stack
  - `tags`: Tags phân loại (Stackable, Seed, Consumable, etc.)
  - `sellPrice`, `buyPrice`: Giá bán/mua
- **Liên quan**: Inventory, Shop, Crafting

---

## 🌾 FARMING & CROPS

### **CropDataSO**
- **Path**: `Game/Crop Data`
- **Chức năng**: Định nghĩa cây trồng
- **Thuộc tính chính**:
  - `cropId`: ID duy nhất
  - `cropName`: Tên cây trồng
  - Sprites cho các giai đoạn (Seed, Sprout, Growing, Mature, Wilted) - cả Dry và Wet state
  - `daysToGrow`: Số ngày để trưởng thành
  - `daysToWilt`: Số ngày không tưới sẽ héo
  - `harvestYield`: Số lượng thu hoạch
  - `harvestItemId`: ID vật phẩm thu hoạch
  - `seedItemId`: ID hạt giống
- **Liên quan**: Farming, TileStateSO, ItemDataSO

---

## 🔧 TOOLS

### **ToolDataSO**
- **Path**: `Game/Tool Data`
- **Chức năng**: Định nghĩa công cụ (cuốc, xẻng, búa, etc.)
- **Thuộc tính chính**:
  - `toolId`: ID duy nhất
  - `toolName`: Tên công cụ
  - `icon`: Icon trong tool selection
  - `heldSprite`: Sprite khi cầm trên tay
  - `useAnimation`: Animation sử dụng tool
  - `hasDirectionalAnimation`: Có animation 4 hướng không
  - `useAnimationUp/Down/Left/Right`: Animation theo hướng
- **Liên quan**: ToolSelectionUI, PlayerController, InputActionSO

---

## 🗺️ MAP & TILES

### **TileStateSO**
- **Path**: `Game/Tile State`
- **Chức năng**: Định nghĩa trạng thái tile (đất bình thường, đã cuốc, đã tưới, etc.)
- **Thuộc tính chính**:
  - `stateId`: ID của state
  - `displayName`: Tên hiển thị
  - `stateType`: Loại state (Normal, Plowed, Watered, Seeded, Growing, Mature, etc.)
  - `allowCropGrowth`: Cho phép cây phát triển
  - `canPlow`, `canPlant`, `canWater`, `canHarvest`: Các hành động được phép
  - `nextValidStates`: Danh sách state có thể chuyển tiếp
  - `sprite`, `tileBase`, `displayColor`: Visual (optional)
- **Liên quan**: Farming, TilemapManager, CropDataSO

---

## 🏠 BUILDINGS

### **BuildingSO**
- **Path**: `Game/Building`
- **Chức năng**: Định nghĩa tòa nhà (nhà, siêu thị, trung tâm thương mại)
- **Thuộc tính chính**:
  - `buildingId`: ID duy nhất
  - `buildingName`: Tên tòa nhà
  - `buildingType`: Loại tòa nhà
  - `buildingScene`: Scene của tòa nhà
  - `floors`: Danh sách tầng (FloorSO)
  - `doors`: Danh sách cửa (DoorSO)
  - `hasBed`: Có giường để ngủ
  - `bedPosition`: Vị trí giường
  - `defaultSpawnPosition`, `returnPosition`: Vị trí spawn
- **Liên quan**: FloorSO, RoomSO, DoorSO, StairSO

### **FloorSO**
- **Path**: `Game/Building/Floor`
- **Chức năng**: Định nghĩa tầng trong tòa nhà
- **Thuộc tính chính**:
  - `floorId`: ID tầng
  - `floorName`: Tên tầng
  - `floorNumber`: Số tầng
  - `rooms`: Danh sách phòng (RoomSO)
  - `stairs`: Danh sách cầu thang (StairSO)
- **Liên quan**: BuildingSO, RoomSO, StairSO

### **RoomSO**
- **Path**: `Game/Building/Room`
- **Chức năng**: Định nghĩa phòng trong tòa nhà
- **Thuộc tính chính**:
  - `roomId`: ID phòng
  - `roomName`: Tên phòng
  - `roomType`: Loại phòng
  - `sceneName`: Scene của phòng
  - `spawnPosition`: Vị trí spawn trong phòng
- **Liên quan**: BuildingSO, FloorSO, DoorSO

### **DoorSO**
- **Path**: `Game/Building/Door`
- **Chức năng**: Định nghĩa cửa trong tòa nhà
- **Thuộc tính chính**:
  - `doorId`: ID cửa
  - `doorName`: Tên cửa
  - `targetRoomId`: Phòng đích (nếu đi vào phòng)
  - `targetBuildingId`: Tòa nhà đích (nếu đi vào tòa nhà khác)
  - `targetSceneName`: Scene đích
  - `spawnPosition`: Vị trí spawn khi đi qua cửa
  - `isEnabled`, `isLocked`: Trạng thái cửa
  - `autoTransition`: Tự động chuyển hay cần nhấn phím
- **Liên quan**: BuildingSO, RoomSO

### **StairSO**
- **Path**: `Game/Building/Stair`
- **Chức năng**: Định nghĩa cầu thang trong tòa nhà
- **Thuộc tính chính**:
  - `stairId`: ID cầu thang
  - `stairName`: Tên cầu thang
  - `fromFloorId`: Tầng xuất phát
  - `toFloorId`: Tầng đích
  - `spawnPosition`: Vị trí spawn khi lên/xuống
- **Liên quan**: BuildingSO, FloorSO

---

## 👥 NPC & DIALOGUE

### **NPCDefinitionSO**
- **Path**: `Game/NPC Definition`
- **Chức năng**: Định nghĩa NPC
- **Thuộc tính chính**:
  - `npcId`: ID duy nhất
  - `displayName`: Tên hiển thị
  - `type`: Loại NPC (Generic, Shop, Quest, Enemy)
  - `portrait`: Ảnh chân dung
  - `isShop`: Có phải shop không
  - `baseBuyMultiplier`, `baseSellMultiplier`: Hệ số mua/bán
  - `friendshipConfig`: Cấu hình độ thân mật (NPCFriendshipSO)
- **Liên quan**: DialogueSO, QuestSO, ShopNPC, QuestNPC

### **NPCFriendshipSO**
- **Path**: `Game/NPC/Friendship`
- **Chức năng**: Cấu hình hệ thống độ thân mật với NPC
- **Thuộc tính chính**:
  - `npcId`: ID NPC
  - `currentFriendshipPoints`: Điểm thân mật hiện tại
  - `maxFriendshipPoints`: Điểm tối đa
  - `friendshipLevels`: Danh sách level thân mật
- **Liên quan**: NPCDefinitionSO, NPCFriendshipSystem

### **DialogueSO**
- **Path**: `Game/Dialogue`
- **Chức năng**: Định nghĩa hội thoại/conversation
- **Thuộc tính chính**:
  - `dialogueId`: ID duy nhất
  - `npcId`: NPC nào nói dialogue này
  - `nodes`: Danh sách dialogue nodes
    - `nodeId`: ID node
    - `speakerName`: Tên người nói
    - `text`: Nội dung
    - `speakerPortrait`: Ảnh chân dung
    - `choices`: Danh sách lựa chọn
    - `actions`: Danh sách hành động
    - `nextNodeId`: Node tiếp theo
- **Liên quan**: NPCDefinitionSO, DialogueController

---

## 📜 QUEST & EVENTS

### **QuestSO**
- **Path**: `Game/Quest`
- **Chức năng**: Định nghĩa quest
- **Thuộc tính chính**:
  - `questId`: ID duy nhất
  - `questName`: Tên quest
  - `description`: Mô tả
  - `objectives`: Danh sách mục tiêu
    - `objectiveId`: ID mục tiêu
    - `description`: Mô tả
    - `type`: Loại (CollectItem, KillEnemy, TalkToNPC, ReachLocation, etc.)
    - `targetId`: ID đích
    - `targetCount`: Số lượng cần
    - `isCompleted`: Đã hoàn thành chưa
  - `moneyReward`, `expReward`: Phần thưởng
  - `itemRewards`: Danh sách vật phẩm thưởng
  - `isMainQuest`: Quest chính
  - `isRepeatable`: Có thể lặp lại
  - `prerequisite`: Điều kiện tiên quyết
- **Liên quan**: QuestNPC, QuestSystem, ItemDataSO

### **GameEventSO**
- **Path**: `Game/Event`
- **Chức năng**: Định nghĩa event trong game
- **Thuộc tính chính**:
  - `eventId`: ID duy nhất
  - `eventName`: Tên event
  - `description`: Mô tả
  - `triggerType`: Loại trigger (OnEnterLocation, OnInteract, OnItemUse, OnQuestComplete, OnTime, OnCustom)
  - `triggerParameter`: Tham số trigger
  - `actions`: Danh sách hành động (GiveItem, TakeItem, StartQuest, CompleteQuest, etc.)
  - `conditions`: Danh sách điều kiện
  - `isOneTimeOnly`: Chỉ xảy ra 1 lần
  - `isActive`: Có active không
- **Liên quan**: EventSystem, QuestSO, ItemDataSO

### **StorySO**
- **Path**: `Game/Story`
- **Chức năng**: Định nghĩa câu chuyện/story
- **Thuộc tính chính**:
  - `storyId`: ID duy nhất
  - `storyName`: Tên story
  - `chapters`: Danh sách chapter
- **Liên quan**: StoryController, DialogueSO

---

## ⌨️ INPUT & ACTIONS

### **InputActionSO**
- **Path**: `Game/Input Action`
- **Chức năng**: Định nghĩa input action (phím bấm)
- **Thuộc tính chính**:
  - `actionId`: ID duy nhất
  - `displayName`: Tên hiển thị
  - `key`: Phím bấm
  - `enabled`: Có enabled không
  - `actionType`: Loại action
  - `group`: Nhóm action (UI, Interact, Skill, Tool, Movement, Cheat, Custom)
  - `inputMode`: Chế độ (Press, Hold, Toggle)
  - `holdDurationThreshold`: Thời gian giữ tối thiểu
  - `cooldownSeconds`: Cooldown
  - `linkedAction`: Action liên kết (ActionSOBase)
  - `animatorTriggerName`: Trigger animator
- **Liên quan**: InputActionManager, ToolSO, SkillSO, UIToggleSO, CheatSO

### **ToolSO**
- **Path**: `Game/Input/Actions/Tool`
- **Chức năng**: Action cho tool (kế thừa ActionSOBase)
- **Liên quan**: InputActionSO, ToolDataSO

### **SkillSO**
- **Path**: `Game/Input/Actions/Skill`
- **Chức năng**: Action cho skill (kế thừa ActionSOBase)
- **Liên quan**: InputActionSO, SkillSystem

### **UIToggleSO**
- **Path**: `Game/Input/Actions/UI Toggle`
- **Chức năng**: Action để toggle UI (kế thừa ActionSOBase)
- **Liên quan**: InputActionSO, UI Controllers

### **CheatSO**
- **Path**: `Game/Input/Actions/Cheat`
- **Chức năng**: Action cho cheat (kế thừa ActionSOBase)
- **Liên quan**: InputActionSO, CheatConfigSO

---

## ⚙️ SETTINGS & UI

### **GameSettingsDataSO**
- **Path**: `Game/Settings Data`
- **Chức năng**: Lưu tất cả cài đặt game
- **Thuộc tính chính**:
  - `isFullScreen`: Full screen mode
  - `bgmVolume`, `seVolume`, `gameVoicesVolume`, `eventVoicesVolume`: Volume (0-100)
  - `textSpeed`: Tốc độ text (Slow, Normal, Fast, NoWait)
  - `autoModeTextSpeed`: Tốc độ text auto mode (Off, Slow, Normal, Fast)
  - `continuePlayingVoices`: Tiếp tục phát voice đến voice tiếp theo
  - `currentLanguage`: Ngôn ngữ hiện tại
- **Liên quan**: SettingsMenuController, AudioManager

### **BaseSettingMenuSO**
- **Path**: Base class (abstract)
- **Chức năng**: Base class cho tất cả menu settings
- **Thuộc tính chính**:
  - `menuId`: ID duy nhất
  - `menuName`: Tên menu
  - `menuIcon`: Icon menu
  - `subMenus`: Danh sách sub-menus
  - `isEnabled`, `isVisible`: Trạng thái
  - `displayOrder`: Thứ tự hiển thị
- **Liên quan**: SubSettingMenuSO, MainSettingsMenuSO

### **SubSettingMenuSO**
- **Path**: `Game/Settings/Sub Menu`
- **Chức năng**: Menu con có thể nested
- **Thuộc tính chính**:
  - Kế thừa từ BaseSettingMenuSO
  - `subMenuType`: Loại menu (Custom, VolumeSettings, KeyConfiguration, DisplaySettings, AudioSettings, LanguageSettings, EventSceneSettings)
  - `customUIPrefab`: Prefab UI tùy chỉnh
  - `customData`: Dữ liệu tùy chỉnh
  - `nestedSubMenus`: Sub-menus lồng nhau
- **Liên quan**: BaseSettingMenuSO, DynamicSettingsController

### **MainSettingsMenuSO**
- **Path**: `Game/Settings/Main Menu`
- **Chức năng**: Menu settings chính
- **Thuộc tính chính**:
  - Kế thừa từ BaseSettingMenuSO
  - `mainMenuUIPrefab`: Prefab UI cho main menu
- **Liên quan**: BaseSettingMenuSO, SettingMenuRegistrySO

### **SettingMenuRegistrySO**
- **Path**: `Game/Settings/Registry`
- **Chức năng**: Registry quản lý tất cả menu settings
- **Thuộc tính chính**:
  - `mainSettingsMenu`: Menu settings chính
  - `pauseMenuItems`: Danh sách menu trong pause menu
  - `allMenus`: Tất cả menus đã đăng ký (auto collect)
  - `autoCollectMenus`: Tự động collect menus
- **Liên quan**: DynamicSettingsController, BaseSettingMenuSO

### **GameSettingsSO**
- **Path**: `Game/Settings`
- **Chức năng**: Cấu hình toàn bộ game (features on/off)
- **Thuộc tính chính**:
  - `enableOnlineMode`: Bật online mode
  - `enableNPCFriendship`: Bật hệ thống độ thân mật
  - `enableNPCQuests`: Bật quest từ NPC
  - `enableNPCDialogue`: Bật dialogue với NPC
  - `enableLevelSystem`: Bật hệ thống level
  - `enableSkillSystem`: Bật hệ thống skill
  - `enableCrafting`: Bật crafting
  - `enableTrading`: Bật trading
  - `enableBattle`: Bật battle/combat
- **Liên quan**: GameManager, Feature flags

### **UIStyleSO**
- **Path**: `Game/UI Style`
- **Chức năng**: Cấu hình style UI (theme)
- **Thuộc tính chính**:
  - `styleName`: Tên style
  - Font settings
  - Button colors
  - Text colors
  - Background colors
  - Shadow/Border effects
- **Liên quan**: UIStyleApplier, UI Controllers

### **MenuItemSO**
- **Path**: `Game/Menu/Menu Item`
- **Chức năng**: Định nghĩa item trong menu
- **Thuộc tính chính**:
  - `displayName`: Tên hiển thị
  - `icon`: Icon
  - `menuType`: Loại (Action, SubMenu, SceneLoad)
  - `subMenu`: Menu con (nếu menuType = SubMenu)
  - `sceneToLoad`: Scene load (nếu menuType = SceneLoad)
  - `onSelectAction`: Action khi chọn
  - `isEnabled`, `isVisible`, `isLocked`: Trạng thái
- **Liên quan**: MenuSO, MenuController

### **MenuSO**
- **Path**: `Game/Menu/Menu`
- **Chức năng**: Định nghĩa cấu trúc menu (Pause Menu, Item Menu, etc.)
- **Thuộc tính chính**:
  - `menuTitle`: Tiêu đề menu
  - `menuIcon`: Icon menu
  - `menuItems`: Danh sách items (MenuItemSO)
  - `parentMenu`: Menu cha
  - `allowBackNavigation`: Cho phép quay lại
  - `canCloseWithEscape`: Có thể đóng bằng Escape
- **Liên quan**: MenuItemSO, MenuController

---

## 🎵 AUDIO

### **AudioClipSO**
- **Path**: `Game/Audio/Audio Clip`
- **Chức năng**: Định nghĩa audio clip với metadata
- **Thuộc tính chính**:
  - `clipId`: ID duy nhất
  - `clipName`: Tên clip
  - `audioClip`: AudioClip
  - `volume`: Volume
  - `pitch`: Pitch
  - `loop`: Có loop không
  - `audioType`: Loại (Music, SFX, Voice, Ambient)
- **Liên quan**: AudioManager

---

## 🔄 INTERACTION SYSTEM

### **InteractionActionSO**
- **Path**: `Game/Interaction/Action`
- **Chức năng**: Định nghĩa hành động tương tác (Plow, Water, Plant, Harvest, etc.)
- **Thuộc tính chính**:
  - `actionId`: ID duy nhất
  - `description`: Mô tả
- **Liên quan**: InteractionGraphSO, InteractionStateSO

### **InteractionStateSO**
- **Path**: `Game/Interaction/State`
- **Chức năng**: Định nghĩa trạng thái tương tác
- **Thuộc tính chính**:
  - `stateId`: ID duy nhất
  - `stateName`: Tên state
- **Liên quan**: InteractionGraphSO, InteractionActionSO

### **InteractionTransitionSO**
- **Path**: `Game/Interaction/Transition`
- **Chức năng**: Định nghĩa transition giữa các state
- **Thuộc tính chính**:
  - `fromState`: State xuất phát
  - `toState`: State đích
  - `action`: Action gây ra transition
- **Liên quan**: InteractionGraphSO

### **InteractionGraphSO**
- **Path**: `Game/Interaction/Graph`
- **Chức năng**: Định nghĩa graph tương tác (state machine)
- **Thuộc tính chính**:
  - `graphId`: ID duy nhất
  - `states`: Danh sách states
  - `transitions`: Danh sách transitions
- **Liên quan**: InteractionActionSO, InteractionStateSO, InteractionTransitionSO

---

## 💾 DATABASE & CONFIGURATION

### **GameDatabase**
- **Path**: `Game/Game Database`
- **Chức năng**: Database chính quản lý tất cả SOs
- **Thuộc tính chính**:
  - `items`: Danh sách ItemDataSO
  - `crops`: Danh sách CropDataSO
  - `tools`: Danh sách ToolDataSO
  - `tileStates`: Danh sách TileStateSO
  - `buildings`: Danh sách BuildingSO
- **Liên quan**: GameDatabaseManager, Tất cả SOs

### **CheatConfigSO**
- **Path**: `Game/Cheat Config`
- **Chức năng**: Cấu hình cheat codes
- **Thuộc tính chính**:
  - `cheatId`: ID cheat
  - `cheatName`: Tên cheat
  - `keyCode`: Phím bấm
  - `enabled`: Có enabled không
- **Liên quan**: CheatSO, QuickCheatManager

---

## 📊 TỔNG KẾT

### **Theo chức năng:**

- **Item & Inventory**: ItemDataSO
- **Farming**: CropDataSO, TileStateSO
- **Tools**: ToolDataSO, ToolSO
- **Map & Buildings**: BuildingSO, FloorSO, RoomSO, DoorSO, StairSO
- **NPC**: NPCDefinitionSO, NPCFriendshipSO, DialogueSO
- **Quest & Events**: QuestSO, GameEventSO, StorySO
- **Input**: InputActionSO, ToolSO, SkillSO, UIToggleSO, CheatSO
- **Settings**: GameSettingsDataSO, BaseSettingMenuSO, SubSettingMenuSO, MainSettingsMenuSO, SettingMenuRegistrySO, GameSettingsSO
- **UI**: UIStyleSO, MenuItemSO, MenuSO
- **Audio**: AudioClipSO
- **Interaction**: InteractionActionSO, InteractionStateSO, InteractionTransitionSO, InteractionGraphSO
- **Database**: GameDatabase, CheatConfigSO

### **Tổng số SOs**: ~35 ScriptableObjects

---

## 🔗 QUAN HỆ GIỮA CÁC SOs

```
GameDatabase
├── ItemDataSO
├── CropDataSO
├── ToolDataSO
├── TileStateSO
└── BuildingSO
    ├── FloorSO
    │   ├── RoomSO
    │   └── StairSO
    └── DoorSO

NPCDefinitionSO
├── NPCFriendshipSO
├── DialogueSO
└── QuestSO

InputActionSO
├── ToolSO
├── SkillSO
├── UIToggleSO
└── CheatSO

SettingMenuRegistrySO
├── MainSettingsMenuSO
│   └── SubSettingMenuSO (nested)
└── SubSettingMenuSO (pause menu)

InteractionGraphSO
├── InteractionStateSO
├── InteractionActionSO
└── InteractionTransitionSO
```

---

*Tài liệu này được tạo tự động từ codebase. Cập nhật: 2024*

