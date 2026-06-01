using UnityEngine;

/// <summary>
/// Skill Infinity Gauntlet : pas de cooldown.
/// - Vague classique (1-9) : tue le joueur avec le message "I am inevitable".
/// - Vague 10 (boss) : tue instantanément le boss et déclenche la victoire.
/// </summary>
public class InfinityGauntletSkill : Skill
{
    public InfinityGauntletSkill(Sprite image = null, Sprite miniImage = null)
        : base("Infinity Gauntlet", cooldown: 5f, image, miniImage)
    {
    }

    public override void Effect(GameObject target)
    {
        // Jouer le SFX sans couper la musique (géré via base si sfx != null)
        base.Effect(target);

        if (WaveManager.Instance == null) return;

        if (WaveManager.Instance.currentWave == 10)
        {
            // Tuer le boss instantanément
            Boss[] bosses = Object.FindObjectsByType<Boss>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var boss in bosses)
            {
                boss.TakeDamage(int.MaxValue / 2);
            }

            Debug.Log("Infinity Gauntlet : le boss est vaincu !");
        }
        else
        {
            // Tuer le joueur avec message "I am inevitable"
            PlayerStats playerStats = target.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                Debug.Log("Infinity Gauntlet : le joueur se sacrifie.");
                playerStats.KillInstant();
            }
        }
    }
}
