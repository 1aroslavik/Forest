using UnityEngine;
using TMPro;

public class InventoryTooltip : MonoBehaviour
{
    public static InventoryTooltip Instance;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI amount;

    InventorySlotData currentSlot;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(InventorySlotData slot)
    {
        if (slot == null || slot.isEmpty) return;

        currentSlot = slot;

        itemName.text = slot.data.itemName;
        description.text = slot.data.description;
        amount.text = "x" + slot.amount;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentSlot = null;
        gameObject.SetActive(false);
    }
}
