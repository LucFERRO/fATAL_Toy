using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Collections;
using System.Linq;

public class NeighbourTileProcessor : MonoBehaviour
{
    [Header("References")]
    public static GameManager gameManager;
    private static PhysicalDiceSpawner diceSpawner;
    private static UnlockManager unlockManager;
    private static Camera cam;
    [Header("Combo")]
    public string tileType;
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
    [Header("FMOD")]
    private FMOD.Studio.EventInstance lockStatusEventInstance;
    private FMOD.Studio.EventInstance tileBounceEventInstance;
    private FMOD.Studio.EventInstance tileUnlockEventInstance;

    public bool IsLocked
    {
        get { return isLocked; }
        set
        {
            isLocked = value;
            currentLockedTiles += value ? 1 : -1;
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
        if (cam == null)
        {
            cam = Camera.main;
        }
        if (gameManager == null)
        {
            GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameManager");
            gameManager = gameManagerGO.GetComponent<GameManager>();
            unlockManager = gameManager.GetComponent<UnlockManager>();
        }
        if (diceSpawner == null)
        {
            diceSpawner = GameObject.FindGameObjectWithTag("DiceSpawner").GetComponent<PhysicalDiceSpawner>();
        }
        grid = transform.parent.parent.GetComponent<Grid>();
        cellPosition = grid.WorldToCell(transform.position);
        InitiateSoundInstances();
    }
    private void InitiateSoundInstances()
    {
        lockStatusEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Lock");
        lockStatusEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        tileBounceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/DiceOnTile");
        tileUnlockEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/TilesUnlocking");
        //if (tileType == TileType.forest.ToString() || tileType == TileType.mountain.ToString())
        //{
        //    tileBounceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Dice bouncing-Glockenspiel");
        //}
        //else
        //{
        //    tileBounceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Dice bouncing-Cello");
        //}
        tileBounceEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void PlayTileBounceSound(int nbOfBounces)
    {
        tileBounceEventInstance.setParameterByName("numberOfTiles", nbOfBounces);
        tileBounceEventInstance.start();
    }

    public string GetMajorTile()
    {
        Dictionary<string, int> neighbourSplitTypes = new Dictionary<string, int>();

        foreach (KeyValuePair<string, int> kvp in neighbourTilesDictionnary)
        {
            string[] splitTypes = Regex.Split(kvp.Key, @"(?<!^)(?=[A-Z])");

            HashSet<string> uniqueTypes = new HashSet<string>(splitTypes);

            foreach (string type in uniqueTypes)
            {
                if (neighbourSplitTypes.ContainsKey(type))
                {
                    neighbourSplitTypes[type] += kvp.Value;
                }
                else
                {
                    neighbourSplitTypes[type] = kvp.Value;
                }
            }
        }

        string potentialMajorTile = null;
        int maxCount = 0;
        foreach (KeyValuePair<string, int> entry in neighbourSplitTypes)
        {
            if (entry.Value < gameManager.comboThreshold)
            {
                continue;
            }
            if (entry.Value > maxCount)
            {
                potentialMajorTile = entry.Key;
                maxCount = entry.Value;
            }
        }
        if (potentialMajorTile == "empty")
        {
            potentialMajorTile = null;
        }
        majorTile = potentialMajorTile ?? "";

        return majorTile;
    }

    public void UpdateCurrentNeighbourTiles()
    {

        neighbourTilesDictionnary.Clear();
        for (int i = 0; i < neighbourTiles.Length; i++)
        {
            string newTileTypeNeighbour = neighbourTiles[i].tileType;
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
        //if (Input.GetKeyDown(KeyCode.T) && !transform.CompareTag("BaseHex"))
        if (Input.GetKeyDown(KeyCode.T))
        {
            UpdateComboTile();
        }
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    for (int i = 0; i < transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs.Length; i++)
        //    {
        //        Color randomColor = UnityEngine.Random.ColorHSV();
        //        transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs[i].transform.GetChild(0).GetComponent<MeshRenderer>().material.color = randomColor;
        //    }
        //}
        if (Input.GetMouseButtonDown(1) && !gameManager.isPreviewing && gameManager.transform.childCount == 0)
        {
            ToggleLockTile();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateHex();
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

    private string GetComboTilev0()
    {

        int typeValue1 = 0;
        int typeValue2 = 0;
        int typeValue3 = 0;

        //Debug.Log("GetComboTile launched" + gameManager.baseTileDictionary.Count);
        foreach (KeyValuePair<int, string> kvp in gameManager.baseTileDictionary)
        {
            //Debug.Log("ENTERING THE BOUUCLE");
            if (kvp.Value == tileType)
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

    public string GetComboTile()
    {
        if (string.IsNullOrEmpty(majorTile))
        {
            return tileType;
        }

        string[] tiles = new string[] { tileType, majorTile };
        Array.Sort(tiles, StringComparer.Ordinal);
        if (tileType == "empty")
        {
            tiles[0] = tiles[1];
        }
        string newCombo = tiles[0] + FirstLetterToUpper(tiles[1]);
        return newCombo;
    }

    public void UpdateComboTile()
    {
        if (majorTile.Length == 0)
        {
            return;
        }
        for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; i++)
        {
            // array.sort GetComboTile dans gameManager.tiletype
            if (Enum.GetNames(typeof(TileType))[i] == GetComboTile())
            {
                Enum.TryParse(Enum.GetNames(typeof(TileType))[i], out TileType tileType);
                if (gameManager.unlockManager.HandleUnlockComboTile(tileType))
                {
                    Debug.Log($"Combo tile {Enum.GetNames(typeof(TileType))[i]} unlocked!");
                    GameObject newSporeItem = Instantiate(gameManager.unlockManager.sporeItem, transform.position, Quaternion.identity);
                    for (int j = 0; j < newSporeItem.transform.GetChild(0).childCount; j++)
                    {
                        newSporeItem.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>().material = gameManager.faceMaterials[i];
                    }
                    // AYMERIC SON PLEASE //// JESUS M'AGGRESSE
                    tileUnlockEventInstance.start();
                }
                gameManager.chosenPrefab = gameManager.tilePrefabs[i];
                UpdateHex();
                gameManager.chosenPrefab = gameManager.tilePrefabs[0];
                gameManager.chosenTileType = Enum.GetNames(typeof(TileType))[0];
            }
        }
    }
    private void ToggleLockTile()
    {
        GlowingHexes glowingHex = gameObject.GetComponent<GlowingHexes>();
        if (currentLockedTiles == gameManager.maxLockedTiles && !IsLocked)
        {
            return;
        }
        if (isLocked)
        {
            lockStatusEventInstance.setParameterByName("LockState", 1);
            lockStatusEventInstance.start();
            //Debug.Log("Lock released");
            //lockStatusEventInstance.release();
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Lock", transform.position);
            if (transform.parent.childCount > 1)
            {
                for (int i = 1; i < transform.parent.childCount; i++)
                {
                    GameObject particles = transform.parent.transform.GetChild(i).gameObject;
                    particles.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    particles.GetComponent<ParticlesToBeDestroyed>().isToBeDestroyed = true;
                }
                StartCoroutine(glowingHex.ClearParticlesCoroutine(3f));
            }
            GetNeighbourTiles();
            UpdateCurrentNeighbourTiles();
            GetMajorTile();
            UpdateComboTile();
        }
        else
        {
            lockStatusEventInstance.setParameterByName("LockState", 0);
            lockStatusEventInstance.start();
            //Debug.Log("Lock acquired");
            //lockStatusEventInstance.release();
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Lock", transform.position);
            if (transform.parent.childCount == 1)
            {
                GameObject spark = Instantiate(sparks, transform.parent);
                Vector3 fixedHeight = spark.transform.position;
                fixedHeight.y += 0.6f;
                spark.transform.position = fixedHeight;
                GameObject particles = transform.parent.transform.GetChild(1).gameObject;
                particles.GetComponent<ParticleSystem>().Play(true);
            }
            else if (transform.parent.childCount > 1)
            {
                int destroyedParts = 0;
                for (int i = 1; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.transform.GetChild(i).GetComponent<ParticlesToBeDestroyed>().isToBeDestroyed)
                    {
                        destroyedParts++;
                    }
                    ;
                }
                if (destroyedParts > 0)
                {
                    GameObject spark = Instantiate(sparks, transform.parent);
                    Vector3 fixedHeight = spark.transform.position;
                    fixedHeight.y += 0.6f;
                    spark.transform.position = fixedHeight;
                    GameObject particles = transform.parent.transform.GetChild(1).gameObject;
                    particles.GetComponent<ParticleSystem>().Play(true);
                }
            }
        }
        IsLocked = !IsLocked;
        unlockManager.HandleLockIconUnlocks(gameManager.maxLockedTiles);
        glowingHex.ToggleLock(IsLocked);
    }


    private void UpdateHex()
    {
        Vector3Int hexPosition = cellPosition;
        GameObject newHex = Instantiate(gameManager.chosenPrefab, transform.parent);
        newHex.GetComponent<GlowingHexes>().ToggleMaterialize(true);
        StartCoroutine(newHex.GetComponent<GlowingHexes>().TransitionAppear());
        newHex.transform.position = transform.position;
        newHex.transform.SetAsFirstSibling();

        NeighbourTileProcessor newGridCoordinates = newHex.GetComponent<NeighbourTileProcessor>();
        newGridCoordinates.tileType = gameManager.chosenPrefab.GetComponent<NeighbourTileProcessor>().tileType;
        newGridCoordinates.cellPosition = hexPosition;
        StartCoroutine(GetComponent<GlowingHexes>().TransitionDisappear());
        transform.parent.GetComponent<GridNeighbourHandler>().UpdateNeighbourTiles();
    }
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Vector3Int hexPosition = cellPosition;
    //    //Debug.Log("Hex Coordinates:" + hexPosition);

    //    //gridCoordinates.tiletype = gameManager.chosenTileType;
    //    currentPrefab = gameManager.chosenPrefab;
    //}
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