using UnityEngine;
using TMPro;

public class RunManager : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private float runDuration = 180f;

    private float timer;
    private bool runEnded;

    public float TimeRemaining => Mathf.Max(0f, runDuration - timer);

    private PlayerHealth playerHealth;
    private LevelUpUI levelUpUI;

    private void Awake()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        levelUpUI = FindFirstObjectByType<LevelUpUI>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        if (runEnded) return;

        timer += Time.deltaTime;

        if (playerHealth && playerHealth.gameObject.activeSelf == false)
        {
            EndRun(false);
        }

        if (timer >= runDuration)
        {
            EndRun(true);
        }
    }

    private void EndRun(bool won)
    {
        runEnded = true;
        Time.timeScale = 0f;

        if (endPanel)
        {
            endPanel.SetActive(true);
            resultText.text = won ? "YOU SURVIVED!" : "GAME OVER";
        }
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
