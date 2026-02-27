using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Base Health")]
    [SerializeField] private float baseMaxHealth = 3f;

    [Header("Damage Rules")]
    [SerializeField] private float invulnSeconds = 0.75f;

    [Header("Regen")]
    [SerializeField] private float regenPerSecond = 0.1f;
    [SerializeField] private float regenDelayAfterHit = 2.0f;

    public float Current { get; private set; }
    public float MaxHealth => baseMaxHealth + (stats ? stats.maxHealthBonus : 0);

    public event Action Changed; // for UI updates
    public event Action Died;

    private float invulnTimer;
    private float regenDelayTimer;

    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        Current = MaxHealth;
        Changed?.Invoke();
    }

    private void Update()
    {
        if (invulnTimer > 0f) invulnTimer -= Time.deltaTime;
        if (regenDelayTimer > 0f) regenDelayTimer -= Time.deltaTime;

        if (regenPerSecond > 0f && regenDelayTimer <= 0f && Current > 0f && Current < MaxHealth)
        {
            Current = Mathf.Min(MaxHealth, Current + regenPerSecond * Time.deltaTime);
            Changed?.Invoke();
        }

        // If max HP increases mid-run, keep Current clamped
        if (Current > MaxHealth)
        {
            Current = MaxHealth;
            Changed?.Invoke();
        }
    }

    public bool TryTakeDamage(float amount, Health attackerHealth = null)
    {
        if (Current <= 0f) return false;
        if (invulnTimer > 0f) return false;
        if (stats && stats.dodgeChance > 0f && UnityEngine.Random.value <= stats.dodgeChance)
            return false;

        Current = Mathf.Max(0f, Current - amount);
        invulnTimer = invulnSeconds;
        regenDelayTimer = regenDelayAfterHit;

        if (stats && attackerHealth && stats.damageReflection > 0f)
        {
            float reflectedDamage = amount * stats.damageReflection;
            if (reflectedDamage > 0f)
                attackerHealth.TakeDamage(reflectedDamage);
        }

        Changed?.Invoke();

        if (Current <= 0f)
        {
            Died?.Invoke();
            gameObject.SetActive(false);
        }

        return true;
    }

    // Call this after a MaxHP upgrade if you want a small heal-on-upgrade
    public void Heal(float amount)
    {
        if (amount <= 0f || Current <= 0f) return;
        Current = Mathf.Min(MaxHealth, Current + amount);
        Changed?.Invoke();
    }

    public void DebugKill()
    {
        if (Current <= 0f) return;

        Current = 0f;
        invulnTimer = 0f;
        regenDelayTimer = regenDelayAfterHit;

        Changed?.Invoke();
        Died?.Invoke();
        gameObject.SetActive(false);
    }
}
