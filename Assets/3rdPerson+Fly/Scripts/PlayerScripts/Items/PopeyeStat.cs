using UnityEngine;

/// <summary>
/// Statistique Popeye : augmente la force de 15
/// </summary>
public class PopeyeStat : Stat
{
    private const int STRENGTH_BONUS = 15;

    public PopeyeStat(Sprite image = null) : base("Popeye", image)
    {
    }

    public override void Effect(GameObject target)
    {
        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.strength += STRENGTH_BONUS;
            Debug.Log($"Popeye: force +15 ({playerStats.strength})");

            playerStats.UpdateHUDDirectly();
        }
    }
}
