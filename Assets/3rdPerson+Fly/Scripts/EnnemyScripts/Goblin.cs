using UnityEngine;

public class Goblin : Enemy
{
    protected override void Start()
    {
        // Définir les statistiques du Goblin
        maxHealth = 35;
        currentHealth = maxHealth;
        damage = 5;
        moveSpeed = 3f;
        stoppingDistance = 1.2f;
        attackCooldown = 1.5f;

        // Appeler la méthode Start de la classe parente
        base.Start();
    }
}
