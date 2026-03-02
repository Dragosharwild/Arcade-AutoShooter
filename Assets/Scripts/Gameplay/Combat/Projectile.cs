using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;

    private float damage;
    private Vector2 dir;
    private float timer;
    private PlayerStats sourceStats;
    private PlayerHealth sourceHealth;

    public void Init(Vector2 direction, float damageAmount)
    {
        Init(direction, damageAmount, null, null);
    }

    public void Init(Vector2 direction, float damageAmount, PlayerStats stats, PlayerHealth health)
    {
        dir = direction.normalized;
        damage = damageAmount;
        timer = 0f;
        sourceStats = stats;
        sourceHealth = health;

        transform.right = dir;
    }

    private void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var hp = other.GetComponent<Health>();
        if (hp)
        {
            float finalDamage = ResolveFinalDamage(damage);
            hp.TakeDamage(finalDamage);
            TryApplyLifesteal(finalDamage);
        }

        Destroy(gameObject);
    }

    private float ResolveFinalDamage(float baseDamage)
    {
        if (!sourceStats)
            return baseDamage;

        bool isCritical = Random.value <= sourceStats.critChance;
        if (!isCritical)
            return baseDamage;

        return baseDamage * sourceStats.CritDamageMultiplier;
    }

    private void TryApplyLifesteal(float dealtDamage)
    {
        if (!sourceStats || !sourceHealth || dealtDamage <= 0f)
            return;

        float lifestealHeal = dealtDamage * sourceStats.lifesteal;
        if (lifestealHeal <= 0f)
            return;

        sourceHealth.Heal(lifestealHeal);
    }
}
