using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Baseline")]
    [SerializeField] private PlayerModifierCatalogSO modifierCatalog;

    [Header("Runtime Modifiers")]
    public float moveSpeedMult { get; private set; } = 1f;
    public int maxHealthBonus { get; private set; } = 0;
    public float damageMult { get; private set; } = 1f;
    public float cooldownMult { get; private set; } = 1f;
    public float rangeBonus { get; private set; } = 0f;

    public event Action Changed;
    private readonly Dictionary<UpgradeDefinitionSO, int> appliedUpgrades = new();

    private void Awake()
    {
        if (!modifierCatalog)
        {
            Debug.LogError("PlayerStats requires a PlayerModifierCatalogSO reference.", this);
            enabled = false;
            return;
        }

        ResetToBaseline();
    }

    public void ResetToBaseline()
    {
        moveSpeedMult = modifierCatalog.BaseMoveSpeedMult;
        maxHealthBonus = modifierCatalog.BaseMaxHealthBonus;
        damageMult = modifierCatalog.BaseDamageMult;
        cooldownMult = modifierCatalog.BaseCooldownMult;
        rangeBonus = modifierCatalog.BaseRangeBonus;
        appliedUpgrades.Clear();
        Changed?.Invoke();
    }

    public void ApplyPersistedModifiers(
        float persistedMoveSpeedMult,
        int persistedMaxHealthBonus,
        float persistedDamageMult,
        float persistedCooldownMult,
        float persistedRangeBonus)
    {
        moveSpeedMult = persistedMoveSpeedMult;
        maxHealthBonus = persistedMaxHealthBonus;
        damageMult = persistedDamageMult;
        cooldownMult = persistedCooldownMult;
        rangeBonus = persistedRangeBonus;
        Changed?.Invoke();
    }

    public void ApplyUpgrade(UpgradeDefinitionSO upgrade)
    {
        if (!upgrade) return;

        if (!appliedUpgrades.ContainsKey(upgrade))
            appliedUpgrades[upgrade] = 0;
        appliedUpgrades[upgrade]++;

        switch (upgrade.Stat)
        {
            case UpgradeStat.DamageMultiplier:
                damageMult = ApplyFloat(damageMult, upgrade.Operation, upgrade.Value);
                break;
            case UpgradeStat.CooldownMultiplier:
                cooldownMult = ApplyFloat(cooldownMult, upgrade.Operation, upgrade.Value);
                break;
            case UpgradeStat.MoveSpeedMultiplier:
                moveSpeedMult = ApplyFloat(moveSpeedMult, upgrade.Operation, upgrade.Value);
                break;
            case UpgradeStat.RangeBonus:
                rangeBonus = ApplyFloat(rangeBonus, upgrade.Operation, upgrade.Value);
                break;
            case UpgradeStat.MaxHealthBonus:
                maxHealthBonus = ApplyInt(maxHealthBonus, upgrade.Operation, upgrade.Value);
                break;
        }

        Changed?.Invoke();
    }

    public Dictionary<string, int> GetAppliedUpgrades()
    {
        Dictionary<string, int> result = new();

        foreach (var pair in appliedUpgrades)
        {
            if (!pair.Key) continue;

            string key = pair.Key.DisplayName;
            if (!result.ContainsKey(key))
                result[key] = 0;

            result[key] += pair.Value;
        }

        return result;
    }

    private static float ApplyFloat(float current, UpgradeOperation operation, float value)
    {
        return operation == UpgradeOperation.Multiply
            ? current * value
            : current + value;
    }

    private static int ApplyInt(int current, UpgradeOperation operation, float value)
    {
        int roundedValue = Mathf.RoundToInt(value);
        return operation == UpgradeOperation.Multiply
            ? Mathf.RoundToInt(current * value)
            : current + roundedValue;
    }
}
