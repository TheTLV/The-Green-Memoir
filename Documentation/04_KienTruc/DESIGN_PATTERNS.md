# 🎨 DESIGN PATTERNS TRONG PROJECT

Tài liệu này mô tả tất cả design patterns được sử dụng trong project **The Green Memoir**.

---

## 📋 TỔNG QUAN

Project sử dụng **11 design patterns** phù hợp với Unity game development và Clean Architecture:

1. **Singleton Pattern** - Quản lý managers
2. **Service Locator Pattern** - GameManager
3. **Command Pattern** - Undo/Redo actions
4. **Repository Pattern** - Data access layer
5. **Observer Pattern (Event Bus)** - Event-driven architecture
6. **Strategy Pattern** - Network services
7. **Adapter Pattern** - Database adapters
8. **State Pattern** - Game state management ⭐ MỚI
9. **Factory Pattern** - Entity creation ⭐ MỚI
10. **Object Pool Pattern** - GameObject pooling ⭐ MỚI
11. **MVP Pattern** - UI architecture ⭐ MỚI

---

## 1. 🎯 SINGLETON PATTERN

**Mục đích:** Đảm bảo chỉ có một instance của class tồn tại.

**Ví dụ:**
- `GameManager` - Quản lý game services
- `AudioManager` - Quản lý âm thanh
- `NetworkServiceManager` - Quản lý network
- `PoolManager` - Quản lý object pools
- `GameStateManager` - Quản lý game states

**Cách sử dụng:**
```csharp
// Lấy instance
var audioManager = AudioManager.Instance;
audioManager.PlaySFX(clip);
```

---

## 2. 📦 SERVICE LOCATOR PATTERN

**Mục đích:** Cung cấp global access point cho services.

**File:** `Assets/Game/Unity/Managers/GameManager.cs`

**Cách sử dụng:**
```csharp
// Lấy service từ GameManager
var player = GameManager.PlayerService.GetPlayer(PlayerId.Default);
GameManager.FarmingService.PlowTile(position, PlayerId.Default);
```

**Lưu ý:** Có thể chuyển sang Dependency Injection sau nếu cần.

---

## 3. ⚡ COMMAND PATTERN

**Mục đích:** Encapsulate requests thành objects, hỗ trợ undo/redo.

**Files:**
- `Assets/Game/Core/Application/Commands/ICommand.cs`
- `Assets/Game/Core/Application/Commands/CommandInvoker.cs`
- `Assets/Game/Core/Application/Commands/*Command.cs`

**Cách sử dụng:**
```csharp
var command = new PlowTileCommand(
    GameManager.FarmingService,
    position,
    PlayerId.Default
);

var result = GameManager.CommandInvoker.ExecuteCommand(command);
```

---

## 4. 🗄️ REPOSITORY PATTERN

**Mục đích:** Tách biệt data access logic khỏi business logic.

**Files:**
- `Assets/Game/Core/Domain/Interfaces/IPlayerRepository.cs`
- `Assets/Game/Core/Infrastructure/Repositories/PlayerRepository.cs`
- `Assets/Game/Core/Infrastructure/Repositories/FarmRepository.cs`
- `Assets/Game/Core/Infrastructure/Repositories/InventoryRepository.cs`

**Cách sử dụng:**
```csharp
var player = GameManager.PlayerRepository.GetPlayer(PlayerId.Default);
GameManager.PlayerRepository.SavePlayer(player);
```

---

## 5. 👁️ OBSERVER PATTERN (EVENT BUS)

**Mục đích:** Decouple components thông qua events.

**Files:**
- `Assets/Game/Core/Domain/Interfaces/IEventBus.cs`
- `Assets/Game/Core/Infrastructure/Services/EventBus.cs`
- `Assets/Game/Core/Application/Events/*Event.cs`

**Cách sử dụng:**
```csharp
// Subscribe
GameManager.EventBus.Subscribe<ItemAddedEvent>(OnItemAdded);

// Publish
GameManager.EventBus.Publish(new ItemAddedEvent(itemId, quantity));

// Unsubscribe
GameManager.EventBus.Unsubscribe<ItemAddedEvent>(OnItemAdded);
```

---

## 6. 🎲 STRATEGY PATTERN

**Mục đích:** Cho phép chuyển đổi algorithm/behavior lúc runtime.

**Files:**
- `Assets/Game/Unity/Network/INetworkService.cs`
- `Assets/Game/Unity/Network/OfflineNetworkService.cs`
- `Assets/Game/Unity/Network/OnlineNetworkService.cs`

**Cách sử dụng:**
```csharp
// Chuyển đổi giữa offline và online mode
var networkService = NetworkServiceManager.Instance.CurrentService;
networkService.SendMessageToServer("playerMove", data);
```

---

## 7. 🔌 ADAPTER PATTERN

**Mục đích:** Chuyển đổi interface của một class thành interface khác.

**Files:**
- `Assets/Game/Unity/Data/Adapters/ItemDatabaseAdapter.cs`
- `Assets/Game/Unity/Data/Adapters/TileStateDatabaseAdapter.cs`

**Cách sử dụng:**
```csharp
// Adapter chuyển đổi GameDatabase (Unity) thành IItemDatabase (Core)
var adapter = new ItemDatabaseAdapter(database);
GameManager.ItemDatabase = adapter;
```

---

## 8. 🎮 STATE PATTERN (MỚI)

**Mục đích:** Quản lý các trạng thái của game (Menu, Playing, Paused, etc.)

**Files:**
- `Assets/Game/Core/Application/States/IGameState.cs`
- `Assets/Game/Core/Application/States/GameStateMachine.cs`
- `Assets/Game/Core/Application/States/BaseGameState.cs`
- `Assets/Game/Unity/States/*GameState.cs`
- `Assets/Game/Unity/Managers/GameStateManager.cs`

**Cách sử dụng:**
```csharp
// Chuyển state
GameStateManager.Instance.ChangeState("Playing");

// Kiểm tra state hiện tại
if (GameStateManager.Instance.IsInState("Menu"))
{
    // Do something
}
```

**Các States:**
- `MenuGameState` - Menu/Title screen
- `PlayingGameState` - Game đang chơi
- `DialogueGameState` - Đang trong dialogue

**Thêm State mới:**
```csharp
public class ShopGameState : BaseGameState
{
    public override string StateName => "Shop";
    
    public override void Enter()
    {
        // Pause game, show shop UI
    }
    
    public override void Exit()
    {
        // Resume game, hide shop UI
    }
}
```

---

## 9. 🏭 FACTORY PATTERN (MỚI)

**Mục đích:** Tạo objects mà không cần chỉ định class cụ thể.

**Files:**
- `Assets/Game/Core/Application/Factories/IEntityFactory.cs`
- `Assets/Game/Core/Application/Factories/EntityFactory.cs`

**Cách sử dụng:**
```csharp
// Tạo factory (có thể thêm vào GameManager)
var factory = new EntityFactory(
    GameManager.ItemDatabase,
    GameManager.CropDatabase,
    GameManager.ToolDatabase
);

// Tạo entities
var item = factory.CreateItem(new ItemId("seed_wheat"));
var crop = factory.CreateCrop(new CropId("wheat"));
var tool = factory.CreateTool(new ToolId("hoe"));
```

**Lợi ích:**
- Cache entities để tối ưu performance
- Tập trung logic tạo entities
- Dễ dàng thêm logic tạo entities phức tạp

---

## 10. 🏊 OBJECT POOL PATTERN (MỚI)

**Mục đích:** Tái sử dụng GameObjects để tối ưu performance.

**Files:**
- `Assets/Game/Unity/Pools/IGameObjectPool.cs`
- `Assets/Game/Unity/Pools/GameObjectPool.cs`
- `Assets/Game/Unity/Pools/PoolManager.cs`

**Cách sử dụng:**
```csharp
// Tạo pool (trong Start hoặc Awake)
PoolManager.Instance.CreatePool(
    "ParticleEffects",
    particlePrefab,
    parentTransform,
    initialSize: 20,
    maxSize: 100
);

// Lấy object từ pool
var particle = PoolManager.Instance.Get("ParticleEffects");
particle.transform.position = spawnPosition;

// Trả về pool
PoolManager.Instance.Return("ParticleEffects", particle);
```

**Lợi ích:**
- Giảm garbage collection
- Tăng performance khi spawn nhiều objects
- Phù hợp cho particles, projectiles, UI elements

---

## 11. 🎨 MVP PATTERN (MỚI)

**Mục đích:** Tách biệt UI logic (Model-View-Presenter).

**Files:**
- `Assets/Game/Unity/UI/MVP/IView.cs`
- `Assets/Game/Unity/UI/MVP/IPresenter.cs`
- `Assets/Game/Unity/UI/MVP/BaseView.cs`
- `Assets/Game/Unity/UI/MVP/BasePresenter.cs`

**Cách sử dụng:**
```csharp
// View (Unity MonoBehaviour)
public class InventoryView : BaseView
{
    [SerializeField] private Text itemCountText;
    
    public void UpdateItemCount(int count)
    {
        itemCountText.text = $"Items: {count}";
    }
}

// Presenter
public class InventoryPresenter : BasePresenter<InventoryView>
{
    private IInventoryRepository _inventoryRepository;
    
    public InventoryPresenter(InventoryView view, IInventoryRepository repository)
        : base(view)
    {
        _inventoryRepository = repository;
    }
    
    public override void Initialize()
    {
        // Subscribe events
        GameManager.EventBus.Subscribe<ItemAddedEvent>(OnItemAdded);
        UpdateView();
    }
    
    private void OnItemAdded(ItemAddedEvent evt)
    {
        UpdateView();
    }
    
    private void UpdateView()
    {
        var inventory = _inventoryRepository.GetInventory(PlayerId.Default);
        View.UpdateItemCount(inventory.GetTotalItemCount());
    }
}
```

**Lợi ích:**
- Tách biệt UI và business logic
- Dễ test
- Dễ maintain và mở rộng

---

## 📊 SO SÁNH CÁC PATTERNS

| Pattern | Mục đích | Khi nào dùng |
|---------|----------|--------------|
| **Singleton** | Một instance duy nhất | Managers, Services |
| **Service Locator** | Global access point | Centralized services |
| **Command** | Encapsulate actions | Undo/Redo, Actions |
| **Repository** | Data access | Database operations |
| **Observer** | Event-driven | Decouple components |
| **Strategy** | Algorithm selection | Runtime behavior change |
| **Adapter** | Interface conversion | Legacy code integration |
| **State** | State management | Game states, AI states |
| **Factory** | Object creation | Complex object creation |
| **Object Pool** | Object reuse | Performance optimization |
| **MVP** | UI architecture | Complex UI logic |

---

## 🚀 KHUYẾN NGHỊ

### **Nên dùng:**
1. **State Pattern** cho game states, player states, AI states
2. **Factory Pattern** khi tạo entities phức tạp
3. **Object Pool** cho particles, projectiles, UI elements
4. **MVP Pattern** cho UI phức tạp

### **Tránh:**
1. **Singleton** quá nhiều (có thể dùng Service Locator hoặc DI)
2. **Service Locator** có thể thay bằng Dependency Injection nếu cần

---

## 📝 GHI CHÚ

- Tất cả patterns đều tuân theo Clean Architecture
- Core layer không phụ thuộc Unity
- Unity layer implement các interfaces từ Core
- Dễ dàng test và maintain

---

## 🔗 TÀI LIỆU THAM KHẢO

- [Game Programming Patterns](https://gameprogrammingpatterns.com/)
- [Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns)
- [Unity Best Practices](https://unity.com/how-to/unity-best-practices)

---

**Cập nhật lần cuối:** 2024

