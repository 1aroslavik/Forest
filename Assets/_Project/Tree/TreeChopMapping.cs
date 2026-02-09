using UnityEngine;

[CreateAssetMenu(menuName = "Tree/Chop Mapping")]
public class TreeChopMapping : ScriptableObject
{
    public GameObject terrainPrefab;    // prefab в Terrain Tree Prototype
    public GameObject choppablePrefab;  // prefab для рубки
}
