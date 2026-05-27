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
    protected Animator anim;
    protected EnemyHitbox hitbox;
    protected Color originalColor;
    protected float lastAttackTime = -999f;
    protected float attackDuration = 2f;
    protected float timeEnteredStoppingDistance = -1f;
    protected const float DELAY_BEFORE_FIRST_ATTACK = 0f;
    protected bool isFrozen = false;
    protected bool isAttacking = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }

        anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.applyRootMotion = false;

        hitbox = GetComponentInChildren<EnemyHitbox>();
        if (hitbox != null) hitbox.Init(this);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (player != null && isAttacking)
        {
            Vector3 direction = (player.position - transform.position);
            direction.y = 0f;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);

            if (Time.time >= lastAttackTime + attackDuration)
                OnAttackEnd();
        }

        if (agent != null && agent.isOnNavMesh && player != null && !isFrozen && !isAttacking)
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(player.position);

            // Attaquer si le joueur est à portée
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= stoppingDistance)
            {
                if (timeEnteredStoppingDistance < 0)
                {
                    timeEnteredStoppingDistance = Time.time;
                }

                if (Time.time >= timeEnteredStoppingDistance + DELAY_BEFORE_FIRST_ATTACK &&
                    Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
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
        if (isFrozen) return;

        if (player != null)
        {
            Vector3 direction = (player.position - transform.position);
            direction.y = 0f;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        isAttacking = true;
        if (agent != null) agent.isStopped = true;
        if (anim != null) anim.SetTrigger("attack");
    }

    public virtual void EnableHitbox()
    {
        if (hitbox != null) hitbox.EnableHitbox();
    }

    public virtual void DisableHitbox()
    {
        if (hitbox != null) hitbox.DisableHitbox();
    }

    public virtual void OnAttackEnd()
    {
        isAttacking = false;
        if (agent != null) agent.isStopped = false;
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
        Debug.Log($"[Enemy] Die() appelé — anim={anim}, agent={agent}");

        if (anim != null)
        {
            anim.SetBool("isDead", true);
            if (agent != null) agent.enabled = false;
            Destroy(gameObject, 2f);
        }
        else
        {
            Debug.LogWarning("[Enemy] Pas d'Animator trouvé — destruction immédiate sans anim");
            Destroy(gameObject);
        }
        WaveManager.Instance.EnemyKilled();
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
