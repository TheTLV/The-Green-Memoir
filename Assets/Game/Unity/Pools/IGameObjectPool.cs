using UnityEngine;

namespace TheGreenMemoir.Unity.Pools
{
    /// <summary>
    /// Interface cho GameObject Pool - Object Pool Pattern
    /// Quản lý pool của GameObjects để tái sử dụng
    /// </summary>
    public interface IGameObjectPool
    {
        /// <summary>
        /// Lấy GameObject từ pool
        /// </summary>
        GameObject Get();

        /// <summary>
        /// Trả GameObject về pool
        /// </summary>
        void Return(GameObject obj);

        /// <summary>
        /// Xóa tất cả objects trong pool
        /// </summary>
        void Clear();
    }
}

