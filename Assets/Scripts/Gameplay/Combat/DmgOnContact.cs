using UnityEngine;

public class DmgOnContact : MonoBehaviour
{
    [SerializeField] private float damage = 1f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        var playerHealth = collision.collider.GetComponent<PlayerHealth>();
        if (playerHealth) playerHealth.TryTakeDamage(damage);
    }
}
