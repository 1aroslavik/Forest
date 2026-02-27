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
                TryPickup();
            else
                DropLog();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetBool("isHolding", true);
            Debug.Log("Holding ON");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetBool("isHolding", false);
            Debug.Log("Holding OFF");
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
                worldLog = hit.collider.gameObject;

                // 1️⃣ создаём копию ДО выключения
                carriedLog = Instantiate(worldLog, holdPoint.position, holdPoint.rotation);
                carriedLog.transform.SetParent(holdPoint);
                carriedLog.transform.localPosition = Vector3.zero;
                carriedLog.transform.localRotation = Quaternion.identity;

                // 2️⃣ удаляем физику у копии
                if (carriedLog.TryGetComponent(out Rigidbody rb))
                    Destroy(rb);

                if (carriedLog.TryGetComponent(out Collider col))
                    Destroy(col);

                // 3️⃣ теперь выключаем оригинал
                worldLog.SetActive(false);

                animator.SetBool("isHolding", true);
            }
        }
    }
    void DropLog()
    {
        // удаляем визуальную копию
        Destroy(carriedLog);

        // возвращаем оригинал в мир
        worldLog.transform.position =
            holdPoint.position + transform.forward * 1f;

        worldLog.SetActive(true);

        animator.SetBool("isHolding", false);

        carriedLog = null;
        worldLog = null;
    }
}