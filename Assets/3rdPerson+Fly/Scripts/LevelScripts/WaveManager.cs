using UnityEngine;
using TMPro;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int currentWave = 1;
    public TMP_Text waveText;

    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;

    public int enemiesPerWave = 10;
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

        StartWave();
    }

    void StartWave()
    {
        waveInProgress = true;

        waveText.text = "Wave : " + currentWave;

        // À la vague 10, spawner le boss uniquement
        if (currentWave == 10)
        {
            enemiesAlive = 1;
            StartCoroutine(SpawnBossWave());
        }
        else
        {
            enemiesAlive = enemiesPerWave;
            StartCoroutine(SpawnWave());
        }
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    public void EnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && waveInProgress)
        {
            waveInProgress = false;
            
            // Vérifier si le boss vient d'être vaincu (wave 10)
            if (currentWave == 10)
            {
                GameWon();
            }
            else
            {
                Invoke(nameof(NextWave), 2f);
            }
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

    IEnumerator SpawnWave()
{
    enemiesAlive = enemiesPerWave;

    for (int i = 0; i < enemiesPerWave; i++)
    {
        SpawnEnemy();
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
/// Appelé quand le boss est vaincu
/// </summary>
public void GameWon()
{
    waveInProgress = false;
    Debug.Log("========== GAME WON ==========");
    Debug.Log("You have defeated the Boss! Congratulations!");
    
    waveText.text = "VICTORY!";
    
    // Arrêter le spawn d'ennemis et désactiver la progression
    Time.timeScale = 0f; // Mettre le jeu en pause (optionnel)
}

/// <summary>
/// Redémarre le jeu à la wave 1 après la mort du joueur
/// </summary>
public void RestartGame()
{
    currentWave = 1;
    waveInProgress = false;
    
    // Détruire tous les ennemis actuels
    Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    foreach (var enemy in enemies)
    {
        Destroy(enemy.gameObject);
    }
    
    // Redémarrer la wave
    StartWave();
}
}