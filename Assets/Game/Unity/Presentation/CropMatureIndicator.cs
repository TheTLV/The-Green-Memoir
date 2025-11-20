using UnityEngine;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Hiển thị icon/animation trên đầu cây khi đã trưởng thành (có thể thu hoạch)
    /// </summary>
    public class CropMatureIndicator : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Crop entity (tự động lấy từ FarmTile nếu để trống)")]
        [SerializeField] private Core.Domain.Entities.Crop crop;

        [Tooltip("CropDataSO (tự động lấy từ GameDatabase nếu để trống)")]
        [SerializeField] private CropDataSO cropData;

        [Header("Visual")]
        [Tooltip("GameObject chứa icon/animation (sẽ hiện/ẩn tự động)")]
        [SerializeField] private GameObject indicatorObject;

        [Tooltip("Offset từ vị trí cây (để icon ở trên đầu cây)")]
        [SerializeField] private Vector3 offset = new Vector3(0, 1, 0);

        [Header("Animation (Optional)")]
        [Tooltip("Có tự động animate icon không (ví dụ: nhấp nháy, xoay)")]
        [SerializeField] private bool animateIcon = true;

        [Tooltip("Tốc độ animation (nếu animateIcon = true)")]
        [SerializeField] private float animationSpeed = 2f;

        [Tooltip("Biên độ animation (nếu animateIcon = true)")]
        [SerializeField] private float animationAmplitude = 0.2f;

        private Vector3 originalPosition;
        private bool isMature = false;

        private void Start()
        {
            if (indicatorObject != null)
            {
                originalPosition = indicatorObject.transform.localPosition;
                indicatorObject.SetActive(false);
            }

            // Tự động lấy CropDataSO từ GameDatabase nếu chưa có
            if (cropData == null && crop != null)
            {
                var database = Managers.GameDatabaseManager.GetDatabase();
                if (database != null)
                {
                    cropData = database.GetCropData(crop.Id);
                }
            }
        }

        private void Update()
        {
            if (crop == null || cropData == null || indicatorObject == null)
                return;

            // Kiểm tra cây đã trưởng thành chưa
            bool wasMature = isMature;
            isMature = cropData.IsCurrentSpriteMature(crop);

            // Hiển thị/ẩn indicator
            if (isMature != wasMature)
            {
                indicatorObject.SetActive(isMature);
            }

            // Animation nếu đã trưởng thành
            if (isMature && animateIcon)
            {
                float yOffset = Mathf.Sin(Time.time * animationSpeed) * animationAmplitude;
                indicatorObject.transform.localPosition = originalPosition + new Vector3(0, yOffset, 0);
            }
        }

        /// <summary>
        /// Set crop và cropData thủ công (nếu cần)
        /// </summary>
        public void SetCrop(Core.Domain.Entities.Crop crop, CropDataSO cropData)
        {
            this.crop = crop;
            this.cropData = cropData;
        }

        /// <summary>
        /// Kiểm tra cây đã trưởng thành chưa
        /// </summary>
        public bool IsMature()
        {
            if (crop == null || cropData == null)
                return false;

            return cropData.IsCurrentSpriteMature(crop);
        }
    }
}

