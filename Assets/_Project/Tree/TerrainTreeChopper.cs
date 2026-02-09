using System.Collections.Generic;
using UnityEngine;

public class TerrainTreeChopper : MonoBehaviour
{
    [Header("Terrain")]
    public Terrain terrain;

    [Header("Choppable prefabs (order = Terrain Tree Prototypes)")]
    public GameObject[] treePrefabs;   // 0..3

    [Header("Settings")]
    public float searchRadius = 2.5f;

    private TerrainData data;

    void Awake()
    {
        if (!terrain)
            terrain = Terrain.activeTerrain;

        if (!terrain)
        {
            Debug.LogError("TerrainTreeChopper: No Terrain found!");
            enabled = false;
            return;
        }

        data = terrain.terrainData;
    }

    /// <summary>
    /// Удаляет ближайшее Terrain-дерево и спавнит prefab
    /// с тем же типом и размером
    /// </summary>
    public GameObject TryChopAndSpawn(Vector3 hitPoint)
    {
        var trees = new List<TreeInstance>(data.treeInstances);

        int closestIndex = -1;
        float closestDist = float.MaxValue;

        // 🔍 ищем ближайшее Terrain-дерево
        for (int i = 0; i < trees.Count; i++)
        {
            Vector3 worldPos = TreeToWorld(trees[i]);
            float dist = Vector3.Distance(hitPoint, worldPos);

            if (dist < closestDist && dist <= searchRadius)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        if (closestIndex == -1)
            return null;

        TreeInstance ti = trees[closestIndex];
        int protoIndex = ti.prototypeIndex;

        if (protoIndex < 0 || protoIndex >= treePrefabs.Length)
        {
            Debug.LogError($"No prefab for prototype index {protoIndex}");
            return null;
        }

        // 🌍 позиция дерева
        Vector3 spawnPos = TreeToWorld(ti);

        // ❌ удаляем дерево из Terrain
        trees.RemoveAt(closestIndex);
        data.treeInstances = trees.ToArray();

        // 🔥 ОБЯЗАТЕЛЬНО
        terrain.Flush();

        // 🌳 спавним prefab
        GameObject spawned = Instantiate(
            treePrefabs[protoIndex],
            spawnPos,
            Quaternion.identity
        );

        // 📏 ПРИМЕНЯЕМ РАЗМЕР TERRAIN-ДЕРЕВА
        Vector3 scale = spawned.transform.localScale;
        scale.x *= ti.widthScale;
        scale.z *= ti.widthScale;
        scale.y *= ti.heightScale;
        spawned.transform.localScale = scale;

        // 🔄 (опционально) поворот как у Terrain
        spawned.transform.rotation =
            Quaternion.Euler(0f, ti.rotation * Mathf.Rad2Deg, 0f);

        return spawned;
    }

    // Перевод координат Terrain → World
    private Vector3 TreeToWorld(TreeInstance tree)
    {
        Vector3 p = tree.position;
        p.x *= data.size.x;
        p.y *= data.size.y;
        p.z *= data.size.z;
        return terrain.transform.position + p;
    }
}
