using UnityEngine;

public class LevelUpHook : MonoBehaviour
{
    [SerializeField] private PlayerXp playerXp;
    [SerializeField] private LevelUpUI levelUpUI;

    private void Awake()
    {
        if (!playerXp) playerXp = FindFirstObjectByType<PlayerXp>();
        if (!levelUpUI) levelUpUI = FindFirstObjectByType<LevelUpUI>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        playerXp.LeveledUp += OnLeveledUp;
    }

    private void OnDisable()
    {
        playerXp.LeveledUp -= OnLeveledUp;
    }

    private void OnLeveledUp(int newLevel)
    {
        levelUpUI.Show();
    }
}
