using UnityEngine;

public class WeaponEquipment : MonoBehaviour
{
    public InventoryModel inventory;
    public Transform handPoint;

    GameObject currentHands;
    Animator currentAnimator;

    public void EquipFromSlot(int slotIndex)
    {
        Debug.Log("CLICK SLOT " + slotIndex);

        if (slotIndex >= inventory.slots.Count)
            return;

        var slot = inventory.slots[slotIndex];

        if (slot.isEmpty)
        {
            Debug.Log("Slot empty");
            return;
        }

        ItemData item = slot.data;

        Debug.Log("Item = " + item.itemName);

        if (item.itemType != ItemType.Weapon)
        {
            Debug.Log("Not weapon");
            return;
        }

        Equip(item);
    }

    public  void Equip(ItemData item)
    {
        // удаляем старые руки
        if (currentHands != null)
            Destroy(currentHands);

        // создаём новые
        currentHands = Instantiate(item.handPrefab, handPoint);

        // берём аниматор из рук
        currentAnimator = currentHands.GetComponent<Animator>();
    }

    public Animator GetAnimator()
    {
        return currentAnimator;
    }
}