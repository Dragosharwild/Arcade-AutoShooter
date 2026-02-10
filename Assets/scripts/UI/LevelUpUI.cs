using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour
{
    public enum UpgradeType
    {
        DamageUp,
        FireRateUp,
        MoveSpeedUp,
        RangeUp,
        MaxHpUp
    }

    [Serializable]
    public class UpgradeDef
    {
        public UpgradeType type;
        public string label;
    }

    [Header("Buttons (3)")]
    [SerializeField] private Button[] buttons;
    [SerializeField] private TMP_Text[] buttonTexts;

    private readonly List<UpgradeDef> pool = new()
    {
        new UpgradeDef{ type = UpgradeType.DamageUp,   label = "+10% Damage" },
        new UpgradeDef{ type = UpgradeType.FireRateUp, label = "+15% Fire Rate" },
        new UpgradeDef{ type = UpgradeType.MoveSpeedUp,label = "+10% Move Speed" },
        new UpgradeDef{ type = UpgradeType.RangeUp,    label = "+2 Range" },
        new UpgradeDef{ type = UpgradeType.MaxHpUp,    label = "+1 Max HP" },
    };

    private PlayerStats stats;

    private void Awake()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);

        // pick 3 distinct upgrades
        List<int> picks = PickDistinct(pool.Count, 3);

        for (int i = 0; i < 3; i++)
        {
            var def = pool[picks[i]];
            int idx = i;

            buttonTexts[i].text = def.label;

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() =>
            {
                Apply(def.type);
                Hide();
            });
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Apply(UpgradeType type)
    {
        if (!stats) return;

        switch (type)
        {
            case UpgradeType.DamageUp:
                stats.damageMult *= 1.10f;
                break;
            case UpgradeType.FireRateUp:
                stats.cooldownMult *= 0.85f; // lower cooldown = faster
                break;
            case UpgradeType.MoveSpeedUp:
                stats.moveSpeedMult *= 1.10f;
                break;
            case UpgradeType.RangeUp:
                stats.rangeBonus += 2f;
                break;
            case UpgradeType.MaxHpUp:
                stats.maxHealthBonus += 1;
                break;
        }
    }

    private static List<int> PickDistinct(int maxExclusive, int count)
    {
        List<int> indices = new();
        while (indices.Count < count)
        {
            int r = UnityEngine.Random.Range(0, maxExclusive);
            if (!indices.Contains(r)) indices.Add(r);
        }
        return indices;
    }
}
