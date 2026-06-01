using UnityEngine;

/// <summary>
/// Skill Star : rend le joueur invincible pendant 10 secondes avec clignotement multicolore.
/// Coupe la musique en cours et joue le SFX associé.
/// </summary>
public class StarSkill : Skill
{
    private const float INVINCIBILITY_DURATION = 10f;
    private const float COOLDOWN = 60f;

    public StarSkill(Sprite image = null, Sprite miniImage = null, AudioClip sfx = null)
        : base("Star", COOLDOWN, image, miniImage, sfx: sfx, cutMusic: true)
    {
    }

    public override void Effect(GameObject target)
    {
        if (!CanUse()) return;

        base.Effect(target);

        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.ActivateStar(INVINCIBILITY_DURATION);
            Debug.Log("Star activée : invincibilité 10s !");
        }
    }
}
