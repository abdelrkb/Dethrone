using UnityEngine;

/// <summary>
/// Statistique Gear 2 : force et vitesse x2, mais divise les PV par 2
/// </summary>
public class Gear2Stat : Stat
{
    private const float MULTIPLIER = 2f;

    public Gear2Stat(Sprite image = null) : base("Gear 2", image)
    {
    }

    public override void Effect(GameObject target)
    {
        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.strength = (int)(playerStats.strength * MULTIPLIER);
            playerStats.speed = (int)(playerStats.speed * MULTIPLIER);
            playerStats.currentHealth = Mathf.Max(1, playerStats.currentHealth / 2);
            Debug.Log($"Gear 2: force x2 ({playerStats.strength}), vitesse x2 ({playerStats.speed}), PV /2 ({playerStats.currentHealth})");

            playerStats.UpdateHUDDirectly();
        }

        MoveBehaviour moveBehaviour = target.GetComponent<MoveBehaviour>();
        if (moveBehaviour != null)
        {
            moveBehaviour.walkSpeed *= MULTIPLIER;
            moveBehaviour.runSpeed *= MULTIPLIER;
            moveBehaviour.sprintSpeed *= MULTIPLIER;

            moveBehaviour.RefreshSpeedSeeker();
        }
    }
}
