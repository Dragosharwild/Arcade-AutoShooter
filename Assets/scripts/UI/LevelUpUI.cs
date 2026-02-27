using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour
{
    [Header("Buttons (Auto Generated)")]
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private Button optionButtonPrefab;

    [Header("Choice Counts")]
    [SerializeField] private int optionsPerLevel = 3;

    [Header("Upgrade Data")]
    [SerializeField] private UpgradePoolSO upgradePool;

    [Header("Autonomy Controls")]
    [SerializeField] private int startingRerolls = 1;
    [SerializeField] private int startingBanishes = 1;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TMP_Text rerollButtonText;
    [SerializeField] private Button banishButton;
    [SerializeField] private TMP_Text banishButtonText;
    [SerializeField] private TMP_Text autonomyHintText;

    private PlayerStats stats;
    private PlayerHealth playerHealth;
    private readonly HashSet<UpgradeDefinitionSO> banishedUpgrades = new();
    private readonly List<UpgradeDefinitionSO> currentPicks = new();
    private readonly List<Button> generatedButtons = new();
    private readonly List<TMP_Text> generatedButtonTexts = new();
    private int skipLevelStacks;
    private int nextLevelExtraOptions;
    private int rerollsRemaining;
    private int banishesRemaining;
    private bool banishArmed;
    private bool isShowing;

    private void Awake()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        rerollsRemaining = Mathf.Max(0, startingRerolls);
        banishesRemaining = Mathf.Max(0, startingBanishes);

        if (rerollButton)
        {
            rerollButton.onClick.RemoveAllListeners();
            rerollButton.onClick.AddListener(TryUseReroll);
        }

        if (banishButton)
        {
            banishButton.onClick.RemoveAllListeners();
            banishButton.onClick.AddListener(ArmBanish);
        }

        if (!upgradePool)
            Debug.LogError("LevelUpUI requires an UpgradePoolSO reference.", this);

        InitializeOptionButtons();
    }

    public void Show()
    {
        isShowing = true;
        banishArmed = false;
        Time.timeScale = 0f;
        gameObject.SetActive(true);

        RefreshChoices();
        RefreshAutonomyUI();
    }

    private void RefreshChoices()
    {
        currentPicks.Clear();

        int choiceCount = GetChoiceCountForCurrentLevelUp();

        List<UpgradeDefinitionSO> sourcePool = GetPool();
        if (sourcePool.Count == 0)
        {
            Hide();
            return;
        }

        UpgradeSelectionContext selectionContext = new(banishedUpgrades, GetSelectionWeightMultiplier());
        currentPicks.AddRange(UpgradeSelectionService.PickDistinct(sourcePool, choiceCount, selectionContext));

        if (currentPicks.Count == 0 && banishedUpgrades.Count > 0)
        {
            banishedUpgrades.Clear();
            currentPicks.AddRange(UpgradeSelectionService.PickDistinct(sourcePool, choiceCount));
        }

        if (currentPicks.Count == 0)
        {
            Hide();
            return;
        }

        Button[] activeButtons = GetActiveOptionButtons();
        TMP_Text[] activeButtonTexts = GetActiveOptionTexts();

        for (int i = 0; i < activeButtons.Length; i++)
        {
            bool hasOption = i < currentPicks.Count;
            activeButtons[i].gameObject.SetActive(hasOption);
            if (!hasOption) continue;

            UpgradeDefinitionSO def = currentPicks[i];
            int idx = i;

            if (idx < activeButtonTexts.Length && activeButtonTexts[idx])
                activeButtonTexts[idx].text = def.DisplayName;

            activeButtons[i].onClick.RemoveAllListeners();
            activeButtons[i].onClick.AddListener(() => OnUpgradeOptionClicked(def));
        }

        nextLevelExtraOptions = 0;
    }

    private void OnUpgradeOptionClicked(UpgradeDefinitionSO selectedUpgrade)
    {
        if (!selectedUpgrade)
            return;

        if (banishArmed)
        {
            TryBanishSelectedUpgrade(selectedUpgrade);
            return;
        }

        Apply(selectedUpgrade);
        Hide();
    }

    private void Hide()
    {
        isShowing = false;
        banishArmed = false;
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

    public void BanishUpgrade(UpgradeDefinitionSO upgrade)
    {
        if (upgrade)
            banishedUpgrades.Add(upgrade);
    }

    public void TryUseReroll()
    {
        if (!isShowing || rerollsRemaining <= 0)
            return;

        rerollsRemaining--;
        banishArmed = false;
        RefreshChoices();
        RefreshAutonomyUI();
    }

    public void ArmBanish()
    {
        if (!isShowing || banishesRemaining <= 0)
            return;

        banishArmed = true;
        RefreshAutonomyUI();
    }

    public void AddRerolls(int amount)
    {
        if (amount <= 0)
            return;

        rerollsRemaining += amount;
        RefreshAutonomyUI();
    }

    public void AddBanishes(int amount)
    {
        if (amount <= 0)
            return;

        banishesRemaining += amount;
        RefreshAutonomyUI();
    }

    public void RegisterSkipLevelBuff()
    {
        skipLevelStacks++;
    }

    public void RegisterEmpoweredLevelUp(int extraOptions = 1)
    {
        if (extraOptions <= 0)
            return;

        nextLevelExtraOptions += extraOptions;
    }

    private float GetSelectionWeightMultiplier()
    {
        if (skipLevelStacks <= 0)
            return 1f;

        float multiplier = 1f + (0.25f * skipLevelStacks);
        skipLevelStacks = 0;
        return multiplier;
    }

    private void TryBanishSelectedUpgrade(UpgradeDefinitionSO selectedUpgrade)
    {
        if (!selectedUpgrade || banishesRemaining <= 0)
            return;

        banishesRemaining--;
        banishArmed = false;
        banishedUpgrades.Add(selectedUpgrade);

        RefreshChoices();
        RefreshAutonomyUI();
    }

    private void RefreshAutonomyUI()
    {
        if (rerollButton)
            rerollButton.interactable = isShowing && rerollsRemaining > 0;
        if (banishButton)
            banishButton.interactable = isShowing && banishesRemaining > 0;

        if (rerollButtonText)
            rerollButtonText.text = $"Reroll ({rerollsRemaining})";
        if (banishButtonText)
            banishButtonText.text = $"Banish ({banishesRemaining})";

        if (autonomyHintText)
            autonomyHintText.text = banishArmed
                ? "Banish is active: click an upgrade to remove it from this run."
                : string.Empty;
    }

    private int GetChoiceCountForCurrentLevelUp()
    {
        int baseChoices = Mathf.Max(1, optionsPerLevel);
        return baseChoices + Mathf.Max(0, nextLevelExtraOptions);
    }

    private void InitializeOptionButtons()
    {
        if (!optionsContainer || !optionButtonPrefab)
        {
            Debug.LogError("LevelUpUI requires optionsContainer and optionButtonPrefab for option generation.", this);
            enabled = false;
            return;
        }

        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);

        generatedButtons.Clear();
        generatedButtonTexts.Clear();
    }

    private void EnsureGeneratedButtonCapacity(int requiredCount)
    {
        requiredCount = Mathf.Max(1, requiredCount);

        while (generatedButtons.Count < requiredCount)
        {
            Button spawnedButton = Instantiate(optionButtonPrefab, optionsContainer);
            TMP_Text spawnedText = spawnedButton.GetComponentInChildren<TMP_Text>(true);
            generatedButtons.Add(spawnedButton);
            generatedButtonTexts.Add(spawnedText);
        }

        for (int i = 0; i < generatedButtons.Count; i++)
            generatedButtons[i].gameObject.SetActive(i < requiredCount);
    }

    private Button[] GetActiveOptionButtons()
    {
        int requiredCount = GetChoiceCountForCurrentLevelUp();
        EnsureGeneratedButtonCapacity(requiredCount);
        return generatedButtons.ToArray();
    }

    private TMP_Text[] GetActiveOptionTexts()
    {
        return generatedButtonTexts.ToArray();
    }
}
