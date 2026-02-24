using System.Collections.Generic; // NEW: Needed for Lists
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

    [Header("Hearts UI")]
    [SerializeField] private GameObject heartPrefab;    // The Prefab 
    [SerializeField] private Transform heartsContainer; // The Layout Group that holds the hearts
    
    private List<Animator> heartsList = new List<Animator>(); // Keeps track of all spawned hearts
    private float lastKnownHealth;
    private float lastKnownMaxHealth;

    private PlayerHealth playerHealth;
    private PlayerStats stats;
    private UpgradeTracker tracker;

    private void Awake()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        stats = FindFirstObjectByType<PlayerStats>();
        tracker = FindFirstObjectByType<UpgradeTracker>();

        if (playerHealth) 
        {
            playerHealth.Changed += Refresh;
            lastKnownHealth = playerHealth.Current; 
            lastKnownMaxHealth = playerHealth.MaxHealth;
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

        // --- 1. HANDLE MAX HEALTH UPGRADES ---
        // If player max health changed, it needs to update how many hearts exist on screen
        if (max != lastKnownMaxHealth || heartsList.Count == 0)
        {
            AdjustHeartCount(max);
            lastKnownMaxHealth = max;
        }

        // --- 2. HANDLE DAMAGE ANIMATIONS ---
        // We use CeilToInt because your game has floating-point regen (e.g. 2.5 health).
        // This ensures a heart only "breaks" when we drop below a full number (like dropping from 3 to 2).
        int currentFullHearts = Mathf.CeilToInt(cur);
        int previousFullHearts = Mathf.CeilToInt(lastKnownHealth);

        if (currentFullHearts < previousFullHearts)
        {
            // We lost at least one full heart! Trigger the animation on the specific lost heart(s)
            for (int i = previousFullHearts - 1; i >= currentFullHearts; i--)
            {
                if (i >= 0 && i < heartsList.Count)
                {
                    heartsList[i].SetTrigger("TakeDamage");
                }
            }
        }
        else if (currentFullHearts > previousFullHearts)
        {
            // The player healed! Refill the empty hearts
            for (int i = previousFullHearts; i < currentFullHearts; i++)
            {
                if (i >= 0 && i < heartsList.Count)
                {
                    // Bulletproof check before playing the animation
                    if (heartsList[i].isActiveAndEnabled && heartsList[i].runtimeAnimatorController != null)
                    {
                        // Snap the heart back to its full, normal state
                        heartsList[i].Play("Heart_Idle");
                    }
                }
            }
        }

        lastKnownHealth = cur;

        // --- FACE LOGIC ---
        if (faceDisplay != null)
        {
            if (cur >= 3f) faceDisplay.sprite = superHappyFace;
            else if (cur >= 2f) faceDisplay.sprite = happyFace;
            else if (cur >= 1f) faceDisplay.sprite = neutralFace;
            else faceDisplay.sprite = sadFace;
        }
        
        if (healthBar) healthBar.value = (max <= 0f) ? 0f : (cur / max);
        if (healthText) healthText.text = $"{cur:0.0} / {max:0.0}";
        if (modifiersText) modifiersText.text = BuildModifiersText();
    }

    // Spawns more hearts if Player max health increases
    private void AdjustHeartCount(float maxHealth)
    {
        int targetHeartCount = Mathf.CeilToInt(maxHealth);

        // If we need more hearts (e.g., player got an upgrade)
        while (heartsList.Count < targetHeartCount)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartsContainer);
            Animator anim = newHeart.GetComponent<Animator>();
            heartsList.Add(anim);
        }

        // If we have too many hearts (rare, but good practice to handle)
        while (heartsList.Count > targetHeartCount)
        {
            int lastIndex = heartsList.Count - 1;
            Destroy(heartsList[lastIndex].gameObject);
            heartsList.RemoveAt(lastIndex);
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