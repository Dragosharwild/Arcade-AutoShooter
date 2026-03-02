using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCheatMenu : MonoBehaviour
{
    private enum NumberKey
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine
    }

    [Header("Cheat Keys")]
    [SerializeField] private NumberKey setOneSecondRemainingKey = NumberKey.One;
    [SerializeField] private NumberKey killPlayerKey = NumberKey.Two;
    [SerializeField] private NumberKey instantLevelUpKey = NumberKey.Three;
    [SerializeField] private NumberKey addRerollKey = NumberKey.Four;
    [SerializeField] private NumberKey addBanishKey = NumberKey.Five;
    [SerializeField] private NumberKey empowerNextLevelUpKey = NumberKey.Six;

    [Header("Cheat Targets")]
    [SerializeField] private RunManager runManager;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerXp playerXp;
    [SerializeField] private LevelUpUI levelUpUI;

    [Header("Cheat Values")]
    [SerializeField] private int rerollsPerPress = 2;
    [SerializeField] private int banishesPerPress = 2;
    [SerializeField] private int empoweredExtraOptions = 1;

    private void Awake()
    {
        if (!runManager) runManager = FindFirstObjectByType<RunManager>();
        if (!playerHealth) playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (!playerXp) playerXp = FindFirstObjectByType<PlayerXp>();
        if (!levelUpUI) levelUpUI = FindFirstObjectByType<LevelUpUI>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        if (WasPressedThisFrame(setOneSecondRemainingKey))
            runManager?.DebugSetRemainingTime(1f);

        if (WasPressedThisFrame(killPlayerKey))
            playerHealth?.DebugKill();

        if (WasPressedThisFrame(instantLevelUpKey) && playerXp)
            playerXp.AddXp(playerXp.XpToNext);

        if (WasPressedThisFrame(addRerollKey) && levelUpUI)
            levelUpUI.AddRerolls(rerollsPerPress);

        if (WasPressedThisFrame(addBanishKey) && levelUpUI)
            levelUpUI.AddBanishes(banishesPerPress);

        if (WasPressedThisFrame(empowerNextLevelUpKey) && levelUpUI)
            levelUpUI.RegisterEmpoweredLevelUp(empoweredExtraOptions);
    }

    private static bool WasPressedThisFrame(NumberKey numberKey)
    {
        if (Keyboard.current == null)
            return false;

        (Key digitKey, Key numpadKey) = ConvertToInputSystemKeys(numberKey);
        return Keyboard.current[digitKey].wasPressedThisFrame || Keyboard.current[numpadKey].wasPressedThisFrame;
    }

    private static (Key digitKey, Key numpadKey) ConvertToInputSystemKeys(NumberKey numberKey)
    {
        return numberKey switch
        {
            NumberKey.Zero => (Key.Digit0, Key.Numpad0),
            NumberKey.One => (Key.Digit1, Key.Numpad1),
            NumberKey.Two => (Key.Digit2, Key.Numpad2),
            NumberKey.Three => (Key.Digit3, Key.Numpad3),
            NumberKey.Four => (Key.Digit4, Key.Numpad4),
            NumberKey.Five => (Key.Digit5, Key.Numpad5),
            NumberKey.Six => (Key.Digit6, Key.Numpad6),
            NumberKey.Seven => (Key.Digit7, Key.Numpad7),
            NumberKey.Eight => (Key.Digit8, Key.Numpad8),
            NumberKey.Nine => (Key.Digit9, Key.Numpad9),
            _ => (Key.Digit1, Key.Numpad1)
        };
    }
}