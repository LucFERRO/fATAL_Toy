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
    public Material[] faceMaterials;
    [Header("Variations")]
    [Range(0,5)] public int diceMaxDisappearanceTimer = 1;
    public bool onlyReplacesClosestTile;
    public bool dicesCanReplaceAllHexes;


    [Header("Debug")]
    public GameObject debugUIGameObject;
    public bool mountain;
    public bool lake;
    public bool forest;
    public bool plain;
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
    public string chosenTileType;

    public DebugVariableHolder instance = new DebugVariableHolder();
    void Start()
    {
        typeBools = new bool[tileTypes.Length];
        typeBools[0] = true;
    }

    void Update()
    {
        UpdateChosenTile();

    }

    public void ToggleDebugUI()
    {
        DebugUI = !DebugUI;
    }

    public void DebugChooseTile(int tileTypeId)
    {
        for (int i = 0; i < tileTypes.Length; i++) 
        {
            if (i == tileTypeId)
            {
                typeBools[i] = true;
            }

            else
            {
                typeBools[i] = false;
            }
        }
    }

    private void UpdateChosenTile()
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (typeBools[i])
            {
                chosenTileType = tileTypes[i];
                chosenPrefab = tilePrefabs[i];
            }
        }
    }
}