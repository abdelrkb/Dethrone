using UnityEngine;

/// <summary>
/// Skill Mystery Box : spawne 5 boss dans l'arène.
/// Ces boss s'ajoutent aux ennemis vivants de la vague en cours.
/// </summary>
public class MysteryBoxSkill : Skill
{
    private const int BOSS_COUNT = 5;
    private const float COOLDOWN = 30f;

    public MysteryBoxSkill(Sprite image = null, Sprite miniImage = null, AudioClip sfx = null)
        : base("Mystery Box", COOLDOWN, image, miniImage, sfx: sfx, cutMusic: false)
    {
    }

    protected override float sfxVolume => 3f;

    public override void Effect(GameObject target)
    {
        if (!CanUse()) return;

        base.Effect(target);

        if (WaveManager.Instance == null || WaveManager.Instance.bossPrefab == null)
        {
            Debug.LogError("MysteryBoxSkill : WaveManager ou bossPrefab manquant !");
            return;
        }

        Transform[] spawnPoints = WaveManager.Instance.spawnPoints;
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("MysteryBoxSkill : aucun spawnPoint dans WaveManager !");
            return;
        }

        for (int i = 0; i < BOSS_COUNT; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Object.Instantiate(WaveManager.Instance.bossPrefab,
                               spawnPoints[randomIndex].position,
                               Quaternion.identity);
        }

        // Informer le WaveManager des ennemis supplémentaires
        WaveManager.Instance.AddEnemies(BOSS_COUNT);

        Debug.Log($"Mystery Box : {BOSS_COUNT} boss spawnés !");
    }
}
