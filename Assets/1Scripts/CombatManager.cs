using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum Zone { Head, Torso, Legs }
public enum CombatPhase { Init, Phase1_Player, Phase2_AI, Phase3_Armor, Phase4_Damage, Phase5_Result }

public class CombatManager : MonoBehaviour
{
    [Header("⏱️ Таймер")]
    public TextMeshProUGUI timerText;
    private float timeLeft = 15f;
    private bool timerRunning = false;

    [Header("🖱️ UI Кнопки")]
    public Button[] blockButtons;
    public Button[] attackButtons;
    public Button confirmButton;

    [Header(" Лог и HP")]
    public TextMeshProUGUI combatLogText;
    public Slider playerHPBar;
    public Slider enemyHPBar;

    [Header("⚔️ Данные боя")]
    public int enemyMaxHP = 5;
    public int enemyCurrentHP;
    public int enemyBaseDamage = 2;

    // Выбор игрока
    private Zone selectedAttackZone = Zone.Torso;
    private Zone selectedBlockZone = Zone.Torso;
    private bool playerHasConfirmed = false;

    // Выбор ИИ
    private Zone aiAttackZone = Zone.Torso;
    private Zone aiBlockZone = Zone.Torso;

    private CombatPhase currentPhase = CombatPhase.Init;

    private PlayerStats playerStats;

    private void Start()
    {
        // Ищет объект PlayerStats в любой активной сцене
        playerStats = FindAnyObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("[Combat] PlayerStats не найден! Убедись, что объект с DontDestroyOnLoad существует.");
            return;
        }

        // Инициализация боя...
        enemyCurrentHP = enemyMaxHP;
        UpdateHPBars();
        StartPhase1();
    }

    private void Update()
    {
        if (timerRunning)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();

            if (timeLeft <= 0)
            {
                timerRunning = false;
                OnTimerExpired();
            }
        }
    }

    // === ФАЗЫ БОЯ ===
    private void StartPhase1()
    {
        currentPhase = CombatPhase.Phase1_Player;
        timeLeft = 15f;
        timerRunning = true;
        playerHasConfirmed = false;
        EnableButtons(true);
        combatLogText.text += "🔹 Ваш ход: выберите зону атаки и защиты.\n";
        UpdateButtonHighlights();
    }

    private void OnTimerExpired()
    {
        if (!playerHasConfirmed)
        {
            combatLogText.text += "⏳ Время вышло! Выбор сброшен.\n";
            // Сброс выбора не требуется, берём текущие selected* как есть
        }
        ProceedToPhase2();
    }

    public void OnConfirmClicked()
    {
        if (currentPhase != CombatPhase.Phase1_Player || playerHasConfirmed) return;
        playerHasConfirmed = true;
        timerRunning = false;
        combatLogText.text += "✅ Выбор подтверждён.\n";
        ProceedToPhase2();
    }

    private void ProceedToPhase2()
    {
        currentPhase = CombatPhase.Phase2_AI;
        EnableButtons(false);
        combatLogText.text += "🤖 Противник выбирает...\n";
        StartCoroutine(DelayPhase(1.5f, () =>
        {
            aiAttackZone = (Zone)Random.Range(0, 3);
            aiBlockZone = (Zone)Random.Range(0, 3);
            combatLogText.text += $"🤖 Враг атакует в {GetZoneName(aiAttackZone)}, блокирует {GetZoneName(aiBlockZone)}.\n";
            ProceedToPhase3();
        }));
    }

    private void ProceedToPhase3()
    {
        currentPhase = CombatPhase.Phase3_Armor;
        // Здесь будет расчёт брони (GDD 4.3.3)
        ProceedToPhase4();
    }

    private void ProceedToPhase4()
    {
        currentPhase = CombatPhase.Phase4_Damage;
        // Здесь будет расчёт урона (GDD 4.3.4)
        ProceedToPhase5();
    }

    private void ProceedToPhase5()
    {
        currentPhase = CombatPhase.Phase5_Result;
        // Здесь будет применение урона и проверка победы/поражения
        // Пока просто запускаем следующий ход для теста
        combatLogText.text += " Следующий ход...\n";
        StartCoroutine(DelayPhase(2f, StartPhase1));
    }

    // === УТИЛИТЫ ===
    private IEnumerator DelayPhase(float delay, System.Action nextAction)
    {
        yield return new WaitForSeconds(delay);
        nextAction?.Invoke();
    }

    private void EnableButtons(bool state)
    {
        foreach (var btn in blockButtons) btn.interactable = state;
        foreach (var btn in attackButtons) btn.interactable = state;
        confirmButton.interactable = state;
    }

    private void UpdateHPBars()
    {
        if (playerStats != null) playerHPBar.value = (float)playerStats.currentHealth / playerStats.maxHealth;
        enemyHPBar.value = (float)enemyCurrentHP / enemyMaxHP;
    }

    private string GetZoneName(Zone z) => z.ToString();

    
    public void SelectAttackHead() => SetAttackZone(Zone.Head);
    public void SelectAttackTorso() => SetAttackZone(Zone.Torso);
    public void SelectAttackLegs() => SetAttackZone(Zone.Legs);

    public void SelectBlockHead() => SetBlockZone(Zone.Head);
    public void SelectBlockTorso() => SetBlockZone(Zone.Torso);
    public void SelectBlockLegs() => SetBlockZone(Zone.Legs);

    private void SetAttackZone(Zone zone)
    {
        selectedAttackZone = zone;
        Debug.Log($"⚔️ Атака: {zone}");
        UpdateButtonHighlights();
    }

    private void SetBlockZone(Zone zone)
    {
        selectedBlockZone = zone;
        Debug.Log($"🛡️ Блок: {zone}");
        UpdateButtonHighlights();
    }

    // Визуальная подсветка выбранных кнопок
    private void UpdateButtonHighlights()
    {
        // Подсветка атаки (жёлтый)
        for (int i = 0; i < attackButtons.Length; i++)
        {
            Image img = attackButtons[i].GetComponent<Image>();
            if (img) img.color = ((Zone)i == selectedAttackZone) ? Color.yellow : Color.white;
        }

        // Подсветка блока (голубой)
        for (int i = 0; i < blockButtons.Length; i++)
        {
            Image img = blockButtons[i].GetComponent<Image>();
            if (img) img.color = ((Zone)i == selectedBlockZone) ? Color.cyan : Color.white;
        }
    }
}