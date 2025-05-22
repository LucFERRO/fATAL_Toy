using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public string[] tileTypes;
    public bool tilePatternRandom;
    public float lakitu;
    public float lakituTreshold;

    public SplineSwitcher splineSwitcher;

    public Texture2D cursor;

    public GameObject[] tilePrefabs;
    public GameObject[] diceFaces;
    public Color baseDiceFaceColor;
    public Material[] faceMaterials;

    [HideInInspector] public UnlockManager unlockManager;
    [HideInInspector] public TileSplitManager tileSplitManager;

    private FMOD.Studio.EventInstance transitionEventInstance;

    [Header("LockedTiles")]
    public int maxLockedTiles;

    public Dictionary<int, string> baseTileDictionary = new();
    public Dictionary<int, string> comboDictionary = new();
    [Header("Variations")]
    public float gravity = -9.81f;
    [Range(0, 5)] public int diceMaxDisappearanceTimer = 1;
    public bool onlyReplacesClosestTile;
    public bool dicesCanReplaceAllHexes;
    public int comboThreshold = 4;
    public bool neighbourColorEnabled;
    public bool isPreviewing;

    [Header("Debug")]
    public GameObject debugUIGameObject;
    public bool debugUI;
    public bool DebugUI
    {
        get { return debugUI; }
        set
        {
            debugUI = value;
            debugUIGameObject.SetActive(value);
        }
    }

    public GameObject chosenPrefab;
    public bool[] typeBools;
    public bool[] TypeBools
    {
        get { return typeBools; }
        set
        {
            typeBools = value;
            UpdateChosenTile();
        }
    }
    public string chosenTileType;
    void Start()
    {
        unlockManager = GetComponent<UnlockManager>();
        baseDiceFaceColor = diceFaces[0].transform.GetChild(0).GetComponent<Image>().color;
        TypeBools = new bool[Enum.GetNames(typeof(TileType)).Length];
        ChooseTileToSpawn(1);
        CreateBaseTileDictionary();
        CreateComboTileDictionary();
        Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
        Physics.gravity = new Vector3(0, gravity, 0);
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

        transitionEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Transition");
    }

    private Vector3 AverageClusterCenter(List<GameObject> tiles)
    {
        Vector3 average = Vector3.zero;
        foreach (GameObject tile in tiles)
        {
            if (tile == null)
            {
                continue;
            }
            average += tile.transform.position;
        }
        average /= tiles.Count;
        return average;
    }

    public IEnumerator TransitionSoundsCoroutine(List<GameObject> tiles, float time, int transitionLevel)
    {
        Vector3 center = AverageClusterCenter(tiles);

        yield return new WaitForSeconds(time);
        Debug.Log($"center: {center}, transition level: {transitionLevel}");
        transitionEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(center));
        transitionEventInstance.setParameterByName("TransitionLevel", transitionLevel);
        transitionEventInstance.start();
    }

    public void UpdateNeighboursAfterDiceDestroy(List<GameObject> tiles)
    {
        List<GameObject> NeighbourCascade = UpdateNeighboursCascade(tiles);
        StartCoroutine(TransitionSoundsCoroutine(tiles, 0f, 0));
        StartCoroutine(UpdateNeighboursCoroutine(tiles, 0.6f, 0));
        // Tweak le 0.2 en 0.4+ si needed
        if (NeighbourCascade.Count != 0)
        {
            StartCoroutine(TransitionSoundsCoroutine(NeighbourCascade, 0.6f, 1));
            StartCoroutine(UpdateNeighboursCoroutine(NeighbourCascade, 0.7f, 1));
        }
        StartCoroutine(GlobalGridUpdateCoroutine(2f));

    }

    public IEnumerator GlobalGridUpdateCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        tileSplitManager.UpdateObjectivePackage();
    }

    public List<GameObject> UpdateNeighboursCascade(List<GameObject> traveledTiles)
    {
        List<GameObject> traveledTilesNeighbours = new List<GameObject>();

        foreach (GameObject tile in traveledTiles)
        {
            if (tile == null)
            {
                continue;
            }
            //tile.GetComponent<MeshRenderer>().material.color = Color.red;

            GridNeighbourHandler gridNeighbourHandler = tile.transform.parent.GetComponent<GridNeighbourHandler>();
            foreach (GameObject neighbourTile in gridNeighbourHandler.neighbourTileGOs)
            {
                GameObject neighbourChild = neighbourTile.transform.GetChild(0).gameObject;
                //if (Array.IndexOf(traveledTiles, neighbourChild) < 0 && !traveledTilesNeighbours.Contains(neighbourChild))
                if (!traveledTiles.Contains(neighbourChild) && !traveledTilesNeighbours.Contains(neighbourChild))
                {
                    traveledTilesNeighbours.Add(neighbourChild);
                }
            }
        }

        foreach (GameObject surroundingTiles in traveledTilesNeighbours)
        {
            //surroundingTiles.GetComponent<MeshRenderer>().material.color = Color.blue;

            GridNeighbourHandler gridNeighbourHandler = surroundingTiles.transform.parent.GetComponent<GridNeighbourHandler>();
            gridNeighbourHandler.UpdateNeighbourTiles();

            NeighbourTileProcessor processor = surroundingTiles.GetComponent<NeighbourTileProcessor>();
            processor.GetNeighbourTiles();
            processor.UpdateCurrentNeighbourTiles();
            processor.GetMajorTile();
            processor.UpdateComboTile();
        }
        return traveledTilesNeighbours;
    }

    private IEnumerator UpdateNeighboursCoroutine(List<GameObject> tiles, float time, int transitionLevel)
    {
        Debug.Log(tiles.Count + ", " + transitionLevel);
        yield return new WaitForSeconds(time);

        foreach (GameObject tile in tiles)
        {
            if (tile == null)
            {
                continue;
            }

            GridNeighbourHandler gridNeighbourHandler = tile.transform.parent.GetComponent<GridNeighbourHandler>();
            gridNeighbourHandler.UpdateNeighbourTiles();
            //A remettre si jamais? mais devrait pas
            //gridNeighbourHandler?.UpdateNeighbourTiles();

            NeighbourTileProcessor processor = tile.GetComponent<NeighbourTileProcessor>();
            processor.GetNeighbourTiles();
            processor.UpdateCurrentNeighbourTiles();
            processor.GetMajorTile();
            //Debug.Log($"name {tile.name},tileType: {tile.GetComponent<NeighbourTileProcessor>().tileType}, majorTile: {tile.GetComponent<NeighbourTileProcessor>().majorTile}");
            processor.UpdateComboTile();
            //Debug.Log($"name {tile.name} Update {boo}");

        }
    }

    public void ToggleDebugUI()
    {
        DebugUI = !DebugUI;
    }

    private void CreateBaseTileDictionary()
    {
        baseTileDictionary.Clear();

        baseTileDictionary.Add(1, "mountain");
        baseTileDictionary.Add(2, "lake");
        baseTileDictionary.Add(3, "plain");
        baseTileDictionary.Add(4, "forest");
    }

    private Dictionary<int, string> CreateComboTileDictionary()
    {
        comboDictionary = new Dictionary<int, string>();


        foreach (KeyValuePair<int, string> kvp in baseTileDictionary)
        {
            foreach (KeyValuePair<int, string> kvp2 in baseTileDictionary)
            {
                int newKey = 10 * kvp.Key + kvp2.Key;
                if (!baseTileDictionary.ContainsKey(newKey))
                {
                    comboDictionary.Add(newKey, kvp.Value + "_" + kvp2.Value);
                }
                //Debug.Log(newKey + " " + kvp.Value + "_" + kvp2.Value);
            }
        }

        return comboDictionary;
    }

    public void ChooseTileToSpawn(int tileTypeId)
    {
        bool[] newTypeBoolArray = new bool[Enum.GetNames(typeof(TileType)).Length];
        for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; i++)
        {
            if (i == tileTypeId)
            {
                newTypeBoolArray[i] = true;
            }

            else
            {
                newTypeBoolArray[i] = false;
            }
        }
        TypeBools = newTypeBoolArray;
    }

    private void UpdateChosenTile()
    {
        for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; i++)
        {
            if (TypeBools[i])
            {
                chosenTileType = Enum.GetNames(typeof(TileType))[i];
                chosenPrefab = tilePrefabs[i];
            }
        }
    }
}