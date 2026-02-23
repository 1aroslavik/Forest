using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    int currentHealth;

    [Header("Logs")]
    public GameObject logPrefab;
    public int logsCount = 4;

    [Header("Fall")]
    public float fallForce = 6f;
    public float torqueForce = 8f;

    Rigidbody rb;
    bool fallen = false;
    public bool IsFallen => fallen;   // 👈 ДОБАВИЛИ

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // вызывается топором
    public void Hit(Vector3 hitterPosition)
    {
        if (fallen) return;

        currentHealth--;
        Debug.Log($"TREE HIT | HP = {currentHealth}");

        if (currentHealth <= 0)
            Fall(hitterPosition);
    }

    void Fall(Vector3 hitterPosition)
    {
        fallen = true;
        rb.isKinematic = false;

        // направление падения ОТ игрока
        Vector3 dir = (transform.position - hitterPosition).normalized;

        rb.AddForce(dir * fallForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.Cross(Vector3.up, dir) * torqueForce, ForceMode.Impulse);

        Invoke(nameof(BreakIntoLogs), 2.2f);
    }

    void BreakIntoLogs()
    {
        for (int i = 0; i < logsCount; i++)
        {
            Vector3 pos = transform.position + Vector3.up * (i * 1.2f);
            GameObject log = Instantiate(logPrefab, pos, Quaternion.identity);

            Rigidbody logRb = log.GetComponent<Rigidbody>();
            logRb.mass = 20;
            logRb.linearDamping = 2;
            logRb.angularDamping = 4;
        }

        Destroy(gameObject);
    }
}
