using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    public BuildingData data;

    [Header("Log Setup")]
    public Transform logSlotsParent;
    public GameObject buildLogPrefab; // ВАЖНО: это отдельный prefab без физики
    [Header("Cancel Construction")]
    public GameObject logDropPrefab;
    public float cancelHoldTime = 1.5f;

    float cancelTimer = 0f;
    private int currentLogs = 0;
    private Transform[] logSlots;
    void Update()
    {
        HandleCancel();
    }
    void Start()
    {
        int count = logSlotsParent.childCount;
        logSlots = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            logSlots[i] = logSlotsParent.GetChild(i);
        }
    }
    void HandleCancel()
    {
        if (!Input.GetKey(KeyCode.G))
        {
            cancelTimer = 0;
            return;
        }

        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 4f))
        {
            if (hit.transform == transform)
            {
                cancelTimer += Time.deltaTime;

                if (cancelTimer >= cancelHoldTime)
                {
                    CancelConstruction();
                }
            }
        }
    }
    void CancelConstruction()
    {
        for (int i = 0; i < currentLogs; i++)
        {
            Vector3 spawnPos = transform.position +
                new Vector3(
                    Random.Range(-0.6f, 0.6f),
                    1f,
                    Random.Range(-0.6f, 0.6f));

            Instantiate(logDropPrefab, spawnPos, Random.rotation);
        }

        Destroy(gameObject);
    }
    public bool AddLog()
    {
        Debug.Log("AddLog called");

        if (currentLogs >= data.requiredLogs)
            return false;

        Transform slot = logSlots[currentLogs];

        // создаём бревно
        GameObject log = Instantiate(
            buildLogPrefab,
            slot.position,
            slot.rotation,
            transform);

        // КОПИРУЕМ SCALE СЛОТА
        log.transform.localScale = slot.localScale;

        // удаляем физику
        if (log.TryGetComponent(out Rigidbody rb))
            Destroy(rb);

        if (log.TryGetComponent(out Collider col))
            Destroy(col);

        currentLogs++;

        if (currentLogs >= data.requiredLogs)
            Complete();

        return true;
    }

    void Complete()
    {
        Instantiate(data.finishedPrefab,
            transform.position,
            transform.rotation);

        Destroy(gameObject);
    }
}