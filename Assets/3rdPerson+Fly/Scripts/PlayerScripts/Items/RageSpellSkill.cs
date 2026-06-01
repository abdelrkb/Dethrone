using UnityEngine;

/// <summary>
/// Skill Rage Spell : pendant 20 secondes, double la vitesse et la force du joueur et des ennemis.
/// Tous les personnages deviennent violets avec des particules violettes.
/// </summary>
public class RageSpellSkill : Skill
{
    private const float COOLDOWN = 60f;
    private const float DURATION = 20f;

    public RageSpellSkill(Sprite image = null, Sprite miniImage = null, AudioClip sfx = null)
        : base("Rage Spell", COOLDOWN, image, miniImage, sfx: sfx, cutMusic: false)
    {
    }

    public override void Effect(GameObject target)
    {
        if (!CanUse()) return;

        base.Effect(target);

        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
            playerStats.ActivateRageSpell(DURATION);
    }
}
