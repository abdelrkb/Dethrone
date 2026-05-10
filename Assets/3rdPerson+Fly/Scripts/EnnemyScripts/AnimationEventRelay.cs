using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void EnableHitbox()
    {
        if (enemy != null) enemy.EnableHitbox();
    }

    public void DisableHitbox()
    {
        if (enemy != null) enemy.DisableHitbox();
    }

    public void OnAttackEnd()
    {
        if (enemy != null) enemy.OnAttackEnd();
    }
}
