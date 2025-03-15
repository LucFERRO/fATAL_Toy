using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool mountain;
    public bool lake;
    public bool forest;
    public bool plain;

    public string[] tileTypes;
    public GameObject[] tilePrefabs;
    public GameObject chosenPrefab;
    public bool[] typeBools;
    public string chosenTileType;

    void Start()
    {
        typeBools = new bool[tileTypes.Length];

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
