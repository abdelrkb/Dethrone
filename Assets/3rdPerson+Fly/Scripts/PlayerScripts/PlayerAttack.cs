using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float range = 2f;
    public LayerMask enemyLayer;
    
    // Assets à drag-drop dans l'Inspector
    public GameObject swordFbx; // Drag ta sword ici
    public Sprite swordImage;   // Drag l'image de l'épée ici
    public Sprite fistImage;    // Drag l'image du poing ici
    public Sprite superSonicImage; // Drag l'image de Super Sonic ici
    public Sprite zaWarudoImage; // Drag l'image de ZA WARUDO ici (grande version)
    public Sprite zaWarudoMiniImage; // Drag la mini image de ZA WARUDO ici (pour HUD)
    
    private Animator animator;
    private PlayerStats playerStats;
    private Weapon currentWeapon;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        
        // Initialiser avec l'épée avec les assets de l'Inspector
        EquipWeapon(new Sword(swordFbx, swordImage));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            Attack();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + transform.forward * 1.5f,
            range
        );
    }

    void Attack()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("Aucune arme équipée!");
            return;
        }

        // Calculer les dégâts totaux : force du joueur + dégâts de l'arme
        int totalDamage = playerStats.strength + currentWeapon.damage;

        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward * 1.5f,
            range,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(totalDamage);
                Debug.Log($"Dégâts infligés: {totalDamage} ({playerStats.strength} force + {currentWeapon.damage} arme)");
            }
        }
    }

    /// <summary>
    /// Change l'arme équipée du joueur
    /// </summary>
    public void EquipWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        Debug.Log($"Arme équipée: {weapon.name}");
        
        // Récupérer le WeaponHolder dans la main droite
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        if (rightHand != null)
        {
            Transform weaponHolder = rightHand.Find("WeaponHolder");
            if (weaponHolder != null)
            {
                // Supprimer l'ancienne arme affichée
                foreach (Transform child in weaponHolder)
                {
                    Destroy(child.gameObject);
                }
                
                // Afficher le nouveau fbx si disponible
                if (weapon.fbxPrefab != null)
                {
                    GameObject weaponInstance = Instantiate(weapon.fbxPrefab, weaponHolder);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;
                    Debug.Log($"Modèle de {weapon.name} affiché dans le WeaponHolder");
                }
            }
            else
            {
                Debug.LogWarning("WeaponHolder non trouvé dans la main droite!");
            }
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    /// <summary>
    /// Augmente les dégâts de l'arme équipée
    /// </summary>
    public void IncreaseDamage(int amount)
    {
        if (currentWeapon != null)
        {
            currentWeapon.damage += amount;
            Debug.Log($"Dégâts de {currentWeapon.name} augmentés à {currentWeapon.damage}!");
        }
    }

    /// <summary>
    /// Obtient les dégâts totaux (force + arme)
    /// </summary>
    public int GetTotalDamage()
    {
        if (currentWeapon == null) return 0;
        return playerStats.strength + currentWeapon.damage;
    }
}