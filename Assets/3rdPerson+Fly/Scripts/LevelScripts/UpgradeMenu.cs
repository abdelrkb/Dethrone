using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UpgradeMenu : MonoBehaviour
{
    public static UpgradeMenu Instance;

    public GameObject upgradeMenu;
    public Image weaponImage; // L'image du Weapon (enfant de UpgradeMenu)
    public Image statImage;   // L'image de la Stat (enfant de UpgradeMenu)
    public Image skillImage;  // L'image de la Skill (enfant de UpgradeMenu)
    public TMP_Text slotSelectionText; // Texte pour afficher "Choose which slot..."
    
    private Weapon randomWeapon; // L'arme sélectionnée aléatoirement
    private Stat randomStat;     // La stat sélectionnée aléatoirement
    private Skill randomSkill;   // La compétence sélectionnée aléatoirement
    
    private bool waitingForSlotSelection = false; // En attente de sélection du slot

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Vérifier si on attend la sélection du slot
        if (waitingForSlotSelection)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                AssignSkillToSlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                AssignSkillToSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                AssignSkillToSlot(2);
            }
        }
    }

    public void OpenMenu()
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.SetActive(true);
        }
        
        // S'assurer que les images et le texte sont au bon état
        SetImagesActive(true);
        if (slotSelectionText != null)
        {
            slotSelectionText.gameObject.SetActive(false);
        }
        waitingForSlotSelection = false;
        
        // Générer une arme aléatoire et afficher son image
        GenerateRandomWeapon();
        // Générer une stat aléatoire et afficher son image
        GenerateRandomStat();
        // Générer une compétence aléatoire et afficher son image
        GenerateRandomSkill();
        
        Time.timeScale = 0f; // PAUSE
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f; // REPREND AVANT DE FERMER
        
        if (upgradeMenu != null)
        {
            upgradeMenu.SetActive(false);
        }
    }

    void GenerateRandomWeapon()
    {
        // Récupérer le PlayerAttack pour avoir accès aux images
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerAttack playerAttack = null;
        
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        // Créer aléatoirement une arme parmi les disponibles
        int randomChoice = Random.Range(0, 2);
        
        switch (randomChoice)
        {
            case 0:
                randomWeapon = new Sword(playerAttack.swordFbx, playerAttack.swordImage);
                break;
            case 1:
                randomWeapon = new Fist(playerAttack.fistImage);
                break;
        }

        Debug.Log($"Arme aléatoire choisie: {randomWeapon.name}");
        
        // Afficher l'image de l'arme
        if (weaponImage != null && randomWeapon.image != null)
        {
            weaponImage.sprite = randomWeapon.image;
            Debug.Log($"Image de {randomWeapon.name} affichée");
        }
        else if (weaponImage != null)
        {
            Debug.LogWarning($"L'arme {randomWeapon.name} n'a pas d'image assignée!");
        }
    }

    void GenerateRandomStat()
    {
        // Récupérer le PlayerAttack pour avoir accès aux images
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerAttack playerAttack = null;
        
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        // Créer aléatoirement une stat parmi les disponibles
        int randomChoice = Random.Range(0, 1); // Augmente si tu ajoutes des stats
        
        switch (randomChoice)
        {
            case 0:
                randomStat = new SuperSonicStat(playerAttack.superSonicImage);
                break;
        }

        Debug.Log($"Stat aléatoire choisie: {randomStat.name}");
        
        // Afficher l'image de la stat
        if (statImage != null && randomStat.image != null)
        {
            statImage.sprite = randomStat.image;
            Debug.Log($"Image de {randomStat.name} affichée");
        }
        else if (statImage != null)
        {
            Debug.LogWarning($"La stat {randomStat.name} n'a pas d'image assignée!");
        }
    }

    void GenerateRandomSkill()
    {
        // Récupérer le PlayerAttack pour avoir accès aux images
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerAttack playerAttack = null;
        
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        // Créer aléatoirement une compétence parmi les disponibles
        int randomChoice = Random.Range(0, 1); // Augmente si tu ajoutes des compétences
        
        switch (randomChoice)
        {
            case 0:
                randomSkill = new ZaWarudoSkill(playerAttack.zaWarudoImage, playerAttack.zaWarudoMiniImage);
                break;
        }

        Debug.Log($"Compétence aléatoire choisie: {randomSkill.name}");
        
        // Afficher l'image de la compétence
        if (skillImage != null && randomSkill.image != null)
        {
            skillImage.sprite = randomSkill.image;
            Debug.Log($"Image de {randomSkill.name} affichée");
        }
        else if (skillImage != null)
        {
            Debug.LogWarning($"La compétence {randomSkill.name} n'a pas d'image assignée!");
        }
    }

    // ===== CHOIX =====

    public void ChooseSkillForSlot(int slotIndex)
    {
        Debug.Log($"ChooseSkillForSlot {slotIndex} appelé!");
        try
        {
            if (randomSkill == null)
            {
                Debug.LogWarning("Aucune compétence sélectionnée!");
                return;
            }

            if (slotIndex < 0 || slotIndex > 2)
            {
                Debug.LogWarning("Index de slot invalide!");
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.EquipSkill(slotIndex, randomSkill);
                    string[] slotNames = { "C", "V", "B" };
                    Debug.Log($"Compétence {randomSkill.name} équipée au slot {slotNames[slotIndex]}!");
                }
                else
                {
                    Debug.LogWarning("PlayerStats component not found!");
                }
            }
            else
            {
                Debug.LogWarning("Player not found!");
            }

            CloseMenu();
            WaveManager.Instance.StartNextWave();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur dans ChooseSkillForSlot: " + e.Message);
            CloseMenu();
        }
    }

    public void ChooseSkill()
    {
        Debug.Log("ChooseSkill appelé - En attente de sélection du slot!");
        
        if (randomSkill == null)
        {
            Debug.LogWarning("Aucune compétence sélectionnée!");
            return;
        }

        // Désactiver les images
        SetImagesActive(false);

        // Afficher le texte de sélection du slot
        if (slotSelectionText != null)
        {
            slotSelectionText.text = "Choose which slot you will be putting your skill (press C, V or B)";
            slotSelectionText.gameObject.SetActive(true);
        }

        // Activer le mode de sélection du slot
        waitingForSlotSelection = true;
    }

    /// <summary>
    /// Active ou désactive les images des upgrades
    /// </summary>
    private void SetImagesActive(bool active)
    {
        if (weaponImage != null)
            weaponImage.gameObject.SetActive(active);
        if (statImage != null)
            statImage.gameObject.SetActive(active);
        if (skillImage != null)
            skillImage.gameObject.SetActive(active);
    }

    /// <summary>
    /// Assigne la compétence au slot choisi via le clavier
    /// </summary>
    private void AssignSkillToSlot(int slotIndex)
    {
        try
        {
            if (randomSkill == null)
            {
                Debug.LogWarning("Aucune compétence sélectionnée!");
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.EquipSkill(slotIndex, randomSkill);
                    string[] slotNames = { "C", "V", "B" };
                    Debug.Log($"Compétence {randomSkill.name} équipée au slot {slotNames[slotIndex]}!");
                }
                else
                {
                    Debug.LogWarning("PlayerStats component not found!");
                }
            }
            else
            {
                Debug.LogWarning("Player not found!");
            }

            // Réactiver les images et désactiver le texte de sélection
            SetImagesActive(true);
            waitingForSlotSelection = false;
            if (slotSelectionText != null)
            {
                slotSelectionText.gameObject.SetActive(false);
            }

            CloseMenu();
            WaveManager.Instance.StartNextWave();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur dans AssignSkillToSlot: " + e.Message);
            waitingForSlotSelection = false;
            CloseMenu();
        }
    }

    public void ChooseStats()
    {
        Debug.Log("ChooseStats appelé!");
        try
        {
            if (randomStat == null)
            {
                Debug.LogWarning("Aucune stat sélectionnée!");
                return;
            }

            // Réactiver les images
            SetImagesActive(true);
            
            // Désactiver le texte de sélection du slot
            if (slotSelectionText != null)
            {
                slotSelectionText.gameObject.SetActive(false);
            }
            waitingForSlotSelection = false;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                randomStat.Effect(player);
                Debug.Log($"Stat appliquée: {randomStat.name}!");
            }
            else
            {
                Debug.LogWarning("Player not found!");
            }

            CloseMenu();
            WaveManager.Instance.StartNextWave();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur dans ChooseStats: " + e.Message);
            CloseMenu();
        }
    }

    public void ChooseWeapon()
    {
        Debug.Log("ChooseWeapon appelé!");
        try
        {
            if (randomWeapon == null)
            {
                Debug.LogWarning("Aucune arme sélectionnée!");
                return;
            }

            // Réactiver les images
            SetImagesActive(true);
            
            // Désactiver le texte de sélection du slot
            if (slotSelectionText != null)
            {
                slotSelectionText.gameObject.SetActive(false);
            }
            waitingForSlotSelection = false;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerAttack attack = player.GetComponent<PlayerAttack>();
                if (attack != null)
                {
                    attack.EquipWeapon(randomWeapon);
                    Debug.Log($"Arme équipée: {randomWeapon.name}!");
                }
                else
                {
                    Debug.LogWarning("PlayerAttack component not found!");
                }
            }
            else
            {
                Debug.LogWarning("Player not found!");
            }

            CloseMenu();
            WaveManager.Instance.StartNextWave();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur dans ChooseWeapon: " + e.Message);
            CloseMenu();
        }
    }
}

