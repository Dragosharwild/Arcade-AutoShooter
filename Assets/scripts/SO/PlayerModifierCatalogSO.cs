using UnityEngine;

[CreateAssetMenu(menuName = "AutoShooter/Modifiers/Player Modifier Catalog", fileName = "PlayerModifierCatalog")]
public class PlayerModifierCatalogSO : ScriptableObject
{
    [Header("Player Defaults")]
    [SerializeField] private float baseMoveSpeedMult = 1f;
    [SerializeField] private int baseMaxHealthBonus = 0;

    [Header("Weapon Defaults")]
    [SerializeField] private float baseDamageMult = 1f;
    [SerializeField] private float baseCooldownMult = 1f;
    [SerializeField] private float baseRangeBonus = 0f;

    public float MoveSpeedMultiplier => baseMoveSpeedMult;
    public int MaxHealthBonus => baseMaxHealthBonus;
    public float DamageMultiplier => baseDamageMult;
    public float CooldownMultiplier => baseCooldownMult;
    public float RangeBonus => baseRangeBonus;

    public float BaseMoveSpeedMult => baseMoveSpeedMult;
    public int BaseMaxHealthBonus => baseMaxHealthBonus;
    public float BaseDamageMult => baseDamageMult;
    public float BaseCooldownMult => baseCooldownMult;
    public float BaseRangeBonus => baseRangeBonus;
}
