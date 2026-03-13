using UnityEngine;

public class ItemUseSystem : MonoBehaviour
{
    public static ItemUseSystem Instance;

    PlayerStats playerStats;

    void Awake()
    {
        Instance = this;
        playerStats = FindObjectOfType<PlayerStats>();
    }

    public void UseItem(InventorySlotData slot)
    {
        if (slot == null || slot.isEmpty) return;

        var item = slot.data;

        switch (item.itemType)
        {
            case ItemType.Food:

                playerStats.Eat(item.hungerRestore);
                break;

            case ItemType.Drink:

                playerStats.Drink(item.thirstRestore);
                break;

            case ItemType.Medicine:

                playerStats.Heal(item.healthRestore);
                break;

            case ItemType.Resource:

                AddToCraft(slot);
                return;
        }

        ConsumeItem(slot);
    }

    void ConsumeItem(InventorySlotData slot)
    {
        slot.amount--;

        if (slot.amount <= 0)
            slot.data = null;

        if (InventoryTooltip.Instance != null)
            InventoryTooltip.Instance.Hide();

        FindObjectOfType<InventoryView>().Render();
    }

    void AddToCraft(InventorySlotData slot)
    {
        Debug.Log("Move to craft: " + slot.data.itemName);
    }
}