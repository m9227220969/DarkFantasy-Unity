using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public enum Zone { Head, Torso, Legs }
public enum CombatPhase { Init, Phase1_Player, Phase2_AI, Phase3_Armor, Phase4_Damage, Phase5_Result }

public class CombatManager : MonoBehaviour
{
    [Header("Timer")]
    public TextMeshProUGUI timerText;
    private float timeLeft = 15f;
    private bool timerRunning = false;

    [Header("UI Buttons")]
    public Button[] blockButtons;
    public Button[] attackButtons;
    public Button confirmButton;

    [Header("Log and HP")]
    public TextMeshProUGUI combatLogText;
    public Slider playerHPBar;
    public Slider enemyHPBar;

    [Header("Result Screens")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TextMeshProUGUI xpRewardText;

    [Header("Combat Data")]
    public PlayerStats playerStats;
    public int enemyMaxHP = 5;
    public int enemyCurrentHP;
    public int enemyBaseDamage = 2;

    private Zone selectedAttackZone = Zone.Torso;
    private Zone selectedBlockZone = Zone.Torso;
    private bool playerHasConfirmed = false;

    private Zone aiAttackZone;
    private Zone aiBlockZone;
    private CombatPhase currentPhase = CombatPhase.Init;

    private Image[] attackButtonImages;
    private Image[] blockButtonImages;

    private void Start()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("[Combat] PlayerStats.Instance not found!");
            return;
        }

        playerStats = PlayerStats.Instance;

        // CRITICAL: Force stats recalculation at start of combat
        playerStats.RecalculateStats();

        enemyCurrentHP = enemyMaxHP;
        UpdateHPBars();
        combatLogText.text = "Combat started!\n";
        HideResultPanels();
        CacheButtonImages();
        StartPhase1();
    }

    private void Update()
    {
        if (timerRunning && currentPhase == CombatPhase.Phase1_Player)
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

    private void CacheButtonImages()
    {
        attackButtonImages = new Image[attackButtons.Length];
        blockButtonImages = new Image[blockButtons.Length];

        for (int i = 0; i < attackButtons.Length; i++)
            attackButtonImages[i] = attackButtons[i].GetComponent<Image>();

        for (int i = 0; i < blockButtons.Length; i++)
            blockButtonImages[i] = blockButtons[i].GetComponent<Image>();
    }

    // --- PHASES ---
    private void StartPhase1()
    {
        currentPhase = CombatPhase.Phase1_Player;
        timeLeft = 15f;
        timerRunning = true;
        playerHasConfirmed = false;
        EnableButtons(true);
        combatLogText.text += "Your turn: select attack and block zones.\n";
        UpdateButtonHighlights();
    }

    private void OnTimerExpired()
    {
        combatLogText.text += "Time expired! Choice locked.\n";
        ProceedToPhase2();
    }

    public void OnConfirmClicked()
    {
        if (currentPhase != CombatPhase.Phase1_Player || playerHasConfirmed) return;
        playerHasConfirmed = true;
        timerRunning = false;
        combatLogText.text += "Choice confirmed.\n";
        ProceedToPhase2();
    }

    private void ProceedToPhase2()
    {
        currentPhase = CombatPhase.Phase2_AI;
        EnableButtons(false);
        combatLogText.text += "Enemy is choosing...\n";

        aiAttackZone = (Zone)Random.Range(0, 3);
        aiBlockZone = (Zone)Random.Range(0, 3);

        StartCoroutine(DelayPhase(1f, () =>
        {
            combatLogText.text += $"Enemy attacks {GetZoneName(aiAttackZone)}, blocks {GetZoneName(aiBlockZone)}.\n";
            ProceedToPhase3();
        }));
    }

    private void ProceedToPhase3()
    {
        currentPhase = CombatPhase.Phase3_Armor;
        combatLogText.text += "Calculating armor...\n";
        StartCoroutine(DelayPhase(0.5f, ProceedToPhase4));
    }

    private void ProceedToPhase4()
    {
        currentPhase = CombatPhase.Phase4_Damage;
        combatLogText.text += "Calculating damage...\n";

        // 1. Calculate Player Armor in the zone attacked by Enemy
        // GDD 4.3.3: Passive Armor applies to Torso. Active Armor (Shield) applies if Block Zone matches Attack Zone.
        int playerPassive = (aiAttackZone == Zone.Torso) ? playerStats.totalArmorBonus : 0;
        int playerActive = (selectedBlockZone == aiAttackZone) ? playerStats.totalShieldBonus : 0;
        int playerTotalArmor = playerPassive + playerActive;

        // 2. Enemy Armor (Demo value: 0)
        int enemyArmor = 0;

        // 3. Player Damage Calculation
        // GDD 4.3.4: Damage = (Strength + WeaponBonus) - TargetArmor
        int playerBaseAttack = playerStats.strength;
        int playerWeaponBonus = playerStats.totalWeaponDamageBonus;
        int totalPlayerAttack = playerBaseAttack + playerWeaponBonus;

        int damageToEnemy = Mathf.Max(0, totalPlayerAttack - enemyArmor);

        // 4. Enemy Damage Calculation
        int totalEnemyAttack = enemyBaseDamage;
        int damageToPlayer = Mathf.Max(0, totalEnemyAttack - playerTotalArmor);

        // Debug Log to verify math
        Debug.Log($"Player Attack: {playerBaseAttack}(Str) + {playerWeaponBonus}(Wep) = {totalPlayerAttack}");
        Debug.Log($"Enemy Armor: {enemyArmor}");
        Debug.Log($"Damage to Enemy: {damageToEnemy}");

        Debug.Log($"Enemy Attack: {totalEnemyAttack}");
        Debug.Log($"Player Armor: {playerPassive}(Passive) + {playerActive}(Block) = {playerTotalArmor}");
        Debug.Log($"Damage to Player: {damageToPlayer}");

        StartCoroutine(DelayPhase(0.5f, () =>
        {
            combatLogText.text += $"You dealt {damageToEnemy} damage.\n";
            combatLogText.text += $"Enemy dealt {damageToPlayer} damage.\n";
            ApplyDamage(damageToEnemy, damageToPlayer);
        }));
    }

    private void ApplyDamage(int dmgToEnemy, int dmgToPlayer)
    {
        currentPhase = CombatPhase.Phase5_Result;

        enemyCurrentHP -= dmgToEnemy;
        playerStats.TakeDamage(dmgToPlayer);
        UpdateHPBars();

        bool pDead = playerStats.currentHealth <= 0;
        bool eDead = enemyCurrentHP <= 0;

        if (pDead && eDead) ShowLoseScreen("Mutual destruction!");
        else if (pDead) ShowLoseScreen("You died!");
        else if (eDead)
        {
            int xp = enemyMaxHP;
            playerStats.AddXP(xp);
            ShowWinScreen(xp);
        }
        else
        {
            combatLogText.text += "Next turn...\n\n";
            StartCoroutine(DelayPhase(2f, StartPhase1));
        }
    }

    // --- UI & UTILS ---
    private IEnumerator DelayPhase(float delay, System.Action next)
    {
        yield return new WaitForSeconds(delay);
        next?.Invoke();
    }

    private void EnableButtons(bool state)
    {
        foreach (var b in blockButtons) b.interactable = state;
        foreach (var b in attackButtons) b.interactable = state;
        confirmButton.interactable = state;
    }

    private void UpdateHPBars()
    {
        playerHPBar.value = Mathf.Max(0, (float)playerStats.currentHealth / playerStats.maxHealth);
        enemyHPBar.value = Mathf.Max(0, (float)enemyCurrentHP / enemyMaxHP);
    }

    private string GetZoneName(Zone z) => z.ToString();

    public void SelectBlockHead() => SetBlockZone(Zone.Head);
    public void SelectBlockTorso() => SetBlockZone(Zone.Torso);
    public void SelectBlockLegs() => SetBlockZone(Zone.Legs);
    public void SelectAttackHead() => SetAttackZone(Zone.Head);
    public void SelectAttackTorso() => SetAttackZone(Zone.Torso);
    public void SelectAttackLegs() => SetAttackZone(Zone.Legs);

    private void SetAttackZone(Zone zone)
    {
        selectedAttackZone = zone;
        UpdateButtonHighlights();
    }

    private void SetBlockZone(Zone zone)
    {
        selectedBlockZone = zone;
        UpdateButtonHighlights();
    }

    private void UpdateButtonHighlights()
    {
        for (int i = 0; i < attackButtonImages.Length; i++)
        {
            if (attackButtonImages[i] != null)
                attackButtonImages[i].color = ((Zone)i == selectedAttackZone) ? Color.yellow : Color.white;
        }
        for (int i = 0; i < blockButtonImages.Length; i++)
        {
            if (blockButtonImages[i] != null)
                blockButtonImages[i].color = ((Zone)i == selectedBlockZone) ? Color.cyan : Color.white;
        }
    }

    private void HideResultPanels() { if (winPanel) winPanel.SetActive(false); if (losePanel) losePanel.SetActive(false); }

    private void ShowWinScreen(int xp)
    {
        EnableButtons(false); confirmButton.interactable = false;
        if (xpRewardText) xpRewardText.text = $"+{xp} XP";
        if (winPanel) winPanel.SetActive(true);
    }

    private void ShowLoseScreen(string msg)
    {
        combatLogText.text += msg + "\n";
        EnableButtons(false); confirmButton.interactable = false;
        if (losePanel) losePanel.SetActive(true);
    }

    public void ReturnToHub()
    {
        // Просто загружаем сцену, а InventoryManager сам закроет инвентарь
        SceneManager.LoadScene("Hub_Inn");
    }
    public void RestartCombat() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}