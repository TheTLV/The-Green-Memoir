# Factory Pattern - Hướng Dẫn Sử Dụng

## Tổng Quan

Factory Pattern được sử dụng để tạo các entities (Item, Crop, Tool) từ database với khả năng cache.

## Cách Sử Dụng

### 1. Khởi tạo Factory

Factory có thể được thêm vào GameManager hoặc tạo riêng:

```csharp
var factory = new EntityFactory(
    GameManager.ItemDatabase,
    GameManager.CropDatabase,
    GameManager.ToolDatabase
);
```

### 2. Tạo Entities

```csharp
// Tạo Item
var item = factory.CreateItem(new ItemId("seed_wheat"));

// Tạo Crop
var crop = factory.CreateCrop(new CropId("wheat"));

// Tạo Tool
var tool = factory.CreateTool(new ToolId("hoe"));
```

### 3. Cache

- **Item**: Được cache vì immutable
- **Crop**: Không cache vì mỗi instance có state riêng (growth stage)
- **Tool**: Không cache vì mỗi instance có state riêng (current uses)

### 4. Clear Cache

```csharp
factory.ClearCache(); // Khi cần reload data
```

## Lợi Ích

- Tập trung logic tạo entities
- Cache để tối ưu performance
- Dễ dàng mở rộng cho entities phức tạp hơn

