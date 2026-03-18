using UnityEngine;

public class CraftUI : MonoBehaviour
{
    public static CraftUI Instance;

    public GameObject craftButton;
    public Transform resultSlot;

    GameObject currentResult;

    void Awake()
    {
        Instance = this;
        craftButton.SetActive(false);
    }

    public void ShowResult(ItemData item)
    {
        if (currentResult != null)
            Destroy(currentResult);

        currentResult = Instantiate(
            item.inventoryPrefab,
            resultSlot.position,
            Quaternion.identity,
            resultSlot
        );
    }

    public void HideResult()
    {
        if (currentResult != null)
            Destroy(currentResult);
    }

    public void ShowCraftButton()
    {
        craftButton.SetActive(true);
    }

    public void HideCraftButton()
    {
        craftButton.SetActive(false);
    }
}