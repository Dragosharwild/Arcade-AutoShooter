using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Arena (centered at 0,0)")]
    [SerializeField] private float arenaHalfWidth = 48f;  // 96 / 2
    [SerializeField] private float arenaHalfHeight = 36f; // 72 / 2
    [SerializeField] private float spawnMargin = 2f;      // how far outside walls

    [Header("Spawning")]
    [SerializeField] private float startInterval = 1.0f;
    [SerializeField] private float endInterval = 0.2f;
    [SerializeField] private float rampDuration = 180f;   // 3 minutes

    private float timer;

    private void Update()
    {
        if (!enemyPrefab) return;

        float t = Mathf.Clamp01(Time.time / rampDuration);
        float interval = Mathf.Lerp(startInterval, endInterval, t);

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    private void SpawnOne()
    {
        Vector2 pos = GetSpawnPosOutsideArena();
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }

    private Vector2 GetSpawnPosOutsideArena()
    {
        // Pick a side: 0=Top,1=Bottom,2=Left,3=Right
        int side = Random.Range(0, 4);

        float xMin = -arenaHalfWidth - spawnMargin;
        float xMax = arenaHalfWidth + spawnMargin;
        float yMin = -arenaHalfHeight - spawnMargin;
        float yMax = arenaHalfHeight + spawnMargin;

        return side switch
        {
            0 => new Vector2(Random.Range(-arenaHalfWidth, arenaHalfWidth), yMax), // Top
            1 => new Vector2(Random.Range(-arenaHalfWidth, arenaHalfWidth), yMin), // Bottom
            2 => new Vector2(xMin, Random.Range(-arenaHalfHeight, arenaHalfHeight)), // Left
            _ => new Vector2(xMax, Random.Range(-arenaHalfHeight, arenaHalfHeight)), // Right
        };
    }
}
