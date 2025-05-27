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

    private bool[] isStopingBoolArray;

    public GameObject[] tilePrefabs;
    public GameObject[] diceFaces;
    public Color baseDiceFaceColor;
    public Material[] faceMaterials;

    public bool diceWasChanged;
    public bool diceWasThrown;

    [HideInInspector] public UnlockManager unlockManager;
    [HideInInspector] public TileSplitManager tileSplitManager;

    private FMOD.Studio.EventInstance transitionEventInstance;

    [Header("LockedTiles")]
    public int maxLockedTiles;
    public int minTilesRolled = int.MaxValue;
    public int maxTilesRolled = int.MinValue;
    public string[] faceTypes;

    public int MinTilesRolled => minTilesRolled == int.MaxValue ? -1 : minTilesRolled;
    public int MaxTilesRolled => maxTilesRolled == int.MinValue ? 0 : maxTilesRolled;

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
        Debug.Log($"This game is {(CrossSceneData.isTutorial ? "" : "not")} a tutorial game.");
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
        isStopingBoolArray = new bool[3];
        transitionEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Transition");
        transitionEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
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
        if (transitionLevel == 0)
        {
            transitionEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(center));
            transitionEventInstance.setParameterByName("TransitionLevel", transitionLevel);
            transitionEventInstance.start();
        }

        if (!isStopingBoolArray[transitionLevel] && transitionLevel == 1)
        {
            transitionEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(center));
            transitionEventInstance.setParameterByName("TransitionLevel", transitionLevel);
            transitionEventInstance.start();
        }

        if (!isStopingBoolArray[transitionLevel] && transitionLevel == 2)
        {
            transitionEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(center));
            if (!isStopingBoolArray[transitionLevel - 1])
            {
                transitionEventInstance.setParameterByName("TransitionLevel", transitionLevel);
            } else
            {
                transitionEventInstance.setParameterByName("TransitionLevel", transitionLevel-1);
            }
            transitionEventInstance.start();
        }
    }

    public void UpdateNeighboursAfterDiceDestroy(List<GameObject> tiles)
    {
        int count = tiles?.Count ?? 0;
        if (count > 0)
        {
            if (count < minTilesRolled)
            {
                minTilesRolled = count;
            }

            if (count > maxTilesRolled)
            {
                maxTilesRolled = count;
            }
        }

        List<GameObject> NeighbourCascade = UpdateNeighboursCascade(tiles);

        IEnumerator firstWaveSoundCoroutine = TransitionSoundsCoroutine(tiles, 0f, 0);
        StartCoroutine(firstWaveSoundCoroutine);


        //WAVE 2
        IEnumerator secondWaveUpdateCoroutine = UpdateNeighboursCoroutine(tiles, 0.6f, 1);
        StartCoroutine(secondWaveUpdateCoroutine);


        IEnumerator secondWaveSoundCoroutine = TransitionSoundsCoroutine(NeighbourCascade, 0.7f, 1);
        StartCoroutine(secondWaveSoundCoroutine);

        //WAVE 3
        IEnumerator thirdWaveSoundCoroutine = UpdateNeighboursCoroutine(NeighbourCascade, 1.2f, 2);
        StartCoroutine(thirdWaveSoundCoroutine);

        IEnumerator thirdWaveUpdateCoroutine = TransitionSoundsCoroutine(NeighbourCascade, 1.3f, 2);
        StartCoroutine(thirdWaveUpdateCoroutine);


        //StartCoroutine(firstWaveSoundCoroutine);
        //StartCoroutine(secondWaveSoundCoroutine);
        //StartCoroutine(secondWaveUpdateCoroutine);
        //StartCoroutine(thirdWaveSoundCoroutine);
        //StartCoroutine(thirdWaveUpdateCoroutine);


        StartCoroutine(GlobalGridUpdateCoroutine(2f));
    }

    public int NumberOfCreatedTiles()
    {
        int newTiles = 0;
        for (int i = 0; i < tileSplitManager.transform.childCount; i++)
        {
            List<NeighbourTileProcessor> newProcessors = tileSplitManager.transform.GetChild(i).GetComponentsInChildren<NeighbourTileProcessor>().Where(tile => tile.isNew).ToList();
            if (newProcessors.Count() > 0)
            {
                newTiles++;
            }
        }
        tileSplitManager.ResetAllIsNew();
        return newTiles;
    }

    public IEnumerator GlobalGridUpdateCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        tileSplitManager.UpdateObjectivePackage();
        isStopingBoolArray = new bool[3];
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

            GridNeighbourHandler gridNeighbourHandler = tile.transform.parent.GetComponent<GridNeighbourHandler>();
            foreach (GameObject neighbourTile in gridNeighbourHandler.neighbourTileGOs)
            {
                GameObject neighbourChild = neighbourTile.transform.GetChild(0).gameObject;
                if (!traveledTiles.Contains(neighbourChild) && !traveledTilesNeighbours.Contains(neighbourChild) && !neighbourChild.GetComponent<NeighbourTileProcessor>().isLocked)
                {
                    traveledTilesNeighbours.Add(neighbourChild);
                }
            }
        }

        foreach (GameObject surroundingTiles in traveledTilesNeighbours)
        {
            if (surroundingTiles.GetComponent<NeighbourTileProcessor>().isLocked)
            {
                continue;
            }

            GridNeighbourHandler gridNeighbourHandler = surroundingTiles.transform.parent.GetComponent<GridNeighbourHandler>();
            gridNeighbourHandler.UpdateNeighbourTiles();

            NeighbourTileProcessor processor = surroundingTiles.GetComponent<NeighbourTileProcessor>();
            processor.GetNeighbourTiles();
            processor.UpdateCurrentNeighbourTiles();
            processor.GetMajorTile();
            string tempType = processor.tileType;
            processor.UpdateComboTile();
        }

        return traveledTilesNeighbours;
    }

    private IEnumerator UpdateNeighboursCoroutine(List<GameObject> tiles, float time, int transitionLevel)
    {
        yield return new WaitForSeconds(time);

        foreach (GameObject tile in tiles)
        {
            if (tile == null || tile.GetComponent<NeighbourTileProcessor>().isLocked)
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
            processor.UpdateComboTile();

        }
        int createdTilesThisWave = NumberOfCreatedTiles();
        if (createdTilesThisWave == 0)
        {
            isStopingBoolArray[transitionLevel] = true;
        }
    }

    public void ToggleDebugUI()
    {
        DebugUI = !DebugUI;
    }
    public bool IsDiceMadeOfOneBiome(string targetBiome)
    {
        {
            if (string.IsNullOrEmpty(targetBiome) || faceTypes.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < 6; i++)
            {
                if (faceTypes[i] == null)
                {
                    return false;
                }

                if (!faceTypes[i].Contains(targetBiome, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
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