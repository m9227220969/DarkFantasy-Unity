using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    private void Update()
    {
        // 1. Проверка: работает только в небоевых сценах (GDD 4.2.3)
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Combat_Forest" || currentScene == "Basement")
            return; // Прерываем, если сейчас бой

        // 2. Отслеживание нажатия клавиши I
        if (Input.GetKeyDown(KeyCode.I))
        {
            // 3. Безопасный вызов через Singleton
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ToggleInventory();
            }
            else
            {
                Debug.LogWarning("[Input] InventoryManager.Instance не найден. Убедись, что объект с инвентарём есть в сцене.");
            }
        }
    }
}