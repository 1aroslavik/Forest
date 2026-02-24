using UnityEngine;

public class InventoryItemsView : MonoBehaviour
{
    public ItemData data;
    EquipSystem equipSystem;

    [Header("Random Rotation")]
    public bool randomizeRotation = true;
    public float maxYRotation = 20f;
    public float maxXRotation = 5f;

    void Start()
    {
        equipSystem = FindObjectOfType<EquipSystem>();

        if (randomizeRotation)
        {
            float randomY = Random.Range(-maxYRotation, maxYRotation);
            float randomX = Random.Range(-maxXRotation, maxXRotation);

            transform.localRotation = Quaternion.Euler(randomX, randomY, 0f);
        }
    }

    void OnMouseDown()
    {
        if (equipSystem != null)
        {
            equipSystem.Equip(data);
        }
    }
}