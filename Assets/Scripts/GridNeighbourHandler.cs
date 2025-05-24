using System;
using UnityEngine;

public class GridNeighbourHandler : MonoBehaviour, IDataPersistence
{
    public NeighbourTileProcessor[] neighbourTileProcessors;
    public GameObject[] neighbourTileGOs;
    public GameManager gameManager;
    public GameObject baseHex;

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

        data.mapTypesDict.TryGetValue(id, out string tileType);
        GameObject mapType = null;

        if (string.IsNullOrEmpty(tileType))
        {
            Debug.LogError("Tile type not found for id: " + id);
            return;
        }

        // Trouver le prefab correspondant au tileType
        if (tileType == "empty")
        {
            mapType = baseHex;
        }
        else
        {
            mapType = gameManager.tilePrefabs[Array.IndexOf(Enum.GetNames(typeof(TileType)), tileType)];
        }

        if (mapType == null)
        {
            Debug.LogError("Prefab not found for tile type: " + tileType);
            return;
        }

        // Détruire l'ancien GameObject et instancier le nouveau
        DestroyImmediate(transform.GetChild(0).gameObject);
        GameObject newHex = Instantiate(mapType, transform.position, transform.rotation, this.transform);

        // Mettre à jour le NeighbourTileProcessor avec le tileType
        NeighbourTileProcessor processor = newHex.GetComponent<NeighbourTileProcessor>();
        if (processor != null)
        {
            processor.tileType = tileType;
        }
    }

    public void SaveData(ref GameData data)
    {
            Debug.Log("Saving data");
        if (data == null)
        {
            Debug.LogError("GameData is null. Cannot save data.");
            return;
        }
        if (data.mapTypesDict == null)
        {
            Debug.LogError("mapTypesDict is null in GameData. Cannot save data.");
            return;
        }
        if (data.mapElevationDict.ContainsKey(id))
        {
            data.mapElevationDict.Remove(id);
        }
        data.mapElevationDict.Add(id, transform.position);

        if (data.mapTypesDict.ContainsKey(id))
        {
            data.mapTypesDict.Remove(id);
        }

        // Récupérer le tileType depuis le NeighbourTileProcessor
        if (transform.childCount == 0)
        {
            Debug.LogError("No child found under GridNeighbourHandler. Cannot save data.");
            return;
        }

        NeighbourTileProcessor processor = transform.GetChild(0).GetComponent<NeighbourTileProcessor>();
        if (processor != null)
        {
            string tileType = processor.tileType; // Assurez-vous que tileType est une propriété publique ou un champ accessible
            data.mapTypesDict.Add(id, tileType);
        }
        else
        {
            Debug.LogError("NeighbourTileProcessor not found on child object.");
        }
    }
}