using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats")]
    public int strength = 1;
    public int endurance = 10;
    public int intelligence = 0;

    [Header("Current State")]
    public int maxHealth;
    public int currentHealth;
    public int silver = 0;
    public int currentXP = 0;
    public int level = 1;

    [Header("Equipment Bonuses")]
    public int totalWeaponDamageBonus;
    public int totalArmorBonus;       // Passive (Body armor)
    public int totalShieldBonus;      // Active (Shield bonus when blocking)

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

    private void Start()
    {
        RecalculateMaxHealth();

        // Устанавливаем здоровье только если оно ещё не задано
        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
        }

    }

    public void RecalculateMaxHealth()
    {
        maxHealth = endurance;
    }

    // Call this whenever equipment changes OR before combat starts
    public void RecalculateStats()
    {
        // Reset bonuses
        totalWeaponDamageBonus = 0;
        totalArmorBonus = 0;
        totalShieldBonus = 0;

        // Read from InventoryManager
        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.equippedWeapon != null)
            {
                totalWeaponDamageBonus = InventoryManager.Instance.equippedWeapon.damageBonus;
            }

            if (InventoryManager.Instance.equippedArmor != null)
            {
                totalArmorBonus = InventoryManager.Instance.equippedArmor.armorBonus;
            }

            if (InventoryManager.Instance.equippedShield != null)
            {
                totalShieldBonus = InventoryManager.Instance.equippedShield.armorBonus;
            }
        }

        Debug.Log($"Stats Updated: WeaponBonus={totalWeaponDamageBonus}, ArmorBonus={totalArmorBonus}, ShieldBonus={totalShieldBonus}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        int xpNeeded = Mathf.RoundToInt(10f * Mathf.Pow(1.35f, level - 2));

        if (currentXP >= xpNeeded)
        {
            level++;
            currentXP -= xpNeeded;
            endurance++;
            RecalculateMaxHealth();
            currentHealth = maxHealth;
            Debug.Log($"Level Up! New Level: {level}");
        }
    }
}