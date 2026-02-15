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

        //// ❌ если плаваем — не показываем hint вообще
        //if (waterState != null && waterState.IsSwimming)
        //{
        //    hint.SetActive(false);
        //    return;
        //}

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

        hint.SetActive(currentWater != null);
    }

    void HandleDrink()
    {
        if (currentWater == null) return;
       // if (waterState != null && waterState.IsSwimming) return;
        if (!Input.GetKey(drinkKey)) return;

        if (!currentWater.CanDrink()) return;

        float amount =
            currentWater.GetDrinkRate() * Time.deltaTime;

        stats.Drink(amount);
    }
}
