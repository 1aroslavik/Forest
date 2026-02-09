using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public InventoryModel inventory;
    Camera camera;
    public float pickupDistance = 3f;
    public InventoryView inventoryView;

    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            var worldItem = hit.collider.GetComponent<WorldItem>();
            if (worldItem == null)
                return;

            bool added = inventory.TryAdd(worldItem.data, worldItem.amount);
            if (added)
                Destroy(worldItem.gameObject);
                inventoryView.Render();
        }
    }


}
