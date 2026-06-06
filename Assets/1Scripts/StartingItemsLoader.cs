using UnityEngine;
using System.Collections.Generic;

public class StartingItemsLoader : MonoBehaviour
{
    [Header("Starting Equipment")]
    public ItemData startWeapon;
    public ItemData startShield;
    public ItemData startArmor;
    public ItemData startPie;

    private void Start()
    {
        if (InventoryManager.Instance == null) return;

        var inv = InventoryManager.Instance;

        if (inv.inventory.Count == 0)
        {
            if (startWeapon != null) inv.inventory.Add(startWeapon);
            if (startShield != null) inv.inventory.Add(startShield);
            if (startArmor != null) inv.inventory.Add(startArmor);
            if (startPie != null) inv.inventory.Add(startPie);

            inv.RefreshInventoryUI();

            // Ёкипируем стартовые предметы
            if (startWeapon != null) inv.EquipItem(startWeapon);
            if (startShield != null) inv.EquipItem(startShield);
            if (startArmor != null) inv.EquipItem(startArmor);
        }
    }
}