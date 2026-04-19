using UnityEngine;

/// <summary>
/// Classe représentant une arme avec dégâts et un effet spécifique
/// </summary>
public class Weapon
{
    public string name;
    public int damage;
    public GameObject fbxPrefab; // Modèle 3D de l'arme
    public Sprite image; // Image UI pour les upgrades

    public Weapon(string name, int damage, GameObject fbxPrefab = null, Sprite image = null)
    {
        this.name = name;
        this.damage = damage;
        this.fbxPrefab = fbxPrefab;
        this.image = image;
    }

    /// <summary>
    /// Applique l'effet de l'arme. À surcharger dans les classes dérivées.
    /// </summary>
    /// <param name="target">La cible affectée par l'effet</param>
    public virtual void Effect(GameObject target)
    {
        Debug.Log($"Arme {name} applique son effet sur {target.name}");
    }
}


