using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5f;
    public float Current { get; private set; }
    public float Max => maxHealth;

    public event Action Died;

    private void Awake()
    {
        Current = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        Current -= amount;
        if (Current <= 0f)
        {
            Current = 0f;
            Died?.Invoke();
            Destroy(gameObject);
        }
    }
}
