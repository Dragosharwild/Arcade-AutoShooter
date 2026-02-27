using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct UpgradeSelectionContext
{
    public readonly HashSet<UpgradeDefinitionSO> Banished;
    public readonly float WeightMultiplier;

    public UpgradeSelectionContext(HashSet<UpgradeDefinitionSO> banished, float weightMultiplier = 1f)
    {
        Banished = banished;
        WeightMultiplier = Mathf.Max(0.01f, weightMultiplier);
    }
}

public static class UpgradeSelectionService
{
    public static List<UpgradeDefinitionSO> PickDistinct(IReadOnlyList<UpgradeDefinitionSO> pool, int count)
    {
        return PickDistinct(pool, count, default);
    }

    public static List<UpgradeDefinitionSO> PickDistinct(IReadOnlyList<UpgradeDefinitionSO> pool, int count, UpgradeSelectionContext context)
    {
        List<UpgradeDefinitionSO> selected = new();
        if (pool == null || pool.Count == 0 || count <= 0)
            return selected;

        List<UpgradeDefinitionSO> candidates = pool
            .Where(u => u)
            .Where(u => context.Banished == null || !context.Banished.Contains(u))
            .ToList();

        if (candidates.Count == 0)
            return selected;

        int targetCount = Mathf.Min(count, candidates.Count);
        List<UpgradeDefinitionSO> bag = new(candidates);

        while (selected.Count < targetCount)
        {
            UpgradeDefinitionSO picked = PickWeighted(bag, context.WeightMultiplier);
            if (!picked)
                break;

            selected.Add(picked);
            bag.Remove(picked);
        }

        return selected;
    }

    private static UpgradeDefinitionSO PickWeighted(IReadOnlyList<UpgradeDefinitionSO> source, float contextWeightMultiplier)
    {
        if (source == null || source.Count == 0)
            return null;

        float totalWeight = 0f;
        for (int i = 0; i < source.Count; i++)
            totalWeight += Mathf.Max(1f, source[i].Weight * contextWeightMultiplier);

        float roll = Random.value * totalWeight;
        float running = 0f;

        for (int i = 0; i < source.Count; i++)
        {
            running += Mathf.Max(1f, source[i].Weight * contextWeightMultiplier);
            if (roll <= running)
                return source[i];
        }

        return source[source.Count - 1];
    }
}
