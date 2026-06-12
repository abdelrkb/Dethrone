using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class WaveData
{
    public int goblinCount;
    public int zombieCount;
    public bool spawnBoss;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int currentWave = 1;
    public TMP_Text waveText;

    public GameObject enemyPrefab;   // Gobelin
    public GameObject zombiePrefab;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;

    [Header("Configuration des vagues")]
    public WaveData[] waves = new WaveData[]
    {
        new WaveData { goblinCount = 1,  zombieCount = 0, spawnBoss = false }, // Vague 1
        new WaveData { goblinCount = 2,  zombieCount = 1, spawnBoss = false }, // Vague 2
        new WaveData { goblinCount = 3,  zombieCount = 1, spawnBoss = false }, // Vague 3
        new WaveData { goblinCount = 3,  zombieCount = 2, spawnBoss = false }, // Vague 4
        new WaveData { goblinCount = 4,  zombieCount = 2, spawnBoss = false }, // Vague 5
        new WaveData { goblinCount = 4,  zombieCount = 3, spawnBoss = false }, // Vague 6
        new WaveData { goblinCount = 5,  zombieCount = 3, spawnBoss = false }, // Vague 7
        new WaveData { goblinCount = 5,  zombieCount = 4, spawnBoss = false }, // Vague 8
        new WaveData { goblinCount = 6,  zombieCount = 4, spawnBoss = false }, // Vague 9
        new WaveData { goblinCount = 0,  zombieCount = 0, spawnBoss = true  }, // Vague 10 - Boss
    };

    private int enemiesAlive = 0;
    private bool waveInProgress = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Cherche l'UpgradeMenu même s'il est désactivé
        if (UpgradeMenu.Instance == null)
        {
            UpgradeMenu upgradeMenuFound = FindFirstObjectByType<UpgradeMenu>(FindObjectsInactive.Include);
            if (upgradeMenuFound != null)
            {
                UpgradeMenu.Instance = upgradeMenuFound;
            }
        }

        // Ne pas démarrer si le menu principal est affiché
        if (MainMenuScreen.Instance == null)
            StartWave();
    }

    /// <summary>
    /// Appelé par MainMenuScreen quand le joueur clique Play.
    /// </summary>
    public void StartGameFromMenu()
    {
        currentWave = 1;
        waveInProgress = false;
        if (GameTimer.Instance != null) GameTimer.Instance.StartTimer();
        StartWave();
    }

    void StartWave()
    {
        waveInProgress = true;
        waveText.text = "Wave : " + currentWave;

        int waveIndex = Mathf.Clamp(currentWave - 1, 0, waves.Length - 1);
        WaveData data = waves[waveIndex];

        if (data.spawnBoss)
        {
            enemiesAlive = 1;
            if (MusicManager.Instance != null) MusicManager.Instance.PlayBossMusic();
            StartCoroutine(SpawnBossWave());
        }
        else
        {
            enemiesAlive = data.goblinCount + data.zombieCount;
            if (MusicManager.Instance != null) MusicManager.Instance.PlayGameplayMusic();
            StartCoroutine(SpawnWave(data));
        }
    }

    void SpawnEnemy(GameObject prefab)
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(prefab, spawnPoints[randomIndex].position, Quaternion.identity);
}

    public void EnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && waveInProgress)
        {
            waveInProgress = false;

            int waveIndex = Mathf.Clamp(currentWave - 1, 0, waves.Length - 1);
            if (waves[waveIndex].spawnBoss)
                GameWon();
            else
                Invoke(nameof(NextWave), 2f);
        }
    }

void NextWave()
{
    currentWave++;

    // Cherche automatiquement l'UpgradeMenu s'il ne s'est pas encore initialisé
    if (UpgradeMenu.Instance == null)
    {
        UpgradeMenu upgradeMenuFound = FindFirstObjectByType<UpgradeMenu>();
        if (upgradeMenuFound != null)
        {
            UpgradeMenu.Instance = upgradeMenuFound;
        }
    }

    if (UpgradeMenu.Instance != null)
    {
        UpgradeMenu.Instance.OpenMenu();
    }
    else
    {
        Debug.LogError("UpgradeMenu not found in scene! Créez un GameObject avec le script UpgradeMenu attaché.");
    }
}

    IEnumerator SpawnWave(WaveData data)
    {
        // Mélanger l'ordre : goblins puis zombies dans un tableau shufflé
        int total = data.goblinCount + data.zombieCount;
        GameObject[] queue = new GameObject[total];
        for (int i = 0; i < data.goblinCount; i++) queue[i] = enemyPrefab;
        for (int i = data.goblinCount; i < total; i++) queue[i] = zombiePrefab != null ? zombiePrefab : enemyPrefab;

        // Shuffle
        for (int i = total - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (queue[i], queue[j]) = (queue[j], queue[i]);
        }

        for (int i = 0; i < total; i++)
        {
            SpawnEnemy(queue[i]);
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Spawn le boss pour la vague 10
    /// </summary>
    IEnumerator SpawnBossWave()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        if (bossPrefab != null)
        {
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Le Boss aparaît!");
        }
        else
        {
            Debug.LogError("bossPrefab n'est pas assigné dans le WaveManager!");
        }

        yield return null;
    }

public void StartNextWave()
{
    StartWave();
}

/// <summary>
/// Ajoute des ennemis supplémentaires au compteur de la vague en cours (ex: Mystery Box)
/// </summary>
public void AddEnemies(int count)
{
    enemiesAlive += count;
}

/// <summary>
/// Appelé quand le boss est vaincu
/// </summary>
public void GameWon()
{
    waveInProgress = false;
    Debug.Log("========== GAME WON ==========");
    Debug.Log("You have defeated the Boss! Congratulations!");
    
    waveText.text = "VICTORY!";
    if (GameTimer.Instance != null) GameTimer.Instance.StopAndSave();
    if (MusicManager.Instance != null) MusicManager.Instance.PlayVictoryMusic();

    if (VictoryScreen.Instance != null)
        VictoryScreen.Instance.Show();
    else
        Time.timeScale = 0f;
}

/// <summary>
/// Redémarre le jeu à la wave 1 après la mort du joueur
/// </summary>
public void RestartGame()
{
    currentWave = 1;
    waveInProgress = false;
    if (GameTimer.Instance != null) GameTimer.Instance.StartTimer();
    if (MusicManager.Instance != null) MusicManager.Instance.PlayGameplayMusic();
    
    // Détruire tous les ennemis actuels
    Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    foreach (var enemy in enemies)
    {
        Destroy(enemy.gameObject);
    }
    
    // Redémarrer la wave
    StartWave();
}

/// <summary>
/// Retourne au menu principal (affiche le MainMenuScreen dans la scène courante).
/// </summary>
public void ReturnToMainMenu()
{
    currentWave = 1;
    waveInProgress = false;
    if (GameTimer.Instance != null) GameTimer.Instance.ResetTimer();

    // Détruire tous les ennemis
    Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    foreach (var enemy in enemies)
        Destroy(enemy.gameObject);

    // Afficher le menu principal
    if (MainMenuScreen.Instance != null)
        MainMenuScreen.Instance.Show();
}
}