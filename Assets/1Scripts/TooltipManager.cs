using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("UI Elements")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Показать подсказку с данными предмета
    /// </summary>
    public void ShowTooltip(ItemData item, Vector3 position)
    {
        if (tooltipPanel == null || tooltipText == null) return;

        string info = $"<color=yellow><b>{item.itemName}</b></color>\n";
        info += $"<i>{item.type.ToString()}</i>\n\n";

        // ✅ Безопасные текстовые метки вместо эмодзи
        if (item.damageBonus > 0) info += $"<color=#ff5555>[Атака]</color> +{item.damageBonus}\n";
        if (item.armorBonus > 0) info += $"<color=#5588ff>[Броня]</color> +{item.armorBonus}\n";
        if (item.healAmount > 0) info += $"<color=#55ff55>[Лечение]</color> +{item.healAmount} HP\n";

        if (!string.IsNullOrEmpty(item.description))
        {
            info += $"\n<i>\"{item.description}\"</i>";
        }

        tooltipText.text = info;
        tooltipPanel.transform.position = position + new Vector3(25, -25, 0);
        tooltipPanel.SetActive(true);
    }

    /// <summary>
    /// Скрыть подсказку
    /// </summary>
    public void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}