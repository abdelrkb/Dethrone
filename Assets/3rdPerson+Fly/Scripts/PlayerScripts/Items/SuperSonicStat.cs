using UnityEngine;

/// <summary>
/// Statistique Super Sonic : triple la vitesse du joueur
/// </summary>
public class SuperSonicStat : Stat
{
    private const float SPEED_MULTIPLIER = 3f;

    public SuperSonicStat(Sprite image = null) : base("Super Sonic", image)
    {
    }

    public override void Effect(GameObject target)
    {
        // Modifier PlayerStats
        PlayerStats playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.speed = (int)(playerStats.speed * SPEED_MULTIPLIER);
            Debug.Log($"Vitesse PlayerStats augmentée x3: {playerStats.speed}");
            
            // Mettre à jour le HUD
            playerStats.UpdateHUDDirectly();
        }

        // Modifier MoveBehaviour
        MoveBehaviour moveBehaviour = target.GetComponent<MoveBehaviour>();
        if (moveBehaviour != null)
        {
            moveBehaviour.walkSpeed *= SPEED_MULTIPLIER;
            moveBehaviour.runSpeed *= SPEED_MULTIPLIER;
            moveBehaviour.sprintSpeed *= SPEED_MULTIPLIER;
            
            // Réinitialiser speedSeeker avec les nouvelles valeurs
            moveBehaviour.RefreshSpeedSeeker();
            
            Debug.Log($"Vitesses MoveBehaviour augmentées x3!");
        }

        Debug.Log($"Super Sonic activé sur {target.name}!");
    }
}
