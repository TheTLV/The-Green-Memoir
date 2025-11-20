using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Interface để load Item từ database
    /// Implementation sẽ ở Unity layer (dùng ScriptableObjects)
    /// </summary>
    public interface IItemDatabase
    {
        /// <summary>
        /// Lấy Item từ ItemId
        /// </summary>
        Item GetItem(ItemId itemId);

        /// <summary>
        /// Kiểm tra xem ItemId có tồn tại không
        /// </summary>
        bool HasItem(ItemId itemId);
    }

    /// <summary>
    /// Interface để load Crop từ database
    /// </summary>
    public interface ICropDatabase
    {
        Crop GetCrop(CropId cropId);
        bool HasCrop(CropId cropId);
    }

    /// <summary>
    /// Interface để load Tool từ database
    /// </summary>
    public interface IToolDatabase
    {
        Tool GetTool(ToolId toolId);
        bool HasTool(ToolId toolId);
    }
}

