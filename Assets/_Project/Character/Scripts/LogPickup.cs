using UnityEngine;

public class LogPickup : MonoBehaviour
{
    [Header("Pickup")]
    public float pickupDistance = 3f;
    public Transform holdPoint;

    [Header("Animator")]
    public Animator animator;

    [Header("Arms")]
    public GameObject axeArms;
    public GameObject carryArms;

    private GameObject worldLog;
    private GameObject carriedLog;

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

                carriedLog = Instantiate(worldLog, holdPoint.position, holdPoint.rotation);
                carriedLog.transform.SetParent(holdPoint);
                carriedLog.transform.localPosition = Vector3.zero;
                carriedLog.transform.localRotation = Quaternion.identity;

                if (carriedLog.TryGetComponent(out Rigidbody rb))
                    Destroy(rb);

                if (carriedLog.TryGetComponent(out Collider col))
                    Destroy(col);

                worldLog.SetActive(false);

                // анимация
                animator.SetBool("isHolding", true);

                // переключаем руки
                if (axeArms) axeArms.SetActive(false);
                if (carryArms) carryArms.SetActive(true);
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

                    animator.SetBool("isHolding", false);

                    // возвращаем руки
                    if (axeArms) axeArms.SetActive(true);
                    if (carryArms) carryArms.SetActive(false);

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
                holdPoint.position + transform.forward * 1f;

            worldLog.SetActive(true);
        }

        animator.SetBool("isHolding", false);

        // возвращаем руки
        if (axeArms) axeArms.SetActive(true);
        if (carryArms) carryArms.SetActive(false);

        carriedLog = null;
        worldLog = null;
    }
}