using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float yOffset = 2.2f;
    [SerializeField] private bool hideWhenFull = false;

    private Transform cam;
    private Canvas canvas;
    private bool missingSliderLogged;
    private bool initialized;
    private int cachedMaxHealth = 1;

    private void Awake()
    {
        ResolveReferences();
    }

    public void Init(int maxHealth)
    {
        ResolveReferences();

        if (healthSlider == null)
        {
            if (!missingSliderLogged)
            {
                Debug.LogWarning($"[EnemyHealthBar] Slider introuvable sur '{name}'. Assigne-le dans l'inspector.");
                missingSliderLogged = true;
            }
            return;
        }

        cachedMaxHealth = Mathf.Max(1, maxHealth);
        healthSlider.minValue = 0f;
        healthSlider.maxValue = cachedMaxHealth;
        healthSlider.SetValueWithoutNotify(cachedMaxHealth);
        transform.localPosition = new Vector3(0f, yOffset, 0f);
        initialized = true;

        if (Camera.main != null)
            cam = Camera.main.transform;

        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>();

        if (canvas != null && hideWhenFull)
            canvas.enabled = false;
    }

    public void SetHealth(int current, int max)
    {
        ResolveReferences();

        if (healthSlider == null)
            return;

        if (!initialized)
            Init(max);

        int safeMax = Mathf.Max(1, max);
        if (safeMax != cachedMaxHealth)
        {
            cachedMaxHealth = safeMax;
            healthSlider.maxValue = cachedMaxHealth;
        }
        healthSlider.value = Mathf.Clamp(current, 0, cachedMaxHealth);

        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>();

        if (canvas != null && hideWhenFull)
        {
            canvas.enabled = current < max;
        }
    }

    private void LateUpdate()
    {
        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        if (cam != null)
            transform.forward = cam.forward;
    }

    private void ResolveReferences()
    {
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>(true);

        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>(true);
    }
}
