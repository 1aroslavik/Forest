using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSwimmingSystem : MonoBehaviour
{
    public Animator animator;
    public Transform cameraTransform;

    [Header("Movement")]
    public float swimForce = 20f;
    public float waterDrag = 4f;

    [Header("Buoyancy")]
    public float buoyancyForce = 15f;
    public float waterLevel = 10f;

    private Rigidbody rb;
    private bool inWater = false;
    private int swimLayerIndex = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator.SetLayerWeight(swimLayerIndex, 0f);
    }

    void FixedUpdate()
    {
        if (!inWater) return;

        // === ДВИЖЕНИЕ В НАПРАВЛЕНИИ КАМЕРЫ ===
        float move = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        Vector3 moveDirection = camForward * move + camRight * strafe;

        rb.AddForce(moveDirection * swimForce, ForceMode.Acceleration);

        // === ПЛАВУЧЕСТЬ ===
        float depth = waterLevel - transform.position.y;

        if (depth > 0)
        {
            rb.AddForce(Vector3.up * buoyancyForce * depth, ForceMode.Acceleration);
        }

        animator.SetBool("IsSwimming", moveDirection.magnitude > 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwimZone"))
        {
            inWater = true;

            rb.useGravity = false;
            rb.linearDamping = waterDrag;

            animator.SetBool("InWater", true);
            animator.SetLayerWeight(swimLayerIndex, 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SwimZone"))
        {
            inWater = false;

            rb.useGravity = true;
            rb.linearDamping = 0f;

            animator.SetBool("InWater", false);
            animator.SetBool("IsSwimming", false);
            animator.SetLayerWeight(swimLayerIndex, 0f);
        }
    }
}
