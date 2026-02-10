using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeedMult = 1f;
    public int maxHealthBonus = 0;

    [Header("Weapon")]
    public float damageMult = 1f;
    public float cooldownMult = 1f; // < 1 means faster
    public float rangeBonus = 0f;
}
