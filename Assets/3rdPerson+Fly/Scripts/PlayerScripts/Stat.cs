using UnityEngine;

/// <summary>
/// Classe représentant une statistique avec un effet
/// </summary>
public class Stat
{
    public string name;
    public Sprite image; // Image UI pour les upgrades

    public Stat(string name, Sprite image = null)
    {
        this.name = name;
        this.image = image;
    }

    /// <summary>
    /// Applique l'effet de la statistique
    /// </summary>
    /// <param name="target">La cible affectée par l'effet</param>
    public virtual void Effect(GameObject target)
    {
        Debug.Log($"Statistique {name} applique son effet sur {target.name}");
    }
}
