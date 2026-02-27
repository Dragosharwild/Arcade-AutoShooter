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

    [Header("Cheat Targets")]
    [SerializeField] private RunManager runManager;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerXp playerXp;

    private void Awake()
    {
        if (!runManager) runManager = FindFirstObjectByType<RunManager>();
        if (!playerHealth) playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (!playerXp) playerXp = FindFirstObjectByType<PlayerXp>();
    }

    private void Update()
    {
        if (WasPressedThisFrame(setOneSecondRemainingKey))
            runManager?.DebugSetRemainingTime(1f);

        if (WasPressedThisFrame(killPlayerKey))
            playerHealth?.DebugKill();

        if (WasPressedThisFrame(instantLevelUpKey) && playerXp)
            playerXp.AddXp(playerXp.XpToNext);
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