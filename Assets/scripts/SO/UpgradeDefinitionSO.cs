using UnityEngine;

[CreateAssetMenu(menuName = "AutoShooter/Upgrades/Upgrade Definition", fileName = "Upgrade")]
public class UpgradeDefinitionSO : ScriptableObject
{
    [Header("Display")]
    [SerializeField] private string displayName = "+10% Damage";

    [Header("Effect")]
    [SerializeField] private UpgradeStat stat = UpgradeStat.DamageMultiplier;
    [SerializeField] private UpgradeOperation operation = UpgradeOperation.Multiply;
    [SerializeField] private float value = 1.1f;

    [Header("Optional Heal")]
    [SerializeField] private bool healOnApply;
    [SerializeField] private float healAmount = 1f;

    public string Label => displayName;
    public string DisplayName => displayName;
    public UpgradeStat Stat => stat;
    public UpgradeOperation Operation => operation;
    public float Amount => value;
    public float Value => value;
    public bool HealOnApply => healOnApply;
    public float HealAmount => healAmount;
}
