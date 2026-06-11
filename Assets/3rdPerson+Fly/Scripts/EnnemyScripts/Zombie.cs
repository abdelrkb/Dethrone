using UnityEngine;

public class Zombie : Enemy
{
    protected override void Start()
    {
        maxHealth = 80;
        currentHealth = maxHealth;
        damage = 12;
        moveSpeed = 2.5f;
        stoppingDistance = 1.2f;
        attackCooldown = 1.3f;

        base.Start();
    }
}
