using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    public int SlotCount = 20;
    public List<InventorySlotData> slots = new();

    private void Awake()
    {
        for (int i = 0; i < SlotCount; i++)
        { 
            slots.Add(new InventorySlotData());
        }
    }

    public bool TryAdd(ItemData data, int amount)
    {
        if (data.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.data & slot.amount < data.maxStack)
                { 
                    int space = data.maxStack - slot.amount;
                    int toAdd = Mathf.Min(space, amount);
                    slot.amount += toAdd;
                    amount -= toAdd;
                }
            }
        }

        foreach (var slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.data = data;
                slot.amount = amount;
                return true;
            }
        }
        return false;
    }
}
