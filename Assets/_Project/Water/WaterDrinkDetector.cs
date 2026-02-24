using UnityEngine;

public class WaterDrinkDetector : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public PlayerStats stats;
    public GameObject hint;

    [Header("Settings")]
    public float detectDistance = 3f;
    public KeyCode drinkKey = KeyCode.E;
    public LayerMask waterLayer;

    private IWaterSource currentWater;

    void Update()
    {
        DetectWater();
        HandleDrink();
    }

    void DetectWater()
    {
        currentWater = null;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectDistance, waterLayer))
        {
            IWaterSource water =
                hit.collider.GetComponent<IWaterSource>() ??
                hit.collider.GetComponentInParent<IWaterSource>();

            if (water != null)
            {
                currentWater = water;
            }
        }

        // Включаем hint только если реально смотрим на воду
        if (hint != null)
            hint.SetActive(currentWater != null);
    }

    void HandleDrink()
    {
        if (currentWater == null) return;

        if (!Input.GetKey(drinkKey)) return;

        if (!currentWater.CanDrink()) return;

        float amount = currentWater.GetDrinkRate() * Time.deltaTime;

        stats.Drink(amount);
    }
}
