using UnityEngine;

/// <summary>
/// Arme Sabre Laser : 20 dégâts
/// </summary>
public class LightSaber : Weapon
{
    public LightSaber(GameObject fbxPrefab = null, Sprite imageSprite = null,
                      Vector3 positionOffset = default, Vector3 rotationOffset = default, AudioClip sfx = null)
        : base("Sabre Laser", 20, fbxPrefab, imageSprite, positionOffset, rotationOffset, sfx)
    {
    }

    public override void Effect(GameObject target)
    {
        // Pas d'effet spécifique pour le sabre laser
    }
}
