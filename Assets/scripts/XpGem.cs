using UnityEngine;

public class XpGem : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var xp = other.GetComponent<PlayerXp>();
        if (xp) xp.AddXp(amount);

        Destroy(gameObject);
    }
}
