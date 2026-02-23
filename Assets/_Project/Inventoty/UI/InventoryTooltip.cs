using UnityEngine;
using TMPro;

public class InventoryTooltip : MonoBehaviour
{
    public static InventoryTooltip Instance;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI amount;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Hide();
    }

    public void Show(InventorySlotData slot)
    {
        if (slot == null || slot.isEmpty || slot.data == null)
            return;

        itemName.text = slot.data.itemName;
        amount.text = "x" + slot.amount;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}