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

    [Header("⚔ Equipment Slots")]
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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (inventoryWindow != null) inventoryWindow.SetActive(false);
        if (hudGroup != null) hudGroup.SetActive(true);
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

    // ✅ Этот метод должен быть строго так:
    public void RefreshInventoryUI()
    {
        if (gridContent == null || itemSlotPrefab == null)
        {
            Debug.LogWarning("[Inventory] Не назначены Grid Content или ItemSlot Prefab в инспекторе!");
            return;
        }

        // Очищаем старые ячейки
        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }

        // Создаём новые для каждого предмета
        foreach (ItemData item in inventory)
        {
            if (item == null) continue;

            GameObject slotObj = Instantiate(itemSlotPrefab, gridContent);
            InventoryItem slotScript = slotObj.GetComponent<InventoryItem>();

            if (slotScript != null)
                slotScript.Setup(item);
        }
    }

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
                Debug.Log($"[Equip] Нельзя экипировать: {item.itemName}");
                break;
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