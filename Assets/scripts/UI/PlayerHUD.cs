using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text modifiersText;

    [Header("Face UI")]
    [SerializeField] private SpriteRenderer faceDisplay; // The UI Image component on your Canvas
    [SerializeField] private Sprite superHappyFace; // Assigned to Player_Happy (3 HP)
    [SerializeField] private Sprite happyFace;      // Assigned to Player_Ok (2 HP)
    [SerializeField] private Sprite neutralFace;    // Assigned to Player_Neut.. (1 HP)
    [SerializeField] private Sprite sadFace;        // Assigned to Player_Sad (0 HP)

   

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

        // --- FACE LOGIC ---
        if (faceDisplay != null)
        {
            if (cur >= 3f) faceDisplay.sprite = superHappyFace;
            else if (cur >= 2f) faceDisplay.sprite = happyFace;
            else if (cur >= 1f) faceDisplay.sprite = neutralFace;
            else faceDisplay.sprite = sadFace;
        }
        
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

        // Then show �picked upgrades� counts
        if (tracker != null)
        {
            sb.AppendLine("Picked:");
            foreach (var kv in tracker.Snapshot())
                sb.AppendLine($"{kv.Key} x{kv.Value}");
        }

        return sb.ToString().TrimEnd();
    }
}
