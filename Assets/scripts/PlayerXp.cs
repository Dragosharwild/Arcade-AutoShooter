using UnityEngine;
using System;

public class PlayerXp : MonoBehaviour
{
    [Header("Progression")]
    [SerializeField] private int level = 1;
    [SerializeField] private int xp;
    [SerializeField] private int baseXpToNext = 5;
    [SerializeField] private int xpPerLevel = 3;

    public int Level => level;
    public int Xp => xp;
    public int XpToNext => baseXpToNext + (level - 1) * xpPerLevel;

    public event Action<int> LeveledUp; // new level

    public void AddXp(int amount)
    {
        if (amount <= 0) return;

        xp += amount;

        while (xp >= XpToNext)
        {
            xp -= XpToNext;
            level++;
            LeveledUp?.Invoke(level);
        }
    }
}
