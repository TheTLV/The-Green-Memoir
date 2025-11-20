namespace TheGreenMemoir.Unity.UI.MVP
{
    /// <summary>
    /// Presenter Interface - MVP Pattern
    /// Xử lý logic và điều phối giữa View và Model
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// Khởi tạo presenter
        /// </summary>
        void Initialize();

        /// <summary>
        /// Cleanup khi destroy
        /// </summary>
        void Dispose();
    }
}

