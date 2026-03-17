using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity != Vector3.zero)
        {
            transform.forward = rb.linearVelocity;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        rb.isKinematic = true;
        transform.SetParent(col.transform);
    }
}