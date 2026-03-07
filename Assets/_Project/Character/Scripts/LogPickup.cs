using UnityEngine;

public class LogPickup : MonoBehaviour
{
    public float pickupDistance = 3f;
    public Transform holdPoint;
    public Animator animator;

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
                // не даём подбирать строительные брёвна
                if (hit.collider.GetComponentInParent<ConstructionSite>() != null)
                    return;

                worldLog = hit.collider.gameObject;

                carriedLog = Instantiate(worldLog, holdPoint.position, holdPoint.rotation);
                carriedLog.transform.SetParent(holdPoint);
                carriedLog.transform.localPosition = Vector3.zero;
                carriedLog.transform.localRotation = Quaternion.identity;

                // удаляем физику у копии
                if (carriedLog.TryGetComponent(out Rigidbody rb))
                    Destroy(rb);

                if (carriedLog.TryGetComponent(out Collider col))
                    Destroy(col);

                worldLog.SetActive(false);

                animator.SetBool("isHolding", true);
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

        carriedLog = null;
        worldLog = null;
    }
}