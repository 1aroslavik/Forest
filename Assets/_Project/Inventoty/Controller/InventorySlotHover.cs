using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public int slotIndex;
    public InventoryModel model;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (model == null) return;
        if (slotIndex >= model.slots.Count) return;

        var slot = model.slots[slotIndex];

        if (!slot.isEmpty)
        {
            InventoryTooltip.Instance.Show(slot);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip.Instance.Hide();
    }
}