using System.Collections.Generic;
using UnityEngine;

public class CraftArea : MonoBehaviour
{
    public static CraftArea Instance;

    public Transform[] craftSlots;

    List<GameObject> spawnedItems = new();
    List<ItemData> items = new();

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        int index = items.Count;
        Debug.Log("AddItem called");

        if (index >= craftSlots.Length)
        {
            Debug.Log("No free craft slots");
            return;
        }

        items.Add(item);

        GameObject obj = Instantiate(
            item.inventoryPrefab,
            craftSlots[index].position,
            Quaternion.identity,
            craftSlots[index]
        );

        spawnedItems.Add(obj);

        // ¬Œ“ ›“¿ —“–Œ ¿ Õ”∆Õ¿
        CraftingSystem.Instance.CheckRecipes(items);
    }

    public void Clear()
    {
        foreach (var obj in spawnedItems)
        {
            Destroy(obj);
        }

        spawnedItems.Clear();
        items.Clear();
    }

    public List<ItemData> GetItems()
    {
        return items;
    }
}