using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;           // Иконка предмета
    public string description;    // Описание для тултипа

    // Тип предмета
    public enum ItemType { Weapon, Shield, Armor, Consumable, Quest }
    public ItemType type;

    // Характеристики
    public int damageBonus;       // Для оружия
    public int armorBonus;        // Для брони/щита
    public int healAmount;        // Для зелий
}