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
    protected Renderer[] allRenderers;
    protected Animator anim;
    protected EnemyHitbox hitbox;
    protected EnemyHealthBar healthBar;
    protected Color originalColor;
    protected float lastAttackTime = -999f;
    protected float attackDuration = 3.833f;
    protected float timeEnteredStoppingDistance = -1f;
    protected const float DELAY_BEFORE_FIRST_ATTACK = 0f;
    protected bool isFrozen = false;
    protected bool isAttacking = false;
    private bool isRaged = false;
    private float baseMoveSpeedForRage;
    private float damageMultiplier = 1f;
    private ParticleSystem rageParticles;
    private bool isDying = false;

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
        allRenderers = GetComponentsInChildren<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }

        anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.applyRootMotion = false;

        hitbox = GetComponentInChildren<EnemyHitbox>();
        if (hitbox != null) hitbox.Init(this);

        healthBar = GetComponentInChildren<EnemyHealthBar>();
        if (healthBar != null) healthBar.Init(maxHealth);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (isDying) return;

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
        if (isDying) return;
        currentHealth -= amount;
        FlashRed();

        if (healthBar != null) healthBar.SetHealth(currentHealth, maxHealth);

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
        if (agent != null && agent.enabled && agent.isOnNavMesh) agent.isStopped = false;
    }

    protected virtual void FlashRed()
    {
        if (allRenderers != null)
        {
            foreach (Renderer r in allRenderers)
                r.material.color = Color.red;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    protected virtual void ResetColor()
    {
        if (allRenderers != null)
        {
            Color target = isRaged ? new Color(0.6f, 0f, 1f) : originalColor;
            foreach (Renderer r in allRenderers)
                r.material.color = target;
        }
    }

    protected virtual void Die()
    {
        isDying = true;
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
        return Mathf.RoundToInt(damage * damageMultiplier);
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

    /// <summary>
    /// Active le buff Rage Spell : vitesse et dégâts doublés, couleur violette avec particules.
    /// </summary>
    public void ApplyRageBuff()
    {
        if (isRaged) return;
        isRaged = true;
        baseMoveSpeedForRage = moveSpeed;
        moveSpeed *= 2f;
        damageMultiplier = 2f;
        if (agent != null) agent.speed = moveSpeed;

        if (allRenderers != null)
            foreach (Renderer r in allRenderers)
                r.material.color = new Color(0.6f, 0f, 1f);

        rageParticles = CreateRageParticles();
    }

    /// <summary>
    /// Retire le buff Rage Spell et restaure les valeurs d'origine.
    /// </summary>
    public void RemoveRageBuff()
    {
        if (!isRaged) return;
        isRaged = false;
        moveSpeed = baseMoveSpeedForRage;
        damageMultiplier = 1f;
        if (agent != null) agent.speed = moveSpeed;

        if (allRenderers != null)
            foreach (Renderer r in allRenderers)
                r.material.color = originalColor;

        if (rageParticles != null)
        {
            Destroy(rageParticles.gameObject);
            rageParticles = null;
        }
    }

    private ParticleSystem CreateRageParticles()
    {
        GameObject pGo = new GameObject("RageParticles");
        pGo.transform.SetParent(transform, false);
        pGo.transform.localPosition = Vector3.up;
        ParticleSystem ps = pGo.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = true;
        main.startLifetime = 0.8f;
        main.startSpeed = 1.5f;
        main.startSize = 0.08f;
        main.startColor = new Color(0.6f, 0f, 1f, 1f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 15f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        return ps;
    }
}
