using UnityEngine;

[CreateAssetMenu(menuName = "AutoShooter/Weapons/Weapon Definition", fileName = "WeaponDefinition")]
public class WeaponDefinitionSO : ScriptableObject
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;

    [Header("Base Stats")]
    [SerializeField] private float range = 12f;
    [SerializeField] private float cooldown = 0.6f;
    [SerializeField] private float damage = 2f;

    public Projectile Projectile => projectilePrefab;
    public float BaseRange => range;
    public float BaseCooldown => cooldown;
    public float BaseDamage => damage;

    public Projectile ProjectilePrefab => projectilePrefab;
    public float Range => range;
    public float Cooldown => cooldown;
    public float Damage => damage;
}
