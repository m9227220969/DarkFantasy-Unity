using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD_Controller : MonoBehaviour
{
    public PlayerStats playerStats;

    [Header("UI Elements")]
    public Slider hpSlider;               // ⬅️ Заменили Image на Slider
    public TextMeshProUGUI silverText;

    private void Update()
    {
        if (playerStats != null && hpSlider != null && silverText != null)
        {
            UpdateHealthUI();
            UpdateSilverUI();
        }
    }

    void UpdateHealthUI()
    {
        // Slider сам принимает значения от 0 до 1
        hpSlider.value = (float)playerStats.currentHealth / playerStats.maxHealth;
    }

    void UpdateSilverUI()
    {
        silverText.text = "Серебро: " + playerStats.silver.ToString();
    }
}