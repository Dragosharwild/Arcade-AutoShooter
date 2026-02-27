using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton that persists player progression across levels.
/// Carries over XP, level, upgrades, and stat multipliers from level to level.
/// </summary>
public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    [SerializeField] private bool persistAcrossScenes = true;

    // Level progression
    private int currentLevelNumber = 1;
    
    // Accumulated stats
    private int accumulatedLevel = 1;
    private int accumulatedXp = 0;
    private Dictionary<UpgradeDefinitionSO, int> accumulatedUpgrades = new();
    
    // Multipliers that carry over
    private float damageMultiplier = 1f;
    private float cooldownMultiplier = 1f;
    private float moveSpeedMultiplier = 1f;
    private int maxHealthBonus = 0;
    private float rangeBonus = 0f;

    // Events
    public event System.Action OnLevelCompleted;
    public event System.Action OnProgressionReset;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
            DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called when a level ends (player survives the timer).
    /// Captures current stats before level transition.
    /// Upgrade selections should be tracked via RegisterUpgradeSelection.
    /// </summary>
    public void OnLevelEnded(PlayerXp playerXp, PlayerStats playerStats)
    {
        if (!playerXp || !playerStats)
            return;

        // Save current level's XP and level
        accumulatedXp += playerXp.Xp;
        accumulatedLevel = playerXp.Level + (currentLevelNumber - 1); // Adjusted cumulative level

        // Persist final runtime totals from this level.
        // Avoid compounding previous progression twice.
        damageMultiplier = playerStats.damageMult;
        cooldownMultiplier = playerStats.cooldownMult;
        moveSpeedMultiplier = playerStats.moveSpeedMult;
        maxHealthBonus = playerStats.maxHealthBonus;
        rangeBonus = playerStats.rangeBonus;

        currentLevelNumber++;
        OnLevelCompleted?.Invoke();

        Debug.Log($"[Progression] Level {currentLevelNumber - 1} completed! " +
                  $"Total upgrades: {accumulatedUpgrades.Count}, " +
                  $"Next level: {currentLevelNumber}");
    }

    /// <summary>
    /// Registers an applied upgrade selection using the SO reference.
    /// Call this when a player picks an upgrade from LevelUpUI.
    /// </summary>
    public void RegisterUpgradeSelection(UpgradeDefinitionSO upgrade, int amount = 1)
    {
        if (!upgrade || amount <= 0)
            return;

        if (!accumulatedUpgrades.ContainsKey(upgrade))
            accumulatedUpgrades[upgrade] = 0;

        accumulatedUpgrades[upgrade] += amount;
    }

    /// <summary>
    /// Applies accumulated progression to the player at the start of a new level.
    /// Call this after PlayerXp and PlayerStats are initialized.
    /// </summary>
    public void ApplyProgressionToPlayer(PlayerXp playerXp, PlayerStats playerStats)
    {
        if (!playerXp || !playerStats)
            return;

        // Apply persisted totals through PlayerStats API.
        playerStats.ApplyPersistedModifiers(
            moveSpeedMultiplier,
            maxHealthBonus,
            damageMultiplier,
            cooldownMultiplier,
            rangeBonus);

        Debug.Log($"[Progression] Applied progression to level {currentLevelNumber}. " +
                  $"Damage Mult: {damageMultiplier:F2}, " +
                  $"Speed Mult: {moveSpeedMultiplier:F2}, " +
                  $"Max HP Bonus: {maxHealthBonus}");
    }

    /// <summary>
    /// Resets progression when starting a fresh run (from main menu).
    /// </summary>
    public void ResetProgression()
    {
        currentLevelNumber = 1;
        accumulatedLevel = 1;
        accumulatedXp = 0;
        accumulatedUpgrades.Clear();
        damageMultiplier = 1f;
        cooldownMultiplier = 1f;
        moveSpeedMultiplier = 1f;
        maxHealthBonus = 0;
        rangeBonus = 0f;

        OnProgressionReset?.Invoke();

        Debug.Log("[Progression] Progression reset for new run.");
    }

    // Getters for UI display
    public int CurrentLevelNumber => currentLevelNumber;
    public int AccumulatedLevel => accumulatedLevel;
    public Dictionary<UpgradeDefinitionSO, int> GetAccumulatedUpgradeDefinitions() => new(accumulatedUpgrades);
    public Dictionary<string, int> GetAccumulatedUpgrades()
    {
        Dictionary<string, int> result = new();

        foreach (var pair in accumulatedUpgrades)
        {
            if (!pair.Key) continue;

            string key = pair.Key.DisplayName;
            if (!result.ContainsKey(key))
                result[key] = 0;

            result[key] += pair.Value;
        }

        return result;
    }
    public float DamageMultiplier => damageMultiplier;
    public float CooldownMultiplier => cooldownMultiplier;
    public float MoveSpeedMultiplier => moveSpeedMultiplier;
    public int MaxHealthBonus => maxHealthBonus;
    public float RangeBonus => rangeBonus;
}