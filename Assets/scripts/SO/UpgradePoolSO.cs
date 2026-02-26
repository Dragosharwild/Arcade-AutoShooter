using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AutoShooter/Upgrades/Upgrade Pool", fileName = "UpgradePool")]
public class UpgradePoolSO : ScriptableObject
{
    [SerializeField] private List<UpgradeDefinitionSO> upgrades = new();

    public IReadOnlyList<UpgradeDefinitionSO> Entries => upgrades;
    public IReadOnlyList<UpgradeDefinitionSO> Upgrades => upgrades;
}
