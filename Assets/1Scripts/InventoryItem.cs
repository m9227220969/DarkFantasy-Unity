using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [Header("Data")]
    public ItemData data;

    [Header("UI")]
    public Image iconImage;

    /// <summary>
    /// Вызывается из InventoryManager при создании ячейки
    /// </summary>
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

    /// <summary>
    /// Этот метод нужно привязать к OnClick() кнопки на префабе
    /// </summary>
    public void OnClickItem()
    {
        if (data != null)
        {
            InventoryManager.Instance.EquipItem(data);
        }
        else
        {
            Debug.LogWarning("[InventoryItem] Попытка кликнуть по пустой ячейке");
        }
    }
}