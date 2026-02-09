using UnityEngine;

public class AxeHit : MonoBehaviour
{
    public float hitDistance = 2f;
    public Camera cam;
    public KeyCode hitKey = KeyCode.Mouse0;

    [Header("Terrain Trees")]
    public TerrainTreeChopper terrainChopper;

    void Update()
    {
        if (!Input.GetKeyDown(hitKey)) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, hitDistance))
        {
            // 1️⃣ обычное дерево (prefab)
            TreeHealth tree = hit.collider.GetComponentInParent<TreeHealth>();
            if (tree != null)
            {
                tree.Hit(transform.position);
                Debug.Log("AXE HIT PREFAB TREE");
                return;
            }

            // 2️⃣ terrain-дерево
            if (hit.collider.GetComponent<TerrainCollider>())
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
