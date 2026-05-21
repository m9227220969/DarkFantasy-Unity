using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ⚠️ Важно!

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData data;
    public Image iconImage;

    public void Setup(ItemData itemData)
    {
        data = itemData;
        if (iconImage != null && itemData != null && itemData.icon != null)
        {
            iconImage.sprite = itemData.icon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
        }
    }

    public void OnClickItem()
    {
        if (data != null)
        {
            InventoryManager.Instance.EquipItem(data);
        }
    }

    // --- Методы для Тултипов ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (data != null)
        {
            // Показываем тултип в позиции курсора
            TooltipManager.Instance.ShowTooltip(data, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}