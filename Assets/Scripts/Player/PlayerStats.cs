using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public const float CritChanceCap = 0.75f;
    public const float DodgeChanceCap = 0.50f;
    public const float ReflectionCap = 0.75f;
    public const float LifestealCap = 0.50f;

    [Header("Baseline")]
    [SerializeField] private PlayerModifierCatalogSO modifierCatalog;

    [Header("Runtime Modifiers")]
    public float moveSpeedMult { get; private set; } = 1f;
    public int maxHealthBonus { get; private set; } = 0;
    public float damageMult { get; private set; } = 1f;
    public float cooldownMult { get; private set; } = 1f;
    public float rangeBonus { get; private set; } = 0f;
    public float critChance { get; private set; } = 0f;
    public float critDamageBonus { get; private set; } = 0f;
    public float dodgeChance { get; private set; } = 0f;
    public float damageReflection { get; private set; } = 0f;
    public float lifesteal { get; private set; } = 0f;
    public float CritDamageMultiplier => 2f + critDamageBonus;

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
        critChance = modifierCatalog.BaseCritChance;
        critDamageBonus = modifierCatalog.BaseCritDamageBonus;
        dodgeChance = modifierCatalog.BaseDodgeChance;
        damageReflection = modifierCatalog.BaseDamageReflection;
        lifesteal = modifierCatalog.BaseLifesteal;
        ApplyStatCaps();
        appliedUpgrades.Clear();
        Changed?.Invoke();
    }

    public void ApplyPersistedModifiers(
        float persistedMoveSpeedMult,
        int persistedMaxHealthBonus,
        float persistedDamageMult,
        float persistedCooldownMult,
        float persistedRangeBonus,
        float persistedCritChance,
        float persistedCritDamageBonus,
        float persistedDodgeChance,
        float persistedDamageReflection,
        float persistedLifesteal)
    {
        moveSpeedMult = persistedMoveSpeedMult;
        maxHealthBonus = persistedMaxHealthBonus;
        damageMult = persistedDamageMult;
        cooldownMult = persistedCooldownMult;
        rangeBonus = persistedRangeBonus;
        critChance = persistedCritChance;
        critDamageBonus = persistedCritDamageBonus;
        dodgeChance = persistedDodgeChance;
        damageReflection = persistedDamageReflection;
        lifesteal = persistedLifesteal;
        ApplyStatCaps();
        Changed?.Invoke();
    }

    public void ApplyUpgrade(UpgradeDefinitionSO upgrade)
    {
        if (!upgrade) return;

        if (!appliedUpgrades.ContainsKey(upgrade))
            appliedUpgrades[upgrade] = 0;
        appliedUpgrades[upgrade]++;

        IReadOnlyList<UpgradeEffectEntry> effects = upgrade.Effects;
        if (effects == null || effects.Count == 0)
            return;

        for (int i = 0; i < effects.Count; i++)
        {
            ApplyEffect(effects[i]);
        }

        ApplyStatCaps();
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

    private void ApplyEffect(UpgradeEffectEntry effect)
    {
        switch (effect.Stat)
        {
            case UpgradeStat.DamageMultiplier:
                damageMult = ApplyFloat(damageMult, effect.Operation, effect.Value);
                break;
            case UpgradeStat.CooldownMultiplier:
                cooldownMult = ApplyFloat(cooldownMult, effect.Operation, effect.Value);
                break;
            case UpgradeStat.MoveSpeedMultiplier:
                moveSpeedMult = ApplyFloat(moveSpeedMult, effect.Operation, effect.Value);
                break;
            case UpgradeStat.RangeBonus:
                rangeBonus = ApplyFloat(rangeBonus, effect.Operation, effect.Value);
                break;
            case UpgradeStat.MaxHealthBonus:
                maxHealthBonus = ApplyInt(maxHealthBonus, effect.Operation, effect.Value);
                break;
            case UpgradeStat.CritChance:
                critChance = ApplyFloat(critChance, effect.Operation, effect.Value);
                break;
            case UpgradeStat.CritDamageBonus:
                critDamageBonus = ApplyFloat(critDamageBonus, effect.Operation, effect.Value);
                break;
            case UpgradeStat.DodgeChance:
                dodgeChance = ApplyFloat(dodgeChance, effect.Operation, effect.Value);
                break;
            case UpgradeStat.DamageReflection:
                damageReflection = ApplyFloat(damageReflection, effect.Operation, effect.Value);
                break;
            case UpgradeStat.Lifesteal:
                lifesteal = ApplyFloat(lifesteal, effect.Operation, effect.Value);
                break;
        }
    }

    private void ApplyStatCaps()
    {
        critChance = Mathf.Clamp(critChance, 0f, CritChanceCap);
        dodgeChance = Mathf.Clamp(dodgeChance, 0f, DodgeChanceCap);
        damageReflection = Mathf.Clamp(damageReflection, 0f, ReflectionCap);
        lifesteal = Mathf.Clamp(lifesteal, 0f, LifestealCap);
        critDamageBonus = Mathf.Max(0f, critDamageBonus);
    }
}
