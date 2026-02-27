using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;

    private readonly List<Animator> heartsList = new();
    private float lastKnownHealth;
    private float lastKnownMaxHealth;

    private PlayerHealth playerHealth;
    private PlayerStats stats;

    private void Awake()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        stats = FindFirstObjectByType<PlayerStats>();

        if (playerHealth)
        {
            playerHealth.Changed += Refresh;
            lastKnownHealth = playerHealth.Current;
            lastKnownMaxHealth = playerHealth.MaxHealth;
        }

        if (stats)
            stats.Changed += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (playerHealth) playerHealth.Changed -= Refresh;
        if (stats) stats.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (!playerHealth) return;

        float cur = playerHealth.Current;
        float max = playerHealth.MaxHealth;

        if (max != lastKnownMaxHealth || heartsList.Count == 0)
        {
            AdjustHeartCount(max);
            lastKnownMaxHealth = max;
        }

        int currentFullHearts = Mathf.CeilToInt(cur);
        int previousFullHearts = Mathf.CeilToInt(lastKnownHealth);

        if (currentFullHearts < previousFullHearts)
        {
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
            for (int i = previousFullHearts; i < currentFullHearts; i++)
            {
                if (i >= 0 && i < heartsList.Count)
                {
                    if (heartsList[i].isActiveAndEnabled && heartsList[i].runtimeAnimatorController != null)
                    {
                        heartsList[i].Play("Heart_Idle");
                    }
                }
            }
        }

        lastKnownHealth = cur;

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

    private void AdjustHeartCount(float maxHealth)
    {
        int targetHeartCount = Mathf.CeilToInt(maxHealth);

        while (heartsList.Count < targetHeartCount)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartsContainer);
            Animator anim = newHeart.GetComponent<Animator>();
            heartsList.Add(anim);
        }

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
            sb.AppendLine($"Crit {stats.critChance * 100f:0.#}%");
            sb.AppendLine($"CritDmg x{stats.CritDamageMultiplier:0.00}");
            sb.AppendLine($"Dodge {stats.dodgeChance * 100f:0.#}%");
            sb.AppendLine($"Reflect {stats.damageReflection * 100f:0.#}%");
            sb.AppendLine($"Lifesteal {stats.lifesteal * 100f:0.#}%");
            Dictionary<string, int> selectedUpgrades = stats.GetAppliedUpgrades();

            if (sb.Length > 0)
                sb.AppendLine();

            sb.AppendLine("Upgrades:");

            if (selectedUpgrades.Count == 0)
            {
                sb.AppendLine("None");
            }
            else
            {
                foreach (var pair in selectedUpgrades.OrderByDescending(p => p.Value).ThenBy(p => p.Key))
                {
                    sb.AppendLine(pair.Value > 1 ? $"{pair.Key} x{pair.Value}" : pair.Key);
                }
            }
        }

        return sb.ToString().TrimEnd();
    }
}