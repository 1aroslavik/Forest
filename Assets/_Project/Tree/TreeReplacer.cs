using System.Collections.Generic;
using UnityEngine;

public class TreeReplacer : MonoBehaviour
{
    [Header("References")]
    public Terrain terrain;
    public Camera playerCamera;

    [Header("Interaction")]
    public float replaceDistance = 15f;

    [Header("Performance")]
    public float cellSize = 10f;
    public float despawnDistance = 60f;

    [Header("PlayMode Options")]
    public bool restoreTrees = true;

    [System.Serializable]
    public class TreeReplacement
    {
        public string treeName;
        public GameObject replacementPrefab;
    }

    [Header("Replacements")]
    public TreeReplacement[] replacements;

    private TerrainData tData;
    private Dictionary<string, GameObject> replacementDict;
    private Dictionary<Vector3Int, List<TreeRef>> treeGrid;

    private readonly List<TreeRef> activeSpawned = new List<TreeRef>();

    public class TreeRef
    {
        public TreeInstance original;
        public Vector3 worldPos;
        public bool isReplaced;
        public GameObject spawnedGO;
    }

    void Start()
    {
        if (terrain == null || terrain.terrainData == null)
        {
            enabled = false;
            return;
        }

        tData = terrain.terrainData;

        if (restoreTrees)
        {
            tData = Instantiate(terrain.terrainData);
            terrain.terrainData = tData;
        }

        replacementDict = new Dictionary<string, GameObject>();

        foreach (var r in replacements)
        {
            if (!replacementDict.ContainsKey(r.treeName) && r.replacementPrefab != null)
                replacementDict.Add(r.treeName, r.replacementPrefab);
        }

        BuildTreeGrid();
    }

    void Update()
    {
        AutoReplaceNearbyTrees();
        CheckForDespawn();
    }

    private void BuildTreeGrid()
    {
        treeGrid = new Dictionary<Vector3Int, List<TreeRef>>();

        var trees = tData.treeInstances;

        for (int i = 0; i < trees.Length; i++)
        {
            var tr = new TreeRef
            {
                original = trees[i],
                worldPos = NormalizedToWorld(trees[i].position),
                isReplaced = false,
                spawnedGO = null
            };

            Vector3Int cell = WorldToCell(tr.worldPos);

            if (!treeGrid.TryGetValue(cell, out var list))
            {
                list = new List<TreeRef>();
                treeGrid[cell] = list;
            }

            list.Add(tr);
        }
    }

    private Vector3 NormalizedToWorld(Vector3 normalizedPos)
    {
        return Vector3.Scale(normalizedPos, tData.size) + terrain.transform.position;
    }

    private Vector3Int WorldToCell(Vector3 pos)
    {
        return new Vector3Int(
            Mathf.FloorToInt(pos.x / cellSize),
            0,
            Mathf.FloorToInt(pos.z / cellSize)
        );
    }

    void AutoReplaceNearbyTrees()
    {
        Vector3 playerPos = playerCamera.transform.position;

        Vector3Int centerCell = WorldToCell(playerPos);

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3Int cell = new Vector3Int(centerCell.x + x, 0, centerCell.z + z);

                if (!treeGrid.TryGetValue(cell, out var list))
                    continue;

                foreach (var tr in list)
                {
                    if (tr.isReplaced)
                        continue;

                    float dist = Vector3.Distance(playerPos, tr.worldPos);

                    if (dist < replaceDistance)
                    {
                        string treeName =
                            tData.treePrototypes[tr.original.prototypeIndex].prefab.name;

                        if (replacementDict.TryGetValue(treeName, out var prefab))
                        {
                            ReplaceTreeWithPrefab(tr, prefab);
                        }
                    }
                }
            }
        }
    }

    private void ReplaceTreeWithPrefab(TreeRef tr, GameObject prefab)
    {
        if (tr.isReplaced)
            return;

        if (!RemoveTreeInstanceFromTerrain(tr))
            return;

        Quaternion rot = Quaternion.Euler(
            0f,
            tr.original.rotation * Mathf.Rad2Deg,
            0f
        );

        Vector3 scale = new Vector3(
            tr.original.widthScale,
            tr.original.heightScale,
            tr.original.widthScale
        );

        tr.isReplaced = true;

        GameObject obj = Instantiate(prefab, tr.worldPos, rot);

        obj.transform.localScale = Vector3.Scale(obj.transform.localScale, scale);

        tr.spawnedGO = obj;

        activeSpawned.Add(tr);
    }

    private bool RemoveTreeInstanceFromTerrain(TreeRef tr)
    {
        var trees = new List<TreeInstance>(tData.treeInstances);

        for (int i = 0; i < trees.Count; i++)
        {
            Vector3 world = NormalizedToWorld(trees[i].position);

            float dist = Vector3.Distance(world, tr.worldPos);

            if (dist < 1.5f)
            {
                trees.RemoveAt(i);
                tData.treeInstances = trees.ToArray();
                terrain.Flush();
                return true;
            }
        }

        return false;
    }

    private void CheckForDespawn()
    {
        for (int i = activeSpawned.Count - 1; i >= 0; i--)
        {
            var tr = activeSpawned[i];

            if (!tr.isReplaced || tr.spawnedGO == null)
            {
                activeSpawned.RemoveAt(i);
                continue;
            }

            float d = Vector3.Distance(playerCamera.transform.position, tr.worldPos);

            if (d > despawnDistance)
            {
                Destroy(tr.spawnedGO);

                tr.spawnedGO = null;
                tr.isReplaced = false;

                var list = new List<TreeInstance>(tData.treeInstances)
                {
                    tr.original
                };

                tData.treeInstances = list.ToArray();

                activeSpawned.RemoveAt(i);
            }
        }
    }
}

