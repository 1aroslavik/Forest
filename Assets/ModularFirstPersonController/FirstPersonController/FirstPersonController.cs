using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;
    [Header("Sprint Thresholds")]
    public float staminaToStartSprint = 30f;
    public float staminaToStopSprint = 5f;
    bool sprintLocked;


    [Header("STATS")]
    public PlayerStats stats;

    #region Camera
    public Camera playerCamera;
    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    float yaw;
    float pitch;
    Image crosshairObject;
    #endregion

    #region Movement
    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    bool isWalking;
    bool isGrounded;
    #endregion

    #region Sprint
    public bool enableSprint = true;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    [Header("Stamina Cost")]
    public float sprintStaminaCostPerSecond = 20f;
    public float jumpStaminaCost = 25f;

    public bool useSprintBar = true;
    public Image sprintBar;

    bool isSprinting;
    #endregion

    #region Jump
    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        crosshairObject = GetComponentInChildren<Image>();
        playerCamera.fieldOfView = fov;
    }

    void Start()
    {
        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else crosshairObject.gameObject.SetActive(false);
    }

    void Update()
    {
        #region Camera
        if (cameraCanMove)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch += (invertCamera ? 1 : -1) * Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
        #endregion

        #region Jump
        if (enableJump && Input.GetKeyDown(jumpKey))
            Jump();
        #endregion

        CheckGround();
        UpdateSprintBar();
    }


void FixedUpdate()
    {
        if (!playerCanMove) return;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool hasMoveInput = input.sqrMagnitude > 0.01f;

        Vector3 targetVelocity = transform.TransformDirection(input) * walkSpeed;

        isWalking = hasMoveInput && isGrounded;

        // ==============================
        // SPRINT STATE MACHINE (NO JITTER)
        // ==============================

        if (!sprintLocked)
        {
            // ПЫТАЕМСЯ НАЧАТЬ БЕГ
            if (enableSprint &&
                hasMoveInput &&
                Input.GetKey(sprintKey) &&
                stats != null &&
                stats.stamina >= staminaToStartSprint)
            {
                sprintLocked = true;
            }
        }
        else
        {
            // ПРОВЕРЯЕМ, НУЖНО ЛИ ОСТАНОВИТЬ БЕГ
            if (!Input.GetKey(sprintKey) ||
                !hasMoveInput ||
                stats == null ||
                stats.stamina <= staminaToStopSprint)
            {
                sprintLocked = false;
            }
        }

        // ==============================
        // APPLY MOVEMENT
        // ==============================

        if (sprintLocked)
        {
            targetVelocity = transform.TransformDirection(input) * sprintSpeed;

            if (stats != null)
                stats.UseStamina(sprintStaminaCostPerSecond * Time.fixedDeltaTime);

            isSprinting = true;

            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                sprintFOV,
                sprintFOVStepTime * Time.fixedDeltaTime
            );
        }
        else
        {
            isSprinting = false;

            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                fov,
                sprintFOVStepTime * Time.fixedDeltaTime
            );
        }

        // ==============================
        // SEND STATE TO STATS (REGEN LOGIC)
        // ==============================

        if (stats != null)
        {
            stats.isWalking = isWalking && !isSprinting;
            stats.isSprinting = isSprinting;
        }

        // ==============================
        // PHYSICS (НЕ ТРОГАЕМ)
        // ==============================

        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange.y = 0;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }




    void Jump()
    {
        if (!isGrounded) return;
        if (stats != null && !stats.CanUseStamina(jumpStaminaCost)) return;

        if (stats != null)
            stats.UseStamina(jumpStaminaCost);

        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isGrounded = false;
    }

    void UpdateSprintBar()
    {
        if (!useSprintBar || sprintBar == null || stats == null) return;

        float percent = stats.stamina / stats.maxStamina;
        sprintBar.fillAmount = percent;
    }

    void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.down * 0.5f;
        isGrounded = Physics.Raycast(origin, Vector3.down, 0.7f);
    }
}
