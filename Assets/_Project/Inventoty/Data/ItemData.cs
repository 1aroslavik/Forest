using UnityEngine;

public enum ItemType
{
    Resource,
    Food,
    Tool,
    Weapon
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Base")]
    public string itemName;

    [TextArea]
    public string description;

    public ItemType itemType;

    [Header("Inventory")]
    public bool isStackable = true;
    public int maxStack = 10;

    [Header("Visual")]
    public GameObject inventoryPrefab; // на тенте
    public GameObject handPrefab;       // в руке
}
