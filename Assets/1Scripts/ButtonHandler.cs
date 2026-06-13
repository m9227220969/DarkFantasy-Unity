using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public enum ActionType
    {
        ToggleInventory,
        LoadTestCombat,
        ReturnToMainMenu,
        ExitGame,
        StartNewGame,
        ContinueGame,
        UsePotion  // ← Добавлено
    }

    public ActionType action;

    public void OnButtonClick()
    {
        switch (action)
        {
            case ActionType.ToggleInventory:
                if (SceneManagerController.Instance != null)
                    SceneManagerController.Instance.ToggleInventoryButton();
                break;
            case ActionType.LoadTestCombat:
                if (SceneManagerController.Instance != null)
                    SceneManagerController.Instance.LoadTestCombat();
                break;
            case ActionType.ReturnToMainMenu:
                if (SceneManagerController.Instance != null)
                    SceneManagerController.Instance.ReturnToMainMenu();
                break;
            case ActionType.ExitGame:
                if (SceneManagerController.Instance != null)
                    SceneManagerController.Instance.ExitGame();
                break;
            case ActionType.StartNewGame:
                if (SceneManagerController.Instance != null)
                    SceneManagerController.Instance.StartNewGame();
                break;
            case ActionType.ContinueGame:
                if (SceneManagerController.Instance != null)
                    SceneManagerController.Instance.ContinueGame();
                break;
            case ActionType.UsePotion:
                if (CombatManager.Instance != null)
                    CombatManager.Instance.OnUsePotionClicked();
                break;
        }
    }
}