# 🎮 HƯỚNG DẪN SETUP GAME - THE GREEN MEMOIR
## 📖 Hướng dẫn chi tiết từng bước (Dễ hiểu như cho trẻ 5 tuổi)

---

## 📚 GIẢI THÍCH QUAN TRỌNG - ĐỌC KỸ TRƯỚC KHI LÀM:

### ❓ TileStateSO là gì?
- **TileStateSO** = Trạng thái LOGIC của ĐẤT (tile)
- **KHÔNG CẦN SPRITE!** TileState chỉ quy định: "Với ô đất này, player có thể làm gì?"
- Ví dụ: `canPlow`, `canWater`, `canPlant`, `canHarvest` = các hành động được phép
- **TileState "Growing"** = Trạng thái logic: "Đất có cây đang lớn, có thể tưới, chưa thể thu hoạch"

### ❓ CropDataSO sprites là gì?
- **CropDataSO sprites** = Hình ảnh của CÂY (seedSprite, sproutSprite, growingSprite, matureSprite)
- **ĐÂY LÀ HÌNH ẢNH HIỂN THỊ TRÊN MÀN HÌNH!**
- Khi cây ở GrowthStage.Growing → game hiển thị `growingSprite` từ CropDataSO
- **KHÔNG LẤY TỪ TILESTATE!** TileState không có sprite, chỉ có logic

### ❓ GrowthStage là gì?
- **GrowthStage** = Giai đoạn phát triển của CÂY (Seed → Sprout → Growing → Mature → Wilted)
- Đây là logic của CÂY, không phải của đất
- Game dùng GrowthStage để chọn sprite từ CropDataSO:
  - GrowthStage.Seed → seedSprite
  - GrowthStage.Sprout → sproutSprite
  - GrowthStage.Growing → **growingSprite** ⬅️ **ĐÂY LÀ HÌNH ẢNH CÂY KHI ĐANG LỚN!**
  - GrowthStage.Mature → matureSprite

### ✅ Tóm tắt QUAN TRỌNG:
1. **TileState "Growing"** = Trạng thái LOGIC của đất (KHÔNG CẦN SPRITE)
   - Chỉ quy định: có thể tưới, chưa thể thu hoạch
   - KHÔNG có hình ảnh, chỉ là logic
   
2. **CropDataSO "growingSprite"** = HÌNH ẢNH CÂY khi đang lớn (SPRITE HIỂN THỊ)
   - Đây là hình ảnh bạn thấy trên màn hình
   - Lấy từ CropDataSO, KHÔNG lấy từ TileState
   
3. **GrowthStage.Growing** = Giai đoạn cây đang lớn (LOGIC CÂY)
   - Game dùng GrowthStage để chọn sprite từ CropDataSO
   - Khi GrowthStage = Growing → hiển thị growingSprite

### 🎯 Kết luận:
- **TileState "Growing" KHÔNG CẦN SPRITE** - chỉ là logic state
- **Hình ảnh cây = CropDataSO sprites** (growingSprite, matureSprite, v.v.)
- **TileState chỉ quy định: có thể làm gì với tile này** (canPlow, canWater, canPlant, canHarvest)

### 🔍 CÁCH HỆ THỐNG HOẠT ĐỘNG (Ví dụ cụ thể):

**Khi cây ở giai đoạn "Growing" (đang lớn):**

1. **Crop.CurrentStage = GrowthStage.Growing** (logic của cây)
   - Cây đang ở giai đoạn "Growing" (đã tưới nước 2 lần)

2. **TileState = "Growing"** (logic của đất)
   - TileState quy định: có thể tưới, chưa thể thu hoạch
   - **KHÔNG CÓ SPRITE!** Chỉ là logic

3. **Hình ảnh hiển thị = CropDataSO.growingSprite** (hình ảnh cây)
   - Game lấy sprite từ CropDataSO dựa trên GrowthStage.Growing
   - **ĐÂY LÀ HÌNH ẢNH BẠN THẤY TRÊN MÀN HÌNH!**
   - Ví dụ: Cây ngô đang lớn (chưa chín)

**Tóm lại:**
- **TileState "Growing"** = Logic: "Có thể tưới, chưa thể thu hoạch" (KHÔNG CẦN SPRITE)
- **CropDataSO.growingSprite** = Hình ảnh: "Cây đang lớn" (CẦN SPRITE - đây là hình ảnh bạn thấy)
- **GrowthStage.Growing** = Giai đoạn: "Cây đang lớn" (dùng để chọn sprite từ CropDataSO)

---

## 🔴 BƯỚC 1: TẠO GAMEDATABASE (Bước đầu tiên - BẮT BUỘC)

### 🎯 Mục đích:
Tạo một "thư viện" chứa tất cả items, crops, tools, tile states trong game.

### 📝 Các bước chi tiết:

#### Bước 1.1: Tạo GameDatabase Asset
1. **Mở Unity Editor**
2. **Click chuột phải vào Project window** (cửa sổ bên dưới, nơi hiển thị các file)
3. **Chọn:** `Create` → `Game` → `Game Database`
4. **Đặt tên:** `GameDatabase` (nhấn Enter)
5. **Lưu vào:** `Assets/Game/Unity/Data/` (hoặc bất kỳ đâu bạn muốn)

#### Bước 1.2: Kiểm tra GameDatabase đã tạo
1. **Click vào file `GameDatabase`** vừa tạo trong Project window
2. **Nhìn vào Inspector** (cửa sổ bên phải)
3. **Bạn sẽ thấy các lists trống:**
   - **Items:** (List rỗng)
   - **Crops:** (List rỗng)
   - **Tools:** (List rỗng)
   - **Tile States:** (List rỗng)
   - **Buildings:** (List rỗng)
4. **✅ OK! Database đã tạo xong!**

---

## 🔴 BƯỚC 2: TẠO ITEMDATASO (Items, Seeds, Crops)

### 🎯 Mục đích:
Tạo các vật phẩm trong game (hạt giống, sản phẩm thu hoạch, v.v.)

### 📝 Các bước chi tiết:

#### Bước 2.1: Tạo Seed Item (Hạt giống Ngô)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Item Data`
3. **Đặt tên:** `SeedCorn` (nhấn Enter)
4. **Click vào file `SeedCorn`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **Item ID:** Gõ `seed_corn` (ID duy nhất, không trùng)
   - **Item Name:** Gõ `Hạt Ngô` (tên hiển thị)
   - **Description:** Gõ `Hạt giống ngô, có thể trồng trên đất đã cuốc`

   **🖼️ Hình ảnh:**
   - **Icon:** Kéo sprite hạt giống vào đây (từ Project window)
     - Nếu chưa có sprite: Bỏ qua bước này, game vẫn chạy được

   **⚙️ Thuộc tính:**
   - **Max Stack Size:** Gõ `99` (số lượng tối đa trong 1 stack)
   
   **🏷️ Tags (Quan trọng!):**
   - Tìm phần **Tags**
   - **Tick vào:** `Seed` (đánh dấu đây là hạt giống)
   - **Tick vào:** `Stackable` (đánh dấu có thể xếp chồng)

   **💰 Giá:**
   - **Sell Price:** Gõ `10` (giá bán)
   - **Buy Price:** Gõ `20` (giá mua, nếu có)

6. **✅ OK! Seed Item đã tạo xong!**

#### Bước 2.2: Tạo Crop Item (Sản phẩm thu hoạch - Ngô)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Item Data`
3. **Đặt tên:** `Corn` (nhấn Enter)
4. **Click vào file `Corn`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **Item ID:** Gõ `corn` (ID duy nhất)
   - **Item Name:** Gõ `Ngô` (tên hiển thị)
   - **Description:** Gõ `Ngô thu hoạch, có thể bán`

   **🖼️ Hình ảnh:**
   - **Icon:** Kéo sprite ngô vào đây

   **⚙️ Thuộc tính:**
   - **Max Stack Size:** Gõ `99`

   **🏷️ Tags:**
   - **Tick vào:** `Stackable` (có thể xếp chồng)
   - **Tick vào:** `Sellable` (có thể bán)

   **💰 Giá:**
   - **Sell Price:** Gõ `50` (giá bán)
   - **Buy Price:** Gõ `0` (không mua được)

6. **✅ OK! Crop Item đã tạo xong!**

#### Bước 2.3: Add Items vào GameDatabase
1. **Click vào file `GameDatabase`** trong Project window
2. **Nhìn vào Inspector**
3. **Tìm phần "Items"** (có dấu ▶️ bên cạnh)
4. **Click vào dấu ▶️** để mở rộng
5. **Click vào nút "+"** (thêm item mới)
6. **Kéo file `SeedCorn`** từ Project window vào ô trống vừa tạo
7. **Click vào nút "+"** lần nữa
8. **Kéo file `Corn`** từ Project window vào ô trống vừa tạo
9. **✅ OK! Items đã được add vào Database!**

---

## 🔴 BƯỚC 3: TẠO CROPDATASO (Cây trồng)

### 🎯 Mục đích:
Tạo thông tin về cây trồng (hình ảnh, thời gian phát triển, v.v.)

### 📝 Các bước chi tiết:

#### Bước 3.1: Tạo Crop Data (Cây Ngô)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Crop Data`
3. **Đặt tên:** `CornCrop` (nhấn Enter)
4. **Click vào file `CornCrop`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **Crop ID:** Gõ `corn_crop` (ID duy nhất)
   - **Crop Name:** Gõ `Ngô` (tên hiển thị)

   **🖼️ Hình ảnh - Trạng thái KHÔ (Dry):**
   - Tìm phần **"Visual - Dry State (Khô)"**
   - **Seed Sprite Dry:** Kéo sprite hạt giống (khô) vào đây
   - **Sprout Sprite Dry:** Kéo sprite mầm (khô) vào đây
   - **Growing Sprite Dry:** Kéo sprite cây đang lớn (khô) vào đây ⬅️ **ĐÂY LÀ HÌNH ẢNH CÂY KHI ĐANG LỚN (GIAI ĐOẠN GROWING)!**
   - **Mature Sprite Dry:** Kéo sprite cây chín (khô) vào đây
   - **Wilted Sprite Dry:** Kéo sprite cây héo (khô) vào đây

   **🖼️ Hình ảnh - Trạng thái ƯỚT (Wet):**
   - Tìm phần **"Visual - Wet State (Ướt)"**
   - **Seed Sprite Wet:** Kéo sprite hạt giống (ướt) vào đây
   - **Sprout Sprite Wet:** Kéo sprite mầm (ướt) vào đây
   - **Growing Sprite Wet:** Kéo sprite cây đang lớn (ướt) vào đây ⬅️ **ĐÂY LÀ HÌNH ẢNH CÂY KHI ĐANG LỚN (ĐÃ TƯỚI NƯỚC)!**
   - **Mature Sprite Wet:** Kéo sprite cây chín (ướt) vào đây
   - **Wilted Sprite Wet:** Kéo sprite cây héo (ướt) vào đây

   **⚠️ Lưu ý QUAN TRỌNG về sprites:**
   - **Growing Sprite** = Hình ảnh CÂY khi nó ở giai đoạn "Growing" (đang lớn)
   - **ĐÂY LÀ HÌNH ẢNH HIỂN THỊ TRÊN MÀN HÌNH!**
   - **KHÔNG LẤY TỪ TILESTATE!** TileState "Growing" KHÔNG CẦN sprite
   - Nếu bạn chưa có sprites: Bỏ qua, game vẫn chạy được (sẽ không hiển thị hình ảnh)
   - Nếu bạn chỉ có 1 bộ sprites: Kéo vào phần "Dry" hoặc "Wet" (game sẽ dùng chung)
   - **Khi nào hiển thị growingSprite?** Khi Crop.CurrentStage = GrowthStage.Growing (sau khi tưới nước lần 2)

   **⚙️ Cài đặt phát triển:**
   - **Days To Grow:** Gõ `5` (số ngày phát triển - nhưng tạm thời dùng tưới nước)
   - **Days To Wilt:** Gõ `2` (số ngày không tưới sẽ héo)

   **🌾 Cài đặt thu hoạch:**
   - **Harvest Yield:** Gõ `1` (số lượng thu hoạch)
   - **Harvest Item ID:** Gõ `corn` (ID của item thu hoạch - phải giống với Item ID của `Corn`)
   - **Seed Item ID:** Gõ `seed_corn` (ID của hạt giống - phải giống với Item ID của `SeedCorn`)

6. **✅ OK! Crop Data đã tạo xong!**

#### Bước 3.2: Add Crop vào GameDatabase
1. **Click vào file `GameDatabase`** trong Project window
2. **Nhìn vào Inspector**
3. **Tìm phần "Crops"** (có dấu ▶️ bên cạnh)
4. **Click vào dấu ▶️** để mở rộng
5. **Click vào nút "+"** (thêm crop mới)
6. **Kéo file `CornCrop`** từ Project window vào ô trống vừa tạo
7. **✅ OK! Crop đã được add vào Database!**

---

## 🔴 BƯỚC 4: TẠO TILESTATESO (Trạng thái đất)

### 🎯 Mục đích:
Tạo các trạng thái đất (bình thường, đã cuốc, đã trồng, đang lớn, đã chín, v.v.)

### 📝 Các bước chi tiết:

#### Bước 4.1: Tạo Tile State - Normal (Đất bình thường)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tile State`
3. **Đặt tên:** `TileState_Normal` (nhấn Enter)
4. **Click vào file `TileState_Normal`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **State ID:** Gõ `normal` (ID duy nhất)
   - **Display Name:** Gõ `Normal` (tên hiển thị)
   - **State Type:** Chọn `Normal` (từ dropdown menu)

   **⚙️ Thuộc tính (Properties):**
   - **Allow Crop Growth:** ✗ (không tick) - Đất bình thường chưa có cây
   - **Can Plow:** ✓ (tick) - Có thể cuốc đất
   - **Can Plant:** ✗ (không tick) - Chưa thể trồng
   - **Can Water:** ✗ (không tick) - Chưa thể tưới
   - **Can Harvest:** ✗ (không tick) - Chưa thể thu hoạch

6. **✅ OK! Tile State Normal đã tạo xong!**

#### Bước 4.2: Tạo Tile State - Plowed (Đất đã cuốc)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tile State`
3. **Đặt tên:** `TileState_Plowed` (nhấn Enter)
4. **Click vào file `TileState_Plowed`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **State ID:** Gõ `plowed`
   - **Display Name:** Gõ `Plowed`
   - **State Type:** Chọn `Plowed`

   **⚙️ Thuộc tính:**
   - **Allow Crop Growth:** ✗ (không tick)
   - **Can Plow:** ✗ (không tick) - Đã cuốc rồi, không cuốc nữa
   - **Can Plant:** ✓ (tick) - Có thể trồng
   - **Can Water:** ✓ (tick) - Có thể tưới
   - **Can Harvest:** ✗ (không tick)

6. **✅ OK! Tile State Plowed đã tạo xong!**

#### Bước 4.3: Tạo Tile State - Planted (Đất đã trồng)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tile State`
3. **Đặt tên:** `TileState_Planted` (nhấn Enter)
4. **Click vào file `TileState_Planted`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **State ID:** Gõ `planted`
   - **Display Name:** Gõ `Planted`
   - **State Type:** Chọn `Seeded` (đã gieo hạt)

   **⚙️ Thuộc tính:**
   - **Allow Crop Growth:** ✓ (tick) - Cho phép cây phát triển
   - **Can Plow:** ✗ (không tick)
   - **Can Plant:** ✗ (không tick) - Đã trồng rồi
   - **Can Water:** ✓ (tick) - Có thể tưới
   - **Can Harvest:** ✗ (không tick) - Chưa chín

6. **✅ OK! Tile State Planted đã tạo xong!**

#### Bước 4.4: Tạo Tile State - SeededWatered (Đất đã trồng và tưới)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tile State`
3. **Đặt tên:** `TileState_SeededWatered` (nhấn Enter)
4. **Click vào file `TileState_SeededWatered`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **State ID:** Gõ `seeded_watered`
   - **Display Name:** Gõ `Seeded Watered`
   - **State Type:** Chọn `SeededWatered`

   **⚙️ Thuộc tính:**
   - **Allow Crop Growth:** ✓ (tick) - Cho phép cây phát triển
   - **Can Plow:** ✗ (không tick)
   - **Can Plant:** ✗ (không tick)
   - **Can Water:** ✗ (không tick) - Đã tưới rồi
   - **Can Harvest:** ✗ (không tick) - Chưa chín

6. **✅ OK! Tile State SeededWatered đã tạo xong!**

#### Bước 4.5: Tạo Tile State - Growing (Đất có cây đang lớn) ⬅️ **QUAN TRỌNG!**

**⚠️ LƯU Ý QUAN TRỌNG TRƯỚC KHI LÀM:**
- **TileState "Growing" KHÔNG CẦN SPRITE!**
- TileState chỉ là trạng thái LOGIC (có thể làm gì với tile này)
- **Hình ảnh cây được lấy từ CropDataSO.growingSprite**, KHÔNG lấy từ TileState!
- TileState "Growing" chỉ quy định: có thể tưới, chưa thể thu hoạch

1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tile State`
3. **Đặt tên:** `TileState_Growing` (nhấn Enter)
4. **Click vào file `TileState_Growing`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **State ID:** Gõ `growing`
   - **Display Name:** Gõ `Growing`
   - **State Type:** Chọn `Growing` ⬅️ **ĐÂY LÀ TRẠNG THÁI LOGIC "GROWING"!**

   **⚙️ Thuộc tính:**
   - **Allow Crop Growth:** ✓ (tick) - Cho phép cây phát triển
   - **Can Plow:** ✗ (không tick)
   - **Can Plant:** ✗ (không tick)
   - **Can Water:** ✓ (tick) - Có thể tưới tiếp để cây lớn hơn
   - **Can Harvest:** ✗ (không tick) - Chưa chín

   **🖼️ Visual (Optional):**
   - **Sprite:** ĐỂ TRỐNG! (TileState không cần sprite)
   - **TileBase:** ĐỂ TRỐNG! (TileState không cần tile)
   - **Display Color:** ĐỂ MẶC ĐỊNH (White)

   **💡 Giải thích CHI TIẾT:**
   - **TileState "Growing"** = Trạng thái LOGIC của đất khi có cây đang lớn
   - **KHÔNG CÓ SPRITE!** TileState chỉ quy định: "Có thể tưới, chưa thể thu hoạch"
   - **Hình ảnh cây** = Lấy từ **CropDataSO.growingSprite** khi Crop.CurrentStage = GrowthStage.Growing
   - **TileState chỉ là logic, hình ảnh cây là từ CropDataSO!**
   - **Ví dụ:** Khi cây ở GrowthStage.Growing:
     - TileState = "Growing" (logic: có thể tưới, chưa thể thu hoạch)
     - Hình ảnh hiển thị = CropDataSO.growingSprite (hình ảnh cây đang lớn)

6. **✅ OK! Tile State Growing đã tạo xong! (KHÔNG CẦN SPRITE!)**

#### Bước 4.6: Tạo Tile State - Mature (Cây đã chín)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tile State`
3. **Đặt tên:** `TileState_Mature` (nhấn Enter)
4. **Click vào file `TileState_Mature`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **State ID:** Gõ `mature`
   - **Display Name:** Gõ `Mature`
   - **State Type:** Chọn `Mature`

   **⚙️ Thuộc tính:**
   - **Allow Crop Growth:** ✓ (tick)
   - **Can Plow:** ✗ (không tick)
   - **Can Plant:** ✗ (không tick)
   - **Can Water:** ✗ (không tick) - Đã chín rồi, không cần tưới
   - **Can Harvest:** ✓ (tick) - CÓ THỂ THU HOẠCH! ⬅️ **QUAN TRỌNG!**

6. **✅ OK! Tile State Mature đã tạo xong!**

#### Bước 4.7: Add Tile States vào GameDatabase
1. **Click vào file `GameDatabase`** trong Project window
2. **Nhìn vào Inspector**
3. **Tìm phần "Tile States"** (có dấu ▶️ bên cạnh)
4. **Click vào dấu ▶️** để mở rộng
5. **Click vào nút "+"** nhiều lần để tạo 6 ô trống
6. **Kéo các file TileState vào:**
   - `TileState_Normal`
   - `TileState_Plowed`
   - `TileState_Planted`
   - `TileState_SeededWatered`
   - `TileState_Growing` ⬅️ **QUAN TRỌNG!**
   - `TileState_Mature`
7. **✅ OK! Tile States đã được add vào Database!**

---

## 🔴 BƯỚC 5: SETUP GAMEDATABASEMANAGER VÀ GAMEMANAGER TRONG SCENE

### 🎯 Mục đích:
Kết nối Database với game, để game có thể sử dụng items, crops, tools, v.v.

### 📝 Các bước chi tiết:

#### Bước 5.1: Tạo GameDatabaseManager trong Scene
1. **Mở Scene Game** (scene chính của game)
2. **Click chuột phải vào Hierarchy** (cửa sổ bên trái)
3. **Chọn:** `Create Empty` (tạo GameObject trống)
4. **Đặt tên:** `GameDatabaseManager` (nhấn Enter)
5. **Click vào GameObject `GameDatabaseManager`** vừa tạo
6. **Nhìn vào Inspector**
7. **Click vào nút "Add Component"** (ở dưới cùng)
8. **Gõ:** `GameDatabaseManager` (tìm component)
9. **Click vào `GameDatabaseManager`** trong danh sách
10. **Component đã được add!**

#### Bước 5.2: Link GameDatabase vào GameDatabaseManager ⬅️ **QUAN TRỌNG!**

**⚠️ LƯU Ý:** Bạn PHẢI link GameDatabase asset vào GameDatabaseManager để tránh lỗi!

1. **Vẫn đang chọn GameObject `GameDatabaseManager`**
2. **Nhìn vào Inspector, tìm phần "Game Database Manager"**
3. **Tìm dòng "Database Reference":**
   - **Kéo file `GameDatabase`** từ Project window vào đây ⬅️ **BẮT BUỘC!**
   - **Lưu ý:** Nếu để trống, sẽ có lỗi "GameDatabase is null!"
4. **Tìm dòng "Auto Load From Resources":**
   - **✓ Tick vào** (nếu không có database reference, sẽ tự động load từ Resources)
   - **Lưu ý:** Nếu đã link Database Reference, có thể bỏ tick (nhưng tick vào vẫn OK)

5. **✅ OK! GameDatabaseManager đã setup xong!**

**💡 Giải thích:**
- **Database Reference:** Link trực tiếp GameDatabase asset (KHUYẾN NGHỊ - sẽ tránh lỗi thứ tự khởi tạo)
- **Auto Load From Resources:** Tự động load từ `Resources/GameDatabase.asset` nếu không có reference
- **Cả hai cách đều được**, nhưng link trực tiếp vào Database Reference sẽ tránh lỗi "GameDatabase is null!"

**⚠️ QUAN TRỌNG:**
- Nếu bạn thấy lỗi "GameDatabase is null!" trong Console, đây là do thứ tự khởi tạo
- **Giải pháp:** Link GameDatabase asset vào Database Reference (Bước 5.2)
- Sau khi link xong, lỗi sẽ biến mất và bạn sẽ thấy "GameDatabase loaded successfully"

#### Bước 5.3: Tạo GameManager trong Scene
1. **Click chuột phải vào Hierarchy**
2. **Chọn:** `Create Empty`
3. **Đặt tên:** `GameManager` (nhấn Enter)
4. **Click vào GameObject `GameManager`** vừa tạo
5. **Nhìn vào Inspector**
6. **Click vào nút "Add Component"**
7. **Gõ:** `GameManager` (tìm component)
8. **Click vào `GameManager`** trong danh sách
9. **✅ OK! GameManager đã được add!**
10. **Lưu ý:** GameManager tự động tìm GameDatabaseManager trong scene, không cần link thủ công

#### Bước 5.4: Test Database
1. **Click nút Play** (▶️) ở trên cùng Unity Editor
2. **Nhìn vào Console** (cửa sổ dưới cùng)
3. **Bạn sẽ thấy các dòng chữ:**
   ```
   GameDatabase loaded successfully
   GameDatabase initialized: X items, Y crops, Z tools, W tile states, V buildings
   Database initialized successfully
   GameManager initialized successfully
   ```
4. **Nếu thấy các dòng này = ✅ Database hoạt động đúng!**

5. **Nếu thấy lỗi "GameDatabase is null!" = ❌ Làm theo các bước sau:**

   **🔍 Bước 1: Kiểm tra GameDatabaseManager**
   - Chọn GameObject `GameDatabaseManager` trong Hierarchy
   - Inspector → Tìm phần "Game Database Manager"
   - **Database Reference:** Phải có GameDatabase asset (KHÔNG được để trống!)
   - **Nếu để trống:** Kéo GameDatabase asset từ Project window vào đây

   **🔍 Bước 2: Kiểm tra GameDatabase asset**
   - Chọn GameDatabase asset trong Project window
   - Inspector → Kiểm tra:
     - **Items:** Có ít nhất 1 item (ví dụ: SeedCorn, Corn)
     - **Crops:** Có ít nhất 1 crop (ví dụ: CornCrop)
     - **Tools:** Có ít nhất 1 tool (ví dụ: HoeTool, WaterCanTool)
     - **Tile States:** Có ít nhất 5 tile states (Normal, Plowed, Planted, Growing, Mature)

   **🔍 Bước 3: Kiểm tra GameDatabaseManager trong scene**
   - Hierarchy → Tìm GameObject `GameDatabaseManager`
   - Nếu không có, tạo lại (Bước 5.1)

**⚠️ Lưu ý về lỗi "GameDatabase is null!":**
- **Nguyên nhân:** Thứ tự khởi tạo - GameManager chạy trước GameDatabaseManager
- **Giải pháp:** Link GameDatabase asset vào Database Reference (Bước 5.2) ⬅️ **QUAN TRỌNG!**
- **Sau khi link:** Lỗi sẽ biến mất, bạn sẽ thấy "GameDatabase loaded successfully"
- **Nếu vẫn thấy lỗi:** Kiểm tra lại GameDatabase asset có đúng không (có items, crops, tools, tile states)

---

## 🔴 BƯỚC 6: SETUP FARMING SYSTEM (Trồng, Tưới, Thu hoạch)

### 🎯 Mục đích:
Cho phép player cuốc đất, trồng cây, tưới nước, thu hoạch

### 💡 Giải thích cơ chế tưới nước:
- **Mỗi lần tưới nước = Tăng 1 growth stage:**
  - **Tưới lần 1:** Seed (hạt) → Sprout (mầm)
  - **Tưới lần 2:** Sprout (mầm) → Growing (đang lớn) ⬅️ **ĐÂY LÀ GIAI ĐOẠN "GROWING"!**
  - **Tưới lần 3:** Growing (đang lớn) → Mature (chín)
- **Cây Mature = Có thể thu hoạch!**
- **Logic này ĐÃ ĐƯỢC CODE SẴN**, không cần code thêm!

### 📝 Các bước chi tiết:

#### Bước 6.1: Tạo TilemapManager trong Scene
1. **Click chuột phải vào Hierarchy**
2. **Chọn:** `Create Empty`
3. **Đặt tên:** `TilemapManager` (nhấn Enter)
4. **Click vào GameObject `TilemapManager`** vừa tạo
5. **Nhìn vào Inspector**
6. **Click vào nút "Add Component"**
7. **Gõ:** `TilemapManager` (tìm component)
8. **Click vào `TilemapManager`** trong danh sách

#### Bước 6.2: Link Grid vào TilemapManager
1. **Vẫn đang chọn GameObject `TilemapManager`**
2. **Nhìn vào Inspector, tìm phần "Tilemap Manager"**
3. **Tìm dòng "Grid":**
   - **Tìm GameObject "Grid"** trong Hierarchy (thường có sẵn trong scene)
   - **Kéo GameObject "Grid"** vào đây
4. **Tìm dòng "Ground Layer Key":**
   - **Gõ:** `Ground` (tên layer ground)

#### Bước 6.3: Add Ground Layer vào TilemapManager
1. **Vẫn đang chọn GameObject `TilemapManager`**
2. **Nhìn vào Inspector, tìm phần "Layers"** (có dấu ▶️)
3. **Click vào dấu ▶️** để mở rộng
4. **Click vào nút "+"** (thêm layer mới)
5. **Một ô trống sẽ xuất hiện, điền thông tin:**
   - **Key:** Gõ `Ground` (tên layer)
   - **Tilemap:** Tìm GameObject "Ground" (Tilemap) trong Hierarchy, kéo vào đây
   - **Type:** Chọn `Ground` (từ dropdown menu)
   - **Interactable:** ✓ Tick vào (cho phép tương tác)
6. **✅ OK! Ground Layer đã được add!**

#### Bước 6.4: Tạo ToolInteractionSystem trong Scene
1. **Click chuột phải vào Hierarchy**
2. **Chọn:** `Create Empty`
3. **Đặt tên:** `ToolInteractionSystem` (nhấn Enter)
4. **Click vào GameObject `ToolInteractionSystem`** vừa tạo
5. **Nhìn vào Inspector**
6. **Click vào nút "Add Component"**
7. **Gõ:** `ToolInteractionSystem` (tìm component)
8. **Click vào `ToolInteractionSystem`** trong danh sách

#### Bước 6.5: Link TilemapManager vào ToolInteractionSystem
1. **Vẫn đang chọn GameObject `ToolInteractionSystem`**
2. **Nhìn vào Inspector, tìm phần "Tool Interaction System"**
3. **Tìm dòng "Tilemap Manager":**
   - **Kéo GameObject `TilemapManager`** từ Hierarchy vào đây
4. **Tìm dòng "Player ID":**
   - **Để mặc định:** `Default`

#### Bước 6.6: Link ToolInteractionSystem vào PlayerController
1. **Tìm GameObject "Player"** trong Hierarchy (hoặc tên khác nếu bạn đặt tên khác)
2. **Click vào GameObject "Player"**
3. **Nhìn vào Inspector, tìm component "Player Controller"**
4. **Tìm dòng "Tool System":**
   - **Kéo GameObject `ToolInteractionSystem`** từ Hierarchy vào đây
5. **✅ OK! ToolInteractionSystem đã được link!**

---

## 🔴 BƯỚC 7: TẠO TOOLDATASO (Tools)

### 🎯 Mục đích:
Tạo các công cụ (cuốc, bình tưới, găng tay, lưỡi liềm)

### 📝 Các bước chi tiết:

#### Bước 7.1: Tạo Hoe Tool (Cuốc)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tool Data`
3. **Đặt tên:** `HoeTool` (nhấn Enter)
4. **Click vào file `HoeTool`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **Tool ID:** Gõ `hoe`
   - **Tool Name:** Gõ `Cuốc`
   - **Description:** Gõ `Cuốc đất để chuẩn bị trồng cây`

   **🖼️ Hình ảnh:**
   - **Icon:** Kéo sprite cuốc vào đây

   **⚙️ Thuộc tính:**
   - **Action Type:** Chọn `Plow` (từ dropdown menu)
   - **Can Interact With Tile States:** Tìm phần này, click vào dấu ▶️, tick vào `Normal`

6. **✅ OK! Hoe Tool đã tạo xong!**

#### Bước 7.2: Tạo WaterCan Tool (Bình tưới)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tool Data`
3. **Đặt tên:** `WaterCanTool` (nhấn Enter)
4. **Click vào file `WaterCanTool`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **Tool ID:** Gõ `watercan`
   - **Tool Name:** Gõ `Bình Tưới`
   - **Description:** Gõ `Tưới nước cho cây, mỗi lần tưới = cây lớn thêm 1 giai đoạn`

   **🖼️ Hình ảnh:**
   - **Icon:** Kéo sprite bình tưới vào đây

   **⚙️ Thuộc tính:**
   - **Action Type:** Chọn `Water`
   - **Can Interact With Tile States:** Tick vào `Plowed`, `Planted`, `SeededWatered`
   - **Is Refillable:** ✓ Tick vào (có thể làm đầy lại)

6. **✅ OK! WaterCan Tool đã tạo xong!**

#### Bước 7.3: Tạo Glove Tool (Găng tay - Special Tool)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tool Data`
3. **Đặt tên:** `GloveTool` (nhấn Enter)
4. **Click vào file `GloveTool`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **Tool ID:** Gõ `glove`
   - **Tool Name:** Gõ `Găng Tay`
   - **Description:** Gõ `Trồng hạt giống vào đất`

   **🖼️ Hình ảnh:**
   - **Icon:** Kéo sprite găng tay vào đây

   **⚙️ Thuộc tính:**
   - **Action Type:** Chọn `Plant`
   - **Is Special Tool:** ✓ Tick vào (tool đặc biệt)
   - **Special Interaction Type:** Chọn `Plant`
   - **Filter Item Tag:** Chọn `Seed` (chỉ hiển thị hạt giống)
   - **Can Interact With Tile States:** Tick vào `Plowed`, `Watered`

6. **✅ OK! Glove Tool đã tạo xong!**

#### Bước 7.4: Tạo Slice Tool (Lưỡi liềm - Harvest)
1. **Click chuột phải vào Project window**
2. **Chọn:** `Create` → `Game` → `Tool Data`
3. **Đặt tên:** `SliceTool` (nhấn Enter)
4. **Click vào file `SliceTool`** vừa tạo
5. **Nhìn vào Inspector, điền thông tin:**

   **📋 Thông tin cơ bản:**
   - **Tool ID:** Gõ `slice`
   - **Tool Name:** Gõ `Lưỡi Liềm`
   - **Description:** Gõ `Thu hoạch cây đã chín`

   **🖼️ Hình ảnh:**
   - **Icon:** Kéo sprite lưỡi liềm vào đây

   **⚙️ Thuộc tính:**
   - **Action Type:** Chọn `Harvest`
   - **Can Interact With Tile States:** Tick vào `Mature` ⬅️ **QUAN TRỌNG!**

6. **✅ OK! Slice Tool đã tạo xong!**

#### Bước 7.5: Add Tools vào GameDatabase
1. **Click vào file `GameDatabase`** trong Project window
2. **Nhìn vào Inspector**
3. **Tìm phần "Tools"** (có dấu ▶️ bên cạnh)
4. **Click vào dấu ▶️** để mở rộng
5. **Click vào nút "+"** nhiều lần để tạo 4 ô trống
6. **Kéo các file Tool vào:**
   - `HoeTool`
   - `WaterCanTool`
   - `GloveTool`
   - `SliceTool`
7. **✅ OK! Tools đã được add vào Database!**

---

## 🔴 BƯỚC 8: SETUP INVENTORY SYSTEM

### 🎯 Mục đích:
Hiển thị túi đồ của player, lưu trữ items

### 📝 Các bước chi tiết:

#### Bước 8.1: Tạo Canvas (nếu chưa có)
1. **Nhìn vào Hierarchy**
2. **Tìm GameObject "Canvas"** (nếu có thì bỏ qua bước này)
3. **Nếu không có:**
   - **Click chuột phải vào Hierarchy**
   - **Chọn:** `UI` → `Canvas`
   - **Canvas sẽ được tạo tự động**

#### Bước 8.2: Tạo Inventory Panel
1. **Click chuột phải vào GameObject "Canvas"** trong Hierarchy
2. **Chọn:** `UI` → `Panel`
3. **Đặt tên:** `InventoryPanel` (nhấn Enter)
4. **Click vào GameObject `InventoryPanel`** vừa tạo
5. **Nhìn vào Inspector**
6. **Click vào nút "Add Component"**
7. **Gõ:** `InventoryUIController` (tìm component)
8. **Click vào `InventoryUIController`** trong danh sách

#### Bước 8.3: Setup InventoryUIController
1. **Vẫn đang chọn GameObject `InventoryPanel`**
2. **Nhìn vào Inspector, tìm phần "Inventory UI Controller"**
3. **Tìm dòng "Player ID":**
   - **Để mặc định:** `Default`
4. **Tìm dòng "Toggle Key":**
   - **Để mặc định:** `I` (phím I để mở/đóng inventory)

#### Bước 8.4: Tạo InventoryView
1. **Click chuột phải vào GameObject `InventoryPanel`** trong Hierarchy
2. **Chọn:** `Create Empty`
3. **Đặt tên:** `InventoryView` (nhấn Enter)
4. **Click vào GameObject `InventoryView`** vừa tạo
5. **Nhìn vào Inspector**
6. **Click vào nút "Add Component"**
7. **Gõ:** `InventoryView` (tìm component)
8. **Click vào `InventoryView`** trong danh sách

#### Bước 8.5: Setup InventoryView
1. **Vẫn đang chọn GameObject `InventoryView`**
2. **Nhìn vào Inspector, tìm phần "Inventory View"**
3. **Tìm dòng "Slot Container":**
   - **Click chuột phải vào GameObject `InventoryView`** trong Hierarchy
   - **Chọn:** `Create Empty`
   - **Đặt tên:** `SlotContainer`
   - **Kéo GameObject `SlotContainer`** vào đây
4. **Tìm dòng "Slot Prefab":**
   - **Tạo prefab ItemSlot trước** (xem bước 8.6)
   - **Kéo prefab `ItemSlot`** vào đây

#### Bước 8.6: Tạo ItemSlot Prefab
1. **Click chuột phải vào GameObject `InventoryPanel`** trong Hierarchy
2. **Chọn:** `UI` → `Button`
3. **Đặt tên:** `ItemSlot` (nhấn Enter)
4. **Click vào GameObject `ItemSlot`** vừa tạo
5. **Nhìn vào Inspector, tìm component "Button"**
6. **Tìm phần "On Click ()":**
   - **Bỏ qua bước này** (sẽ setup sau)

#### Bước 8.7: Setup ItemSlot (Thêm Icon và Quantity Text)
1. **Click chuột phải vào GameObject `ItemSlot`** trong Hierarchy
2. **Chọn:** `UI` → `Image`
3. **Đặt tên:** `Icon` (nhấn Enter)
4. **Click vào GameObject `Icon`** vừa tạo
5. **Nhìn vào Inspector, tìm component "Image"**
6. **Tìm dòng "Image Type":**
   - **Chọn:** `Simple`
7. **Click chuột phải vào GameObject `ItemSlot`** trong Hierarchy
8. **Chọn:** `UI` → `Text - TextMeshPro`
9. **Đặt tên:** `QuantityText` (nhấn Enter)
10. **Click vào GameObject `QuantityText`** vừa tạo
11. **Nhìn vào Inspector, tìm component "TextMeshPro - Text (UI)"**
12. **Tìm dòng "Text":**
    - **Gõ:** `99` (số lượng mặc định, sẽ được update bởi code)
13. **Tìm dòng "Alignment":**
    - **Chọn:** Align Right (căn phải)
    - **Chọn:** Align Bottom (căn dưới)

#### Bước 8.8: Lưu ItemSlot thành Prefab
1. **Tạo folder Prefabs** (nếu chưa có):
   - **Click chuột phải vào Project window**
   - **Chọn:** `Create` → `Folder`
   - **Đặt tên:** `Prefabs`
2. **Tạo folder UI** (trong Prefabs):
   - **Click chuột phải vào folder `Prefabs`**
   - **Chọn:** `Create` → `Folder`
   - **Đặt tên:** `UI`
3. **Kéo GameObject `ItemSlot`** từ Hierarchy vào folder `Prefabs/UI` trong Project window
4. **Prefab đã được tạo!**
5. **Xóa GameObject `ItemSlot`** trong Hierarchy (không cần nữa, đã có prefab)

#### Bước 8.9: Link ItemSlot Prefab vào InventoryView
1. **Click vào GameObject `InventoryView`** trong Hierarchy
2. **Nhìn vào Inspector, tìm phần "Inventory View"**
3. **Tìm dòng "Slot Prefab":**
   - **Kéo prefab `ItemSlot`** từ Project window vào đây
4. **✅ OK! Inventory System đã setup xong!**

---

## 🔴 BƯỚC 9: SETUP NPC SHOP SYSTEM

### 🎯 Mục đích:
Cho phép player mua/bán items với NPC

### 📝 Các bước chi tiết:

#### Bước 9.1: Tạo Shop Panel
1. **Click chuột phải vào GameObject "Canvas"** trong Hierarchy
2. **Chọn:** `UI` → `Panel`
3. **Đặt tên:** `ShopPanel` (nhấn Enter)
4. **Click vào GameObject `ShopPanel`** vừa tạo
5. **Nhìn vào Inspector**
6. **Click vào nút "Add Component"**
7. **Gõ:** `NPCShopUI` (tìm component)
8. **Click vào `NPCShopUI`** trong danh sách

#### Bước 9.2: Setup NPCShopUI
1. **Vẫn đang chọn GameObject `ShopPanel`**
2. **Nhìn vào Inspector, tìm phần "NPC Shop UI"**
3. **Tìm dòng "Panel":**
   - **Kéo GameObject `ShopPanel`** vào đây (chính nó)
4. **Tìm dòng "NPC ID":**
   - **Để mặc định:** `Default`
5. **Tìm dòng "Player ID":**
   - **Để mặc định:** `Default`

#### Bước 9.3: Tạo Buy/Sell Containers
1. **Click chuột phải vào GameObject `ShopPanel`** trong Hierarchy
2. **Chọn:** `Create Empty`
3. **Đặt tên:** `BuyItemContainer` (nhấn Enter)
4. **Click chuột phải vào GameObject `ShopPanel`** trong Hierarchy
5. **Chọn:** `Create Empty`
6. **Đặt tên:** `SellItemContainer` (nhấn Enter)

#### Bước 9.4: Link Containers vào NPCShopUI
1. **Click vào GameObject `ShopPanel`** trong Hierarchy
2. **Nhìn vào Inspector, tìm phần "NPC Shop UI"**
3. **Tìm dòng "Buy Item Container":**
   - **Kéo GameObject `BuyItemContainer`** vào đây
4. **Tìm dòng "Sell Item Container":**
   - **Kéo GameObject `SellItemContainer`** vào đây

#### Bước 9.5: Tạo Buy/Sell Tab Buttons
1. **Click chuột phải vào GameObject `ShopPanel`** trong Hierarchy
2. **Chọn:** `UI` → `Button`
3. **Đặt tên:** `BuyTabButton` (nhấn Enter)
4. **Click vào GameObject `BuyTabButton`** vừa tạo
5. **Nhìn vào Inspector, tìm component "Button"**
6. **Tìm phần "On Click ()":**
   - **Bỏ qua** (sẽ được setup tự động bởi NPCShopUI)
7. **Lặp lại để tạo `SellTabButton`**

#### Bước 9.6: Link Tab Buttons vào NPCShopUI
1. **Click vào GameObject `ShopPanel`** trong Hierarchy
2. **Nhìn vào Inspector, tìm phần "NPC Shop UI"**
3. **Tìm dòng "Buy Tab Button":**
   - **Kéo GameObject `BuyTabButton`** vào đây
4. **Tìm dòng "Sell Tab Button":**
   - **Kéo GameObject `SellTabButton`** vào đây

#### Bước 9.7: Tạo Player Money Text
1. **Click chuột phải vào GameObject `ShopPanel`** trong Hierarchy
2. **Chọn:** `UI` → `Text - TextMeshPro`
3. **Đặt tên:** `PlayerMoneyText` (nhấn Enter)
4. **Click vào GameObject `PlayerMoneyText`** vừa tạo
5. **Nhìn vào Inspector, tìm component "TextMeshPro - Text (UI)"**
6. **Tìm dòng "Text":**
   - **Gõ:** `Money: 0` (sẽ được update bởi code)

#### Bước 9.8: Link Player Money Text vào NPCShopUI
1. **Click vào GameObject `ShopPanel`** trong Hierarchy
2. **Nhìn vào Inspector, tìm phần "NPC Shop UI"**
3. **Tìm dòng "Player Money Text":**
   - **Kéo GameObject `PlayerMoneyText`** vào đây
4. **✅ OK! NPC Shop System đã setup xong!**

---

## ✅ TÓM TẮT QUY TRÌNH FARMING:

1. **Cuốc đất:** Chọn Hoe → Click ground tile → Đất chuyển thành Plowed
2. **Trồng cây:** Chọn Glove → Click plowed tile → Chọn seed → Cây được trồng (Seed stage)
3. **Tưới nước:** Chọn WaterCan → Click planted tile → **Mỗi lần tưới = tăng 1 growth stage**
   - **Tưới lần 1:** Seed → Sprout (mầm)
   - **Tưới lần 2:** Sprout → **Growing (đang lớn)** ⬅️ **ĐÂY LÀ GIAI ĐOẠN "GROWING"!**
   - **Tưới lần 3:** Growing → Mature (chín)
4. **Thu hoạch:** Chọn Slice → Click mature tile → Item được thêm vào inventory

---

## 🎮 BƯỚC 10: SETUP PAUSE MENU (Inventory, Save/Load, Quit)

### 🎯 Mục đích:
Tạo Pause Menu với các tính năng:
- **Resume:** Tiếp tục game
- **Inventory:** Mở túi đồ
- **Save:** Lưu game vào slot (multiple slots)
- **Load:** Load game từ slot (multiple slots)
- **Quit:** Thoát game (có thể có confirm panel)

### 📝 Các bước chi tiết:

#### Bước 10.1: Tạo Pause Panel UI

1. **Click chuột phải vào Canvas** (trong Hierarchy)
2. **Chọn:** `UI → Panel`
3. **Đặt tên:** `PausePanel` (nhấn Enter)
4. **Click vào GameObject `PausePanel`**
5. **Nhìn vào Inspector:**
   - **Anchor Presets:** Nhấn `Alt + Shift` + Click vào **stretch/stretch** (full screen)
   - **Color:** Đặt màu nền (ví dụ: đen với alpha 0.8)
6. **✅ OK! PausePanel đã được tạo!**

#### Bước 10.2: Tạo Menu Buttons Panel

1. **Click chuột phải vào `PausePanel`** (trong Hierarchy)
2. **Chọn:** `UI → Panel`
3. **Đặt tên:** `MenuButtonsPanel` (nhấn Enter)
4. **Click vào GameObject `MenuButtonsPanel`**
5. **Nhìn vào Inspector:**
   - **Anchor Presets:** Đặt ở giữa màn hình
   - **Width:** 400
   - **Height:** 500
   - **Color:** Màu nền menu (ví dụ: xám đậm)
6. **✅ OK! MenuButtonsPanel đã được tạo!**

#### Bước 10.3: Tạo Menu Buttons (Resume, Inventory, Save, Load, Quit)

**Tạo Resume Button:**
1. **Click chuột phải vào `MenuButtonsPanel`**
2. **Chọn:** `UI → Button - TextMeshPro`
3. **Đặt tên:** `ResumeButton`
4. **Click vào `ResumeButton` → Text (TMP):**
   - **Text:** `Resume`
   - **Font Size:** 24
   - **Alignment:** Center
5. **✅ OK! ResumeButton đã được tạo!**

**Tạo Inventory Button:**
1. **Click chuột phải vào `MenuButtonsPanel`**
2. **Chọn:** `UI → Button - TextMeshPro`
3. **Đặt tên:** `InventoryButton`
4. **Click vào `InventoryButton` → Text (TMP):**
   - **Text:** `Inventory`
   - **Font Size:** 24
   - **Alignment:** Center
5. **✅ OK! InventoryButton đã được tạo!**

**Tạo Save Button:**
1. **Click chuột phải vào `MenuButtonsPanel`**
2. **Chọn:** `UI → Button - TextMeshPro`
3. **Đặt tên:** `SaveButton`
4. **Click vào `SaveButton` → Text (TMP):**
   - **Text:** `Save`
   - **Font Size:** 24
   - **Alignment:** Center
5. **✅ OK! SaveButton đã được tạo!**

**Tạo Load Button:**
1. **Click chuột phải vào `MenuButtonsPanel`**
2. **Chọn:** `UI → Button - TextMeshPro`
3. **Đặt tên:** `LoadButton`
4. **Click vào `LoadButton` → Text (TMP):**
   - **Text:** `Load`
   - **Font Size:** 24
   - **Alignment:** Center
5. **✅ OK! LoadButton đã được tạo!**

**Tạo Quit Button:**
1. **Click chuột phải vào `MenuButtonsPanel`**
2. **Chọn:** `UI → Button - TextMeshPro`
3. **Đặt tên:** `QuitButton`
4. **Click vào `QuitButton` → Text (TMP):**
   - **Text:** `Quit`
   - **Font Size:** 24
   - **Alignment:** Center
5. **✅ OK! QuitButton đã được tạo!**

**Sắp xếp buttons:**
1. **Chọn tất cả buttons** (ResumeButton, InventoryButton, SaveButton, LoadButton, QuitButton)
2. **Nhìn vào Inspector → Rect Transform:**
   - **Anchor Presets:** Top Center (cho tất cả)
   - **Pos Y:** Điều chỉnh để buttons cách đều nhau (ví dụ: 200, 100, 0, -100, -200)
   - **Width:** 300
   - **Height:** 60
3. **✅ OK! Buttons đã được sắp xếp!**

#### Bước 10.4: Tạo Save Slot List Panel (cho Save/Load)

1. **Click chuột phải vào `PausePanel`**
2. **Chọn:** `UI → Panel`
3. **Đặt tên:** `SaveSlotListPanel` (nhấn Enter)
4. **Click vào GameObject `SaveSlotListPanel`**
5. **Nhìn vào Inspector:**
   - **Anchor Presets:** Đặt ở giữa màn hình
   - **Width:** 600
   - **Height:** 700
   - **Color:** Màu nền (ví dụ: xám đậm)
6. **✅ OK! SaveSlotListPanel đã được tạo!**

**Tạo Title Text:**
1. **Click chuột phải vào `SaveSlotListPanel`**
2. **Chọn:** `UI → Text - TextMeshPro`
3. **Đặt tên:** `TitleText`
4. **Click vào `TitleText`:**
   - **Text:** `Save Game` (hoặc `Load Game`)
   - **Font Size:** 32
   - **Alignment:** Center
   - **Rect Transform:** Anchor Top Center, Pos Y: 300, Width: 500, Height: 50
5. **✅ OK! TitleText đã được tạo!**

**Tạo Save Slot Container (Scroll View):**
1. **Click chuột phải vào `SaveSlotListPanel`**
2. **Chọn:** `UI → Scroll View`
3. **Đặt tên:** `SaveSlotContainer`
4. **Click vào `SaveSlotContainer`:**
   - **Rect Transform:** Anchor Presets stretch/stretch, Left: 50, Right: 50, Top: 100, Bottom: 100
5. **Click vào `Content` (trong SaveSlotContainer):**
   - **Vertical Layout Group:** ✓ Tick vào
   - **Spacing:** 10
   - **Padding:** Left/Right/Top/Bottom: 10
   - **Child Alignment:** Upper Center
   - **Child Force Expand:** Width: ✓, Height: ✗
6. **✅ OK! SaveSlotContainer đã được tạo!**

**Tạo Save Slot Button Template:**
1. **Click chuột phải vào `Content` (trong SaveSlotContainer)**
2. **Chọn:** `UI → Button - TextMeshPro`
3. **Đặt tên:** `SaveSlotButtonTemplate`
4. **Click vào `SaveSlotButtonTemplate`:**
   - **Rect Transform:** Width: 550, Height: 80
5. **Click vào `Text (TMP)` trong SaveSlotButtonTemplate:**
   - **Text:** `Slot 1 - Empty`
   - **Font Size:** 20
   - **Alignment:** Left
6. **Tạo thêm Text cho thông tin:**
   - **Click chuột phải vào `SaveSlotButtonTemplate`**
   - **Chọn:** `UI → Text - TextMeshPro`
   - **Đặt tên:** `InfoText1`
   - **Rect Transform:** Anchor Top Left, Pos X: 10, Pos Y: -25, Width: 250, Height: 20
   - **Text:** `Day 1 - 00:30`
   - **Font Size:** 16
7. **Tạo Text thứ 2:**
   - **Click chuột phải vào `SaveSlotButtonTemplate`**
   - **Chọn:** `UI → Text - TextMeshPro`
   - **Đặt tên:** `InfoText2`
   - **Rect Transform:** Anchor Top Left, Pos X: 10, Pos Y: -45, Width: 250, Height: 20
   - **Text:** `01/01/2024 12:00`
   - **Font Size:** 14
8. **✅ OK! SaveSlotButtonTemplate đã được tạo!**

**Tạo Back Button:**
1. **Click chuột phải vào `SaveSlotListPanel`**
2. **Chọn:** `UI → Button - TextMeshPro`
3. **Đặt tên:** `BackButton`
4. **Click vào `BackButton`:**
   - **Rect Transform:** Anchor Bottom Center, Pos Y: 50, Width: 200, Height: 50
   - **Text (TMP):** `Back`
   - **Font Size:** 24
5. **✅ OK! BackButton đã được tạo!**

#### Bước 10.5: Tạo Quit Confirm Panel (Optional)

1. **Click chuột phải vào `PausePanel`**
2. **Chọn:** `UI → Panel`
3. **Đặt tên:** `QuitConfirmPanel` (nhấn Enter)
4. **Click vào GameObject `QuitConfirmPanel`**
5. **Nhìn vào Inspector:**
   - **Anchor Presets:** Đặt ở giữa màn hình
   - **Width:** 400
   - **Height:** 250
   - **Color:** Màu nền (ví dụ: xám đậm)
6. **✅ OK! QuitConfirmPanel đã được tạo!**

**Tạo Text:**
1. **Click chuột phải vào `QuitConfirmPanel`**
2. **Chọn:** `UI → Text - TextMeshPro`
3. **Đặt tên:** `ConfirmText`
4. **Click vào `ConfirmText`:**
   - **Text:** `Quit to Title Screen?`
   - **Font Size:** 24
   - **Alignment:** Center
   - **Rect Transform:** Anchor Top Center, Pos Y: 80, Width: 350, Height: 50
5. **✅ OK! ConfirmText đã được tạo!**

**Tạo Buttons:**
1. **Tạo "Yes" Button:**
   - **Click chuột phải vào `QuitConfirmPanel`**
   - **Chọn:** `UI → Button - TextMeshPro`
   - **Đặt tên:** `QuitToTitleScreenButton`
   - **Rect Transform:** Anchor Bottom Left, Pos X: 50, Pos Y: 30, Width: 150, Height: 50
   - **Text (TMP):** `Yes`
2. **Tạo "No" Button:**
   - **Click chuột phải vào `QuitConfirmPanel`**
   - **Chọn:** `UI → Button - TextMeshPro`
   - **Đặt tên:** `QuitCancelButton`
   - **Rect Transform:** Anchor Bottom Right, Pos X: -50, Pos Y: 30, Width: 150, Height: 50
   - **Text (TMP):** `No`
3. **✅ OK! QuitConfirmPanel đã được tạo!**

#### Bước 10.6: Link UI vào PauseController

1. **Tạo GameObject `PauseController` trong scene:**
   - **Click chuột phải vào Hierarchy**
   - **Chọn:** `Create Empty`
   - **Đặt tên:** `PauseController`
   - **Click vào GameObject `PauseController`**
   - **Click vào nút "Add Component"**
   - **Gõ:** `PauseController`
   - **Click vào `PauseController`** trong danh sách

2. **Link UI vào PauseController:**
   - **Pause Panel:** Kéo `PausePanel` vào đây
   - **Menu Buttons Panel:** Kéo `MenuButtonsPanel` vào đây
   - **Inventory UI Controller:** Kéo `InventoryUIController` vào đây (hoặc để trống, sẽ tự động tìm)
   - **Save Slot List Controller:** Kéo `SaveSlotListController` vào đây (sẽ tạo ở bước tiếp theo)
   - **Quit Confirm Panel:** Kéo `QuitConfirmPanel` vào đây (optional)

3. **✅ OK! PauseController đã được link!**

#### Bước 10.7: Setup SaveSlotListController

1. **Tạo GameObject `SaveSlotListController` trong scene:**
   - **Click chuột phải vào Hierarchy**
   - **Chọn:** `Create Empty`
   - **Đặt tên:** `SaveSlotListController`
   - **Click vào GameObject `SaveSlotListController`**
   - **Click vào nút "Add Component"**
   - **Gõ:** `SaveSlotListController`
   - **Click vào `SaveSlotListController`** trong danh sách

2. **Link UI vào SaveSlotListController:**
   - **Save Slot List Panel:** Kéo `SaveSlotListPanel` vào đây
   - **Save Slot Container:** Kéo `Content` (trong SaveSlotContainer) vào đây
   - **Save Slot Button Template:** Kéo `SaveSlotButtonTemplate` vào đây
   - **Title Text:** Kéo `TitleText` vào đây
   - **Back Button:** Kéo `BackButton` vào đây

3. **✅ OK! SaveSlotListController đã được setup!**

4. **Link SaveSlotListController vào PauseController:**
   - **Chọn GameObject `PauseController`**
   - **Inspector → Save Slot List Controller:** Kéo `SaveSlotListController` vào đây

5. **✅ OK! SaveSlotListController đã được link!**

#### Bước 10.8: Link Buttons vào PauseController

1. **Chọn GameObject `PauseController`**
2. **Link Resume Button:**
   - **Chọn `ResumeButton`**
   - **Inspector → Button → On Click():** Click dấu **+**
   - **Kéo GameObject `PauseController`** vào đây
   - **Chọn:** `PauseController → OnResumeClicked()`
3. **Link Inventory Button:**
   - **Chọn `InventoryButton`**
   - **Inspector → Button → On Click():** Click dấu **+**
   - **Kéo GameObject `PauseController`** vào đây
   - **Chọn:** `PauseController → OnInventoryClicked()`
4. **Link Save Button:**
   - **Chọn `SaveButton`**
   - **Inspector → Button → On Click():** Click dấu **+**
   - **Kéo GameObject `PauseController`** vào đây
   - **Chọn:** `PauseController → OnSaveClicked()`
5. **Link Load Button:**
   - **Chọn `LoadButton`**
   - **Inspector → Button → On Click():** Click dấu **+**
   - **Kéo GameObject `PauseController`** vào đây
   - **Chọn:** `PauseController → OnLoadClicked()`
6. **Link Quit Button:**
   - **Chọn `QuitButton`**
   - **Inspector → Button → On Click():** Click dấu **+**
   - **Kéo GameObject `PauseController`** vào đây
   - **Chọn:** `PauseController → OnQuitClicked()`
7. **✅ OK! Buttons đã được link!**

#### Bước 10.9: Link Quit Confirm Buttons (nếu có QuitConfirmPanel)

1. **Link Quit To Title Screen Button:**
   - **Chọn `QuitToTitleScreenButton`**
   - **Inspector → Button → On Click():** Click dấu **+**
   - **Kéo GameObject `PauseController`** vào đây
   - **Chọn:** `PauseController → OnQuitToTitleScreenConfirmed()`
2. **Link Quit Cancel Button:**
   - **Chọn `QuitCancelButton`**
   - **Inspector → Button → On Click():** Click dấu **+**
   - **Kéo GameObject `PauseController`** vào đây
   - **Chọn:** `PauseController → OnQuitCancelled()`
3. **✅ OK! Quit Confirm Buttons đã được link!**

#### Bước 10.10: Setup SaveLoadManager (nếu chưa có)

1. **Tạo GameObject `SaveLoadManager` trong scene:**
   - **Click chuột phải vào Hierarchy**
   - **Chọn:** `Create Empty`
   - **Đặt tên:** `SaveLoadManager`
   - **Click vào GameObject `SaveLoadManager`**
   - **Click vào nút "Add Component"**
   - **Gõ:** `SaveLoadManager`
   - **Click vào `SaveLoadManager`** trong danh sách
2. **✅ OK! SaveLoadManager đã được setup!**

#### Bước 10.11: Setup Inventory UI (nếu chưa có)

**Lưu ý:** Inventory UI đã được setup ở Bước 7, nhưng cần đảm bảo nó hoạt động với Pause Menu.

1. **Đảm bảo `InventoryUIController` có trong scene:**
   - Nếu chưa có, xem lại **Bước 7: SETUP INVENTORY SYSTEM**
2. **Link InventoryUIController vào PauseController:**
   - **Chọn GameObject `PauseController`**
   - **Inspector → Inventory UI Controller:** Kéo `InventoryUIController` vào đây (hoặc để trống, sẽ tự động tìm)
3. **✅ OK! Inventory UI đã được link!**

#### Bước 10.12: Test Pause Menu

1. **Click nút Play** (▶️) ở trên cùng Unity Editor
2. **Nhấn phím Escape** (hoặc phím pause đã set) để mở Pause Menu
3. **Test Resume:**
   - Click nút **Resume** → Pause Menu đóng, game tiếp tục
4. **Test Inventory:**
   - Click nút **Inventory** → Inventory UI hiển thị, menu buttons ẩn
   - Click **Back** hoặc nhấn **Escape** → Quay lại menu buttons
5. **Test Save:**
   - Click nút **Save** → Save Slot List hiển thị với mode Save
   - Click vào một slot → Game được lưu vào slot đó
   - Click **Back** → Quay lại menu buttons
6. **Test Load:**
   - Click nút **Load** → Save Slot List hiển thị với mode Load
   - Click vào một slot có save → Game được load từ slot đó, Pause Menu đóng
   - Click **Back** → Quay lại menu buttons
7. **Test Quit:**
   - Click nút **Quit** → Quit Confirm Panel hiển thị (nếu có)
   - Click **Yes** → Quay về Title Screen
   - Click **No** → Quay lại menu buttons

**✅ OK! Pause Menu hoạt động đúng!**

---

## ✅ TEST CHECKLIST:

### Core Systems:
- [ ] GameDatabase được tạo và có items, crops, tools, tile states
- [ ] GameDatabaseManager và GameManager trong scene
- [ ] TilemapManager và ToolInteractionSystem trong scene
- [ ] ToolSelectionUI hoạt động (chọn tool, đóng panel)
- [ ] Cuốc đất hoạt động (Hoe tool)
- [ ] Trồng cây hoạt động (Glove tool + seed selection)
- [ ] Tưới nước hoạt động (WaterCan tool) và tăng growth stage
- [ ] Thu hoạch hoạt động (Slice tool) và item được thêm vào inventory
- [ ] Inventory UI hiển thị items (nhấn I để mở/đóng)
- [ ] NPC Shop UI mở được và hiển thị items có thể bán

### Pause Menu:
- [ ] Pause Menu mở/đóng bằng phím Escape
- [ ] Resume button hoạt động (đóng pause menu)
- [ ] Inventory button mở Inventory UI từ pause menu
- [ ] Save button hiển thị Save Slot List (mode Save)
- [ ] Load button hiển thị Save Slot List (mode Load)
- [ ] Save game vào slot thành công
- [ ] Load game từ slot thành công
- [ ] Save Slot List hiển thị đúng thông tin (slot number, play time, save date, latest marker)
- [ ] Quit button hiển thị Quit Confirm Panel (nếu có)
- [ ] Quit to Title Screen hoạt động
- [ ] Back button trong Save Slot List quay lại menu buttons

---

## 📝 LƯU Ý QUAN TRỌNG:

1. **Growth System:** Tạm thời dùng tưới nước để tăng growth stage (mỗi lần tưới = 1 stage)
2. **TileState "Growing" KHÔNG CẦN SPRITE:**
   - TileState chỉ là trạng thái LOGIC (có thể làm gì với tile này)
   - TileState "Growing" chỉ quy định: có thể tưới, chưa thể thu hoạch
   - **KHÔNG CÓ HÌNH ẢNH!** TileState không có sprite
   - **TileState chỉ là logic, KHÔNG phải hình ảnh!**
3. **CropDataSO "growingSprite" = HÌNH ẢNH CÂY:**
   - Đây là hình ảnh HIỂN THỊ TRÊN MÀN HÌNH khi cây ở giai đoạn "Growing"
   - Lấy từ CropDataSO, KHÔNG lấy từ TileState
   - Khi Crop.CurrentStage = GrowthStage.Growing → game hiển thị growingSprite từ CropDataSO
   - **ĐÂY LÀ HÌNH ẢNH BẠN THẤY TRÊN MÀN HÌNH!**
4. **Database:** Tất cả SOs phải được add vào GameDatabase để game có thể sử dụng
5. **Tile States:** Đảm bảo các tile states được setup đúng (canPlow, canWater, canPlant, canHarvest)
6. **Tóm tắt:**
   - **TileState = LOGIC** (có thể làm gì) - KHÔNG CẦN SPRITE
   - **CropDataSO sprites = HÌNH ẢNH** (hiển thị trên màn hình) - CẦN SPRITE
   - **GrowthStage = GIAI ĐOẠN CÂY** (dùng để chọn sprite từ CropDataSO)
   - **Khi cây ở GrowthStage.Growing:**
     - TileState = "Growing" (logic: có thể tưới, chưa thể thu hoạch)
     - Hình ảnh = CropDataSO.growingSprite (hình ảnh cây đang lớn)

---

## 🎉 HOÀN THÀNH!

Bây giờ bạn đã setup xong tất cả systems! Hãy test game và xem kết quả!

Nếu có lỗi, hãy kiểm tra lại từng bước một cách cẩn thận. Good luck! 🍀
