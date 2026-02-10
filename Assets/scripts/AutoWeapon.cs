using UnityEngine;

public class AutoWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Projectile projectilePrefab;

    [Header("Weapon Stats")]
    [SerializeField] private float range = 12f;
    [SerializeField] private float cooldown = 0.6f;
    [SerializeField] private float damage = 2f;

    private float timer;

    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        float computedRange = range + (stats ? stats.rangeBonus : 0f);
        float computedCooldown = cooldown * (stats ? stats.cooldownMult : 1f);

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Transform target = EnemyRegistry.GetNearest(transform.position, computedRange);
        if (!target) return;

        FireAt(target);
        timer = computedCooldown;
    }

    private void FireAt(Transform target)
    {
        float computedDamage = damage * (stats ? stats.damageMult : 1f);

        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        Projectile p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        p.Init(dir, computedDamage);
    }
}
