using UnityEngine;

public class GridNeighbourHandler : MonoBehaviour, IDataPersistence
{
    public NeighbourTileProcessor[] neighbourTileProcessors;
    public GameObject[] neighbourTileGOs;

    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    void Start()
    {
        neighbourTileProcessors = new NeighbourTileProcessor[neighbourTileGOs.Length];
        UpdateNeighbourTiles();
    }

    public void UpdateNeighbourTiles()
    {
        for (int i = 0; i < neighbourTileGOs.Length; i++)
        {
            neighbourTileProcessors[i] = neighbourTileGOs[i].transform.GetChild(0).GetComponent<NeighbourTileProcessor>();
        }
    }

    public void LoadData(GameData data)
    {
       data.mapElevationDict.TryGetValue(id, out Vector3 elevation);
            transform.position = new Vector3(transform.position.x, elevation.y, transform.position.z);
        data.mapTypesDict.TryGetValue(id, out GameObject mapType);

        if (mapType == null)
        {
            Debug.LogError("Map type not found for id: " + id);
            return;
        }

        DestroyImmediate(transform.GetChild(0).gameObject);
        GameObject gameObject = Instantiate(mapType, this.transform);
    }

    public void SaveData(ref GameData data)
    {
        if (data.mapElevationDict.ContainsKey(id))
        {
            data.mapElevationDict.Remove(id);
        }
            data.mapElevationDict.Add(id, transform.position);


        if (data.mapTypesDict.ContainsKey(id))
        {
            data.mapTypesDict.Remove(id);
        }
        data.mapTypesDict.Add(id, transform.GetChild(0).gameObject);
    }
}