using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float range = 2f;
    public LayerMask enemyLayer;
    
    // Assets à drag-drop dans l'Inspector
    public GameObject swordFbx; // Drag ta sword ici
    public Sprite swordImage;   // Drag l'image de l'épée ici
    public AudioClip swordSFX;
    public Sprite fistImage;    // Drag l'image du poing ici
    public AudioClip fistSFX;
    [Header("Stats images")]
    public Sprite superSonicImage;
    public Sprite gigaChadImage;
    public Sprite onePunchManImage;
    public Sprite popeyeImage;
    public Sprite titanImage;
    public Sprite gear2Image;

    [Header("Skills")]
    public Sprite zaWarudoImage; 
    public Sprite zaWarudoMiniImage; 
    public AudioClip zaWarudoSFX; 
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

    [Header("Offset épée en main")]
    public Vector3 swordPositionOffset = new Vector3(0f, 0.2f, 0f);
    public Vector3 swordRotationOffset = Vector3.zero;
    
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

        EquipWeapon(new Sword(swordFbx, swordImage, swordPositionOffset, swordRotationOffset, swordSFX));
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

        // One Punch Man : si Fist équipé et flag actif, 100 dégâts
        if (weapon is Fist && playerStats != null && playerStats.hasOnePunchMan)
            weapon.damage = 100;

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
                    weaponInstance.transform.localPosition = weapon.positionOffset;
                    weaponInstance.transform.localRotation = Quaternion.Euler(weapon.rotationOffset);
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
        EquipWeapon(new Sword(swordFbx, swordImage, swordPositionOffset, swordRotationOffset, swordSFX));
    }
}