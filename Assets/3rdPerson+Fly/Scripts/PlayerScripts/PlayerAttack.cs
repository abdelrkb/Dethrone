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
    public AudioClip zaWarudoSFX; // Drag le SFX de ZA WARUDO ici
    public Sprite infinityGauntletImage;
    public Sprite infinityGauntletMiniImage;
    public Sprite mysteryBoxImage;
    public Sprite mysteryBoxMiniImage;
    public AudioClip mysteryBoxSFX;
    public Sprite starImage;
    public Sprite starMiniImage;
    public AudioClip starSFX;
    public Sprite frankLeboeufImage;
    public Sprite frankLeboeufMiniImage;
    public AudioClip frankLeboeufSFX;
    public Sprite rageSpellImage;
    public Sprite rageSpellMiniImage;
    public AudioClip rageSpellSFX;
    public Sprite senzuBeanImage;
    public Sprite senzuBeanMiniImage;
    public AudioClip senzuBeanSFX;
    public Sprite berserkerImage;
    public Sprite berserkerMiniImage;
    public AudioClip berserkerSFX;
    
    private Animator animator;
    private PlayerStats playerStats;
    private Weapon currentWeapon;
    private PlayerHitbox hitbox;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();

        hitbox = GetComponentInChildren<PlayerHitbox>();
        if (hitbox != null) hitbox.Init(this);

        EquipWeapon(new Sword(swordFbx, swordImage));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (animator != null)
                animator.SetTrigger("Attack");
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

    public void EnableHitbox()
    {
        if (hitbox != null) hitbox.EnableHitbox();
    }

    public void DisableHitbox()
    {
        if (hitbox != null) hitbox.DisableHitbox();
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
                foreach (Transform child in weaponHolder)
                {
                    if (child.GetComponent<PlayerHitbox>() == null)
                        Destroy(child.gameObject);
                }
                
                // Afficher le nouveau fbx si disponible
                if (weapon.fbxPrefab != null)
                {
                    GameObject weaponInstance = Instantiate(weapon.fbxPrefab, weaponHolder);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

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

    /// <summary>
    /// Rééquipe l'arme de base (appelé après une victoire)
    /// </summary>
    public void FullReset()
    {
        EquipWeapon(new Sword(swordFbx, swordImage));
    }
}