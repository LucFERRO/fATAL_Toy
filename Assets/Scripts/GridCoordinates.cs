using System.Collections.Generic;
using UnityEngine;

public class GridCoordinates : MonoBehaviour
{
    public Color[] debugColors;

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
            Debug.Log($"{transform.name}: {cellPosition}");
            UpdateCurrentNeighbourTiles(GetNeighbourTiles(cellPosition.x == 0, cellPosition.x == 8, cellPosition.z == 0, cellPosition.z == 8, cellPosition.z % 2 == 0));
            foreach (KeyValuePair<string, int> kvp in neighbourTilesDictionnary)
            {
                Debug.Log(kvp.Key + ": " + kvp.Value);
            }
        }
    }

    private List<GameObject> GetNeighbourTiles(bool isLeft, bool isRight, bool isBot, bool isTop, bool isEvenColumn)
    {
        List<GameObject> neighbourTiles = new List<GameObject>();
        //if (isEvenColumn)
        //{
        //    for (int i = (isLeft ? 1 : -1); i < (isRight ? 1 : 2); i++)
        //    {
        //        for (int j = (isBot ? 0 : -1); j < (isTop ? 1 : 2); j++)
        //        {
        //            neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + i, cellPosition.y, cellPosition.z + j)));
        //        }
        //    }
        //}
        //else
        //{
        //    for (int i = (isLeft ? 1 : 0); i < (isRight ? 1 : 2); i++)
        //    {
        //        for (int j = (isBot ? 0 : -1); j < (isTop ? 1 : 2); j++)
        //        {
        //            neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + i, cellPosition.y, cellPosition.z + j)));
        //        }
        //    }
        //}
        if (isEvenColumn)
        {
            if (!isLeft)
            {
                neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x - 1, cellPosition.y, cellPosition.z), 0));
                if (!isTop)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x, cellPosition.y, cellPosition.z + 1), 1));
                }
                if (!isBot)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x, cellPosition.y, cellPosition.z - 1), 2));
                }
            }

            if (!isRight)
            {
                if (!isTop)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z + 1), 3));
                }
                neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z), 4));
                if (!isBot)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z - 1), 5));
                }
            }
        }
        else
        {
            if (!isLeft)
            {
                neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x, cellPosition.y, cellPosition.z), 0));

                if (!isTop)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x, cellPosition.y, cellPosition.z + 1), 1));
                }
                if (!isBot)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x, cellPosition.y, cellPosition.z - 1), 2));
                }
            }

            if (!isRight)
            {
                if (!isTop)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z + 1), 3));
                }
                neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + 2, cellPosition.y, cellPosition.z), 4));
                if (!isBot)
                {
                    neighbourTiles.Add(GetTileAtCoordinates(new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z - 1), 5));
                }
            }
        }
        return neighbourTiles;
    }

    private GameObject GetTileAtCoordinates(Vector3Int cellCoordinates, int id)
    {
        Vector3 coordinates = grid.CellToWorld(cellCoordinates);
        coordinates.y += 10;
        GameObject foundObject;
        Ray ray = new Ray(coordinates, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        Debug.Log(cellCoordinates);
        foundObject = hit.collider.gameObject;
        //foundObject.GetComponent<MeshRenderer>().material.color = debugColors[id];
        //Debug.Log($"{foundObject.name} : {foundObject.GetComponent<GridCoordinates>().tiletype} at coordinates {cellCoordinates}");
        return foundObject;
    }
}