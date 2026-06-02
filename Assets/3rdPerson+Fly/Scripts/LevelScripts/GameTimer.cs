using UnityEngine;
using TMPro;

/// <summary>
/// Mesure le temps de jeu depuis le début jusqu'à la victoire.
/// Sauvegarde le meilleur temps via PlayerPrefs.
/// Affiche le temps en cours et le record dans le HUD.
/// </summary>
public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    private const string RECORD_KEY = "BestTime";

    [Header("HUD")]
    public TMP_Text timerText;
    public TMP_Text recordText;

    private float currentTime = 0f;
    private bool running = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshRecordDisplay();
    }

    void Update()
    {
        if (!running) return;

        currentTime += Time.deltaTime;

        if (timerText != null)
            timerText.text = FormatTime(currentTime);
    }

    /// <summary>Démarre le chrono (appelé au début de la partie).</summary>
    public void StartTimer()
    {
        currentTime = 0f;
        running = true;
    }

    /// <summary>Arrête le chrono et sauvegarde si c'est un record.</summary>
    public void StopAndSave()
    {
        running = false;

        float best = PlayerPrefs.GetFloat(RECORD_KEY, float.MaxValue);
        if (currentTime < best)
        {
            PlayerPrefs.SetFloat(RECORD_KEY, currentTime);
            PlayerPrefs.Save();
        }

        RefreshRecordDisplay();
    }

    /// <summary>Remet le chrono à zéro sans le démarrer.</summary>
    public void ResetTimer()
    {
        running = false;
        currentTime = 0f;
        if (timerText != null)
            timerText.text = FormatTime(0f);
    }

    private void RefreshRecordDisplay()
    {
        if (recordText == null) return;

        float best = PlayerPrefs.GetFloat(RECORD_KEY, -1f);
        if (best < 0f)
            recordText.text = "Record : --:--.-";
        else
            recordText.text = "Record : " + FormatTime(best);
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        int d = Mathf.FloorToInt((seconds * 10f) % 10f);
        return $"{m:00}:{s:00}.{d}";
    }
}
