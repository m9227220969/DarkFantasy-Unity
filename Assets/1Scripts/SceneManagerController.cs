using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagerController : MonoBehaviour
{
    public static SceneManagerController Instance { get; private set; }

    [Header("Fade Settings")]
    public GameObject fadeOverlay;
    public float fadeDuration = 0.5f;

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
        if (fadeOverlay == null)
        {
            Debug.LogWarning("FadeOverlay not assigned in SceneManagerController!");
        }
    }

    /// <summary>
    /// Новая игра - загрузка сцены хаба
    /// </summary>
    public void StartNewGame()
    {
        StartCoroutine(LoadSceneAsync("Hub_Inn"));
    }

    /// <summary>
    /// Продолжить игру - загрузка сохранения и переход в хаб
    /// </summary>
    public void ContinueGame()
    {
        if (SaveSystem.HasSave())
        {
            // Загружаем данные сохранения
            if (PlayerStats.Instance != null)
            {
                SaveSystem.LoadGame(PlayerStats.Instance);
            }

            // Переходим в хаб
            StartCoroutine(LoadSceneAsync("Hub_Inn"));
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }

    /// <summary>
    /// Временный метод для теста боевой сцены
    /// </summary>
    public void LoadTestCombat()
    {
        // Закрываем инвентарь перед переходом
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ForceCloseInventory();
        }

        StartCoroutine(LoadSceneAsync("Combat_Forest"));
    }

    /// <summary>
    /// Вернуться в главное меню
    /// </summary>
    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadSceneAsync("MainMenu"));
    }

    /// <summary>
    /// Выход из игры
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Переключить инвентарь (вызывается кнопкой в хабе)
    /// </summary>
    public void ToggleInventoryButton()
    {
        Debug.Log("[SceneManagerController] ToggleInventoryButton called");
        if (InventoryManager.Instance != null)
        {
            Debug.Log("[SceneManagerController] InventoryManager.Instance found");
            InventoryManager.Instance.ToggleInventory();
        }
        else
        {
            Debug.LogError("[SceneManagerController] InventoryManager.Instance is NULL!");
        }
    }

    /// <summary>
    /// Асинхронная загрузка сцены с затемнением
    /// </summary>
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        // Включаем затемнение
        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
            // Здесь можно добавить анимацию fade in
        }

        // Ждём немного для эффекта
        yield return new WaitForSeconds(fadeDuration);

        // Загружаем сцену
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Выключаем затемнение
        if (fadeOverlay != null)
        {
            // Здесь можно добавить анимацию fade out
            fadeOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// Перезагрузить текущую сцену
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneAsync(currentScene));
    }
}