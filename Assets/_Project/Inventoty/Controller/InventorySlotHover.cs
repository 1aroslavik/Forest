using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHover :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public int slotIndex;
    public InventoryModel model;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var slot = model.slots[slotIndex];
        if (!slot.isEmpty)
            InventoryTooltip.Instance.Show(slot);
   
        Debug.Log("HOVER SLOT " + slotIndex);
    

}

public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip.Instance.Hide();
    }
}
