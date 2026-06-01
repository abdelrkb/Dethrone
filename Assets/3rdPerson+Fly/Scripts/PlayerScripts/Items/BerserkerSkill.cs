using UnityEngine;

/// <summary>
/// Skill Berserker : met le joueur à 1 PV.
/// Tant que le skill est actif, chaque point de dégâts reçu est converti en force
/// au lieu de réduire les HP (les HP restent à 1).
/// </summary>
public class BerserkerSkill : Skill
{
    private const float COOLDOWN = 90f;

    public BerserkerSkill(Sprite image = null, Sprite miniImage = null, AudioClip sfx = null)
        : base("Berserker", COOLDOWN, image, miniImage, sfx: sfx, cutMusic: false)
    {
    }

    public override void Effect(GameObject target)
    {
        if (!CanUse()) return;

        base.Effect(target);

        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
            playerStats.ActivateBerserker();
    }
}
