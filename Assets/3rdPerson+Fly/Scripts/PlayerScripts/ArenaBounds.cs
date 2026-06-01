using UnityEngine;

/// <summary>
/// Maintient le joueur à l'intérieur des limites de l'arène.
/// </summary>
public class ArenaBounds : MonoBehaviour
{
    [Header("Limites de l'arène (coordonnées monde)")]
    public float xMin = -20f;
    public float xMax =  20f;
    public float zMin = -20f;
    public float zMax =  20f;

    [Header("Debug")]
    public bool showBoundsInScene = true;

    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        bool outside = pos.x < xMin || pos.x > xMax || pos.z < zMin || pos.z > zMax;
        if (!outside) return;

        // Corriger la position
        Vector3 corrected = new Vector3(
            Mathf.Clamp(pos.x, xMin, xMax),
            pos.y,
            Mathf.Clamp(pos.z, zMin, zMax)
        );

        // Désactiver temporairement le CharacterController pour téléporter sans conflit
        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = corrected;
            characterController.enabled = true;
        }
        else
        {
            transform.position = corrected;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showBoundsInScene) return;

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.5f);
        Vector3 center = new Vector3((xMin + xMax) / 2f, transform.position.y, (zMin + zMax) / 2f);
        Vector3 size   = new Vector3(xMax - xMin, 2f, zMax - zMin);
        Gizmos.DrawWireCube(center, size);
    }
}
