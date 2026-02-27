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
        // 1️⃣ Если предмет стакаемый — сначала ищем существующий стак
        if (data.isStackable)
        {
            foreach (var slot in slots)
            {
                if (!slot.isEmpty &&
                    slot.data == data &&
                    slot.amount < data.maxStack)
                {
                    int space = data.maxStack - slot.amount;
                    int toAdd = Mathf.Min(space, amount);

                    slot.amount += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                        return true;
                }
            }
        }

        // 2️⃣ Если что-то осталось — кладём в пустой слот
        foreach (var slot in slots)
        {
            if (slot.isEmpty)
            {
                int toAdd = data.isStackable
                    ? Mathf.Min(amount, data.maxStack)
                    : 1;

                slot.data = data;
                slot.amount = toAdd;

                amount -= toAdd;

                if (amount <= 0)
                    return true;
            }
        }

        return false; // если нет места
    }
}
