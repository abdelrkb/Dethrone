using UnityEngine;

/// <summary>
/// Statistique One Punch Man : active un flag persistant.
/// Si les Poings sont équipés (maintenant ou plus tard), leurs dégâts passent à 100.
/// </summary>
public class OnePunchManStat : Stat
{
    private const int FIST_DAMAGE = 100;

    public OnePunchManStat(Sprite image = null) : base("One Punch Man", image)
    {
    }

    public override void Effect(GameObject target)
    {
        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        playerStats.hasOnePunchMan = true;
        Debug.Log("One Punch Man: flag activé.");

        // Si Fist déjà équipé, appliquer immédiatement
        PlayerAttack playerAttack = target.GetComponent<PlayerAttack>();
        if (playerAttack != null && playerAttack.GetCurrentWeapon() is Fist fist)
        {
            fist.damage = FIST_DAMAGE;
            Debug.Log($"One Punch Man: Poings déjà équipés, dégâts à {FIST_DAMAGE}!");
        }
    }
}
