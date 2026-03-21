using UnityEngine;

public class LogPickup : MonoBehaviour
{
    [Header("Pickup")]
    public float pickupDistance = 3f;

    [Header("Hands System")]
    public HandsController hands;
    public GameObject carryHandsPrefab;

    [Header("Weapon")]
    public WeaponEquipment weaponEquipment;

    private GameObject worldLog;
    private GameObject carriedLog;

    Transform currentHoldPoint; // ✅ добавили

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carriedLog == null)
            {
                TryPickup();
            }
            else
            {
                if (!TryAddToConstruction())
                    DropLog();
            }
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position,
                          Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Log"))
            {
                if (hit.collider.GetComponentInParent<ConstructionSite>() != null)
                    return;

                worldLog = hit.collider.gameObject;

                // убираем оружие
                if (weaponEquipment != null)
                    weaponEquipment.Unequip();

                // включаем руки
                hands.SetCarry(carryHandsPrefab);

                // 🔥 правильный поиск HoldPoint
                currentHoldPoint = hands.GetComponentInChildren<Transform>(true);

                foreach (Transform t in hands.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "HoldPoint")
                    {
                        currentHoldPoint = t;
                        break;
                    }
                }

                if (currentHoldPoint == null)
                {
                    Debug.LogError("HoldPoint not found!");
                    return;
                }

                // создаём бревно
                carriedLog = Instantiate(worldLog,
                    currentHoldPoint.position,
                    currentHoldPoint.rotation);

                carriedLog.transform.SetParent(currentHoldPoint);
                carriedLog.transform.localPosition = Vector3.zero;
                carriedLog.transform.localRotation = Quaternion.identity;

                if (carriedLog.TryGetComponent(out Rigidbody rb))
                    Destroy(rb);

                if (carriedLog.TryGetComponent(out Collider col))
                    Destroy(col);

                worldLog.SetActive(false);
            }
        }
    }

    bool TryAddToConstruction()
    {
        Ray ray = new Ray(Camera.main.transform.position,
                          Camera.main.transform.forward);

        LayerMask constructionLayer = LayerMask.GetMask("Construction");

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, constructionLayer))
        {
            ConstructionSite site =
                hit.collider.GetComponentInParent<ConstructionSite>();

            if (site != null)
            {
                bool added = site.AddLog();

                if (added)
                {
                    Destroy(carriedLog);
                    carriedLog = null;

                    if (worldLog != null)
                        Destroy(worldLog);

                    worldLog = null;

                    if (hands != null)
                        hands.ClearHands();

                    return true;
                }
            }
        }

        return false;
    }

    void DropLog()
    {
        Destroy(carriedLog);

        if (worldLog != null)
        {
            worldLog.transform.position =
                Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

            worldLog.SetActive(true);
        }

        if (hands != null)
            hands.ClearHands();

        carriedLog = null;
        worldLog = null;
    }
}