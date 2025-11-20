# 🔴 HƯỚNG DẪN SETUP GAME - TỪNG BƯỚC CHI TIẾT

## 📋 MỤC LỤC
1. [Setup GameDatabase](#1-setup-gamedatabase)
2. [Setup Farming System (Trồng cây, Thu hoạch, Tưới nước)](#2-setup-farming-system)
3. [Setup Inventory System](#3-setup-inventory-system)
4. [Setup NPC Shop System](#4-setup-npc-shop-system)
5. [Test Game](#5-test-game)

---

## 1. SETUP GAMEDATABASE

### Bước 1.1: Tạo GameDatabase Asset
1. **Project Window** → Right-click trong folder `Assets/Game/Unity/Data` (hoặc folder bạn muốn)
2. **Create → Game → Game Database**
3. Đặt tên: `GameDatabase` (hoặc tên bạn muốn)
4. **Lưu ý:** File sẽ có extension `.asset`

### Bước 1.2: Tạo các ScriptableObjects cần thiết

#### **Tạo TileStateSO (Trạng thái đất):**
1. **Create → Game → Tile State**
2. Tạo các states sau:
   - **NormalTileState**: `stateId = "normal"`, `canPlow = true`
   - **PlowedTileState**: `stateId = "plowed"`, `canPlant = true`, `canWater = true`
   - **WateredTileState**: `stateId = "watered"`, `canPlant = true`
   - **SeededTileState**: `stateId = "seeded"`, `canWater = true`, `allowCropGrowth = true`
   - **GrowingTileState**: `stateId = "growing"`, `canWater = true`, `allowCropGrowth = true`
   - **MatureTileState**: `stateId = "mature"`, `canHarvest = true`

#### **Tạo ItemDataSO (Vật phẩm):**
1. **Create → Game → Item Data**
2. Tạo các items sau:
   - **CornItem**: `itemId = "corn"`, `itemName = "Corn"`, `tags = Stackable`, `sellPrice = 50`
   - **SeedCornItem**: `itemId = "seed_corn"`, `itemName = "Corn Seed"`, `tags = Stackable | Seed`, `buyPrice = 20`
   - **WheatItem**: `itemId = "wheat"`, `itemName = "Wheat"`, `tags = Stackable`, `sellPrice = 30`
   - **SeedWheatItem**: `itemId = "seed_wheat"`, `itemName = "Wheat Seed"`, `tags = Stackable | Seed`, `buyPrice = 10`

#### **Tạo CropDataSO (Cây trồng):**
1. **Create → Game → Crop Data**
2. Tạo các crops sau:
   - **CornCrop**: 
     - `cropId = "corn"`, `cropName = "Corn"`
     - `daysToGrow = 3`, `daysToWilt = 2`
     - `harvestYield = 1`, `harvestItemId = "corn"`
     - `seedItemId = "seed_corn"`
   - **WheatCrop**: 
     - `cropId = "wheat"`, `cropName = "Wheat"`
     - `daysToGrow = 2`, `daysToWilt = 1`
     - `harvestYield = 1`, `harvestItemId = "wheat"`
     - `seedItemId = "seed_wheat"`

#### **Tạo ToolDataSO (Công cụ):**
1. **Create → Game → Tool Data**
2. Tạo các tools sau:
   - **HoeTool**: `toolId = "hoe"`, `toolName = "Hoe"`, `actionType = Plow`
   - **WateringCanTool**: `toolId = "watering_can"`, `toolName = "Watering Can"`, `actionType = Water`, `isRefillable = true`
   - **GloveTool**: `toolId = "glove"`, `toolName = "Gloves"`, `actionType = Plant`, `isSpecialTool = true`, `specialInteractionType = SeedSelection`
   - **HarvestTool**: `toolId = "harvest"`, `toolName = "Scythe"`, `actionType = Harvest`

### Bước 1.3: Add vào GameDatabase
1. **Chọn GameDatabase asset** → Inspector
2. **Items:** Click "+" và kéo các ItemDataSO vào
3. **Crops:** Click "+" và kéo các CropDataSO vào
4. **Tools:** Click "+" và kéo các ToolDataSO vào
5. **Tile States:** Click "+" và kéo các TileStateSO vào

### Bước 1.4: Setup GameDatabaseManager trong Scene
1. **Hierarchy** → Tạo Empty GameObject → đặt tên `GameDatabaseManager`
2. **Add Component** → `GameDatabaseManager`
3. **Inspector → Database:** Kéo `GameDatabase` asset vào
4. **Inspector → Auto Load From Resources:** ✓ (nếu muốn tự động load từ Resources)

### Bước 1.5: Test Database
1. **Play game**
2. **Console** sẽ hiển thị: `GameDatabase initialized: X items, Y crops, Z tools, W tile states`

---

## 2. SETUP FARMING SYSTEM

### Bước 2.1: Tạo TileStateManager
1. **Hierarchy** → Tạo Empty GameObject → đặt tên `TileStateManager`
2. **Add Component** → `TileStateManager`
3. **Inspector → Tile State Database:** Sẽ tự động lấy từ GameDatabase

### Bước 2.2: Tạo FarmRepository
1. **Hierarchy** → Tạo Empty GameObject → đặt tên `FarmRepository`
2. **Add Component** → `FarmRepository`
3. **Inspector → Settings:** Có thể để mặc định

### Bước 2.3: Tạo FarmingService (qua GameManager)
1. **GameManager** đã có `FarmingService` (tự động tạo)
2. **Kiểm tra:** `GameManager.FarmingService` không null

### Bước 2.4: Link ToolInteractionSystem với FarmingService
1. **Chọn GameObject có ToolInteractionSystem** → Inspector
2. **Tool Interaction System → References:** 
   - `TilemapManager`: Kéo TilemapManager vào
   - `ToolManager`: Kéo ToolManager vào (nếu có)

### Bước 2.5: Test Farming System

#### **Test Cuốc đất:**
1. **Chọn tool Hoe** trong ToolSelectionUI
2. **Click vào ground tile** (hoặc nhấn Z)
3. **Kết quả:** Tile chuyển sang state "plowed"

#### **Test Trồng cây:**
1. **Chọn tool Glove** trong ToolSelectionUI
2. **Click vào plowed tile** (hoặc nhấn Z)
3. **Kết quả:** Hiện menu chọn hạt giống (nếu có seeds trong inventory)
4. **Chọn seed** → Cây được trồng

#### **Test Tưới nước:**
1. **Chọn tool Watering Can** trong ToolSelectionUI
2. **Click vào seeded tile** (hoặc nhấn Z)
3. **Kết quả:** 
   - Tile chuyển sang state "watered" hoặc "growing"
   - **Cây tăng 1 growth stage** (Seed → Sprout → Growing → Mature)
   - **Mỗi lần tưới = 1 growth stage** (theo yêu cầu)

#### **Test Thu hoạch:**
1. **Chọn tool Harvest (Scythe)** trong ToolSelectionUI
2. **Click vào mature tile** (hoặc nhấn Z)
3. **Kết quả:** 
   - Cây được thu hoạch
   - Item được thêm vào inventory
   - Tile chuyển về state "plowed"

### ✅ Lưu ý về Tưới nước:
- **Mỗi lần tưới nước = tăng 1 growth stage:**
  - Seed → Sprout (tưới lần 1)
  - Sprout → Growing (tưới lần 2)
  - Growing → Mature (tưới lần 3)
- **Cây cần tưới 3 lần để mature** (nếu bắt đầu từ Seed)
- **Sau khi mature, tưới nước không làm gì** (giữ nguyên Mature)

---

## 3. SETUP INVENTORY SYSTEM

### Bước 3.1: Tạo InventoryRepository
1. **Hierarchy** → Tạo Empty GameObject → đặt tên `InventoryRepository`
2. **Add Component** → `InventoryRepository`
3. **Inspector → Settings:** Có thể để mặc định

### Bước 3.2: Tạo InventoryService (qua GameManager)
1. **GameManager** đã có `InventoryService` (tự động tạo)
2. **Kiểm tra:** `GameManager.InventoryService` không null

### Bước 3.3: Setup InventoryUI
1. **Hierarchy** → Tạo UI Canvas (nếu chưa có)
2. **Tạo Inventory Panel:**
   - Right-click Canvas → UI → Panel → đặt tên `InventoryPanel`
   - **Add Component** → `InventoryUIController`
3. **Tạo Inventory Slot Template:**
   - Right-click InventoryPanel → UI → Button → đặt tên `InventorySlotTemplate`
   - **Setup:** Thêm Image (icon), TextMeshPro (quantity)
4. **Link vào InventoryUIController:**
   - `Inventory Panel`: Kéo InventoryPanel vào
   - `Slot Container`: Tạo Empty GameObject `SlotContainer` trong InventoryPanel, kéo vào
   - `Slot Template`: Kéo InventorySlotTemplate vào
   - `Player ID`: Để mặc định (Default)

### Bước 3.4: Test Inventory System

#### **Test Add Item:**
```csharp
// Trong code hoặc Inspector (tạm thời test)
GameManager.InventoryService.AddItemById(PlayerId.Default, new ItemId("corn"), 5);
```

#### **Test Remove Item:**
```csharp
GameManager.InventoryService.RemoveItem(PlayerId.Default, new ItemId("corn"), 2);
```

#### **Test Inventory UI:**
1. **Mở Inventory Panel** (nếu có button)
2. **Kiểm tra:** Items hiển thị đúng với inventory
3. **Kiểm tra:** Quantity hiển thị đúng

### ✅ Lưu ý:
- **Inventory tự động lưu/load** khi save/load game
- **Items từ harvest** tự động được thêm vào inventory
- **Items từ shop** tự động được thêm/xóa khỏi inventory

---

## 4. SETUP NPC SHOP SYSTEM

### Bước 4.1: Tạo NPCShopUI
1. **Hierarchy** → Tạo UI Canvas (nếu chưa có)
2. **Tạo Shop Panel:**
   - Right-click Canvas → UI → Panel → đặt tên `ShopPanel`
   - **Add Component** → `NPCShopUI`
3. **Setup Shop UI:**
   - `Panel`: Kéo ShopPanel vào
   - `Buy Item Container`: Tạo Empty GameObject `BuyItemContainer` trong ShopPanel, kéo vào
   - `Sell Item Container`: Tạo Empty GameObject `SellItemContainer` trong ShopPanel, kéo vào
   - `Shop Item Prefab`: Tạo Button `ShopItemPrefab` (có Image, TextMeshPro), kéo vào
   - `Buy Tab Button`: Tạo Button `BuyTabButton`, kéo vào
   - `Sell Tab Button`: Tạo Button `SellTabButton`, kéo vào
   - `Player Money Text`: Tạo TextMeshPro `PlayerMoneyText`, kéo vào
   - `NPC ID`: Để mặc định (Default)
   - `Player ID`: Để mặc định (Default)

### Bước 4.2: Tạo ShopNPC
1. **Hierarchy** → Tạo Empty GameObject → đặt tên `ShopNPC`
2. **Add Component** → `ShopNPC`
3. **Add Component** → `NPCDefinition` (nếu cần)
4. **Inspector → Shop UI:** Kéo ShopPanel (có NPCShopUI) vào
5. **Inspector → NPC ID:** Set NPC ID (ví dụ: "shopkeeper")

### Bước 4.3: Setup ShopService (qua GameManager)
1. **GameManager** đã có `ShopService` (tự động tạo)
2. **Kiểm tra:** `GameManager.ShopService` không null

### Bước 4.4: Test NPC Shop System

#### **Test Mua Item:**
1. **Tương tác với ShopNPC** (click vào NPC)
2. **Shop Panel mở ra**
3. **Click Buy Tab** → Hiển thị items có thể mua (TODO: Cần setup NPC inventory)
4. **Click item** → Mua item (trừ tiền, thêm item vào inventory)

#### **Test Bán Item:**
1. **Tương tác với ShopNPC**
2. **Shop Panel mở ra**
3. **Click Sell Tab** → Hiển thị items trong inventory (có thể bán)
4. **Click item** → Bán item (thêm tiền, xóa item khỏi inventory)

### ⚠️ Lưu ý:
- **NPC Shop hiện tại chỉ hỗ trợ Sell** (bán items từ inventory)
- **Buy Tab cần setup NPC inventory** (TODO trong code)
- **Items phải có `sellPrice > 0`** để có thể bán
- **Player phải có đủ tiền** để mua items

---

## 5. TEST GAME

### Test Flow hoàn chỉnh:
1. **Cuốc đất** → Tile chuyển sang "plowed"
2. **Trồng cây** → Chọn seed từ inventory → Cây được trồng
3. **Tưới nước** → Cây tăng growth stage (Seed → Sprout → Growing → Mature)
4. **Thu hoạch** → Item được thêm vào inventory
5. **Bán item** → Tương tác với ShopNPC → Bán item → Nhận tiền
6. **Mua seed** → Tương tác với ShopNPC → Mua seed → Trồng lại

### ✅ Kết quả mong đợi:
- **Farming System:** Cuốc → Trồng → Tưới (3 lần) → Thu hoạch
- **Inventory System:** Items được thêm/xóa đúng
- **NPC Shop System:** Bán items, nhận tiền (Mua items - TODO)
- **Save/Load:** Tất cả data được lưu/load đúng

---

## 📝 TÓM TẮT CÁC BƯỚC

### 1. GameDatabase:
- ✅ Tạo GameDatabase asset
- ✅ Tạo TileStateSO, ItemDataSO, CropDataSO, ToolDataSO
- ✅ Add vào GameDatabase
- ✅ Setup GameDatabaseManager trong scene

### 2. Farming System:
- ✅ Setup TileStateManager, FarmRepository
- ✅ Test cuốc đất, trồng cây, tưới nước, thu hoạch
- ✅ **Tưới nước:** Mỗi lần tưới = 1 growth stage

### 3. Inventory System:
- ✅ Setup InventoryRepository, InventoryService
- ✅ Setup InventoryUI
- ✅ Test add/remove items

### 4. NPC Shop System:
- ✅ Setup NPCShopUI, ShopNPC
- ✅ Test bán items (Mua items - TODO)

### 5. Test Game:
- ✅ Test flow hoàn chỉnh: Cuốc → Trồng → Tưới → Thu hoạch → Bán

---

## 🔧 TROUBLESHOOTING

### Lỗi: "GameDatabase not found!"
- **Nguyên nhân:** GameDatabaseManager không tìm thấy GameDatabase
- **Giải pháp:** 
  1. Kiểm tra GameDatabase asset đã được tạo chưa
  2. Kiểm tra GameDatabaseManager có reference đến GameDatabase chưa
  3. Hoặc đặt GameDatabase vào `Resources/GameDatabase.asset`

### Lỗi: "Item not found in database!"
- **Nguyên nhân:** ItemDataSO chưa được add vào GameDatabase
- **Giải pháp:** Add ItemDataSO vào GameDatabase → Items list

### Lỗi: "Cannot plant on this tile!"
- **Nguyên nhân:** Tile chưa được cuốc (chưa ở state "plowed")
- **Giải pháp:** Cuốc đất trước khi trồng cây

### Lỗi: "Cannot harvest this tile!"
- **Nguyên nhân:** Cây chưa mature
- **Giải pháp:** Tưới nước đủ 3 lần để cây mature

---

## 📚 TÀI LIỆU THAM KHẢO

- **GameDatabase:** `Assets/Game/Unity/Data/GameDatabase.cs`
- **FarmingService:** `Assets/Game/Core/Application/Services/FarmingService.cs`
- **InventoryService:** `Assets/Game/Core/Application/Services/InventoryService.cs`
- **ShopService:** `Assets/Game/Core/Application/Services/ShopService.cs`
- **Crop:** `Assets/Game/Core/Domain/Entities/Crop.cs` (Water() method tăng growth stage)

---

**Chúc bạn setup thành công! 🎉**

