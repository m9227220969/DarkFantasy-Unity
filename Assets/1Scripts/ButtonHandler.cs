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
        ContinueGame
    }

    public ActionType action;

    public void OnButtonClick()
    {
        switch (action)
        {
            case ActionType.ToggleInventory:
                Debug.Log("[ButtonHandler] ToggleInventory clicked");
                if (SceneManagerController.Instance != null)
                {
                    Debug.Log("[ButtonHandler] SceneManagerController.Instance found");
                    SceneManagerController.Instance.ToggleInventoryButton();
                }
                else
                {
                    Debug.LogError("[ButtonHandler] SceneManagerController.Instance is NULL!");
                }
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
        }
    }
}