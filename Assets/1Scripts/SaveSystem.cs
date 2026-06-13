using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveGame(PlayerStats stats)
    {
        SaveData data = new SaveData
        {
            currentHealth = stats.currentHealth,
            maxHealth = stats.maxHealth,
            silver = stats.silver,
            currentXP = stats.currentXP,
            level = stats.level,
            strength = stats.strength,
            endurance = stats.endurance
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to: {savePath}");
    }

    public static bool LoadGame(PlayerStats stats)
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found");
            return false;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        stats.currentHealth = data.currentHealth;
        stats.maxHealth = data.maxHealth;
        stats.silver = data.silver;
        stats.currentXP = data.currentXP;
        stats.level = data.level;
        stats.strength = data.strength;
        stats.endurance = data.endurance;
        stats.RecalculateMaxHealth();

        Debug.Log("Game loaded");
        return true;
    }

    public static bool HasSave()
    {
        return File.Exists(savePath);
    }

    [System.Serializable]
    private class SaveData
    {
        public int currentHealth;
        public int maxHealth;
        public int silver;
        public int currentXP;
        public int level;
        public int strength;
        public int endurance;
    }
}