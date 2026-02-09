using UnityEngine;

public class AxeHit : MonoBehaviour
{
    public float hitDistance = 2f;
    public Camera cam;
    public KeyCode hitKey = KeyCode.Mouse0;

    void Update()
    {
        if (!Input.GetKeyDown(hitKey)) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, hitDistance))
        {
            TreeHealth tree = hit.collider.GetComponentInParent<TreeHealth>();
            if (tree != null)
            {
                tree.Hit(transform.position);
                Debug.Log("AXE HIT TREE");
            }
        }
    }
}
