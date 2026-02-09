using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    public InventoryModel model;
    public List<Transform> slotPoints = new();

    Dictionary<int, GameObject> visuals = new();

    void Start()
    {
        //for (int i = 0; i < slotPoints.Count; i++)
        //{
        //    var slotView = slotPoints[i].gameObject.AddComponent<InventorySlotView>();
        //    slotView.slotIndex = i;
        //    slotView.model = model;
        //}

        Render();
    }


    public void Render()
    {
        // очищаем старые визуалы
        foreach (var v in visuals.Values)
            Destroy(v);

        visuals.Clear();

        // для каждого слота модели
        for (int i = 0; i < model.slots.Count && i < slotPoints.Count; i++)
        {
            var slot = model.slots[i];
            if (slot.isEmpty) continue;

            if (slot.data.inventoryPrefab == null)
            {
                Debug.LogWarning($"{slot.data.itemName} has no inventory prefab");
                continue;
            }

            var obj = Instantiate(
                slot.data.inventoryPrefab,
                slotPoints[i].position,
                slotPoints[i].rotation,
                slotPoints[i]
            );
            var itemView = obj.GetComponent<InventoryItemsView>();
            itemView.data = slot.data;

            // небольшой SoTF-стиль
            obj.transform.localPosition += Random.insideUnitSphere * 0.03f;
            obj.transform.localRotation *= Quaternion.Euler(
    0f,
    Random.Range(0f, 360f),
    0f
);

            visuals[i] = obj;
        }
    }
}
