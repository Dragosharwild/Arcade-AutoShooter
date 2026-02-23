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
    [SerializeField] private SpriteRenderer faceDisplay; 
    [SerializeField] private Sprite superHappyFace; 
    [SerializeField] private Sprite happyFace;      
    [SerializeField] private Sprite neutralFace;    
    [SerializeField] private Sprite sadFace;        

    // NEW: Add a reference to your new Heart UI Animator
    [Header("Heart Animation")]
    [SerializeField] private Animator heartAnimator;

    private PlayerHealth playerHealth;
    private PlayerStats stats;
    private UpgradeTracker tracker;

    // NEW: We need to remember the last health amount to know if we took damage
    private float lastKnownHealth;

    private void Awake()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        stats = FindFirstObjectByType<PlayerStats>();
        tracker = FindFirstObjectByType<UpgradeTracker>();

        if (playerHealth) 
        {
            playerHealth.Changed += Refresh;
            // NEW: Initialize lastKnownHealth when the game starts
            lastKnownHealth = playerHealth.Current; 
        }
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

        // NEW: Check if current health is lower than the last time we checked
        if (cur < lastKnownHealth)
        {
            // The player took damage! Trigger the animation.
            if (heartAnimator != null)
            {
                heartAnimator.SetTrigger("TakeDamage");
            }
        }
        
        // NEW: Update the lastKnownHealth for the next time Refresh is called
        lastKnownHealth = cur;

        // --- FACE LOGIC ---
        if (faceDisplay != null)
        {
            if (cur >= 3f) faceDisplay.sprite = superHappyFace;
            else if (cur >= 2f) faceDisplay.sprite = happyFace;
            else if (cur >= 1f) faceDisplay.sprite = neutralFace;
            else faceDisplay.sprite = sadFace;
        }
        
        // Note: You can disable the healthBar game object in the inspector 
        // if you no longer want to see it, and this code will safely skip it!
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

        if (tracker != null)
        {
            sb.AppendLine("Picked:");
            foreach (var kv in tracker.Snapshot())
                sb.AppendLine($"{kv.Key} x{kv.Value}");
        }

        return sb.ToString().TrimEnd();
    }
}