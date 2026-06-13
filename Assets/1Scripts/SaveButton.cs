using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public void OnSaveClicked()
    {
        if (PlayerStats.Instance != null)
        {
            SaveSystem.SaveGame(PlayerStats.Instance);
            Debug.Log("Игра сохранена!");
        }
    }
}