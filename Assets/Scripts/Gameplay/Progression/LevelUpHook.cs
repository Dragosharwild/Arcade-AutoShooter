using UnityEngine;

public class LevelUpHook : MonoBehaviour
{
    [SerializeField] private PlayerXp playerXp;
    [SerializeField] private LevelUpUI levelUpUI;

    private void Awake()
    {
        if (!playerXp) playerXp = FindFirstObjectByType<PlayerXp>();
        if (!levelUpUI) levelUpUI = FindFirstObjectByType<LevelUpUI>(FindObjectsInactive.Include);

        if (levelUpUI && levelUpUI.gameObject.activeSelf)
            levelUpUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerXp)
            playerXp.LeveledUp += OnLeveledUp;
    }

    private void OnDisable()
    {
        if (playerXp)
            playerXp.LeveledUp -= OnLeveledUp;
    }

    private void OnLeveledUp(int newLevel)
    {
        if (levelUpUI)
            levelUpUI.Show();
    }
}
