using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Écran de menu principal, affiché au lancement de la scène.
/// Fonctionne comme DefeatScreen/VictoryScreen : panel dans le Canvas de la scène.
///
/// ══════════════════════════════════════════════════════════
/// SETUP DANS UNITY :
/// ══════════════════════════════════════════════════════════
/// 1. Dans ton Canvas existant, créer un Panel "MainMenuPanel" :
///    - Stretch full screen (ancres 0,0 → 1,1).
///    - Image noire ou une couleur de fond.
///    - Laisser ACTIVÉ par défaut (affiché au démarrage).
///
/// 2. Dans MainMenuPanel, créer :
///    a) Image "Logo" (haut, centré) :
///       - Anchors : top-center. Pivot (0.5, 1).
///       - Pos Y : -40. Width 600, Height 200.
///       - Cocher Preserve Aspect. Assigner ton sprite logo.
///
///    b) GameObject vide "Buttons" (centre) :
///       - Anchors : middle-center. Taille 320×210.
///       - Ajouter Vertical Layout Group : spacing 30,
///         Child Alignment Middle Center, Control Width+Height true.
///       - Dedans : Button "PlayButton" (texte "PLAY")
///                  Button "CreditsButton" (texte "CREDITS")
///         Sur chaque bouton, ajouter Layout Element : preferred 300×80.
///
///    c) Image "UniversityLogo" (bas gauche) :
///       - Anchors : bottom-left. Pivot (0,0).
///       - Pos X : 30, Pos Y : 30. Width 220, Height 110.
///       - Cocher Preserve Aspect. Assigner ton sprite université.
///
///    d) Panel "CreditsPanel" (enfant de MainMenuPanel, désactivé) :
///       - Stretch full screen. Fond noir opaque.
///       - TMP_Text "CreditsText" centré avec ton texte.
///       - Button "BackButton" (texte "RETOUR") en bas centré.
///
/// 3. Créer un GameObject vide "MainMenuScreen".
///    Attacher ce script. Drag-déposer les références.
/// ══════════════════════════════════════════════════════════
/// </summary>
public class MainMenuScreen : MonoBehaviour
{
    public static MainMenuScreen Instance;

    [Header("Panel principal")]
    public GameObject mainMenuPanel;

    [Header("Boutons")]
    public Button playButton;
    public Button creditsButton;

    [Header("Crédits")]
    public GameObject creditsPanel;
    public Button backButton;

    [Header("Confirmation quitter")]
    public GameObject quitPanel;
    public Button yesButton;
    public Button noButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCredits);

        if (backButton != null)
            backButton.onClick.AddListener(OnBack);

        if (yesButton != null)
            yesButton.onClick.AddListener(OnQuitYes);

        if (noButton != null)
            noButton.onClick.AddListener(OnQuitNo);

        if (quitPanel != null)
            quitPanel.SetActive(false);

        // Afficher le menu et mettre en pause
        Show();
    }

    void Update()
    {
        // Ouvrir la fenêtre de quitter avec Échap, seulement si le menu est visible
        if (mainMenuPanel != null && mainMenuPanel.activeSelf
            && (creditsPanel == null || !creditsPanel.activeSelf)
            && (quitPanel == null || !quitPanel.activeSelf)
            && Input.GetKeyDown(KeyCode.Escape))
        {
            if (quitPanel != null)
                quitPanel.SetActive(true);
        }
        // Fermer le panel quit avec Échap si déjà ouvert
        else if (quitPanel != null && quitPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            quitPanel.SetActive(false);
        }
    }

    public void Show()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        Time.timeScale = 0f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMenuMusic();
    }

    private void OnPlay()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayGameplayMusic();

        // Démarrer le jeu
        if (WaveManager.Instance != null)
            WaveManager.Instance.StartGameFromMenu();
    }

    private void OnCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    private void OnBack()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    private void OnQuitYes()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnQuitNo()
    {
        if (quitPanel != null)
            quitPanel.SetActive(false);
    }
}
