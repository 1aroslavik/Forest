using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    public BuildingData data;

    [Header("Log Setup")]
    public Transform logSlotsParent;
    public GameObject buildLogPrefab; // ВАЖНО: это отдельный prefab без физики

    private int currentLogs = 0;
    private Transform[] logSlots;

    void Start()
    {
        int count = logSlotsParent.childCount;
        logSlots = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            logSlots[i] = logSlotsParent.GetChild(i);
        }
    }

    public bool AddLog()
    {
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