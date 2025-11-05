using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; 

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    public ItemData currentItemData;

    public void UpdateSlot(ItemData itemData, int quantity)
    {
        currentItemData = itemData;

        if (itemData != null)
        {
            itemIcon.sprite = itemData.icon;
            itemIcon.enabled = true;

            quantityText.text = quantity > 1 ? quantity.ToString() : "";
            quantityText.enabled = true;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityText.text = "";
        quantityText.enabled = false;
        currentItemData = null;
    }

    public void OnClickSlot()
    {
        if (currentItemData != null)
        {
            Debug.Log($"Clicked on {currentItemData.itemName}");
            // TODO: Logic open Context Menu/ Use Item

            // VD: InventoryUIController.Instance.ShowContextMenu(currentItemData);
        }
    }
}