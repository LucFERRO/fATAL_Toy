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

[Serializable]
public class DebugVariableHolder
{
    public bool var1;
    public float var2 = 150f;
    public float var3 = 25f;
}
public class GameManager : MonoBehaviour
{
    //public string[] tileTypes;
    public bool tilePatternRandom;

    public GameObject[] tilePrefabs;
    public GameObject[] diceFaces;
    public Color baseDiceFaceColor;
    public Material[] faceMaterials;

    [HideInInspector] public UnlockManager unlockManager;

    [Header("Objectives")]
    public Canvas mainCanvas;
    public Canvas winCanvas;
    public GameObject objectiveListGo;
    public bool[] objectiveBools;
    public bool[] ObjectiveBools
    {
        get { return objectiveBools; }
        set
        {
            objectiveBools = value;
            UpdateObjectives();
        }
    }
    public string[] objectiveStrings;


    [Header("LockedTiles")]
    public GameObject[] testCascade;
    public int maxLockedTiles;
    public int currentlyLockedTiles;

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

    public DebugVariableHolder instance = new DebugVariableHolder();
    void Start()
    {
        unlockManager = GetComponent<UnlockManager>();
        baseDiceFaceColor = diceFaces[0].transform.GetChild(0).GetComponent<Image>().color;
        TypeBools = new bool[Enum.GetNames(typeof(TileType)).Length];
        ChooseTileToSpawn(0);
        CreateBaseTileDictionary();
        CreateComboTileDictionary();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        Physics.gravity = new Vector3(0, gravity, 0);
        int objectiveNumber = objectiveListGo.transform.childCount;
        objectiveBools = new bool[objectiveNumber];
        ObjectiveBools = new bool[objectiveNumber];
        objectiveStrings = new string[objectiveNumber];
        for (int i = 0; i < objectiveNumber; i++)
        {
            objectiveStrings[i] = objectiveListGo.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        }
    }

    private void Update()
    {
        Cursor.visible = !isPreviewing;
        if (Input.GetKeyDown(KeyCode.M))
        {
            UpdateObjectives();
        }
    }

    public void ChooseObjectiveToComplete(int objectiveId)
    {
        bool[] newObjectiveBoolArray = new bool[objectiveBools.Length];
        for (int i = 0; i < objectiveBools.Length; i++)
        {
            if (i == objectiveId)
            {
                newObjectiveBoolArray[i] = !objectiveBools[i];
            }

            else
            {
                newObjectiveBoolArray[i] = objectiveBools[i];
            }
        }
        ObjectiveBools = newObjectiveBoolArray;
    }

    private void UpdateObjectives()
    {
        for (int i = 0; i < objectiveBools.Length; i++)
        {
            TextMeshProUGUI textMeshProElement = objectiveListGo.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            if (objectiveBools[i])
            {
                textMeshProElement.color = Color.green;
                textMeshProElement.fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                textMeshProElement.color = Color.white;
                textMeshProElement.fontStyle = FontStyles.Normal;
            }
        }

        if (!objectiveBools.All(b => b))
        {
            return;
        }

        Debug.Log("All objectives are done!");
        //mainCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(true);
        Invoke("ReloadScene", 2f);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateNeighboursAfterDiceDestroy(List<GameObject> tiles)
    {
        StartCoroutine(UpdateNeighboursCoroutine(tiles, 0.6f));
        // Tweak le 0.2 en 0.4+ si needed
        StartCoroutine(UpdateNeighboursCoroutine(UpdateNeighboursCascade(tiles), 0.6f));
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

    private IEnumerator UpdateNeighboursCoroutine(List<GameObject> tiles, float time)
    {
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