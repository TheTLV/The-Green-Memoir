# 🎮 HƯỚNG DẪN LÀM GAME - THE GREEN MEMOIR

**Hướng dẫn đầy đủ từ MVP đến Advanced, khớp với code hiện tại.**

---

## 📋 MỤC LỤC

1. [MVP - Minimum Viable Product](#1-mvp---minimum-viable-product)
2. [MEDIUM - Phát triển thêm](#2-medium---phát-triển-thêm)
3. [ADVANCED - Tính năng nâng cao](#3-advanced---tính-năng-nâng-cao)
4. [Các Systems Đã Có Sẵn](#4-các-systems-đã-có-sẵn)
5. [Setup Chi Tiết](#5-setup-chi-tiết)

---

## 1. MVP - MINIMUM VIABLE PRODUCT

### 🎯 Mục tiêu MVP:
- ✅ Có thể di chuyển nhân vật
- ✅ Có thể vào nhà (chuyển scene)
- ✅ Có thể tương tác với NPC
- ✅ Có thể mua/bán với NPC
- ✅ Có thể mở inventory
- ✅ Có thể save/load game
- ✅ Có title screen và tutorial

---

### 📁 BƯỚC 1: TẠO CÁC SCENES

#### **Scene 1: TitleScreen**

1. **Tạo Scene mới**:
   - Unity Editor → `File → New Scene`
   - Chọn `2D` template
   - `File → Save As` → Đặt tên: `TitleScreen.unity`
   - Lưu vào: `Assets/Scenes/TitleScreen.unity`

2. **Tạo Canvas**:
   - Hierarchy → Right Click → `UI → Canvas`
   - Đặt tên: `TitleCanvas`
   - Inspector → `Canvas Scaler`:
     - `UI Scale Mode`: `Scale With Screen Size`
     - `Reference Resolution`: `X: 1920, Y: 1080`

3. **Tạo Background**:
   - `TitleCanvas` → Right Click → `UI → Image`
   - Đặt tên: `Background`
   - Inspector → `Rect Transform`:
     - Click anchor preset → Chọn `Stretch-Stretch` (Alt + Shift + Click)
     - `Left/Right/Top/Bottom`: `0`
   - Inspector → `Image`:
     - `Color`: `RGBA(20, 20, 20, 255)` (Dark background)

4. **Tạo Title Text**:
   - `TitleCanvas` → Right Click → `UI → TextMeshPro - Text`
   - Đặt tên: `TitleText`
   - Inspector → `Rect Transform`:
     - Anchor: `Middle-Center`
     - `Pos Y`: `200`
   - Inspector → `TextMeshPro`:
     - `Text`: `THE GREEN MEMOIR`
     - `Font Size`: `72`
     - `Font Style`: `Bold`
     - `Color`: `#FFFFFF`
     - `Alignment`: `Center`

5. **Tạo Menu Buttons Panel**:
   - `TitleCanvas` → Right Click → `UI → Panel`
   - Đặt tên: `MenuButtonsPanel`
   - Inspector → `Rect Transform`:
     - Anchor: `Middle-Center`
     - `Width`: `400`, `Height`: `400`
     - `Pos Y`: `-100`
   - Inspector → `Image`:
     - `Color`: `RGBA(30, 30, 30, 200)`
   - Inspector → Add Component → `Vertical Layout Group`:
     - `Spacing`: `20`
     - `Padding`: `Left/Right/Top/Bottom = 20`
     - `Child Force Expand`: `Width: ✓, Height: ✗`

6. **Tạo Buttons** (trong `MenuButtonsPanel`):
   
   **a) New Game Button**:
   - `MenuButtonsPanel` → Right Click → `UI → Button - TextMeshPro`
   - Đặt tên: `NewGameButton`
   - Inspector → `Rect Transform`: `Height`: `60`
   - Inspector → `Button`:
     - `Normal Color`: `#4A90E2` (Blue)
   - Inspector → `TextMeshPro` (child): `Text`: `New Game`
   
   **b) Load Game Button**:
   - Tương tự, đặt tên: `LoadGameButton`
   - `Text`: `Load Game`
   - `Normal Color`: `#7ED321` (Green)
   
   **c) Settings Button**:
   - Đặt tên: `SettingsButton`
   - `Text`: `Settings`
   - `Normal Color`: `#F5A623` (Yellow)
   
   **d) Quit Button**:
   - Đặt tên: `QuitButton`
   - `Text`: `Quit`
   - `Normal Color`: `#D0021B` (Red)

7. **Gắn TitleScreenController**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `TitleScreenController`
   - Inspector → Add Component → `TitleScreenController`
   - Kéo các buttons vào các fields:
     - `NewGameButton` → `newGameButton`
     - `LoadGameButton` → `loadGameButton`
     - `SettingsButton` → `settingsButton`
     - `QuitButton` → `quitButton`

8. **Thêm ButtonSoundHelper** (optional):
   - Chọn từng button → Inspector → Add Component → `ButtonSoundHelper`
   - `Use Default Sounds`: ✓

9. **Tạo GameManager**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `GameManager`
   - Inspector → Add Component → `GameManager`

10. **Tạo AudioManager** (optional):
    - Hierarchy → Right Click → `Create Empty`
    - Đặt tên: `AudioManager`
    - Inspector → Add Component → `AudioManager`

---

#### **Scene 2: Tutorial**

1. **Tạo Scene mới**: `Tutorial.unity`

2. **Tạo Canvas**:
   - Tương tự TitleScreen
   - Đặt tên: `TutorialCanvas`

3. **Tạo Tutorial Panel**:
   - `TutorialCanvas` → Right Click → `UI → Panel`
   - Đặt tên: `TutorialPanel`
   - Inspector → `Rect Transform`:
     - Anchor: `Middle-Center`
     - `Width`: `800`, `Height`: `600`

4. **Tạo Tutorial Text**:
   - `TutorialPanel` → Right Click → `UI → TextMeshPro - Text`
   - Đặt tên: `TutorialText`
   - Inspector → `TextMeshPro`:
     - `Text`: `Welcome to The Green Memoir!`

5. **Tạo Buttons**:
   - `Next Button`: `TutorialPanel` → Right Click → `UI → Button - TextMeshPro`
   - `Skip Button`: Tương tự

6. **Gắn TutorialController**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `TutorialController`
   - Inspector → Add Component → `TutorialController`
   - Kéo `TutorialPanel`, `TutorialText`, buttons vào

7. **Tạo GameManager và AudioManager** (tương tự TitleScreen)

---

#### **Scene 3: Game (Main Game Scene)**

1. **Tạo Scene mới**: `Game.unity`

2. **Tạo Map**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `Map`
   
   **a) Tạo Ground (Tilemap)**:
   - `Map` → Right Click → `2D Object → Tilemap → Rectangular`
   - Đặt tên: `Ground`
   - Inspector → `Tilemap Renderer`: `Sort Order`: `0`
   
   **b) Tạo Map Bounds**:
   - `Map` → Right Click → `Create Empty`
   - Đặt tên: `MapBounds`
   - Inspector → Add Component → `Box Collider 2D`
   - Inspector → `Box Collider 2D`:
     - `Is Trigger`: ✓
     - `Size`: `X: 50, Y: 50` (tùy chỉnh theo map)

3. **Tạo Player**:
   - Hierarchy → Right Click → `2D Object → Sprite`
   - Đặt tên: `Player`
   - Inspector → `Tag`: `Player`
   - Inspector → `Sprite Renderer`: Kéo player sprite vào
   - Inspector → Add Component → `PlayerController`
   - Inspector → Add Component → `Rigidbody 2D`:
     - `Body Type`: `Kinematic` hoặc `Dynamic`
   - Inspector → Add Component → `Box Collider 2D`

4. **Tạo Camera**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `Main Camera Controller`
   - Inspector → Add Component → `Camera`
   - Inspector → `Camera`:
     - `Projection`: `Orthographic`
     - `Size`: `5`
   - Inspector → Add Component → `CameraController`
   - Inspector → `CameraController`:
     - `Main Camera`: Kéo camera vào
     - `Target`: Kéo `Player` vào
     - `Map Bounds`: Kéo `MapBounds` vào
     - `Use Limits`: ✓
     - `Follow Speed`: `5`
     - `Delay Time`: `0.2`

5. **Tạo Building (Nhà)**:
   - Hierarchy → Right Click → `2D Object → Sprite`
   - Đặt tên: `House`
   - Inspector → `Sprite Renderer`: Kéo house sprite vào
   - Inspector → Add Component → `Box Collider 2D`:
     - `Is Trigger`: ✓
   - Inspector → Add Component → `BuildingDoor`
   - Inspector → `BuildingDoor`:
     - `Door SO`: Tạo DoorSO mới hoặc để trống
     - `Door Id`: `"house_door"` (nếu không có DoorSO)
     - `Target Scene Name`: `"House"`

6. **Tạo NPC**:
   - Hierarchy → Right Click → `2D Object → Sprite`
   - Đặt tên: `NPC_Shopkeeper`
   - Inspector → `Sprite Renderer`: Kéo NPC sprite vào
   - Inspector → Add Component → `NPCController`
   - Inspector → `NPCController`:
     - `NPC Definition`: Tạo NPCDefinitionSO mới
     - `Can Trade`: ✓
     - `Auto Refill Inventory`: ✓
     - `Refill Interval`: `300` (5 phút)
     - `Refill Amount`: `5`

7. **Tạo HUD (UI ngoài game)**:
   - Hierarchy → Right Click → `UI → Canvas`
   - Đặt tên: `GameCanvas`
   - Inspector → `Canvas Scaler`:
     - `UI Scale Mode`: `Scale With Screen Size`
     - `Reference Resolution`: `1920 x 1080`
   
   **a) Player Info Panel**:
   - `GameCanvas` → Right Click → `UI → Panel`
   - Đặt tên: `PlayerInfoPanel`
   - Inspector → `Rect Transform`:
     - Anchor: `Top-Left`
     - `Pos X`: `20`, `Pos Y`: `-20`
     - `Width`: `150`, `Height`: `60`
   - Inspector → `Image`: `Color`: `RGBA(40, 40, 40, 255)`
   - Add Component → `Horizontal Layout Group`
   - Thêm `Image` (icon) và `TextMeshPro` (name) vào
   
   **b) Time/Date Panel**:
   - Tương tự Player Info
   - Đặt tên: `TimeDatePanel`
   - `Pos X`: `180`
   - Thêm `TextMeshPro` hiển thị thời gian
   
   **c) Money Panel**:
   - Tương tự
   - Đặt tên: `MoneyPanel`
   - `Pos X`: `340`
   - Thêm icon và text hiển thị tiền
   
   **d) Inventory Button**:
   - `GameCanvas` → Right Click → `UI → Button - TextMeshPro`
   - Đặt tên: `InventoryButton`
   - Inspector → `Rect Transform`:
     - Anchor: `Top-Right`
     - `Pos X`: `-20`, `Pos Y`: `-20`
     - `Width`: `100`, `Height`: `50`
   - Inspector → `Button`: `Normal Color`: `#4A90E2`
   - Inspector → `TextMeshPro`: `Text`: `Inventory`

8. **Gắn HUDController**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `HUDController`
   - Inspector → Add Component → `HUDController`
   - Kéo các UI elements vào:
     - `PlayerInfoPanel` → `playerIcon`, `playerNameText`
     - `TimeDatePanel` → `timeText`, `dateText`
     - `MoneyPanel` → `moneyIcon`, `moneyAmountText`
     - `InventoryButton` → `inventoryButton`

9. **Tạo GameManager**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `GameManager`
   - Inspector → Add Component → `GameManager`

10. **Tạo TimeManager**:
    - Chọn `GameManager` GameObject
    - Inspector → Add Component → `TimeManager`
    - Inspector → `TimeManager`:
      - `Minutes Per Day`: `20` (tùy chỉnh)
      - `Start Day`: `1`
      - `Start Hour`: `6`
      - `Start Minute`: `0`

11. **Tạo AudioManager**:
    - Hierarchy → Right Click → `Create Empty`
    - Đặt tên: `AudioManager`
    - Inspector → Add Component → `AudioManager`

12. **Tạo SaveLoadManager**:
    - Hierarchy → Right Click → `Create Empty`
    - Đặt tên: `SaveLoadManager`
    - Inspector → Add Component → `SaveLoadManager`

13. **Tạo PauseController**:
    - Hierarchy → Right Click → `Create Empty`
    - Đặt tên: `PauseController`
    - Inspector → Add Component → `PauseController`
    - Tạo Pause Menu UI (xem phần MEDIUM)

---

#### **Scene 4: House (Nhà trong)**

1. **Tạo Scene mới**: `House.unity`

2. **Tạo Map**: Tương tự Game scene

3. **Tạo Player**: Tương tự hoặc dùng prefab

4. **Tạo Camera**: Tương tự Game scene

5. **Tạo Door (Cửa ra ngoài)**:
   - Tạo `BuildingDoor` để quay về Game scene
   - Inspector → `BuildingDoor`:
     - `Door Id`: `"house_exit"`
     - `Target Scene Name`: `"Game"`

6. **Tạo Furniture** (optional):
   - Tạo các sprite furniture

---

### 📦 BƯỚC 2: TẠO SCRIPTABLEOBJECTS

#### **Tạo GameDatabase**

1. **Project Window** → Right Click trong `Assets/GameData`
2. **Create → Game → Game Database**
3. Đặt tên: `GameDatabase`
4. Inspector → Sẽ tự động có ReorderableList cho tất cả lists

#### **Tạo ItemDataSO**

1. **Project Window** → Right Click → `Create → Game → Item Data`
2. Đặt tên: `CornItem`
3. Inspector:
   - `Item Id`: `"corn"`
   - `Item Name`: `"Corn"`
   - `Sell Price`: `50`
   - Kéo icon sprite vào `Icon`

4. **Lặp lại** cho các items khác:
   - `SeedCornItem`: `itemId = "seed_corn"`, `buyPrice = 20`
   - `WheatItem`: `itemId = "wheat"`, `sellPrice = 30`
   - `SeedWheatItem`: `itemId = "seed_wheat"`, `buyPrice = 10`

5. **Thêm vào GameDatabase**:
   - Mở `GameDatabase` asset
   - Inspector → `Items` list → Click nút `+`
   - Kéo các ItemDataSO vào

#### **Tạo NPCDefinitionSO**

1. **Project Window** → Right Click → `Create → Game → NPC Definition`
2. Đặt tên: `ShopkeeperNPC`
3. Inspector:
   - `NPC Id`: `"shopkeeper"`
   - `Display Name`: `"Shopkeeper"`
   - `Type`: `Shop`
   - `Is Shop`: ✓
   - `Base Buy Multiplier`: `80` (NPC mua với giá 80%)
   - `Base Sell Multiplier`: `120` (NPC bán với giá 120%)

#### **Tạo DoorSO**

1. **Project Window** → Right Click → `Create → Game → Door`
2. Đặt tên: `HouseDoor`
3. Inspector:
   - `Door Id`: `"house_door"`
   - `Door Name`: `"House"`
   - `Target Scene Name`: `"House"`
   - `Spawn Position`: `X: 0, Y: 0, Z: 0`
   - `Auto Transition`: ✗
   - `Interact Key`: `E`
   - `Show Prompt`: ✓

4. **Tạo DoorSO cho cửa ra ngoài**:
   - Đặt tên: `HouseExitDoor`
   - `Door Id`: `"house_exit"`
   - `Target Scene Name`: `"Game"`

---

### 🔧 BƯỚC 3: SETUP SCRIPTS

#### **Setup PlayerController**

1. Script đã có sẵn: `Assets/Game/Unity/Presentation/PlayerController.cs`
2. Gắn vào Player GameObject
3. Inspector → Setup các fields

#### **Setup NPCController**

1. Chọn NPC GameObject
2. Inspector → `NPCController`:
   - `NPC Definition`: Kéo `ShopkeeperNPC` vào
   - `Interaction Distance`: `2`
   - `Interact Key`: `E`
   - `Can Trade`: ✓
   - `Auto Refill Inventory`: ✓
   - `Refill Items`: Click `+` → Kéo các ItemDataSO vào

#### **Setup BuildingDoor**

1. Chọn Building GameObject
2. Inspector → `BuildingDoor`:
   - `Door SO`: Kéo `HouseDoor` vào
   - Hoặc set `Door Id`: `"house_door"`

#### **Setup CameraController**

1. Chọn Camera GameObject
2. Inspector → `CameraController`:
   - `Main Camera`: Kéo camera vào
   - `Target`: Kéo Player vào
   - `Map Bounds`: Kéo MapBounds vào
   - `Use Limits`: ✓

---

### 🎮 BƯỚC 4: TEST MVP

1. **Build Settings**:
   - `File → Build Settings`
   - Add Open Scenes:
     - `TitleScreen`
     - `Tutorial`
     - `Game`
     - `House`

2. **Test từng tính năng**:
   - ✅ Di chuyển player (WASD)
   - ✅ Vào nhà (E khi đứng gần cửa)
   - ✅ Ra ngoài (E khi đứng gần cửa trong nhà)
   - ✅ Tương tác với NPC (E)
   - ✅ Mở shop UI
   - ✅ Mua/bán items
   - ✅ Mở inventory (I hoặc button)
   - ✅ Save game
   - ✅ Load game

---

## 2. MEDIUM - PHÁT TRIỂN THÊM

### 🎯 Mục tiêu MEDIUM:
- ✅ Thêm farming system (trồng cây, thu hoạch)
- ✅ Thêm inventory system đầy đủ
- ✅ Thêm pause menu
- ✅ Thêm settings menu
- ✅ Thêm save/load slots
- ✅ Thêm nhiều NPCs
- ✅ Thêm nhiều buildings

---

### 📁 BƯỚC 1: THÊM SCRIPTABLEOBJECTS

#### **Tạo CropDataSO**

1. **Create → Game → Crop Data**
2. Đặt tên: `CornCrop`
3. Inspector:
   - `Crop Id`: `"corn"`
   - `Crop Name`: `"Corn"`
   - `Days To Grow`: `3`
   - `Days To Wilt`: `2`
   - `Harvest Yield`: `1`
   - `Harvest Item Id`: `"corn"`
   - `Seed Item Id`: `"seed_corn"`
   - Kéo sprites vào các growth stages

4. **Thêm vào GameDatabase** → `Crops` list

#### **Tạo TileStateSO**

1. **Create → Game → Tile State**
2. Tạo các states:
   - `NormalTileState`: `canPlow = true`
   - `PlowedTileState`: `canPlant = true`, `canWater = true`
   - `WateredTileState`: `canPlant = true`
   - `GrowingTileState`: `canWater = true`, `allowCropGrowth = true`
   - `MatureTileState`: `canHarvest = true`

3. **Thêm vào GameDatabase** → `Tile States` list

#### **Tạo ToolDataSO**

1. **Create → Game → Tool Data**
2. Tạo các tools:
   - `HoeTool`: `toolId = "hoe"`, `action = "Plow"`
   - `WateringCanTool`: `toolId = "watering_can"`, `action = "Water"`
   - `ScytheTool`: `toolId = "scythe"`, `action = "Harvest"`

3. **Thêm vào GameDatabase** → `Tools` list

---

### 🔧 BƯỚC 2: THÊM FARMING SYSTEM

#### **Setup FarmingController**

1. Script đã có sẵn: `Assets/Game/Unity/Presentation/FarmingController.cs`
2. Trong Game scene:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `FarmingController`
   - Inspector → Add Component → `FarmingController`
   - Inspector → `FarmingController`:
     - `Player Id`: `Default`
     - `Current Tool`: `Plow`

#### **Setup TilemapManager**

1. Script đã có sẵn: `Assets/Game/Unity/Presentation/TilemapManager.cs`
2. Trong Game scene:
   - Chọn `Ground` (Tilemap)
   - Inspector → Add Component → `TilemapManager`
   - Inspector → `TilemapManager`:
     - Gán `Tile State Manager` (nếu có)

---

### 🎨 BƯỚC 3: THÊM PAUSE MENU

1. **Tạo Pause Panel**:
   - `GameCanvas` → Right Click → `UI → Panel`
   - Đặt tên: `PausePanel`
   - Inspector → `Rect Transform`:
     - Anchor: `Stretch-Stretch`
     - `Left/Right/Top/Bottom`: `0`
   - Inspector → `Image`:
     - `Color`: `RGBA(0, 0, 0, 200)` (Semi-transparent overlay)

2. **Tạo Menu Buttons Panel**:
   - `PausePanel` → Right Click → `UI → Panel`
   - Đặt tên: `MenuButtonsPanel`
   - Inspector → `Rect Transform`:
     - Anchor: `Middle-Center`
     - `Width`: `400`, `Height`: `500`
   - Inspector → Add Component → `Vertical Layout Group`:
     - `Spacing`: `20`
     - `Padding`: `20`

3. **Tạo Buttons**:
   - `Resume Button`: `Text`: `Resume`
   - `Inventory Button`: `Text`: `Inventory`
   - `Save Button`: `Text`: `Save Game`
   - `Load Button`: `Text`: `Load Game`
   - `Settings Button`: `Text`: `Settings`
   - `Quit Button`: `Text`: `Quit to Title`

4. **Gắn PauseController**:
   - Chọn `PauseController` GameObject
   - Inspector → `PauseController`:
     - `Pause Panel`: Kéo `PausePanel` vào
     - `Menu Buttons Panel`: Kéo `MenuButtonsPanel` vào
     - `Pause Key`: `Escape`

---

### ⚙️ BƯỚC 4: THÊM SETTINGS MENU

1. **Tạo Settings Panel**:
   - `GameCanvas` → Right Click → `UI → Panel`
   - Đặt tên: `SettingsPanel`
   - Inspector → `Rect Transform`:
     - Anchor: `Middle-Center`
     - `Width`: `600`, `Height`: `700`

2. **Tạo Scroll View**:
   - `SettingsPanel` → Right Click → `UI → Scroll View`
   - Đặt tên: `SettingsScrollView`

3. **Tạo Volume Sliders**:
   - `SettingsScrollView/Content` → Right Click → `UI → Slider`
   - Đặt tên: `MusicVolumeSlider`
   - Inspector → `Slider`:
     - `Min Value`: `0`
     - `Max Value`: `1`
     - `Value`: `0.7`
   - Lặp lại cho: `SFXVolumeSlider`, `AmbientVolumeSlider`, `GameVoicesVolumeSlider`, `EventVoicesVolumeSlider`

4. **Gắn SettingsController hoặc DynamicSettingsController**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `SettingsController`
   - Inspector → Add Component → `SettingsController` hoặc `DynamicSettingsController`
   - Kéo các sliders vào

---

### 💾 BƯỚC 5: THÊM SAVE/LOAD SLOTS

1. **Tạo Save Slot List Panel**:
   - `GameCanvas` → Right Click → `UI → Panel`
   - Đặt tên: `SaveSlotListPanel`
   - Inspector → `Rect Transform`:
     - Anchor: `Middle-Center`
     - `Width`: `800`, `Height`: `600`

2. **Tạo Save Slot List Controller**:
   - Hierarchy → Right Click → `Create Empty`
   - Đặt tên: `SaveSlotListController`
   - Inspector → Add Component → `SaveSlotListController`
   - Inspector → `SaveSlotListController`:
     - `Save Slot Panel`: Kéo `SaveSlotListPanel` vào
     - `Save Slot Prefab`: Tạo prefab cho mỗi slot (optional)

3. **Gắn vào PauseController**:
   - Chọn `PauseController`
   - Inspector → `PauseController`:
     - `Save Slot List Controller`: Kéo `SaveSlotListController` vào

---

## 3. ADVANCED - TÍNH NĂNG NÂNG CAO

### 🎯 Mục tiêu ADVANCED:
- Quest system
- Dialogue system
- Friendship system với NPCs
- Event system
- Skill system
- Crafting system
- Weather system
- Day/Night cycle (đã có TimeManager)

---

### 📦 BƯỚC 1: THÊM SCRIPTABLEOBJECTS

#### **Tạo QuestSO**

1. **Create → Game → Quest**
2. Setup quest data:
   - Quest ID, name, description
   - Objectives
   - Rewards

#### **Tạo DialogueSO**

1. **Create → Game → Dialogue**
2. Setup dialogue tree:
   - Dialogue nodes
   - Choices
   - Conditions

#### **Tạo GameEventSO**

1. **Create → Game → Game Event**
2. Setup event data:
   - Event triggers
   - Event actions
   - Event conditions

---

## 4. CÁC SYSTEMS ĐÃ CÓ SẴN

### ✅ **Scripts Đã Tích Hợp:**

1. **AudioManager** (`Assets/Game/Unity/Audio/AudioManager.cs`)
   - Ambient sound, button sounds
   - Volume controls cho Music, SFX, Ambient, Game Voices, Event Voices
   - Flexible: Không lỗi nếu thiếu components

2. **CameraController** (`Assets/Game/Unity/Presentation/CameraController.cs`)
   - Camera limits, delay
   - Follow player
   - Map bounds

3. **BuildingDoor** (`Assets/Game/Unity/Presentation/BuildingDoor.cs`)
   - Camera delay trigger
   - Scene transitions
   - Flexible: Không lỗi nếu thiếu components

4. **NPCController** (`Assets/Game/Unity/NPC/NPCController.cs`)
   - Inventory, trading, auto refill
   - Save/Load support (NPC inventory và money)
   - Flexible: Không lỗi nếu thiếu components

5. **ButtonSoundHelper** (`Assets/Game/Unity/UI/ButtonSoundHelper.cs`)
   - Auto button sounds

6. **TitleScreenController** (`Assets/Game/Unity/UI/TitleScreenController.cs`)
   - Button sounds
   - Scene loading

7. **TutorialController** (`Assets/Game/Unity/UI/TutorialController.cs`)
   - Tutorial steps
   - Scene transitions

8. **HUDController** (`Assets/Game/Unity/UI/HUDController.cs`)
   - Player info, time/date, money
   - Inventory button
   - Auto update

9. **FarmingController** (`Assets/Game/Unity/Presentation/FarmingController.cs`)
   - Plow, water, plant, harvest
   - Command pattern

10. **TimeManager** (`Assets/Game/Unity/Managers/TimeManager.cs`)
    - In-game time (không phải real-time)
    - Pause/resume
    - Adjustable time scale
    - Day/Hour/Minute management

11. **SaveLoadManager** (`Assets/Game/Unity/SaveLoad/SaveLoadManager.cs`)
    - Save/Load game state
    - Multiple save slots
    - NPC inventory và money save/load
    - Flexible: Không lỗi nếu thiếu components

12. **PauseController** (`Assets/Game/Unity/UI/PauseController.cs`)
    - Pause menu
    - Inventory, Save/Load integration
    - Time pause/resume

13. **GameManager** (`Assets/Game/Unity/Managers/GameManager.cs`)
    - Service locator
    - Centralized game services
    - Scene loading

---

### 🔧 **Cách Sử Dụng:**

1. **Audio**: Gọi `AudioManager.Instance.PlayXXX()`
2. **Camera**: Gắn CameraController, set MapBounds
3. **NPCs**: Gắn NPCController, setup auto refill
4. **Doors**: Gắn BuildingDoor, gán DoorSO
5. **UI**: Sử dụng HUDController, PauseController
6. **Farming**: Gắn FarmingController, setup tools
7. **Time**: Gắn TimeManager vào GameManager
8. **Save/Load**: Gắn SaveLoadManager, gọi `SaveGame()` / `LoadGame()`

---

## 5. SETUP CHI TIẾT

### 📝 **Lưu Ý Quan Trọng:**

#### **Flexible Code:**
Tất cả scripts đã được thiết kế để:
- ✅ Không lỗi nếu thiếu components
- ✅ Tự động tìm components nếu cần
- ✅ Optional references (có thể để null)

#### **Time Management:**
- Time không phải real-time
- Có thể chỉnh trong Unity Inspector (TimeManager)
- Có thể pause/resume
- Có thể điều chỉnh time scale

#### **Save/Load:**
- Lưu player data, inventory, farm tiles
- Lưu NPC inventory và money (nếu có trading)
- Không lưu time (time được chỉnh trong Unity)
- Flexible: Không lỗi nếu thiếu NPC hoặc trading system

#### **NPC Trading:**
- NPC tự động refill inventory và money
- Player chỉ mua được với số tiền NPC có
- NPC inventory và money được lưu trong save

---

### ✅ **Checklist Tổng Hợp:**

#### **MVP Checklist:**
- [ ] TitleScreen scene
- [ ] Tutorial scene
- [ ] Game scene với player
- [ ] House scene
- [ ] Player movement
- [ ] Building door system
- [ ] NPC interaction
- [ ] Shop system
- [ ] Basic inventory
- [ ] Save/Load
- [ ] HUD (player info, time, money, inventory button)
- [ ] Audio system

#### **MEDIUM Checklist:**
- [ ] Farming system
- [ ] Full inventory system
- [ ] Pause menu
- [ ] Settings menu
- [ ] Multiple save slots
- [ ] Multiple NPCs
- [ ] Multiple buildings

#### **ADVANCED Checklist:**
- [ ] Quest system
- [ ] Dialogue system
- [ ] Friendship system
- [ ] Event system
- [ ] Skill system
- [ ] Crafting system
- [ ] Weather system

---

## 📚 TÀI LIỆU THAM KHẢO

- **Cấu trúc GameData**: `Documentation/01_CauTrucGame/`
- **Setup Game**: `Documentation/03_SetupGame/`
- **Kiến trúc**: `Documentation/04_KienTruc/`

---

*Hướng dẫn này được cập nhật theo code hiện tại. Tất cả scripts đã được tích hợp và sẵn sàng sử dụng.*

