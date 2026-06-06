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

    [Header("Data")]
    public List<ItemData> inventory = new List<ItemData>();
    public ItemData equippedWeapon;
    public ItemData equippedShield;
    public ItemData equippedArmor;

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
        DontDestroyOnLoad(gameObject);

        // Подписываемся на загрузку любой сцены
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Когда загружается Hub_Inn — закрываем инвентарь
        if (scene.name == "Hub_Inn")
        {
            // Ищем окно инвентаря в только что загруженной сцене
            inventoryWindow = GameObject.Find("Inventory_Window");

            if (inventoryWindow != null)
            {
                inventoryWindow.SetActive(false);
                Debug.Log("Inventory closed on scene load");
            }

            if (hudGroup != null)
            {
                hudGroup.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        // Отписываемся при уничтожении
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private bool isInitialized = false;

    private void Start()
    {
        if (isInitialized) return;

        playerStats = PlayerStats.Instance;

        // Загружаем стартовые предметы только один раз
        if (inventory.Count == 0)
        {
            LoadStartingItems();
        }

        // Закрываем инвентарь по умолчанию
        if (inventoryWindow != null)
            inventoryWindow.SetActive(false);

        if (hudGroup != null)
            hudGroup.SetActive(true);

        RefreshInventoryUI();

        // КРИТИЧНО: пересчитываем статы ПОСЛЕ загрузки предметов
        RecalculateStats();

        isInitialized = true;

        // Подписываемся на загрузку сцен
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void RecalculateStats()
    {
        if (playerStats == null)
        {
            playerStats = PlayerStats.Instance;
        }

        if (playerStats != null)
        {
            playerStats.RecalculateStats();
            Debug.Log($"[Inventory] Stats recalculated at start: Weapon={playerStats.totalWeaponDamageBonus}, Armor={playerStats.totalArmorBonus}, Shield={playerStats.totalShieldBonus}");
        }
    }

    private void LoadStartingItems()
    {
        // Создаём ScriptableObject-предметы программно или через Resources.Load
        // Для демо можно задать через инспектор
    }

    // Вызывай этот метод перед переходом в любую сцену
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


    public void ToggleInventory()
    {
        if (inventoryWindow == null)
        {
            inventoryWindow = GameObject.Find("Inventory_Window");
        }

        if (inventoryWindow == null) return;

        bool isOpen = !inventoryWindow.activeSelf;
        inventoryWindow.SetActive(isOpen);

        if (isOpen)
        {
            // При открытии обновляем UI
            RefreshInventoryUI();
        }

        if (hudGroup != null)
            hudGroup.SetActive(!isOpen);
    }


    public void RefreshInventoryUI()
    {
        if (gridContent == null || itemSlotPrefab == null) return;

        // Очищаем старые ячейки
        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }

        // Создаём новые только для предметов в inventory
        foreach (ItemData item in inventory)
        {
            if (item == null) continue;

            GameObject slotObj = Instantiate(itemSlotPrefab, gridContent);
            InventoryItem slotScript = slotObj.GetComponent<InventoryItem>();
            if (slotScript != null) slotScript.Setup(item);
        }
    }

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
        RecalculateStats();

        if (TooltipManager.Instance != null)
            TooltipManager.Instance.HideTooltip();
    }

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
            RecalculateStats();

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