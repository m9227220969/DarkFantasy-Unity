using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("🖼 UI References")]
    public GameObject inventoryWindow;
    public Transform gridContent;
    public GameObject itemSlotPrefab;

    [Header(" Equipment Slots")]
    public Transform slotWeapon;
    public Transform slotShield;
    public Transform slotArmor;

    [Header("📦 Data")]
    public List<ItemData> inventory = new List<ItemData>();
    public ItemData equippedWeapon;
    public ItemData equippedShield;
    public ItemData equippedArmor;

    [Header("🔽 HUD Group")]
    public GameObject hudGroup;

    private PlayerStats playerStats;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (inventoryWindow != null) inventoryWindow.SetActive(false);
        if (hudGroup != null) hudGroup.SetActive(true);

        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    private void Start()
    {
        RefreshInventoryUI();
    }

    public void ToggleInventory()
    {
        if (inventoryWindow == null) return;

        bool isOpen = !inventoryWindow.activeSelf;
        inventoryWindow.SetActive(isOpen);

        if (hudGroup != null)
            hudGroup.SetActive(!isOpen);
    }

    public void RefreshInventoryUI()
    {
        if (gridContent == null || itemSlotPrefab == null) return;

        foreach (Transform child in gridContent)
            Destroy(child.gameObject);

        foreach (ItemData item in inventory)
        {
            if (item == null) continue;

            GameObject slotObj = Instantiate(itemSlotPrefab, gridContent);
            InventoryItem slotScript = slotObj.GetComponent<InventoryItem>();
            if (slotScript != null) slotScript.Setup(item);
        }
    }

    /// <summary>
    /// Экипировать предмет из сумки (с заменой старого)
    /// </summary>
    public void EquipItem(ItemData item)
    {
        if (item == null || !inventory.Contains(item)) return;

        ItemData oldItem = null;

        switch (item.type)
        {
            case ItemData.ItemType.Weapon:
                oldItem = equippedWeapon;
                equippedWeapon = item;
                UpdateEquipmentSlotVisual(slotWeapon, item);
                break;
            case ItemData.ItemType.Shield:
                oldItem = equippedShield;
                equippedShield = item;
                UpdateEquipmentSlotVisual(slotShield, item);
                break;
            case ItemData.ItemType.Armor:
                oldItem = equippedArmor;
                equippedArmor = item;
                UpdateEquipmentSlotVisual(slotArmor, item);
                break;
            default:
                return;
        }

        inventory.Remove(item);
        if (oldItem != null) inventory.Add(oldItem);

        RefreshInventoryUI();
        if (playerStats != null) playerStats.RecalculateStats();

        // 🔑 Принудительно скрываем тултип после изменения инвентаря
        if (TooltipManager.Instance != null)
            TooltipManager.Instance.HideTooltip();
    
}

    /// <summary>
    /// Снять предмет из слота экипировки
    /// </summary>
    public void UnequipItem(ItemData.ItemType type)
    {
        ItemData itemToReturn = null;
        Transform targetSlot = null;

        switch (type)
        {
            case ItemData.ItemType.Weapon:
                if (equippedWeapon == null) return;
                itemToReturn = equippedWeapon;
                equippedWeapon = null;
                targetSlot = slotWeapon;
                break;
            case ItemData.ItemType.Shield:
                if (equippedShield == null) return;
                itemToReturn = equippedShield;
                equippedShield = null;
                targetSlot = slotShield;
                break;
            case ItemData.ItemType.Armor:
                if (equippedArmor == null) return;
                itemToReturn = equippedArmor;
                equippedArmor = null;
                targetSlot = slotArmor;
                break;
        }

        if (itemToReturn != null)
        {
            inventory.Add(itemToReturn);
            UpdateEquipmentSlotVisual(targetSlot, null);
            RefreshInventoryUI();
            if (playerStats != null) playerStats.RecalculateStats();

            // 🔑 Принудительно скрываем тултип после изменения инвентаря
            if (TooltipManager.Instance != null)
                TooltipManager.Instance.HideTooltip();
        }
    }

    private void UpdateEquipmentSlotVisual(Transform slot, ItemData item)
    {
        if (slot == null) return;
        Image iconImage = slot.GetComponentInChildren<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = (item != null && item.icon != null) ? item.icon : null;
            iconImage.enabled = true;
        }
    }
}