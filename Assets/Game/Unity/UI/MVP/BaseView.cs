using UnityEngine;

namespace TheGreenMemoir.Unity.UI.MVP
{
    /// <summary>
    /// Base View - MVP Pattern
    /// Cung cấp implementation mặc định cho View
    /// </summary>
    public abstract class BaseView : MonoBehaviour, IView
    {
        [SerializeField] protected GameObject viewPanel;

        public bool IsVisible { get; private set; }

        protected virtual void Awake()
        {
            if (viewPanel == null)
            {
                viewPanel = gameObject;
            }
        }

        public virtual void Show()
        {
            if (viewPanel != null)
            {
                viewPanel.SetActive(true);
            }
            IsVisible = true;
            OnShow();
        }

        public virtual void Hide()
        {
            if (viewPanel != null)
            {
                viewPanel.SetActive(false);
            }
            IsVisible = false;
            OnHide();
        }

        /// <summary>
        /// Override để thêm logic khi show
        /// </summary>
        protected virtual void OnShow()
        {
            // Override nếu cần
        }

        /// <summary>
        /// Override để thêm logic khi hide
        /// </summary>
        protected virtual void OnHide()
        {
            // Override nếu cần
        }
    }
}

