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
    [Header("Variations")]
    [Range(0,5)] public int diceMaxDisappearanceTimer = 1;
    public bool onlyReplacesClosestTile;
    public bool dicesCanReplaceAllHexes;


    [Header("Debug")]
    public bool mountain;
    public bool lake;
    public bool forest;
    public bool plain;

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

    private void UpdateChosenTile()
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (typeBools[i])
            {
                chosenTileType = tileTypes[i];
            }
        }

        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (typeBools[i])
            {
                chosenPrefab = tilePrefabs[i];
            }
        }
    }
}