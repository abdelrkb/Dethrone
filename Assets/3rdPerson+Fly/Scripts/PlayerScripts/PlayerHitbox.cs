using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private bool hasHitThisSwing = false;

    public void Init(PlayerAttack attack)
    {
        playerAttack = attack;
        gameObject.SetActive(false);
    }

    public void EnableHitbox()
    {
        hasHitThisSwing = false;
        gameObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PlayerHitbox] Collision avec : {other.gameObject.name}");

        if (hasHitThisSwing) return;

        Enemy enemy = other.GetComponent<Enemy>() ?? other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Debug.Log($"[PlayerHitbox] Ennemi touché ! Dégâts : {playerAttack.GetTotalDamage()}");
            enemy.TakeDamage(playerAttack.GetTotalDamage());
            hasHitThisSwing = true;

            Weapon w = playerAttack.GetCurrentWeapon();
            if (w?.sfx != null)
                MusicManager.Instance?.PlayWeaponSFX(w.sfx);
        }
        else
        {
            Debug.Log($"[PlayerHitbox] Pas d'ennemi trouvé sur {other.gameObject.name}");
        }
    }
}
