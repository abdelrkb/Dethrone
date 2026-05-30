using UnityEngine;

/// <summary>
/// Gère le menu pause (touche P).
///
/// Setup dans Unity :
/// 1. Dans ton Canvas, créer un Panel "PausePanel" :
///    - Image noire, Alpha ~160, stretch full screen.
/// 2. Dans PausePanel, ajouter :
///    - TMP_Text "PAUSE" : grand, centré, blanc.
///    - TMP_Text "Press P to resume" : plus petit, centré, blanc, position Y légèrement plus bas.
/// 3. Attacher ce script sur un GameObject vide "PauseMenu".
/// 4. Drag PausePanel dans l'Inspecteur.
/// 5. Laisser PausePanel désactivé par défaut.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("Références UI")]
    public GameObject pausePanel;

    private bool isPaused = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Toggle();
    }

    public void Toggle()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        isPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        if (MusicManager.Instance != null)
            MusicManager.Instance.PauseMusic();
        Time.timeScale = 0f;
    }

    private void Resume()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (MusicManager.Instance != null)
            MusicManager.Instance.ResumeMusic();
        Time.timeScale = 1f;
    }
}
