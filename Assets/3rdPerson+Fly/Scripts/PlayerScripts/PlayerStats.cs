using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public int strength = 1;
    public int speed = 1;

    public Transform healthBar; // Health_Fill
    public TMP_Text strengthText;
    public TMP_Text speedText;

    private Renderer playerRenderer;
    private Color originalColor;
    private Color originalEmission;
    private bool isInvincible = false;
    private Coroutine starCoroutine;
    private bool isRaged = false;
    public bool hasOnePunchMan = false;
    private Coroutine rageCoroutine;
    private ParticleSystem rageParticles;
    
    // Compétences
    private Skill[] skills = new Skill[3];
    public Image[] skillImages = new Image[3]; // Images pour C, V, B
    public TMP_Text[] skillCooldownTexts = new TMP_Text[3]; // Affichage cooldown + lettres (C, V, B)
    private float skillInputBlockedUntil = 0f;

    /// <summary>Bloque les inputs skills pendant <duration> secondes (temps réel).</summary>
    public void BlockSkillInput(float duration)
    {
        skillInputBlockedUntil = Time.unscaledTime + duration;
    }

    void Awake()
    {
        TryResolveHUDReferences();
    }

    void Start()
    {
        currentHealth = maxHealth;
        TryResolveHUDReferences();
        UpdateHUD();

        playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
            originalEmission = playerRenderer.material.GetColor("_EmissionColor");
        }
        
        // Initialiser les textes des touches
        string[] keyLabels = { "C", "V", "B" };
        for (int i = 0; i < 3; i++)
        {
            if (skillCooldownTexts[i] != null)
            {
                skillCooldownTexts[i].text = keyLabels[i];
                skillCooldownTexts[i].transform.SetAsLastSibling();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
        
        // Gérer les inputs des compétences
        bool menuWaiting = UpgradeMenu.Instance != null && UpgradeMenu.Instance.IsWaitingForSlotSelection;
        if (!menuWaiting && Time.unscaledTime >= skillInputBlockedUntil)
        {
            if (Input.GetKeyDown(KeyCode.C) && skills[0] != null)
                UseSkill(0);
            if (Input.GetKeyDown(KeyCode.V) && skills[1] != null)
                UseSkill(1);
            if (Input.GetKeyDown(KeyCode.B) && skills[2] != null)
                UseSkill(2);
        }
        
        // Mettre à jour l'affichage des cooldowns
        UpdateSkillCooldowns();
    }

    void UpdateHUD()
    {
        float hpPercent = (float)currentHealth / maxHealth;

        // Health bar (scale X)
        if (healthBar != null)
            healthBar.localScale = new Vector3(hpPercent, 1, 1);
        else
            Debug.LogWarning("[PlayerStats] healthBar non assignée. Assigne 'Health_Fill' dans l'inspector.");

        if (strengthText != null)
            strengthText.text = "Strength : " + strength;

        if (speedText != null)
            speedText.text = "Speed : " + speed;
    }

    public void UpdateHUDDirectly()
    {
        UpdateHUD();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHUD();
        FlashRed();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void FlashRed()
    {
        if (playerRenderer == null) return;
        StopCoroutine(nameof(FlashRoutine));
        StartCoroutine(nameof(FlashRoutine));
    }

    private IEnumerator FlashRoutine()
    {
        playerRenderer.material.EnableKeyword("_EMISSION");

        float fadeIn = 0.08f;
        float fadeOut = 0.35f;

        // Montée rapide vers le rouge
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeIn;
            playerRenderer.material.color = Color.Lerp(originalColor, Color.red, t);
            playerRenderer.material.SetColor("_EmissionColor", Color.Lerp(originalEmission, Color.red * 4f, t));
            yield return null;
        }

        // Descente douce vers la couleur d'origine
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeOut;
            playerRenderer.material.color = Color.Lerp(Color.red, originalColor, t);
            playerRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.red * 4f, originalEmission, t));
            yield return null;
        }

        Color flashRestore = isRaged ? new Color(0.6f, 0f, 1f) : originalColor;
        Color flashRestoreEmission = isRaged ? new Color(0.6f, 0f, 1f) * 2f : originalEmission;
        playerRenderer.material.color = flashRestore;
        playerRenderer.material.SetColor("_EmissionColor", flashRestoreEmission);
    }

    public void KillInstant()
    {
        currentHealth = 0;
        UpdateHUD();

        if (DefeatScreen.Instance != null)
            DefeatScreen.Instance.Show(this);
        else
            ExecuteRetry();
    }

    /// <summary>
    /// Gère la mort du joueur : affiche l'écran de défaite
    /// </summary>
    private void Die()
    {
        Debug.Log("Le joueur est mort!");

        if (DefeatScreen.Instance != null)
        {
            DefeatScreen.Instance.Show(this);
        }
        else
        {
            // Fallback si DefeatScreen absent
            ExecuteRetry();
        }
    }

    /// <summary>
    /// Appelé par DefeatScreen quand le joueur clique Retry.
    /// Perd un skill, réinitialise la santé et repart à la wave 1.
    /// </summary>
    public void ExecuteRetry()
    {
        // Perdre un skill aléatoire
        LoseRandomSkill();

        // Réinitialiser la santé
        currentHealth = maxHealth;
        UpdateHUD();

        // Redémarrer à la wave 1
        WaveManager.Instance.RestartGame();
    }
    
    /// <summary>
    /// Perd un skill aléatoire parmi ceux équipés
    /// </summary>
    private void LoseRandomSkill()
    {
        // Trouver tous les slots avec un skill
        int[] slotsWithSkills = new int[3];
        int count = 0;
        
        for (int i = 0; i < 3; i++)
        {
            if (skills[i] != null)
            {
                slotsWithSkills[count] = i;
                count++;
            }
        }
        
        // Si au moins un skill est équipé, en perdre un aléatoire
        if (count > 0)
        {
            int randomSlotIndex = Random.Range(0, count);
            int slotToLose = slotsWithSkills[randomSlotIndex];
            
            Debug.Log($"Skill perdu au slot {slotToLose}: {skills[slotToLose].name}");
            
            // Supprimer le skill
            skills[slotToLose] = null;
            
            // Réinitialiser l'affichage du slot
            if (skillImages[slotToLose] != null)
            {
                skillImages[slotToLose].sprite = null;
            }
            if (skillCooldownTexts[slotToLose] != null)
            {
                skillCooldownTexts[slotToLose].text = "CVBX"[slotToLose].ToString(); // C, V, ou B
            }
        }
    }
    
    /// <summary>
    /// Équipe une compétence à un slot
    /// </summary>
    public void EquipSkill(int slotIndex, Skill skill)
    {
        if (slotIndex >= 0 && slotIndex < 3)
        {
            skills[slotIndex] = skill;
            
            // Afficher la mini image de la compétence dans le slot HUD
            if (skillImages[slotIndex] != null && skill.miniImage != null)
            {
                skillImages[slotIndex].sprite = skill.miniImage;
                Debug.Log($"Compétence {skill.name} équipée au slot {slotIndex}");
            }
            else if (skillImages[slotIndex] != null && skill.image != null)
            {
                // Fallback sur l'image complète si miniImage n'est pas définie
                skillImages[slotIndex].sprite = skill.image;
                Debug.Log($"Compétence {skill.name} équipée au slot {slotIndex} (image complète)");
            }
            
            // S'assurer que le texte des touches reste au-dessus de l'image
            if (skillCooldownTexts[slotIndex] != null)
            {
                skillCooldownTexts[slotIndex].transform.SetAsLastSibling();
            }
        }
    }
    
    /// <summary>
    /// Utilise une compétence
    /// </summary>
    private void UseSkill(int slotIndex)
    {
        if (skills[slotIndex] != null && skills[slotIndex].CanUse())
        {
            Skill skill = skills[slotIndex];
            skill.Effect(gameObject);
            Debug.Log($"Compétence utilisée: {skill.name}");
        }
    }
    
    /// <summary>
    /// Met à jour l'affichage des cooldowns
    /// </summary>
    private void UpdateSkillCooldowns()
    {
        string[] keyLabels = { "C", "V", "B" };
        
        for (int i = 0; i < 3; i++)
        {
            if (skillCooldownTexts[i] != null)
            {
                if (skills[i] != null)
                {
                    float remaining = skills[i].GetRemainingCooldown();
                    
                    if (remaining > 0)
                    {
                        // Afficher le cooldown par-dessus la lettre
                        skillCooldownTexts[i].text = remaining.ToString("F1");
                    }
                    else
                    {
                        // Afficher la lettre quand le cooldown est fini
                        skillCooldownTexts[i].text = keyLabels[i];
                    }
                }
                else
                {
                    // Afficher la lettre si pas de compétence équipée
                    skillCooldownTexts[i].text = keyLabels[i];
                }
            }
        }
    }

    /// <summary>
    /// Active l'invincibilité Star pendant une durée donnée avec clignotement multicolore.
    /// </summary>
    public void ActivateStar(float duration)
    {
        if (starCoroutine != null) StopCoroutine(starCoroutine);
        starCoroutine = StartCoroutine(StarRoutine(duration));
    }

    private IEnumerator StarRoutine(float duration)
    {
        isInvincible = true;

        if (playerRenderer != null)
            playerRenderer.material.EnableKeyword("_EMISSION");

        Color[] rainbow = {
            Color.red, new Color(1f, 0.5f, 0f), Color.yellow,
            Color.green, Color.cyan, Color.blue, Color.magenta
        };

        float elapsed = 0f;
        int colorIndex = 0;
        float colorInterval = 0.12f;
        float nextColorTime = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= nextColorTime)
            {
                Color c = rainbow[colorIndex % rainbow.Length];
                if (playerRenderer != null)
                {
                    playerRenderer.material.color = c;
                    playerRenderer.material.SetColor("_EmissionColor", c * 3f);
                }
                colorIndex++;
                nextColorTime = elapsed + colorInterval;
            }

            yield return null;
        }

        // Restaurer couleurs d'origine
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
            playerRenderer.material.SetColor("_EmissionColor", originalEmission);
        }

        isInvincible = false;

        // Arrêter le SFX de l'étoile et reprendre la musique
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopSFXAndResumeMusic();
    }

    private void TryResolveHUDReferences()    {
        if (healthBar == null)
        {
            GameObject healthFillObj = GameObject.Find("Health_Fill");
            if (healthFillObj != null)
                healthBar = healthFillObj.transform;
        }
    }

    /// <summary>
    /// Remet le joueur à zéro complet (après victoire) :
    /// stats de base, HP max, tous les skills vidés, arme de base réequipée.
    /// </summary>
    public void FullReset()
    {
        // Stats de base
        strength = 0;
        speed = 1;
        maxHealth = 100;
        currentHealth = maxHealth;
        hasOnePunchMan = false;

        // Vider tous les skills
        string[] keyLabels = { "C", "V", "B" };
        for (int i = 0; i < 3; i++)
        {
            skills[i] = null;
            if (skillImages[i] != null)
                skillImages[i].sprite = null;
            if (skillCooldownTexts[i] != null)
                skillCooldownTexts[i].text = keyLabels[i];
        }

        UpdateHUD();

        // Réequipper l'arme de base
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
            playerAttack.FullReset();
    }
    /// <summary>
    /// Active le Rage Spell pendant une durée donnée.
    /// Double vitesse + force du joueur et des ennemis, couleur violette + particules.
    /// </summary>
    public void ActivateRageSpell(float duration)
    {
        if (rageCoroutine != null) StopCoroutine(rageCoroutine);
        rageCoroutine = StartCoroutine(RageSpellRoutine(duration));
    }

    private IEnumerator RageSpellRoutine(float duration)
    {
        isRaged = true;

        // Doubler force et vitesse du joueur
        int origStrength = strength;
        int origSpeed = speed;
        strength = origStrength * 2;
        speed = origSpeed * 2;
        UpdateHUD();

        // Doubler la vitesse physique (MoveBehaviour)
        MoveBehaviour moveBehaviour = GetComponent<MoveBehaviour>();
        float origWalk = 0f, origRun = 0f, origSprint = 0f;
        if (moveBehaviour != null)
        {
            origWalk   = moveBehaviour.walkSpeed;
            origRun    = moveBehaviour.runSpeed;
            origSprint = moveBehaviour.sprintSpeed;
            moveBehaviour.walkSpeed   *= 2f;
            moveBehaviour.runSpeed    *= 2f;
            moveBehaviour.sprintSpeed *= 2f;
            moveBehaviour.RefreshSpeedSeeker();
        }

        // Couleur violette joueur
        if (playerRenderer != null)
        {
            playerRenderer.material.EnableKeyword("_EMISSION");
            playerRenderer.material.color = new Color(0.6f, 0f, 1f);
            playerRenderer.material.SetColor("_EmissionColor", new Color(0.6f, 0f, 1f) * 2f);
        }

        // Particules violettes joueur
        rageParticles = CreateRageParticles(transform);

        // Buff ennemis présents
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var e in enemies)
            e.ApplyRageBuff();

        yield return new WaitForSeconds(duration);

        // Restaurer force et vitesse joueur
        strength = origStrength;
        speed = origSpeed;
        UpdateHUD();

        if (moveBehaviour != null)
        {
            moveBehaviour.walkSpeed   = origWalk;
            moveBehaviour.runSpeed    = origRun;
            moveBehaviour.sprintSpeed = origSprint;
            moveBehaviour.RefreshSpeedSeeker();
        }

        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
            playerRenderer.material.SetColor("_EmissionColor", originalEmission);
        }

        if (rageParticles != null)
        {
            Destroy(rageParticles.gameObject);
            rageParticles = null;
        }

        // Retirer buff des ennemis survivants
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var e in enemies)
            e.RemoveRageBuff();

        isRaged = false;
        rageCoroutine = null;
    }

    private ParticleSystem CreateRageParticles(Transform parent)
    {
        GameObject pGo = new GameObject("RageParticles");
        pGo.transform.SetParent(parent, false);
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
        emission.rateOverTime = 20f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        return ps;
    }

    /// <summary>
    /// Active le mode Berserker : ajoute les HP courants - 1 à la force, puis met les HP à 1.
    /// </summary>
    public void ActivateBerserker()
    {
        if (currentHealth > 1)
            strength += currentHealth - 1;
        currentHealth = 1;
        UpdateHUD();
    }
}