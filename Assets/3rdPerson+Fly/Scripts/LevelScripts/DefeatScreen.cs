using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gère l'écran de défaite.
/// 
/// Setup dans Unity :
/// 1. Créer un Canvas (si pas déjà fait) en Screen Space - Overlay.
/// 2. Sous ce Canvas, créer un Panel nommé "DefeatPanel" :
///    - Image couleur noire, Alpha ~180 (semi-transparent).
/// 3. Dans DefeatPanel, ajouter :
///    - Un TMP_Text "DEFEAT" centré (grand, blanc ou rouge).
///    - Un Button "RetryButton" avec texte "RETRY".
/// 4. Attacher ce script sur un GameObject vide "DefeatScreen".
/// 5. Drag-déposer les références dans l'Inspecteur.
/// 6. Laisser DefeatPanel désactivé par défaut.
/// </summary>
public class DefeatScreen : MonoBehaviour
{
    public static DefeatScreen Instance;

    [Header("Références UI")]
    public GameObject defeatPanel;
    public Button retryButton;
    public Button homeButton;
    public TMP_Text defeatText;

    private PlayerStats playerStats;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (defeatPanel != null)
            defeatPanel.SetActive(false);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);

        if (homeButton != null)
            homeButton.onClick.AddListener(OnHome);
    }

    /// <summary>
    /// Affiche l'écran de défaite et met le jeu en pause.
    /// </summary>
    public void Show(PlayerStats stats)
    {
        ShowWithMessage(stats, "DEFEAT");
    }

    /// <summary>
    /// Affiche l'écran de défaite avec un message personnalisé.
    /// </summary>
    public void ShowWithMessage(PlayerStats stats, string message)
    {
        playerStats = stats;

        if (defeatText != null)
            defeatText.text = message;

        if (defeatPanel != null)
            defeatPanel.SetActive(true);

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayDefeatMusic();

        Time.timeScale = 0f;
    }

    private void OnRetry()
    {
        Time.timeScale = 1f;

        if (defeatPanel != null)
            defeatPanel.SetActive(false);

        if (playerStats != null)
            playerStats.ExecuteRetry();
    }

    private void OnHome()
    {
        if (defeatPanel != null)
            defeatPanel.SetActive(false);

        // Reset complet avant de retourner au menu
        if (playerStats != null)
            playerStats.FullReset();

        if (WaveManager.Instance != null)
            WaveManager.Instance.ReturnToMainMenu();
    }
}
