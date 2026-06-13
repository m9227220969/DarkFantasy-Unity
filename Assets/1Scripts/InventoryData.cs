using UnityEngine;
using System.Collections.Generic;

public class InventoryData : MonoBehaviour
{
    public static InventoryData Instance { get; private set; }

    public List<ItemData> inventory = new List<ItemData>();
    public ItemData equippedWeapon;
    public ItemData equippedShield;
    public ItemData equippedArmor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}