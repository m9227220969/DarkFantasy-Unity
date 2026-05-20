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
}