using UnityEngine;

/// <summary>
/// Classe représentant une arme avec dégâts et un effet spécifique
/// </summary>
public class Weapon
{
    public string name;
    public int damage;
    public GameObject fbxPrefab;
    public Sprite image;
    public AudioClip sfx;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    public Weapon(string name, int damage, GameObject fbxPrefab = null, Sprite image = null,
                  Vector3 positionOffset = default, Vector3 rotationOffset = default, AudioClip sfx = null)
    {
        this.name = name;
        this.damage = damage;
        this.fbxPrefab = fbxPrefab;
        this.image = image;
        this.positionOffset = positionOffset;
        this.rotationOffset = rotationOffset;
        this.sfx = sfx;
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


