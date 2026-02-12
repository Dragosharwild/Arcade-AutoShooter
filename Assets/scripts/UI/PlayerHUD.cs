using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text modifiersText;

    private PlayerHealth playerHealth;
    private PlayerStats stats;
    private UpgradeTracker tracker;

    private void Awake()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        stats = FindFirstObjectByType<PlayerStats>();
        tracker = FindFirstObjectByType<UpgradeTracker>();

        if (playerHealth) playerHealth.Changed += Refresh;
        if (tracker) tracker.Changed += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (playerHealth) playerHealth.Changed -= Refresh;
        if (tracker) tracker.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (!playerHealth) return;

        float cur = playerHealth.Current;
        float max = playerHealth.MaxHealth;

        if (healthBar)
        {
            healthBar.value = (max <= 0f) ? 0f : (cur / max);
        }

        if (healthText)
        {
            healthText.text = $"{cur:0.0} / {max:0.0}";
        }

        if (modifiersText)
        {
            modifiersText.text = BuildModifiersText();
        }
    }

    private string BuildModifiersText()
    {
        var sb = new StringBuilder();

        // Always show current effective stats
        if (stats)
        {
            sb.AppendLine("Modifiers:");
            sb.AppendLine($"DMG x{stats.damageMult:0.00}");
            sb.AppendLine($"CD  x{stats.cooldownMult:0.00}");
            sb.AppendLine($"RNG +{stats.rangeBonus:0.0}");
            sb.AppendLine($"MS  x{stats.moveSpeedMult:0.00}");
            sb.AppendLine($"MaxHP +{stats.maxHealthBonus}");
            sb.AppendLine();
        }

        // Then show “picked upgrades” counts
        if (tracker != null)
        {
            sb.AppendLine("Picked:");
            foreach (var kv in tracker.Snapshot())
                sb.AppendLine($"{kv.Key} x{kv.Value}");
        }

        return sb.ToString().TrimEnd();
    }
}
