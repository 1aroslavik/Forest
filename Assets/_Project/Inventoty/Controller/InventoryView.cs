using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    public InventoryModel model;
    public List<Transform> slotPoints = new();

    Dictionary<int, List<GameObject>> visuals = new();

    void Start()
    {
        Render();
    }

    public void Render()
    {
        // удаляем старые визуалы
        foreach (var list in visuals.Values)
        {
            foreach (var obj in list)
                Destroy(obj);
        }

        visuals.Clear();

        for (int i = 0; i < model.slots.Count && i < slotPoints.Count; i++)
        {
            var slot = model.slots[i];
            if (slot.isEmpty) continue;

            if (slot.data.inventoryPrefab == null)
            {
                Debug.LogWarning($"{slot.data.itemName} has no inventory prefab");
                continue;
            }

            int visualCount = Mathf.Min(slot.amount, 5);

            visuals[i] = new List<GameObject>();

            float radius = 0.06f;

            for (int j = 0; j < visualCount; j++)
            {
                var obj = Instantiate(
                    slot.data.inventoryPrefab,
                    slotPoints[i].position,
                    slotPoints[i].rotation,
                    slotPoints[i]
                );

                var itemView = obj.GetComponent<InventoryItemsView>();
                itemView.data = slot.data;
                itemView.slot = slot;

                // размещение по кругу
                float angle = j * Mathf.PI * 2f / visualCount;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                obj.transform.localPosition += offset;

                // случайный поворот
                obj.transform.localRotation = Quaternion.Euler(
                    Random.Range(-5f, 5f),
                    Random.Range(0f, 360f),
                    Random.Range(-5f, 5f)
                );

                visuals[i].Add(obj);
            }
        }
    }
}