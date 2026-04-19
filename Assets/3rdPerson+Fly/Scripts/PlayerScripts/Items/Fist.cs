using UnityEngine;

/// <summary>
/// Arme de combat rapproché : les poings (mains vides)
/// </summary>
public class Fist : Weapon
{
    public Fist(Sprite imageSprite = null) 
        : base("Poings", 5, null, imageSprite)
    {
    }

    public override void Effect(GameObject target)
    {
        // Pas d'effet spécifique pour les poings
    }
}

