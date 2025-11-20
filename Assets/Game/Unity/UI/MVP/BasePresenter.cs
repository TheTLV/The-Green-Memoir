using TheGreenMemoir.Unity.UI.MVP;

namespace TheGreenMemoir.Unity.UI.MVP
{
    /// <summary>
    /// Base Presenter - MVP Pattern
    /// Cung cấp implementation mặc định cho Presenter
    /// </summary>
    public abstract class BasePresenter<TView> : IPresenter where TView : IView
    {
        protected TView View { get; }

        protected BasePresenter(TView view)
        {
            View = view;
        }

        public virtual void Initialize()
        {
            // Override nếu cần
        }

        public virtual void Dispose()
        {
            // Override nếu cần cleanup
        }
    }
}

