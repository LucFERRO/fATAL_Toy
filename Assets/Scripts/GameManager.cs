using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DebugVariableHolder
{
    public bool var1;
    public float var2 = 150f;
    public float var3 = 25f;
}
public class GameManager : MonoBehaviour
{
    public string[] tileTypes;
    public GameObject[] tilePrefabs;
    public GameObject[] diceFaces;
    public Color baseDiceFaceColor;
    public Material[] faceMaterials;
    public int maxLockedTiles;

    public Dictionary<int, string> baseTileDictionary = new();
    public Dictionary<int, string> comboDictionary = new();
    [Header("Variations")]
    [Range(0, 5)] public int diceMaxDisappearanceTimer = 1;
    public bool onlyReplacesClosestTile;
    public bool dicesCanReplaceAllHexes;
    public int comboThreshold = 4;



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
        TypeBools = new bool[tileTypes.Length];
        ChooseTileToSpawn(0);
        CreateBaseTileDictionary();
        CreateComboTileDictionary();
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
                Debug.Log(newKey + " " + kvp.Value + "_" + kvp2.Value);
            }
        }

        return comboDictionary;
    }

    public void ChooseTileToSpawn(int tileTypeId)
    {
        bool[] newTypeBoolArray = new bool[tileTypes.Length];
        for (int i = 0; i < tileTypes.Length; i++)
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
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (TypeBools[i])
            {
                chosenTileType = tileTypes[i];
                chosenPrefab = tilePrefabs[i];
            }
        }
    }
}