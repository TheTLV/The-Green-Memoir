using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectionBorder;
    [SerializeField] private TextMeshProUGUI quantityText;

    /// <summary>
    /// Updates the slot with item visual data and displays uses/quantity.
    /// </summary>
    public void UpdateSlot(ItemData item, int quantity)
    {
        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;

            // Display quantity only if needed (for Water Can Uses or Stackable Items)
            if (quantity > 1 || item.isStackable)
            {
                quantityText.text = quantity.ToString();
            }
            else
            {
                quantityText.text = "";
            }
        }
        else
        {
            // Clear the slot
            iconImage.enabled = false;
            quantityText.text = "";
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionBorder != null)
        {
            selectionBorder.gameObject.SetActive(isSelected);
        }
    }
}