using System.Collections.Generic;
using UnityEngine;

public static class EnemyRegistry
{
    private static readonly List<Transform> enemies = new();

    public static void Register(Transform t)
    {
        if (t != null && !enemies.Contains(t)) enemies.Add(t);
    }

    public static void Unregister(Transform t)
    {
        enemies.Remove(t);
    }

    public static Transform GetNearest(Vector2 from, float maxRange)
    {
        Transform best = null;
        float bestDistSq = maxRange * maxRange;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Transform t = enemies[i];
            if (!t)
            {
                enemies.RemoveAt(i);
                continue;
            }

            float d = ((Vector2)t.position - from).sqrMagnitude;
            if (d < bestDistSq)
            {
                bestDistSq = d;
                best = t;
            }
        }

        return best;
    }
}
