using UnityEngine;

public class InventoryItemsView : MonoBehaviour
{
    public ItemData data;
    EquipSystem equipSystem;

    void Start() 
    {
        equipSystem = FindObjectOfType<EquipSystem>();
    }

    void OnMouseDown() 
    {
        if(equipSystem != null) 
        {
            equipSystem.Equip(data);
        }
    }
}
