using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected int maxHealth = 20;
    protected int currentHealth;
    protected int damage = 5;
    protected float moveSpeed = 3f;
    protected float stoppingDistance = 5f;
    protected float attackCooldown = 1.5f;

    protected Transform player;
    protected NavMeshAgent agent;
    protected Renderer rend;
    protected Color originalColor;
    protected float lastAttackTime = 0f;
    protected float timeEnteredStoppingDistance = -1f;
    protected const float DELAY_BEFORE_FIRST_ATTACK = 1f;
    protected bool isFrozen = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (agent != null && agent.isOnNavMesh && player != null && !isFrozen)
        {
            agent.speed = moveSpeed;
            agent.SetDestination(player.position);

            // Attaquer si le joueur est à portée
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= stoppingDistance)
            {
                // Tracker le temps depuis que l'ennemi est devenu proche
                if (timeEnteredStoppingDistance < 0)
                {
                    timeEnteredStoppingDistance = Time.time;
                }

                // Attaquer après le délai initial ET respecter le cooldown
                if (Time.time >= timeEnteredStoppingDistance + DELAY_BEFORE_FIRST_ATTACK &&
                    Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Réinitialiser le délai si le joueur s'éloigne
                timeEnteredStoppingDistance = -1f;
            }
        }
        else if (isFrozen && agent != null && agent.isOnNavMesh)
        {
            // Pendant le gel, continuer à mettre à jour la destination mais sans se déplacer
            agent.speed = 0f;
        }
    }

    /// <summary>
    /// Inflige des dégâts à l'ennemi
    /// </summary>
    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        FlashRed();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// L'ennemi attaque le joueur
    /// </summary>
    protected virtual void AttackPlayer()
    {
        if (isFrozen) return; // Ne pas attaquer si gelé

        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
            Debug.Log($"{gameObject.name} inflige {damage} dégâts au joueur!");
        }
    }

    protected virtual void FlashRed()
    {
        if (rend != null)
        {
            rend.material.color = Color.red;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    protected virtual void ResetColor()
    {
        if (rend != null)
        {
            rend.material.color = originalColor;
        }
    }

    protected virtual void Die()
    {
        WaveManager.Instance.EnemyKilled();
        Destroy(gameObject);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// Modifie la vitesse de déplacement de l'ennemi
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    /// <summary>
    /// Retourne la vitesse actuelle de l'ennemi
    /// </summary>
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    /// <summary>
    /// Gèle l'ennemi (utilisé par ZA WARUDO)
    /// </summary>
    public void Freeze()
    {
        isFrozen = true;
        if (agent != null)
        {
            agent.speed = 0f;
        }
    }

    /// <summary>
    /// Dégèle l'ennemi (utilisé par ZA WARUDO)
    /// </summary>
    public void Unfreeze()
    {
        isFrozen = false;
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    /// <summary>
    /// Vérifie si l'ennemi est gelé
    /// </summary>
    public bool IsFrozen()
    {
        return isFrozen;
    }
}
