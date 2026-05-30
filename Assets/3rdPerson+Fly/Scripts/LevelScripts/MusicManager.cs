using UnityEngine;
using System.Collections;

/// <summary>
/// Gère la musique du jeu. Singleton persistant entre les scènes.
/// Attacher ce script sur un GameObject vide appelé "MusicManager" dans la scène principale.
/// Assigner les AudioClips dans l'Inspecteur.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Pistes musicales")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip bossMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    [Header("Paramètres")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    public float fadeDuration = 1.5f;

    private AudioSource audioSource;
    private AudioSource sfxSource;
    private Coroutine fadeCoroutine;
    private Coroutine resumeCoroutine;
    private bool isGamePaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.volume = musicVolume;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = 1f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayGameplayMusic();
    }

    // ──────────────────────────────────────────────────
    // API publique
    // ──────────────────────────────────────────────────

    public void PlayMenuMusic()
    {
        PlayWithFade(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayWithFade(gameplayMusic);
    }

    public void PlayBossMusic()
    {
        PlayWithFade(bossMusic);
    }

    public void PlayVictoryMusic()
    {
        PlayWithFade(victoryMusic, loop: false);
    }

    public void PlayDefeatMusic()
    {
        PlayWithFade(defeatMusic);
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    public void StopMusic()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    public void PauseMusic()
    {
        isGamePaused = true;
        audioSource.Pause();
        sfxSource.Pause();
    }

    public void ResumeMusic()
    {
        isGamePaused = false;
        audioSource.UnPause();
        sfxSource.UnPause();
    }

    /// <summary>
    /// Joue un SFX de skill.
    /// Si cutMusic est true, la musique est mise en pause pendant la durée du SFX puis reprend.
    /// </summary>
    public void PlaySkillSFX(AudioClip clip, bool cutMusic = false)
    {
        if (clip == null) return;

        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();

        if (cutMusic)
        {
            if (resumeCoroutine != null) StopCoroutine(resumeCoroutine);
            resumeCoroutine = StartCoroutine(PauseMusicForSFX(clip.length));
        }
    }

    private IEnumerator PauseMusicForSFX(float duration)
    {
        audioSource.Pause();
        yield return new WaitForSeconds(duration);
        // Ne reprendre la musique que si le jeu n'est pas en pause
        if (!isGamePaused)
            audioSource.UnPause();
    }

    // ──────────────────────────────────────────────────
    // Interne
    // ──────────────────────────────────────────────────

    private void PlayWithFade(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(CrossFade(clip, loop));
    }

    private IEnumerator CrossFade(AudioClip newClip, bool loop = true)
    {
        // Fondu sortant
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.loop = loop;
        audioSource.clip = newClip;
        audioSource.Play();

        // Fondu entrant
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = musicVolume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
    }
}
