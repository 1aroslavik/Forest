using UnityEngine;

[System.Serializable]
public class InventorySlotData 
{
   public ItemData data;
   public int amount;

   public bool isEmpty => data == null;
}
