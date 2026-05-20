using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header(" UI References")]
    public GameObject inventoryWindow;      // Панель инвентаря
    public Transform gridContent;           // Объект "Content" внутри Scroll View
    public GameObject itemSlotPrefab;       // Префаб ячейки инвентаря

    [Header(" Equipment Slots")]
    public Transform slotWeapon;            // Слот оружия (Правая рука)
    public Transform slotShield;            // Слот щита (Левая рука)
    public Transform slotArmor;             // Слот брони (Туловище)

    [Header("📦 Data")]
    public List<ItemData> inventory = new List<ItemData>(); // Сумка
    public ItemData equippedWeapon;
    public ItemData equippedShield;
    public ItemData equippedArmor;

    private void Awake()
    {
        // Паттерн Singleton для удобного доступа из других скриптов
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // По умолчанию инвентарь закрыт
        if (inventoryWindow != null)
            inventoryWindow.SetActive(false);
    }

    private void Start()
    {
        RefreshInventoryUI();
    }

    /// <summary>
    /// Открыть/закрыть инвентарь (GDD 4.2.3)
    /// </summary>
    public void ToggleInventory()
    {
        if (inventoryWindow == null) return;

        bool isActive = !inventoryWindow.activeSelf;
        inventoryWindow.SetActive(isActive);
    }

    /// <summary>
    /// Перерисовывает сетку инвентаря на основе списка inventory
    /// </summary>
    public void RefreshInventoryUI()
    {
        if (gridContent == null || itemSlotPrefab == null)
        {
            Debug.LogWarning("[Inventory] Не назначены Grid Content или ItemSlot Prefab в инспекторе!");
            return;
        }

        // 1. Очищаем старые ячейки
        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }

        // 2. Создаём новые ячейки для каждого предмета
        foreach (ItemData item in inventory)
        {
            if (item == null) continue; // Пропускаем пустые слоты

            GameObject slotObj = Instantiate(itemSlotPrefab, gridContent);
            InventoryItem slotScript = slotObj.GetComponent<InventoryItem>();

            if (slotScript != null)
            {
                slotScript.Setup(item);
            }
        }
    }

    /// <summary>
    /// Экипировать предмет (вызывается из InventoryItem при клике)
    /// </summary>
    public void EquipItem(ItemData item)
    {
        if (item == null) return;

        switch (item.type)
        {
            case ItemData.ItemType.Weapon:
                equippedWeapon = item;
                Debug.Log($"[Equip] Надето: {item.itemName} (+{item.damageBonus} урон)");
                UpdateEquipmentSlotVisual(slotWeapon, item);
                break;

            case ItemData.ItemType.Shield:
                equippedShield = item;
                Debug.Log($"[Equip] Надето: {item.itemName} (+{item.armorBonus} защита)");
                UpdateEquipmentSlotVisual(slotShield, item);
                break;

            case ItemData.ItemType.Armor:
                equippedArmor = item;
                Debug.Log($"[Equip] Надето: {item.itemName} (+{item.armorBonus} защита)");
                UpdateEquipmentSlotVisual(slotArmor, item);
                break;

            default:
                Debug.Log($"[Equip] Нельзя экипировать: {item.itemName} (тип: {item.type})");
                break;
        }
    }

    /// <summary>
    /// Обновляет иконку в слоте экипировки
    /// </summary>
    private void UpdateEquipmentSlotVisual(Transform slot, ItemData item)
    {
        if (slot == null) return;

        // Ищем компонент Image внутри слота
        Image iconImage = slot.GetComponentInChildren<Image>();
        if (iconImage != null)
        {
            if (item != null && item.icon != null)
            {
                iconImage.sprite = item.icon;
                iconImage.enabled = true;
            }
            else
            {
                // Если предмет снят, можно поставить иконку "пустой слот"
                // Пока просто скрываем или оставляем как есть
                iconImage.enabled = true;
            }
        }
    }
}