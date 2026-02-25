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
    private Dictionary<string, int> accumulatedUpgrades = new();
    
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
    /// </summary>
    public void OnLevelEnded(PlayerXp playerXp, PlayerStats playerStats, UpgradeTracker upgradeTracker)
    {
        if (!playerXp || !playerStats || !upgradeTracker)
            return;

        // Save current level's XP and level
        accumulatedXp += playerXp.Xp;
        accumulatedLevel = playerXp.Level + (currentLevelNumber - 1); // Adjusted cumulative level

        // Accumulate multipliers
        damageMultiplier *= playerStats.damageMult;
        cooldownMultiplier *= playerStats.cooldownMult;
        moveSpeedMultiplier *= playerStats.moveSpeedMult;
        maxHealthBonus += playerStats.maxHealthBonus;
        rangeBonus += playerStats.rangeBonus;

        // Capture upgrade history
        var upgrades = upgradeTracker.Snapshot();
        foreach (var upgrade in upgrades)
        {
            if (!accumulatedUpgrades.ContainsKey(upgrade.Key))
                accumulatedUpgrades[upgrade.Key] = 0;

            accumulatedUpgrades[upgrade.Key] += upgrade.Value;
        }

        currentLevelNumber++;
        OnLevelCompleted?.Invoke();

        Debug.Log($"[Progression] Level {currentLevelNumber - 1} completed! " +
                  $"Total upgrades: {accumulatedUpgrades.Count}, " +
                  $"Next level: {currentLevelNumber}");
    }

    /// <summary>
    /// Applies accumulated progression to the player at the start of a new level.
    /// Call this after PlayerXp, PlayerStats, and UpgradeTracker are initialized.
    /// </summary>
    public void ApplyProgressionToPlayer(PlayerXp playerXp, PlayerStats playerStats, UpgradeTracker upgradeTracker)
    {
        if (!playerXp || !playerStats || !upgradeTracker)
            return;

        // Apply accumulated multipliers
        playerStats.damageMult = damageMultiplier;
        playerStats.cooldownMult = cooldownMultiplier;
        playerStats.moveSpeedMult = moveSpeedMultiplier;
        playerStats.maxHealthBonus = maxHealthBonus;
        playerStats.rangeBonus = rangeBonus;

        // Restore upgrade history to tracker
        foreach (var upgrade in accumulatedUpgrades)
        {
            for (int i = 0; i < upgrade.Value; i++)
            {
                upgradeTracker.Add(upgrade.Key);
            }
        }

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
    public Dictionary<string, int> GetAccumulatedUpgrades() => new(accumulatedUpgrades);
    public float DamageMultiplier => damageMultiplier;
    public float CooldownMultiplier => cooldownMultiplier;
    public float MoveSpeedMultiplier => moveSpeedMultiplier;
    public int MaxHealthBonus => maxHealthBonus;
    public float RangeBonus => rangeBonus;
}