using UnityEngine;

/// <summary>
/// Statistique GigaChad : divise la vitesse par 2 mais augmente la force de 30
/// </summary>
public class GigaChadStat : Stat
{
    private const float SPEED_DIVISOR = 2f;
    private const int STRENGTH_BONUS = 30;

    public GigaChadStat(Sprite image = null) : base("GigaChad", image)
    {
    }

    public override void Effect(GameObject target)
    {
        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.speed = (int)(playerStats.speed / SPEED_DIVISOR);
            playerStats.strength += STRENGTH_BONUS;
            Debug.Log($"GigaChad: vitesse divisée par 2 ({playerStats.speed}), force +30 ({playerStats.strength})");

            playerStats.UpdateHUDDirectly();
        }

        MoveBehaviour moveBehaviour = target.GetComponent<MoveBehaviour>();
        if (moveBehaviour != null)
        {
            moveBehaviour.walkSpeed /= SPEED_DIVISOR;
            moveBehaviour.runSpeed /= SPEED_DIVISOR;
            moveBehaviour.sprintSpeed /= SPEED_DIVISOR;

            moveBehaviour.RefreshSpeedSeeker();

            Debug.Log("GigaChad: vitesses MoveBehaviour divisées par 2!");
        }

        Debug.Log($"GigaChad activé sur {target.name}!");
    }
}
