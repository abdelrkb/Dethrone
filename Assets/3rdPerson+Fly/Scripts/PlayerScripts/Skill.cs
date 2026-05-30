using UnityEngine;

/// <summary>
/// Classe représentant une compétence avec un effet et un cooldown
/// </summary>
public class Skill
{
    public string name;
    public float cooldown; // en secondes
    public Sprite image; // Image UI pour les upgrades (grande version)
    public Sprite miniImage; // Image UI pour les slots HUD (petite version)

    /// <summary>Son joué lors de l'activation du skill (optionnel)</summary>
    public AudioClip sfx;
    /// <summary>Si true, la musique est mise en pause pendant la durée du SFX</summary>
    public bool cutMusic = false;

    private float lastUsedTime = -float.MaxValue;

    public Skill(string name, float cooldown, Sprite image = null, Sprite miniImage = null,
                 AudioClip sfx = null, bool cutMusic = false)
    {
        this.name = name;
        this.cooldown = cooldown;
        this.image = image;
        this.miniImage = miniImage;
        this.sfx = sfx;
        this.cutMusic = cutMusic;
    }

    /// <summary>
    /// Vérifie si la compétence peut être utilisée (cooldown écoulé)
    /// </summary>
    public bool CanUse()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    /// <summary>
    /// Applique l'effet de la compétence
    /// </summary>
    /// <param name="target">La cible affectée par l'effet</param>
    public virtual void Effect(GameObject target)
    {
        if (CanUse())
        {
            lastUsedTime = Time.time;
            if (sfx != null && MusicManager.Instance != null)
                MusicManager.Instance.PlaySkillSFX(sfx, cutMusic);
            Debug.Log($"Compétence {name} applique son effet sur {target.name}");
        }
        else
        {
            float remainingCooldown = (lastUsedTime + cooldown) - Time.time;
            Debug.Log($"Compétence {name} en cooldown pour {remainingCooldown:F1}s");
        }
    }

    /// <summary>
    /// Obtient le temps de cooldown restant
    /// </summary>
    public float GetRemainingCooldown()
    {
        float remaining = (lastUsedTime + cooldown) - Time.time;
        return Mathf.Max(0, remaining);
    }
}

