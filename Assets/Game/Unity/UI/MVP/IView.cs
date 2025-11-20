namespace TheGreenMemoir.Unity.UI.MVP
{
    /// <summary>
    /// View Interface - MVP Pattern
    /// Đại diện cho UI layer, chỉ hiển thị và nhận input
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Hiển thị view
        /// </summary>
        void Show();

        /// <summary>
        /// Ẩn view
        /// </summary>
        void Hide();

        /// <summary>
        /// Kiểm tra xem view có đang hiển thị không
        /// </summary>
        bool IsVisible { get; }
    }
}

