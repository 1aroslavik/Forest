using UnityEngine;
using System.Collections;

public class SpearController : MonoBehaviour
{
    public Animator animator;

    public GameObject spearPrefab;
    public Transform spearSpawn;

    public float throwForce = 30f;
    public float throwDelay = 0.25f; // момент вылета копья

    bool isAiming;
    bool isThrowing;

    void Update()
    {
        // ПКМ — прицеливание
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("Aim", true);
            isAiming = true;
        }

        // отпустили ПКМ
        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("Aim", false);
            isAiming = false;
        }

        // обычный удар копьем
        if (Input.GetMouseButtonDown(0) && !isAiming)
        {
            animator.SetTrigger("Stab");
        }

        // бросок копья
        if (Input.GetMouseButtonDown(0) && isAiming && !isThrowing)
        {
            animator.SetTrigger("Throw");
            StartCoroutine(ThrowRoutine());
        }
    }

    IEnumerator ThrowRoutine()
    {
        isThrowing = true;

        // ждём момент когда копье должно вылететь
        yield return new WaitForSeconds(throwDelay);

        ThrowSpear();

        isThrowing = false;
    }

    void ThrowSpear()
    {
        GameObject spear = Instantiate(spearPrefab, spearSpawn.position, spearSpawn.rotation);

        Rigidbody rb = spear.GetComponent<Rigidbody>();

        // игнорировать коллайдеры игрока
        Collider spearCollider = spear.GetComponent<Collider>();

        foreach (Collider col in GetComponentsInParent<Collider>())
        {
            Physics.IgnoreCollision(spearCollider, col);
        }

        rb.linearVelocity = spearSpawn.forward * throwForce;
    }
}