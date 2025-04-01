using System.Collections.Generic;
using UnityEngine;

public class GridCoordinates : MonoBehaviour
{
    public Vector3Int cellPosition;
    public string tiletype;
    public GameObject currentPrefab;
    private Grid grid;
    public Dictionary<string, int> neighbourTilesDictionnary = new();

    void Start()
    {
        grid = transform.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);
        //Debug.Log($"");
        //UpdateCurrentNeighbourTiles(GetNeighbourTiles(cellPosition.x == 0, cellPosition.x == 8, cellPosition.z == 0, cellPosition.z == 8));
    }

    public void UpdateCurrentNeighbourTiles(List<GameObject> neighbourList)
    {
        neighbourTilesDictionnary.Clear();
        for (int i = 0; i < neighbourList.Count; i++)
        {
            string newTileTypeNeighbour = neighbourList[i].GetComponent<GridCoordinates>().tiletype;
            if (!neighbourTilesDictionnary.ContainsKey(newTileTypeNeighbour))
            {
                neighbourTilesDictionnary.Add(newTileTypeNeighbour, 1);
            }
            else
            {
                neighbourTilesDictionnary[newTileTypeNeighbour]++;
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateCurrentNeighbourTiles(GetNeighbourTiles(cellPosition.x == 0, cellPosition.x == 8, cellPosition.z == 0, cellPosition.z == 8));
            Debug.Log(transform.name);
            foreach (KeyValuePair<string,int> kvp in neighbourTilesDictionnary)
            {
                Debug.Log(kvp.Key+": "+kvp.Value);
            }
        }
    }

    private List<GameObject> GetNeighbourTiles(bool isLeft, bool isRight, bool isBot, bool isTop)
    {
        List<GameObject> neighbourTiles = new List<GameObject>();
        for (int i = (isLeft ? 1 : -1); i < (isRight ? 1 : 2); i += 2)
        {
            for (int j = (isBot ? 0 : -1); j < (isTop ? 1 : 2); j++)
            {
                neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + i, cellPosition.y, cellPosition.z + j)));
            }
        }
        return neighbourTiles;
    }

    private GameObject GetTileAtCoordinates(Vector3Int cellCoordinates)
    {
        Vector3 coordinates = grid.CellToWorld(cellCoordinates);
        coordinates.y += 10;
        GameObject foundObject;
        Ray ray = new Ray(coordinates, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        foundObject = hit.collider.gameObject;
        Debug.Log(foundObject.name+": "+foundObject.GetComponent<GridCoordinates>().tiletype);
        return foundObject;
    }
}