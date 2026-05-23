using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Характеристики")]
    public int maxHealth = 10;      // GDD 4.5.2: Выносливость = 10
    public int currentHealth;
    public int silver = 0;          // GDD 4.7.2: Старт с 0 серебра

    private void Start()
    {
        // При старте здоровье полное
        currentHealth = maxHealth;
    }
    private static PlayerStats instance;

    private void Awake()
    {
        // 🔑 Защита от дубликатов при повторном нажатии Play
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // 🔑 КРИТИЧЕСКИ ВАЖНО: Объект останется в памяти при переходе между сценами
        DontDestroyOnLoad(gameObject);

        // Инициализация только при первом создании
        if (currentHealth == 0) currentHealth = maxHealth;
    }
    // Метод получения урона
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // Позже здесь будет вызов Game Over
        Debug.Log($"Получено {damage} урона. Осталось HP: {currentHealth}");
    }

    // Метод лечения
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log($"Восстановлено {amount} HP. Текущее HP: {currentHealth}");
    }

    // Метод добавления серебра
    public void AddSilver(int amount)
    {
        silver += amount;
        Debug.Log($"Получено {amount} серебра. Всего: {silver}");
    }

    // ... (твои старые переменные maxHealth, currentHealth, silver) ...

    // Эти переменные будем хранить для боевых расчётов
    public int totalWeaponDamageBonus;
    public int totalArmorBonus; // Для тела
    public int totalShieldBonus; // Для блока

    /// <summary>
    /// Вызывается из InventoryManager при смене экипировки
    /// </summary>
    public void RecalculateStats()
    {
        // Сбрасываем бонусы перед пересчётом
        totalWeaponDamageBonus = 0;
        totalArmorBonus = 0;
        totalShieldBonus = 0;

        // Если есть менеджер инвентаря и он существует
        if (InventoryManager.Instance != null)
        {
            // 1. Оружие
            if (InventoryManager.Instance.equippedWeapon != null)
            {
                totalWeaponDamageBonus = InventoryManager.Instance.equippedWeapon.damageBonus;
            }

            // 2. Броня (Туловище)
            if (InventoryManager.Instance.equippedArmor != null)
            {
                totalArmorBonus = InventoryManager.Instance.equippedArmor.armorBonus;
            }

            // 3. Щит
            if (InventoryManager.Instance.equippedShield != null)
            {
                totalShieldBonus = InventoryManager.Instance.equippedShield.armorBonus;
            }
        }

        // Логируем для проверки
        Debug.Log($"[Stats] Recalculated: DmgBonus={totalWeaponDamageBonus}, Armor={totalArmorBonus}, Shield={totalShieldBonus}");
    }
}