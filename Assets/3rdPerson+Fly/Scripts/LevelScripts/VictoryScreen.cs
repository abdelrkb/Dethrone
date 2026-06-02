using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'écran de victoire.
/// </summary>
public class VictoryScreen : MonoBehaviour
{
    public static VictoryScreen Instance;

    [Header("Références UI")]
    public GameObject victoryPanel;
    public Button playAgainButton;
    public Button homeButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgain);

        if (homeButton != null)
            homeButton.onClick.AddListener(OnHome);
    }

    /// <summary>
    /// Affiche l'écran de victoire.
    /// </summary>
    public void Show()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void OnPlayAgain()
    {
        Time.timeScale = 1f;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        // Reset complet : stats, skills, arme de base
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
            playerStats.FullReset();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayGameplayMusic();

        WaveManager.Instance.RestartGame();
    }

    private void OnHome()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
            playerStats.FullReset();

        WaveManager.Instance.ReturnToMainMenu();
    }
}
