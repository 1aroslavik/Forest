using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("Buildings")]
    public BuildingData[] buildings;

    GameObject previewObject;
    BuildingData currentBuilding;

    public float snapDistance = 2f;
    public float rotationStep = 45f;

    float currentRotation = 0f;

    void Update()
    {
        HandleSelection();

        if (currentBuilding == null)
            return;

        if (previewObject == null)
            CreatePreview();

        MovePreview();
        HandleRotation();

        if (Input.GetMouseButtonDown(0))
            Place();

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelBuilding();
    }

    void HandleSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectBuilding(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectBuilding(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectBuilding(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectBuilding(3);
    }

    void SelectBuilding(int index)
    {
        if (index >= buildings.Length)
            return;

        currentBuilding = buildings[index];

        if (previewObject != null)
            Destroy(previewObject);
    }

    void CreatePreview()
    {
        previewObject = Instantiate(currentBuilding.constructionPrefab);

        // УБИРАЕМ ВСЮ ФИЗИКУ у preview
        foreach (Collider col in previewObject.GetComponentsInChildren<Collider>())
            Destroy(col);

        foreach (Rigidbody rb in previewObject.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);
    }

    void MovePreview()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
            return;

        previewObject.transform.position = hit.point;

        int snapMask = LayerMask.GetMask("SnapPoint");

        Collider[] snapPoints =
            Physics.OverlapSphere(previewObject.transform.position,
                                  snapDistance,
                                  snapMask);

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var col in snapPoints)
        {
            float dist = Vector3.Distance(
                previewObject.transform.position,
                col.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = col.transform;
            }
        }

        if (closest != null)
        {
            // Ищем SnapPoints у preview
            Transform previewSnapPoints =
                previewObject.transform.Find("SnapPoints");

            if (previewSnapPoints == null)
                return;

            Transform previewClosest = null;
            float minPreviewDist = float.MaxValue;

            foreach (Transform child in previewSnapPoints)
            {
                float dist = Vector3.Distance(
                    child.position,
                    closest.position);

                if (dist < minPreviewDist)
                {
                    minPreviewDist = dist;
                    previewClosest = child;
                }
            }

            if (previewClosest != null)
            {
                Vector3 offset =
                    previewObject.transform.position -
                    previewClosest.position;

                previewObject.transform.position =
                    closest.position + offset;
            }
        }
    }

    void HandleRotation()
    {
        if (previewObject == null)
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
            currentRotation += rotationStep;

        if (scroll < 0f)
            currentRotation -= rotationStep;

        previewObject.transform.rotation =
            Quaternion.Euler(0, currentRotation, 0);
    }

    void Place()
    {
        Instantiate(currentBuilding.constructionPrefab,
            previewObject.transform.position,
            previewObject.transform.rotation);

        Destroy(previewObject);
    }

    void CancelBuilding()
    {
        currentBuilding = null;

        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
    }
}