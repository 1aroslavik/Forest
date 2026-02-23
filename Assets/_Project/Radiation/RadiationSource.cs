using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RadiationSource : MonoBehaviour
{
    [Header("Main Settings")]
    public float radius = 5f;
    public float maxDamagePerSecond = 8f;
    public bool useDistanceFalloff = true;

    [Header("Extra Effects")]
    public float thirstDrainPerSecond = 2f;
    public float staminaDrainPerSecond = 5f;

    [Header("Target")]
    public string targetTag = "Player";

    private SphereCollider trigger;

    void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = radius;
    }

    void OnValidate()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
            col.radius = radius;
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag))
            return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null)
            return;

        float distance = Vector3.Distance(transform.position, other.transform.position);
        float multiplier = 1f;

        if (useDistanceFalloff)
        {
            float t = Mathf.Clamp01(distance / radius);
            multiplier = 1f - t;
        }

        float damage = maxDamagePerSecond * multiplier * Time.deltaTime;
        float thirstLoss = thirstDrainPerSecond * multiplier * Time.deltaTime;
        float staminaLoss = staminaDrainPerSecond * multiplier * Time.deltaTime;

        stats.TakeDamage(damage);

        stats.thirst -= thirstLoss;
        stats.thirst = Mathf.Clamp(stats.thirst, 0, stats.maxThirst);

        stats.stamina -= staminaLoss;
        stats.stamina = Mathf.Clamp(stats.stamina, 0, stats.maxStamina);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
