using UnityEngine;

public class Boss : Enemy
{
    protected override void Start()
    {
        // Définir les statistiques du Boss
        maxHealth = 200;
        currentHealth = maxHealth;
        damage = 20;
        moveSpeed = 3f;
        stoppingDistance = 1.2f;
        attackCooldown = 1f;        // Attaque plus vite qu'un Goblin (1s vs 1.5s)

        // Appeler la méthode Start de la classe parente
        base.Start();
    }
}
