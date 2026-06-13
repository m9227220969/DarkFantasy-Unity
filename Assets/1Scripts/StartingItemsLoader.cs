using UnityEngine;

public class StartingItemsLoader : MonoBehaviour
{
    [Header("Starting Equipment")]
    public ItemData startWeapon;
    public ItemData startShield;
    public ItemData startArmor;
    public ItemData startPie;

    private void Start()
    {
        if (InventoryData.Instance == null)
        {
            Debug.LogError("InventoryData.Instance is null!");
            return;
        }

        var invData = InventoryData.Instance;

        // Только при первом запуске (если инвентарь пуст)
        if (invData.inventory.Count == 0)
        {
            if (startWeapon != null) invData.inventory.Add(startWeapon);
            if (startShield != null) invData.inventory.Add(startShield);
            if (startArmor != null) invData.inventory.Add(startArmor);
            if (startPie != null) invData.inventory.Add(startPie);

            Debug.Log($"Starting items loaded: Weapon={startWeapon != null}, Shield={startShield != null}, Armor={startArmor != null}, Pie={startPie != null}");
        }
    }
}