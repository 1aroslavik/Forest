using UnityEngine;
using TMPro;
public class ItemPickUp : MonoBehaviour
{
    [Header("References")]
    public InventoryModel inventory;
    public InventoryView inventoryView;
    public Camera playerCamera;
    public LayerMask pickupLayer;

    [Header("UI")]
    public GameObject pickupHint;

    [Header("Settings")]
    public float pickupDistance = 3f;
    public KeyCode pickupKey = KeyCode.E;

    WorldItem currentItem;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (pickupHint != null)
            pickupHint.SetActive(false);
    }

    void Update()
    {
        CheckForItem();

        if (Input.GetKeyDown(pickupKey) && currentItem != null)
        {
            TryPickUp();
        }
    }

    void CheckForItem()
    {
        currentItem = null;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, pickupLayer))
        {
            WorldItem item = hit.collider.GetComponentInParent<WorldItem>();

            if (item != null)
            {
                currentItem = item;

                if (pickupHint != null)
                    pickupHint.SetActive(true);

                return;
            }
        }

        if (pickupHint != null)
            pickupHint.SetActive(false);
    }

    void TryPickUp()
    {
        bool added = inventory.TryAdd(currentItem.data, currentItem.amount);

        if (added)
        {
            Destroy(currentItem.gameObject);

            if (inventoryView != null)
                inventoryView.Render();

            if (pickupHint != null)
                pickupHint.SetActive(false);
        }
    }
}