using UnityEngine;

public class Boss : Enemy
{
    protected override void Start()
    {
        // Stats du boss roi — à ajuster quand le skin est prêt
        maxHealth = 300;
        currentHealth = maxHealth;
        damage = 25;
        moveSpeed = 3.5f;
        stoppingDistance = 1.2f;
        attackCooldown = 0.8f;

        base.Start();
    }
}
