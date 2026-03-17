using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rb;
    bool stuck;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!stuck && rb.linearVelocity != Vector3.zero)
        {
            transform.forward = rb.linearVelocity;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (stuck) return;

        stuck = true;

        rb.isKinematic = true;

        // приклеиваем стрелу к объекту
        transform.parent = col.transform;
    }

    void OnMouseDown()
    {
        if (stuck)
        {
            Destroy(gameObject);
        }
    }
}