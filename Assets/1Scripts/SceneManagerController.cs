using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    // Ссылка на объект затемнения (создадим на след. шаге)
    [SerializeField] private GameObject fadePanel;

    public void StartNewGame()
    {
        StartCoroutine(LoadSceneAsync("Hub_Inn"));
    }
    public void LoadTestCombat()
    {
        // Закрываем инвентарь ПЕРЕД переходом в бой
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ForceCloseInventory();
        }

        StartCoroutine(LoadSceneAsync("Combat_Forest"));
    }

    // Добавь метод для возврата в хаб из боя
    public void ReturnToHubFromCombat()
    {
        // Закрываем инвентарь при возврате
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ForceCloseInventory();
        }

        StartCoroutine(LoadSceneAsync("Hub_Inn"));
    }
    public void ContinueGame()
    {
        // Пока просто загружаем хаб, позже добавим проверку сохранения
        if (PlayerPrefs.HasKey("SaveExists"))
            StartCoroutine(LoadSceneAsync("Hub_Inn"));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void BackMainMenu()
    {
        StartCoroutine(LoadSceneAsync("MainMenu"));
    }
    // Метод для плавной загрузки сцен
    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        // 1. Включаем затемнение
        if (fadePanel != null) fadePanel.SetActive(true);

        // Ждем пока экран станет черным (можно настроить время в анимации)
        yield return new WaitForSeconds(0.5f);

        // 2. Загружаем сцену
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        // 3. Сцена загружена, затемнение останется активным до инициализации новой сцены
    }
}