using UnityEngine;

public class AxeHit : MonoBehaviour
{
    public float hitDistance = 2f;
    public Camera cam;

    [Header("Terrain Trees")]
    public TerrainTreeChopper terrainChopper;

    bool canHit; // 🔥 управляется анимацией

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        // просто проверяем кнопку
        if (Input.GetMouseButtonDown(0))
        {
            TryHit();
        }
    }

    void TryHit()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, hitDistance))
        {
            // 1️⃣ Prefab дерево
            TreeHealth tree = hit.collider.GetComponentInParent<TreeHealth>();
            if (tree != null)
            {
                tree.Hit(transform.position);
                Debug.Log("AXE HIT PREFAB TREE");
                return;
            }

            // 2️⃣ Terrain дерево
            if (hit.collider is TerrainCollider)
            {
                GameObject spawnedTree =
                    terrainChopper.TryChopAndSpawn(hit.point);

                if (spawnedTree != null)
                {
                    TreeHealth spawnedHealth =
                        spawnedTree.GetComponent<TreeHealth>();

                    if (spawnedHealth != null)
                    {
                        spawnedHealth.Hit(transform.position);
                        Debug.Log("AXE HIT TERRAIN TREE");
                    }
                }
            }
        }
    }
}