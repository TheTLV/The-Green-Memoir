using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Factories
{
    /// <summary>
    /// Interface cho Entity Factory - Factory Pattern
    /// Tạo các entities (Item, Crop, Tool, etc.)
    /// </summary>
    public interface IEntityFactory
    {
        /// <summary>
        /// Tạo Item từ ID
        /// </summary>
        Item CreateItem(ItemId itemId);

        /// <summary>
        /// Tạo Crop từ ID
        /// </summary>
        Crop CreateCrop(CropId cropId);

        /// <summary>
        /// Tạo Tool từ ID
        /// </summary>
        Tool CreateTool(ToolId toolId);
    }
}

