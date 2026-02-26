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
    }

    public void ApplyUpgrade(UpgradeDefinitionSO upgrade)
    {
        if (!upgrade) return;

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
