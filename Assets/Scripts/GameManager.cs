using System;
using System.Collections.Generic;
using UnityEngine;
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
    public string[] tileTypes;
    public GameObject[] tilePrefabs;
    public GameObject[] diceFaces;
    public Color baseDiceFaceColor;
    public Material[] faceMaterials;
    [Header("Variations")]
    [Range(0,5)] public int diceMaxDisappearanceTimer = 1;
    public bool onlyReplacesClosestTile;
    public bool dicesCanReplaceAllHexes;


    [Header("Debug")]
    public GameObject debugUIGameObject;
    public bool debugUI;
    public bool DebugUI { 
        get { return debugUI;} 
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
        baseDiceFaceColor = diceFaces[0].transform.GetChild(0).GetComponent<Image>().color;
    }

    public void ToggleDebugUI()
    {
        DebugUI = !DebugUI;
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