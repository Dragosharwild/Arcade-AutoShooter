using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyXpDrop : MonoBehaviour
{
    [SerializeField] private XpGem gemPrefab;

    private void Awake()
    {
        GetComponent<Health>().Died += OnDied;
    }

    private void OnDied(Vector2 pos)
    {
        if (!gemPrefab) return;

        XpGem gem = Instantiate(gemPrefab, pos, Quaternion.identity);
    }
}