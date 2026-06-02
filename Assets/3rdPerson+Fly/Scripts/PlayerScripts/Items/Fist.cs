using UnityEngine;

/// <summary>
/// Arme de combat rapproché : les poings (mains vides)
/// </summary>
public class Fist : Weapon
{
    public Fist(Sprite imageSprite = null, AudioClip sfx = null)
        : base("Poings", 5, null, imageSprite, default, default, sfx)
    {
    }

    public override void Effect(GameObject target)
    {
        // Pas d'effet spécifique pour les poings
    }
}

