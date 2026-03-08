using UnityEngine;

[CreateAssetMenu(menuName = "Building/Building Data")]
public class BuildingData : ScriptableObject
{
    public GameObject constructionPrefab;
    public GameObject finishedPrefab;
    public int requiredLogs = 4;
}