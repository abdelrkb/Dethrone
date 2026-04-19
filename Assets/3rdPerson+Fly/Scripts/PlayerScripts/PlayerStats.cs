using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public int strength = 0;
    public int speed = 1;

    public Transform healthBar; // Health_Fill
    public TMP_Text strengthText;
    public TMP_Text speedText;
    
    // Compétences
    private Skill[] skills = new Skill[3];
    public Image[] skillImages = new Image[3]; // Images pour C, V, B
    public TMP_Text[] skillCooldownTexts = new TMP_Text[3]; // Affichage cooldown + lettres (C, V, B)

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHUD();
        
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
        if (Input.GetKeyDown(KeyCode.C) && skills[0] != null)
        {
            UseSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.V) && skills[1] != null)
        {
            UseSkill(1);
        }
        if (Input.GetKeyDown(KeyCode.B) && skills[2] != null)
        {
            UseSkill(2);
        }
        
        // Mettre à jour l'affichage des cooldowns
        UpdateSkillCooldowns();
    }

    void UpdateHUD()
    {
        float hpPercent = (float)currentHealth / maxHealth;

        // Health bar (scale X)
        healthBar.localScale = new Vector3(hpPercent, 1, 1);

        strengthText.text = "Strength : " + strength;
        speedText.text = "Speed : " + speed;
    }

    public void UpdateHUDDirectly()
    {
        UpdateHUD();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHUD();

        // Vérifier si le joueur est mort
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Gère la mort du joueur
    /// </summary>
    private void Die()
    {
        Debug.Log("Le joueur est mort! Redémarrage à la wave 1...");
        
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
            GameObject player = gameObject;
            skills[slotIndex].Effect(player);
            Debug.Log($"Compétence utilisée: {skills[slotIndex].name}");
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
}