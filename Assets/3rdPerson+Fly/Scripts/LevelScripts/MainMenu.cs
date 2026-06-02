using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gère le menu principal du jeu.
///
/// ══════════════════════════════════════════════════════════
/// SETUP DANS UNITY :
/// ══════════════════════════════════════════════════════════
///
/// 1. CRÉER UNE NOUVELLE SCÈNE nommée "MainMenu" :
///    File > New Scene > Basic (Built-in) ou Empty.
///    Sauvegarder dans Assets/Scenes/MainMenu.unity.
///
/// 2. AJOUTER LA SCÈNE AU BUILD :
///    File > Build Settings > Add Open Scenes.
///    Mettre "MainMenu" en index 0, "ScenePrincipale" en index 1.
///
/// 3. CRÉER UN CANVAS :
///    GameObject > UI > Canvas.
///    - Canvas Scaler : "Scale With Screen Size", Reference 1920×1080, Match 0.5.
///    - Graphic Raycaster : laisser par défaut.
///
/// 4. ARRIÈRE-PLAN (optionnel) :
///    Sous le Canvas, créer un Panel "Background" :
///    - Stretch full screen.
///    - Color noire ou image de fond.
///
/// 5. LOGO (image en haut, centré) :
///    Sous le Canvas, créer un GameObject > UI > Image nommé "Logo".
///    - Anchors : haut-centré (preset top-center).
///    - Pivot : (0.5, 1).
///    - Pos Y : -40 (marge du haut).
///    - Width/Height : selon ton image (ex. 600×200).
///    - Drag ton sprite logo dans "Source Image".
///    - Cocher "Preserve Aspect".
///
/// 6. BOUTONS (centre) :
///    Créer un GameObject vide "Buttons" sous le Canvas :
///    - Anchors : centré (preset middle-center).
///    - Ajouter un composant Vertical Layout Group :
///      Spacing : 30, Child Alignment : Middle Center,
///      Control Child Size Width+Height : true.
///      Preferred Width : 300, Preferred Height : 80 via un Layout Element sur chaque bouton.
///
///    Sous "Buttons", créer deux boutons :
///    a) Button "PlayButton" > TextMeshPro > texte "PLAY"
///    b) Button "CreditsButton" > TextMeshPro > texte "CREDITS"
///
///    Style suggéré : fond blanc/gris, texte noir bold 40pt.
///
/// 7. LOGO UNIVERSITÉ (bas gauche) :
///    Sous le Canvas, créer UI > Image nommé "UniversityLogo".
///    - Anchors : bas-gauche (preset bottom-left).
///    - Pivot : (0, 0).
///    - Pos X : 30, Pos Y : 30.
///    - Width/Height : ex. 200×100.
///    - Drag ton sprite université dans "Source Image".
///    - Cocher "Preserve Aspect".
///
/// 8. ÉCRAN CREDITS (désactivé par défaut) :
///    Sous le Canvas, créer un Panel "CreditsPanel" :
///    - Stretch full screen, fond noir semi-transparent.
///    - Ajouter un TMP_Text "CreditsText" centré.
///    - Ajouter un Button "BackButton" > texte "RETOUR".
///    - Désactiver le panel par défaut (décocher dans l'Inspector).
///
/// 9. GAMEOBJECT MAINMENU :
///    Créer un GameObject vide nommé "MainMenu".
///    Attacher ce script.
///    Drag-déposer dans l'Inspector :
///    - playButton, creditsButton, creditsPanel, backButton.
///
/// 10. MUSIC MANAGER :
///     Si MusicManager est dans la scène de jeu uniquement,
///     ajoute-le aussi dans MainMenu ou crée un prefab DontDestroyOnLoad.
///     Appelle MusicManager.Instance.PlayMenuMusic() dans Start().
/// ══════════════════════════════════════════════════════════
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Boutons principaux")]
    public Button playButton;
    public Button creditsButton;

    [Header("Écran Crédits")]
    public GameObject creditsPanel;
    public Button backButton;

    void Start()
    {
        // Musique du menu
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMenuMusic();

        playButton.onClick.AddListener(OnPlay);
        creditsButton.onClick.AddListener(OnCredits);

        if (backButton != null)
            backButton.onClick.AddListener(OnBack);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    void OnPlay()
    {
        // Charge la scène de jeu (index 1 dans le Build Settings)
        SceneManager.LoadScene(1);
    }

    void OnCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    void OnBack()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}
