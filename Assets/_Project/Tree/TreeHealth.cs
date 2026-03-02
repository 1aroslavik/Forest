using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    int currentHealth;

    [Header("Chopping Segments")]
    public GameObject[] choppingSegments;
    int choppedCount = 0;

    [Header("Logs")]
    public GameObject logPrefab;
    public int logsCount = 4;

    [Header("Fall")]
    public float fallForce = 6f;
    public float torqueForce = 8f;

    [Header("Fixed Hit Point")]
    public Transform hitPoint; // 🔥 фиксированная точка удара

    bool fallen = false;
    bool hasBroken = false;

    Rigidbody fallingRb;

    void Awake()
    {
        currentHealth = maxHealth;

        fallingRb = GetComponentInChildren<Rigidbody>(true);

        if (fallingRb == null)
        {
            Debug.LogError("TREE HAS NO RIGIDBODY");
            return;
        }

        fallingRb.isKinematic = true;
        fallingRb.useGravity = true;
        fallingRb.constraints = RigidbodyConstraints.None;
    }

    public void Hit(Vector3 hitterPosition)
    {
        if (fallen) return;

        currentHealth--;
        Debug.Log("TREE HIT | HP = " + currentHealth);

        // 🔥 используем фиксированную точку
        Vector3 fixedPoint = hitPoint != null
            ? hitPoint.position
            : transform.position;

        MakeHole(fixedPoint);

        if (currentHealth <= 0)
            Fall(hitterPosition, fixedPoint);
    }

    void Fall(Vector3 hitterPosition, Vector3 forcePoint)
    {
        if (fallingRb == null) return;

        fallen = true;

        StartCoroutine(FallNextFixedUpdate(hitterPosition, forcePoint));
    }

    System.Collections.IEnumerator FallNextFixedUpdate(Vector3 hitterPosition, Vector3 forcePoint)
    {
        yield return new WaitForFixedUpdate();

        fallingRb.isKinematic = false;

        Vector3 dir = transform.position - hitterPosition;
        dir.y = 0f;
        dir.Normalize();

        // 🔥 сила прикладывается в фиксированной точке
        fallingRb.AddForceAtPosition(
            dir * fallForce,
            forcePoint,
            ForceMode.Impulse
        );

        fallingRb.AddTorque(
            Vector3.Cross(Vector3.up, dir) * torqueForce,
            ForceMode.Impulse
        );
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!fallen || hasBroken) return;

        if (!collision.gameObject.CompareTag("Ground"))
            return;

        if (collision.relativeVelocity.magnitude > 3f)
        {
            hasBroken = true;
            BreakIntoLogs();
        }
    }

    void MakeHole(Vector3 hitPoint)
    {
        if (choppedCount >= choppingSegments.Length)
            return;

        int index = -1;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < choppingSegments.Length; i++)
        {
            if (!choppingSegments[i].activeSelf)
                continue;

            float dist = Vector3.Distance(
                choppingSegments[i].transform.position,
                hitPoint
            );

            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        if (index != -1)
        {
            choppingSegments[index].SetActive(false);
            choppedCount++;
        }
    }

    void BreakIntoLogs()
    {
        Vector3 startPos = fallingRb.transform.position;
        Vector3 direction = fallingRb.transform.up;

        float spacing = 1.2f;

        for (int i = 0; i < logsCount; i++)
        {
            Vector3 pos = startPos + direction * (i * spacing);
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, direction);

            GameObject log = Instantiate(logPrefab, pos, rot);

            Rigidbody logRb = log.GetComponent<Rigidbody>();
            if (logRb != null)
            {
                logRb.mass = 20;
                logRb.linearDamping = 2;
                logRb.angularDamping = 4;
            }
        }

        Destroy(gameObject);
    }
}