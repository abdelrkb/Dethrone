using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Compétence ZA WARUDO : arrête tous les ennemis pendant 10 secondes
/// </summary>
public class ZaWarudoSkill : Skill
{
    private const float FREEZE_DURATION = 10f; // Durée d'arrêt en secondes
    private const float COOLDOWN = 60f; // Cooldown en secondes

    public ZaWarudoSkill(Sprite image = null, Sprite miniImage = null) : base("ZA WARUDO", COOLDOWN, image, miniImage)
    {
    }

    public override void Effect(GameObject target)
    {
        if (CanUse())
        {
            ActivateZaWarudo(target);
            Debug.Log("ZA WARUDO activé!");
        }
        else
        {
            float remaining = GetRemainingCooldown();
            Debug.Log($"ZA WARUDO en cooldown pour {remaining:F1}s");
        }
        
        base.Effect(target);
    }

    void ActivateZaWarudo(GameObject target)
    {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var e in enemies)
        {
            // Geler l'ennemi
            e.Freeze();
        }

        // Créer une coroutine qui dégèle les ennemis après FREEZE_DURATION
        MonoBehaviour mono = target.GetComponent<MonoBehaviour>();
        if (mono != null)
        {
            mono.StartCoroutine(ResumeEnemiesAfterDelay(FREEZE_DURATION));
        }
    }

    IEnumerator ResumeEnemiesAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // Dégeler tous les ennemis
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            e.Unfreeze();
        }

        Debug.Log("Les ennemis peuvent se déplacer à nouveau!");
    }
}
