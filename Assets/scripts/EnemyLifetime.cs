using UnityEngine;

public class EnemyLifetime : MonoBehaviour
{
    private void OnEnable() => EnemyRegistry.Register(transform);
    private void OnDisable() => EnemyRegistry.Unregister(transform);
    private void OnDestroy() => EnemyRegistry.Unregister(transform);
}
