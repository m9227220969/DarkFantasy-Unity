using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject inventoryWindow;
    public Transform gridContent;
    public GameObject itemSlotPrefab;

    [Header("Equipment Slots")]
    public Transform slotWeapon;
    public Transform slotShield;
    public Transform slotArmor;

    [Header("HUD Group")]
    public GameObject hudGroup;

    private PlayerStats playerStats;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerStats = PlayerStats.Instance;

        if (inventoryWindow != null)
            inventoryWindow.SetActive(false);

        if (hudGroup != null)
            hudGroup.SetActive(true);

        RefreshInventoryUI();
        RecalculateStats();
    }

    public void ToggleInventory()
    {
        Debug.Log("[InventoryManager] ToggleInventory called");
        Debug.Log($"[InventoryManager] inventoryWindow = {(inventoryWindow != null ? inventoryWindow.name : "NULL")}");

        if (inventoryWindow == null)
        {
            Debug.LogError("[InventoryManager] inventoryWindow is NULL! Assign it in Inspector.");
            return;
        }

        bool isOpen = !inventoryWindow.activeSelf;
        inventoryWindow.SetActive(isOpen);

        Debug.Log($"[InventoryManager] Inventory {(isOpen ? "OPENED" : "CLOSED")}");

        if (hudGroup != null)
            hudGroup.SetActive(!isOpen);
    }

    public void RefreshInventoryUI()
    {
        if (gridContent == null || itemSlotPrefab == null) return;

        foreach (Transform child in gridContent)
            Destroy(child.gameObject);

        if (InventoryData.Instance == null) return;

        foreach (ItemData item in InventoryData.Instance.inventory)
        {
            if (item == null) continue;

            GameObject slotObj = Instantiate(itemSlotPrefab, gridContent);
            InventoryItem slotScript = slotObj.GetComponent<InventoryItem>();
            if (slotScript != null) slotScript.Setup(item);
        }
    }

    public void EquipItem(ItemData item)
    {
        if (item == null || !InventoryData.Instance.inventory.Contains(item)) return;

        ItemData oldItem = null;

        switch (item.type)
        {
            case ItemData.ItemType.Weapon:
                oldItem = InventoryData.Instance.equippedWeapon;
                InventoryData.Instance.equippedWeapon = item;
                UpdateEquipmentSlotVisual(slotWeapon, item);
                break;
            case ItemData.ItemType.Shield:
                oldItem = InventoryData.Instance.equippedShield;
                InventoryData.Instance.equippedShield = item;
                UpdateEquipmentSlotVisual(slotShield, item);
                break;
            case ItemData.ItemType.Armor:
                oldItem = InventoryData.Instance.equippedArmor;
                InventoryData.Instance.equippedArmor = item;
                UpdateEquipmentSlotVisual(slotArmor, item);
                break;
            default:
                return;
        }

        InventoryData.Instance.inventory.Remove(item);
        if (oldItem != null) InventoryData.Instance.inventory.Add(oldItem);

        RefreshInventoryUI();
        RecalculateStats();
    }

    public void UnequipItem(ItemData.ItemType type)
    {
        ItemData itemToReturn = null;
        Transform targetSlot = null;

        switch (type)
        {
            case ItemData.ItemType.Weapon:
                if (InventoryData.Instance.equippedWeapon == null) return;
                itemToReturn = InventoryData.Instance.equippedWeapon;
                InventoryData.Instance.equippedWeapon = null;
                targetSlot = slotWeapon;
                break;
            case ItemData.ItemType.Shield:
                if (InventoryData.Instance.equippedShield == null) return;
                itemToReturn = InventoryData.Instance.equippedShield;
                InventoryData.Instance.equippedShield = null;
                targetSlot = slotShield;
                break;
            case ItemData.ItemType.Armor:
                if (InventoryData.Instance.equippedArmor == null) return;
                itemToReturn = InventoryData.Instance.equippedArmor;
                InventoryData.Instance.equippedArmor = null;
                targetSlot = slotArmor;
                break;
        }

        if (itemToReturn != null)
        {
            InventoryData.Instance.inventory.Add(itemToReturn);
            UpdateEquipmentSlotVisual(targetSlot, null);
            RefreshInventoryUI();
            RecalculateStats();
        }
    }

    private void RecalculateStats()
    {
        if (playerStats == null) playerStats = PlayerStats.Instance;
        if (playerStats != null) playerStats.RecalculateStats();
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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ForceCloseInventory()
    {
        if (inventoryWindow != null && inventoryWindow.activeSelf)
        {
            inventoryWindow.SetActive(false);
        }

        if (hudGroup != null && !hudGroup.activeSelf)
        {
            hudGroup.SetActive(true);
        }
    }
}