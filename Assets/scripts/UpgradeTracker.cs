using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTracker : MonoBehaviour
{
    private readonly Dictionary<string, int> counts = new();

    public event Action Changed;

    public void Add(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;

        if (!counts.TryAdd(key, 1))
            counts[key]++;

        Changed?.Invoke();
    }

    public IReadOnlyDictionary<string, int> Snapshot() => counts;
}
