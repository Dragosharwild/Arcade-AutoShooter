using UnityEngine;

public class AutoWeapon : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private WeaponDefinitionSO weaponDefinition;

    private float timer;

    private PlayerStats stats;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        if (!weaponDefinition)
        {
            Debug.LogError("AutoWeapon requires a WeaponDefinitionSO reference.", this);
            enabled = false;
            return;
        }

        stats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        float computedRange = weaponDefinition.Range + (stats ? stats.rangeBonus : 0f);
        float computedCooldown = weaponDefinition.Cooldown * (stats ? stats.cooldownMult : 1f);
        computedCooldown = Mathf.Max(0.01f, computedCooldown);

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Transform target = EnemyRegistry.GetNearest(transform.position, computedRange);
        if (!target) return;

        FireAt(target);
        timer = computedCooldown;
    }

    private void FireAt(Transform target)
    {
        Projectile prefabToSpawn = weaponDefinition.ProjectilePrefab;
        if (!prefabToSpawn) return;

        float computedDamage = weaponDefinition.Damage * (stats ? stats.damageMult : 1f);

        Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Projectile p = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        p.Init(dir, computedDamage, stats, playerHealth);
    }
}
