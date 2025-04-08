using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GridCoordinates : MonoBehaviour
{
    public Color[] debugColors;

    public Vector3Int cellPosition;
    public string tiletype;
    public GameObject currentPrefab;
    private Grid grid;
    public Dictionary<string, int> neighbourTilesDictionnary = new();
    public string majorTile;
    public GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        grid = transform.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);
        //Debug.Log($"");
        //UpdateCurrentNeighbourTiles(GetNeighbourTiles(cellPosition.x == 0, cellPosition.x == 8, cellPosition.z == 0, cellPosition.z == 8));
    }


    public string GetMajorTile()
    {
        Dictionary<string, int> neighbourSplitTypes = new Dictionary<string, int>();
        foreach (KeyValuePair<string, int> kvp in neighbourTilesDictionnary)
        {
            Debug.Log(kvp.Key);
            if (Regex.IsMatch(kvp.Key, "(?<!^)(?=[A-Z])"))
            {
                string[] types = SplitAtUpperCase(kvp.Key).Split(" ");
                Debug.Log("key needing split " + types.Length);
                string type1 = types[1];
                string type2 = FirstLetterToLower(types[2]);
                Debug.Log($"Type 1 : {type1}");
                Debug.Log($"Type 2 : {type2}");

                if (!neighbourSplitTypes.ContainsKey(type1))
                {
                    neighbourSplitTypes.Add(type1, kvp.Value);
                }
                else
                {
                    neighbourSplitTypes[type1]+= kvp.Value;
                }

                //if (type1 == type2)
                //{
                //    return;
                //}
                //else
                if (type1 != type2)
                {
                    if (!neighbourSplitTypes.ContainsKey(type2))
                    {
                        neighbourSplitTypes.Add(type2, kvp.Value);
                    }
                    else
                    {
                        neighbourSplitTypes[type2]+= kvp.Value;
                    }
                }

            }
            else
            {
                if (!neighbourSplitTypes.ContainsKey(kvp.Key))
                {
                    neighbourSplitTypes.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    neighbourSplitTypes[kvp.Key]+= kvp.Value;
                }
            }

        }
        foreach (KeyValuePair<string, int> kvp in neighbourSplitTypes)
        {
            Debug.Log("Final types : " + kvp.Key + " " + kvp.Value);
            if (kvp.Value >= gameManager.comboThreshold)
            {
                majorTile = kvp.Key;
            }
            else
            {
                Debug.LogWarning("No major Tiles determined");
            }
        }
        return majorTile;
    }

    public string SplitAtUpperCase(string str)
    {
        string res = "";
        string[] split = Regex.Split(str, @"(?<!^)(?=[A-Z])");
        for (int i = 0; i < split.Length; i++)
        {
            res += " " + split[i];
        }
        return res;
    }

    public string FirstLetterToLower(string str)
    {
        if (str == null)
        {
            return null;
        }

        if (str.Length > 1)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        return str.ToLower();
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
            GetMajorTile();
            foreach (KeyValuePair<string, int> kvp in neighbourTilesDictionnary)
            {
                Debug.Log(kvp.Key + ": " + kvp.Value);
            }
        }
    }

    private List<GameObject> GetNeighbourTiles(bool isLeft, bool isRight, bool isBot, bool isTop, bool isEvenColumn)
    {
        List<GameObject> neighbourTiles = new List<GameObject>();

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