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
    private bool isMusicPausedBySkill = false;

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
        // Ne mettre en pause la musique que si elle n'est pas déjà coupée par un skill
        if (!isMusicPausedBySkill)
            audioSource.Pause();
        sfxSource.Pause();
    }

    public void ResumeMusic()
    {
        isGamePaused = false;
        // Ne reprendre la musique que si elle n'est pas coupée par un skill
        if (!isMusicPausedBySkill)
            audioSource.UnPause();
        sfxSource.UnPause();
    }

    /// <summary>
    /// Joue un SFX de skill.
    /// Si cutMusic est true, la musique est mise en pause pendant la durée du SFX puis reprend.
    /// volume : multiplicateur de volume (>1 possible pour booster le son).
    /// </summary>
    public void PlaySkillSFX(AudioClip clip, bool cutMusic = false, float volume = 1f)
    {
        if (clip == null) return;

        sfxSource.Stop();
        sfxSource.volume = 1f;
        // PlayOneShot permet un volumeScale > 1 pour amplifier le son
        sfxSource.PlayOneShot(clip, volume);

        if (cutMusic)
        {
            if (resumeCoroutine != null) StopCoroutine(resumeCoroutine);
            resumeCoroutine = StartCoroutine(PauseMusicForSFX(clip.length));
        }
    }

    /// <summary>
    /// Arrête le SFX en cours et reprend la musique (utilisé par le skill Star).
    /// </summary>
    public void StopSFXAndResumeMusic()
    {
        if (resumeCoroutine != null) StopCoroutine(resumeCoroutine);
        sfxSource.Stop();
        isMusicPausedBySkill = false;
        if (!isGamePaused)
            audioSource.UnPause();
    }

    private IEnumerator PauseMusicForSFX(float duration)
    {
        isMusicPausedBySkill = true;
        audioSource.Pause();
        yield return new WaitForSeconds(duration);
        isMusicPausedBySkill = false;
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
