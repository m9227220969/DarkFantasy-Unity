using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // В Инспекторе выбери тип слота: Weapon, Shield или Armor
    public ItemData.ItemType slotType;

    /// <summary>
    /// Клик по слоту: снимаем предмет и скрываем тултип
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance != null)
        {
            // 1. Пытаемся снять предмет
            InventoryManager.Instance.UnequipItem(slotType);

            // 2. 🔑 ИСПРАВЛЕНИЕ: Принудительно скрываем тултип.
            // Это предотвращает "залипание", так как курсор не покидает зону кнопки при клике.
            if (TooltipManager.Instance != null)
            {
                TooltipManager.Instance.HideTooltip();
            }
        }
    }

    /// <summary>
    /// Наведение мыши: показываем тултип текущего предмета в слоте
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemData itemToShow = null;

        // Определяем, что сейчас надето в этом слоте
        if (InventoryManager.Instance != null)
        {
            switch (slotType)
            {
                case ItemData.ItemType.Weapon: itemToShow = InventoryManager.Instance.equippedWeapon; break;
                case ItemData.ItemType.Shield: itemToShow = InventoryManager.Instance.equippedShield; break;
                case ItemData.ItemType.Armor: itemToShow = InventoryManager.Instance.equippedArmor; break;
            }
        }

        // Если слот занят — показываем тултип
        if (itemToShow != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.ShowTooltip(itemToShow, eventData.position);
        }
    }

    /// <summary>
    /// Уход мыши: скрываем тултип
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}