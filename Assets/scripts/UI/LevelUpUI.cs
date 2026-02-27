using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour
{
    [Header("Buttons (3)")]
    [SerializeField] private Button[] buttons;
    [SerializeField] private TMP_Text[] buttonTexts;
    [SerializeField] private int optionsPerLevel = 3;

    [Header("Upgrade Data")]
    [SerializeField] private UpgradePoolSO upgradePool;

    private PlayerStats stats;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (!upgradePool)
            Debug.LogError("LevelUpUI requires an UpgradePoolSO reference.", this);
    }

    public void Show()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);

        List<UpgradeDefinitionSO> sourcePool = GetPool();
        if (sourcePool.Count == 0)
        {
            Hide();
            return;
        }

        List<UpgradeDefinitionSO> picks = UpgradeSelectionService.PickDistinct(sourcePool, optionsPerLevel);

        for (int i = 0; i < buttons.Length; i++)
        {
            bool hasOption = i < picks.Count;
            buttons[i].gameObject.SetActive(hasOption);
            if (!hasOption) continue;

            UpgradeDefinitionSO def = picks[i];
            int idx = i;

            if (idx < buttonTexts.Length)
                buttonTexts[idx].text = def.DisplayName;

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() =>
            {
                Apply(def);
                Hide();
            });
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Apply(UpgradeDefinitionSO upgrade)
    {
        if (!stats || !upgrade) return;

        stats.ApplyUpgrade(upgrade);

        if (upgrade.HealOnApply)
            playerHealth?.Heal(upgrade.HealAmount);
    }

    private List<UpgradeDefinitionSO> GetPool()
    {
        if (!upgradePool || upgradePool.Upgrades == null)
            return new List<UpgradeDefinitionSO>();

        return new List<UpgradeDefinitionSO>(upgradePool.Upgrades);
    }
}
