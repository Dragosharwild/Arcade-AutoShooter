using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct UpgradeEffectEntry
{
    [SerializeField] private UpgradeStat stat;
    [SerializeField] private UpgradeOperation operation;
    [SerializeField] private float value;

    public UpgradeEffectEntry(UpgradeStat stat, UpgradeOperation operation, float value)
    {
        this.stat = stat;
        this.operation = operation;
        this.value = value;
    }

    public UpgradeStat Stat => stat;
    public UpgradeOperation Operation => operation;
    public float Value => value;
}

[CreateAssetMenu(menuName = "AutoShooter/Upgrades/Upgrade Definition", fileName = "Upgrade")]
public class UpgradeDefinitionSO : ScriptableObject
{
    [Header("Display")]
    [SerializeField] private string displayName = "+10% Damage";

    [Header("Selection")]
    [SerializeField] private int weight = 1;
    [SerializeField] private UpgradeTag[] tags;

    [Header("Effects")]
    [SerializeField] private List<UpgradeEffectEntry> effects = new();

    [Header("Optional Heal")]
    [SerializeField] private bool healOnApply;
    [SerializeField] private float healAmount = 1f;

    public string Label => displayName;
    public string DisplayName => displayName;
    public int Weight => Mathf.Max(1, weight);
    public IReadOnlyList<UpgradeEffectEntry> Effects => effects;
    public IReadOnlyList<UpgradeTag> Tags => tags;
    public bool HealOnApply => healOnApply;
    public float HealAmount => healAmount;
}
