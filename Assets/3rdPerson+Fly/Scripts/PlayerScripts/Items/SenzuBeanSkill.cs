using UnityEngine;

/// <summary>
/// Skill Senzu Bean : remet les HP du joueur au maximum.
/// </summary>
public class SenzuBeanSkill : Skill
{
    private const float COOLDOWN = 60f;

    public SenzuBeanSkill(Sprite image = null, Sprite miniImage = null, AudioClip sfx = null)
        : base("Senzu Bean", COOLDOWN, image, miniImage, sfx: sfx, cutMusic: false)
    {
    }

    public override void Effect(GameObject target)
    {
        if (!CanUse()) return;

        base.Effect(target);

        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.currentHealth = playerStats.maxHealth;
            playerStats.UpdateHUDDirectly();
        }
    }
}
