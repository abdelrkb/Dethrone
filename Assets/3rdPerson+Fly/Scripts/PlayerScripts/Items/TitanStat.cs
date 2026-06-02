using UnityEngine;

/// <summary>
/// Statistique Titan : redonne tous les PV et double la force
/// </summary>
public class TitanStat : Stat
{
    public TitanStat(Sprite image = null) : base("Titan", image)
    {
    }

    public override void Effect(GameObject target)
    {
        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.currentHealth = playerStats.maxHealth;
            playerStats.strength *= 2;
            Debug.Log($"Titan: PV restaurés ({playerStats.currentHealth}), force x2 ({playerStats.strength})");

            playerStats.UpdateHUDDirectly();
        }
    }
}
