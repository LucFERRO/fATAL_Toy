using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler
{
    private GameManager gameManager;
    private PhysicalDiceSpawner diceSpawner;
    private Camera cam;
    private GridCoordinates gridCoordinates;
    private GameObject chosenComboTile;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        diceSpawner = GameObject.FindGameObjectWithTag("DiceSpawner").GetComponent<PhysicalDiceSpawner>();
        cam = Camera.main;
        gridCoordinates = GetComponent<GridCoordinates>();
    }

    private string GetComboTile()
    {
        int typeValue1 = 0;
        int typeValue2 = 0;
        int typeValue3 = 0;

        foreach (KeyValuePair<int, string> kvp in gameManager.baseTileDictionary)
        {
            if (kvp.Value == gridCoordinates.tiletype)
            {
                typeValue1 = kvp.Key * 10;
                Debug.Log(kvp.Key);
            }

            if (kvp.Value == gridCoordinates.majorTile)
            {
                typeValue2 = kvp.Key;
            }
        }

        //FirstLetterToUpper(type2);

        Debug.Log(typeValue1 + "," + typeValue2);
        typeValue3 = typeValue1 + typeValue2;

        gameManager.comboDictionary.TryGetValue(typeValue3, out string comboTile);
        
        string[] types = comboTile.Split("_");
        string type1 = types[0];
        string type2 = types[1];

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

    private void UpdateComboTile()
    {
        for (int i = 0; i < gameManager.tileTypes.Length; i++)
        {
            if (gameManager.tileTypes[i] == GetComboTile())
            {
                gameManager.chosenPrefab = gameManager.tilePrefabs[i];
                Debug.Log(gameManager.chosenPrefab);
                UpdateHex();
            }
        }
    }


    //public string SplitAtUnderscore(string str)
    //{
    //    string res = "";
    //    string[] split = Regex.Split(str, @"(?<!^)(?=[_])");
    //    //for (int i = 0; i < split.Length; i++)
    //    //{
    //    //    res += " " + split[i];
    //    //}
    //    split[1] = split[1].Subs tring(1);
    //    Debug.Log(split[0] + "," + split[1]);
    //    return res;
    //}

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

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Debug.Log(GetComboTile());
            UpdateComboTile();
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(transform.position);
            diceSpawner.SpawnDice(transform.position - cam.transform.position, cam.transform);
        }
        if (Input.GetMouseButtonDown(1))
        {
            UpdateHex();
        }
    }
    private void UpdateHex()
    {
        Vector3Int hexPosition = gridCoordinates.cellPosition;
        GameObject newHex = Instantiate(gameManager.chosenPrefab, transform.parent);
        newHex.transform.position = transform.position;

        GridCoordinates newGridCoordinates = newHex.GetComponent<GridCoordinates>();
        newGridCoordinates.tiletype = gameManager.chosenPrefab.GetComponent<GridCoordinates>().tiletype;
        newGridCoordinates.cellPosition = hexPosition;

        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3Int hexPosition = gridCoordinates.cellPosition;
        //Debug.Log("Hex Coordinates:" + hexPosition);

        //gridCoordinates.tiletype = gameManager.chosenTileType;
        gridCoordinates.currentPrefab = gameManager.chosenPrefab;
    }
}
