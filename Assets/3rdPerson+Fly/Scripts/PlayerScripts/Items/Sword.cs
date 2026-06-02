using UnityEngine;

/// <summary>
/// Arme épée avec fbx personnalisé
/// </summary>
public class Sword : Weapon
{
    public Sword(GameObject fbxPrefab = null, Sprite imageSprite = null,
                 Vector3 positionOffset = default, Vector3 rotationOffset = default, AudioClip sfx = null)
        : base("Épée", 10, fbxPrefab, imageSprite, positionOffset, rotationOffset, sfx)
    {
    }

    public override void Effect(GameObject target)
    {
        // Pas d'effet spécifique pour l'épée
    }
}

