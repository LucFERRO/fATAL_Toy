using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class NeighbourTileProcessor : MonoBehaviour
{
    [Header("References")]
    private GameManager gameManager;
    private PhysicalDiceSpawner diceSpawner;
    private Camera cam;
    [Header("Combo")]
    public string tiletype;
    private GameObject chosenComboTile;
    public GameObject currentPrefab;
    public Dictionary<string, int> neighbourTilesDictionnary = new();

    private NeighbourTileProcessor[] neighbourTiles;
    private int neighbourNumber;
    public string majorTile;
    [Header("Coordinates")]
    public Vector3Int cellPosition;
    private Grid grid;
    [Header("Locking")]
    static public int currentLockedTiles;
    public bool isLocked;
    public GameObject sparks;
    public bool IsLocked
    {
        get { return isLocked; }
        set
        {
            isLocked = value;
            currentLockedTiles += value ? 1 : -1;
            Debug.Log($"CHANGED ISLOCKED : CURRENTLOCKEDTILES VALUE {(value ? 1 : -1)} TO {currentLockedTiles}");
        }
    }

    [Header("TRASH")]
    public Color[] debugColors;

    void Awake()
    {
        neighbourNumber = transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs.Length;
        neighbourTiles = new NeighbourTileProcessor[neighbourNumber];
        for (int i = 0; i < neighbourNumber; i++)
        {
            neighbourTiles[i] = transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs[i].GetComponent<NeighbourTileProcessor>();
        }

        debugColors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.white };
        cam = Camera.main;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        diceSpawner = GameObject.FindGameObjectWithTag("DiceSpawner").GetComponent<PhysicalDiceSpawner>();
        grid = transform.parent.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);
    }


    public string GetMajorTile()
    {
        Dictionary<string, int> neighbourSplitTypes = new Dictionary<string, int>();
        foreach (KeyValuePair<string, int> kvp in neighbourTilesDictionnary)
        {
            if (Regex.IsMatch(kvp.Key, "(?<!^)(?=[A-Z])"))
            {
                string[] types = SplitAtUpperCase(kvp.Key).Split(" ");
                //Debug.Log("key needing split " + types.Length);
                string type1 = types[1];
                string type2 = FirstLetterToLower(types[2]);
                //Debug.Log($"Type 1 : {type1}");
                //Debug.Log($"Type 2 : {type2}");

                if (!neighbourSplitTypes.ContainsKey(type1))
                {
                    neighbourSplitTypes.Add(type1, kvp.Value);
                }
                else
                {
                    neighbourSplitTypes[type1] += kvp.Value;
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
                        neighbourSplitTypes[type2] += kvp.Value;
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
                    neighbourSplitTypes[kvp.Key] += kvp.Value;
                }
            }

        }
        foreach (KeyValuePair<string, int> kvp in neighbourSplitTypes)
        {
            if (kvp.Value < gameManager.comboThreshold)
            {
                continue;
            }
            else
            {
                majorTile = kvp.Key;
                break;
            }
        }
        if (majorTile == "empty")
        {
            majorTile = "";
        }
        return majorTile;
    }

    public void UpdateCurrentNeighbourTiles()
    {

        neighbourTilesDictionnary.Clear();
        for (int i = 0; i < neighbourTiles.Length; i++)
        {
            string newTileTypeNeighbour = neighbourTiles[i].tiletype;
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
            GetNeighbourTiles();
            UpdateCurrentNeighbourTiles();
            GetMajorTile();
            foreach (KeyValuePair<string, int> kvp in neighbourTilesDictionnary)
            {
                Debug.Log(kvp.Key + ": " + kvp.Value);
            }
        }
        if (Input.GetKeyDown(KeyCode.T) && !transform.CompareTag("BaseHex"))
        {
            UpdateComboTile();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            for (int i = 0; i < transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs.Length; i++)
            {
                Color randomColor = UnityEngine.Random.ColorHSV();
                transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs[i].transform.GetChild(0).GetComponent<MeshRenderer>().material.color = randomColor;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ToggleLockTile();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateHex();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(currentLockedTiles);
        }
    }

    public void GetNeighbourTiles()
    {
        for (int i = 0; i < neighbourNumber; i++)
        {
            neighbourTiles[i] = transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs[i].transform.GetChild(0).GetComponent<NeighbourTileProcessor>();
        }
    }

    private GameObject GetTileAtCoordinates(Vector3Int cellCoordinates, int id)
    {
        Vector3 coordinates = grid.CellToWorld(cellCoordinates);
        coordinates.y += 10;
        GameObject foundObject;
        Ray ray = new Ray(coordinates, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        foundObject = hit.collider.gameObject;
        if (gameManager.neighbourColorEnabled)
        {
            foundObject.GetComponent<MeshRenderer>().material.color = debugColors[id];
            //Debug.Log($"{foundObject.name} : {foundObject.GetComponent<NeighbourTileProcessor>().tiletype} at coordinates {cellCoordinates}");
        }
        return foundObject;
    }

    private string GetComboTile()
    {

        int typeValue1 = 0;
        int typeValue2 = 0;
        int typeValue3 = 0;

        //Debug.Log("GetComboTile launched" + gameManager.baseTileDictionary.Count);
        foreach (KeyValuePair<int, string> kvp in gameManager.baseTileDictionary)
        {
            //Debug.Log("ENTERING THE BOUUCLE");
            if (kvp.Value == tiletype)
            {
                typeValue1 = kvp.Key * 10;
                //Debug.Log("Debug ligne 40" + kvp.Key);
            }

            if (kvp.Value == majorTile)
            {
                typeValue2 = kvp.Key;
            }
        }

        //FirstLetterToUpper(type2);

        typeValue3 = typeValue1 + typeValue2;
        //Debug.Log($"Type values: {typeValue1} / {typeValue2} / {typeValue3}");
        gameManager.comboDictionary.TryGetValue(typeValue3, out string comboTile);

        string[] types = comboTile.Split("_");
        string type1 = types[0];
        string type2 = types[1];
        //Debug.Log($"{type1} / {type2} ");

        string[] involvedTypes = new string[2];
        involvedTypes[0] = type1;
        involvedTypes[1] = type2;
        Array.Sort(involvedTypes, (x, y) => String.Compare(involvedTypes[0], involvedTypes[1]));

        if (involvedTypes[0] == "forest")
        {
            foreach (KeyValuePair<int, string> kvp in gameManager.baseTileDictionary)
            {
                if (involvedTypes[1] == kvp.Value)
                {
                    comboTile = involvedTypes[0] + FirstLetterToUpper(involvedTypes[1]);
                }
            }
        }
        else if (involvedTypes[0] == "lake")
        {
            foreach (KeyValuePair<int, string> kvp in gameManager.baseTileDictionary)
            {
                if (involvedTypes[1] == kvp.Value)
                {
                    comboTile = involvedTypes[0] + FirstLetterToUpper(involvedTypes[1]);
                }
            }
        }
        else
        {
            foreach (KeyValuePair<int, string> kvp in gameManager.baseTileDictionary)
            {
                if (involvedTypes[1] == kvp.Value)
                {
                    comboTile = involvedTypes[0] + FirstLetterToUpper(involvedTypes[1]);
                }
            }
        }
        return comboTile;
    }

    public void UpdateComboTile()
    {
        if (majorTile.Length == 0)
        {
            return;
        }
        for (int i = 0; i < gameManager.tileTypes.Length; i++)
        {
            // array.sort GetComboTile dans gameManager.tiletype
            if (gameManager.tileTypes[i] == GetComboTile())
            {
                gameManager.chosenPrefab = gameManager.tilePrefabs[i];
                UpdateHex();
                gameManager.chosenPrefab = gameManager.tilePrefabs[0];
                gameManager.chosenTileType = gameManager.tileTypes[0];
            }
        }
    }
    private void ToggleLockTile()
    {
        if (currentLockedTiles == gameManager.maxLockedTiles && !IsLocked)
        {
            return;
        }
        IsLocked = !IsLocked;
        if (isLocked)
        {
            GameObject spark = Instantiate(sparks, transform.parent);
            Vector3 fixedHeight = spark.transform.position;
            fixedHeight.y += 0.6f;
            spark.transform.position = fixedHeight;
            //Quaternion newRotation = Quaternion.Euler(spark.transform.rotation.x, spark.transform.rotation.y, spark.transform.rotation.z);
            //newRotation.x = -90;
            //spark.transform.rotation = newRotation;
            Debug.Log(spark.transform.position);
            Debug.Log(spark.transform.rotation);
        }
        else
        {
        }
        gameObject.GetComponent<GlowingHexes>().ToggleLock(IsLocked ? true : false);

    }
    private void UpdateHex()
    {
        Vector3Int hexPosition = cellPosition;
        GameObject newHex = Instantiate(gameManager.chosenPrefab, transform.parent);
        newHex.transform.position = transform.position;

        NeighbourTileProcessor newGridCoordinates = newHex.GetComponent<NeighbourTileProcessor>();
        newGridCoordinates.tiletype = gameManager.chosenPrefab.GetComponent<NeighbourTileProcessor>().tiletype;
        newGridCoordinates.cellPosition = hexPosition;

        Destroy(gameObject);
        transform.parent.GetComponent<GridNeighbourHandler>().UpdateNeighbourTiles();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3Int hexPosition = cellPosition;
        //Debug.Log("Hex Coordinates:" + hexPosition);

        //gridCoordinates.tiletype = gameManager.chosenTileType;
        currentPrefab = gameManager.chosenPrefab;
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
    public string FirstLetterToUpper(string str)
    {
        if (str == null)
        {
            return null;
        }

        if (str.Length > 1)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        return str.ToUpper();
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
}