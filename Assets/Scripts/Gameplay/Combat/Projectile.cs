using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;

    private float damage;
    private Vector2 dir;
    private float timer;

    public void Init(Vector2 direction, float damageAmount)
    {
        dir = direction.normalized;
        damage = damageAmount;
        timer = 0f;

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
        if (hp) hp.TakeDamage(damage);

        Destroy(gameObject);
    }
}
