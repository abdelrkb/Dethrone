using UnityEngine;

/// <summary>
/// Arme épée avec fbx personnalisé
/// </summary>
public class Sword : Weapon
{
    public Sword(GameObject fbxPrefab = null, Sprite imageSprite = null) 
        : base("Épée", 10, fbxPrefab, imageSprite)
    {
    }

    public override void Effect(GameObject target)
    {
        // Pas d'effet spécifique pour l'épée
    }
}

