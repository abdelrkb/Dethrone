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
    private float lastUsedTime = -float.MaxValue;

    public Skill(string name, float cooldown, Sprite image = null, Sprite miniImage = null)
    {
        this.name = name;
        this.cooldown = cooldown;
        this.image = image;
        this.miniImage = miniImage;
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

