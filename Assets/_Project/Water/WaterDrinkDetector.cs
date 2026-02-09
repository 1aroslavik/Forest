using UnityEngine;

public class WaterDrinkDetector : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public PlayerStats stats;
    public GameObject hint;

    [Header("Settings")]
    public float forwardOffset = 1.2f;
    public float downDistance = 3f;
    public float maxDistance = 2.5f;
    public KeyCode drinkKey = KeyCode.E;
    public LayerMask waterLayer;

    IWaterSource currentWater;

    void Update()
    {
        DetectWater();
        HandleDrink();
    }

    void DetectWater()
    {
        currentWater = null;

        // точка перед игроком
        Vector3 origin =
            cam.transform.position +
            cam.transform.forward * forwardOffset;

        Debug.DrawRay(origin, Vector3.down * downDistance, Color.cyan);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, downDistance, waterLayer))
        {
            currentWater =
                hit.collider.GetComponent<IWaterSource>() ??
                hit.collider.GetComponentInParent<IWaterSource>();
        }

        if (currentWater != null)
            hint.SetActive(true);
        else
            hint.SetActive(false);

    }

    void HandleDrink()
    {
        if (currentWater == null) return;
        if (!Input.GetKey(drinkKey)) return;

        if (!currentWater.CanDrink()) return;

        float amount =
            currentWater.GetDrinkRate() * Time.deltaTime;

        stats.Drink(amount);
    }
}
