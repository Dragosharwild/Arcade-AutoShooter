using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float invulnSeconds = 0.75f;

    public float Current { get; private set; }

    private float invulnTimer;

    private void Awake()
    {
        Current = maxHealth;
    }

    private void Update()
    {
        if (invulnTimer > 0f)
            invulnTimer -= Time.deltaTime;
    }

    public bool TryTakeDamage(float amount)
    {
        if (invulnTimer > 0f) return false;

        Current -= amount;
        invulnTimer = invulnSeconds;

        if (Current <= 0f)
        {
            Current = 0f;
            // For now: just disable player to prove death works.
            gameObject.SetActive(false);
        }
        return true;
    }
}
