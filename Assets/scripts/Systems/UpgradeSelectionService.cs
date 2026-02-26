using System.Collections.Generic;
using UnityEngine;

public static class UpgradeSelectionService
{
    public static List<UpgradeDefinitionSO> PickDistinct(IReadOnlyList<UpgradeDefinitionSO> pool, int count)
    {
        List<UpgradeDefinitionSO> selected = new();
        if (pool == null || pool.Count == 0 || count <= 0)
            return selected;

        int targetCount = Mathf.Min(count, pool.Count);
        List<int> usedIndices = new();

        while (selected.Count < targetCount)
        {
            int randomIndex = Random.Range(0, pool.Count);
            if (usedIndices.Contains(randomIndex))
                continue;

            usedIndices.Add(randomIndex);
            selected.Add(pool[randomIndex]);
        }

        return selected;
    }
}
