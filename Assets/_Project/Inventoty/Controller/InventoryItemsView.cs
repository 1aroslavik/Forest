using UnityEngine;

public class InventoryItemsView : MonoBehaviour
{
    public ItemData data;
    public InventorySlotData slot;

    bool isHovering;

    void OnMouseEnter()
    {
        isHovering = true;

        if (InventoryTooltip.Instance != null)
            InventoryTooltip.Instance.Show(slot);
    }

    void OnMouseExit()
    {
        isHovering = false;

        if (InventoryTooltip.Instance != null)
            InventoryTooltip.Instance.Hide();
    }

    void Update()
    {
        if (isHovering && Input.GetKeyDown(KeyCode.E))
        {
            if (ItemUseSystem.Instance != null)
                ItemUseSystem.Instance.UseItem(slot);
        }
    }
}