using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Image fillImage;
    private Transform cam;

    public void Init(int maxHealth)
    {
        fillImage = GetComponentInChildren<Image>();
        if (Camera.main != null) cam = Camera.main.transform;
        SetHealth(maxHealth, maxHealth);
    }

    public void SetHealth(int current, int max)
    {
        if (fillImage != null)
            fillImage.fillAmount = (float)current / max;
    }

    void LateUpdate()
    {
        if (cam != null)
            transform.forward = cam.forward;
    }
}
