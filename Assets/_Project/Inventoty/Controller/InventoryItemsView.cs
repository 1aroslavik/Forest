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
            if (data == null) return;

            // если оружие → экипировать
            if (data.itemType == ItemType.Weapon)
            {
                WeaponEquipment equipment = FindFirstObjectByType<WeaponEquipment>();

                if (equipment != null)
                {
                    equipment.Equip(data);
                }
            }
            else
            {
                // обычное использование предмета
                if (ItemUseSystem.Instance != null)
                    ItemUseSystem.Instance.UseItem(slot);
            }
        }
    }
}