using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private Enemy owner;
    private bool hasHitThisSwing = false;

    public void Init(Enemy enemy)
    {
        owner = enemy;
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
        if (hasHitThisSwing) return;

        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(owner.GetDamage());
                hasHitThisSwing = true;
            }
        }
    }
}
