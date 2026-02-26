using UnityEngine;

/// <summary>
/// Automatically applies saved progression when a new level starts.
/// Place this script on any GameObject in your game scene.
/// </summary>
public class LevelInitializer : MonoBehaviour
{
    private void Start()
    {
        // Ensure ProgressionManager exists
        if (ProgressionManager.Instance == null)
        {
            var progressionGO = new GameObject("ProgressionManager");
            progressionGO.AddComponent<ProgressionManager>();
        }

        // Get player components
        var playerXp = FindFirstObjectByType<PlayerXp>();
        var playerStats = FindFirstObjectByType<PlayerStats>();

        // Apply saved progression to this level
        if (playerXp && playerStats)
        {
            ProgressionManager.Instance.ApplyProgressionToPlayer(playerXp, playerStats);
        }
    }
}