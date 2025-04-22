using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TileSplitManager : MonoBehaviour
{
    public int numberOfTiles;
    public Dictionary<string, int> gridTileSplitDictionary = new();
    private string[] baseTypes;
    private int[] values;

    void Start()
    {
        numberOfTiles = transform.childCount;
        baseTypes = new string[] { "empty", "mountain", "lake", "plain", "forest" };
        values = new int[baseTypes.Length];
        UpdateTileSplitDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            string test = "forestLake";
            string test2 = "forest";
            string test3 = "lake";
            string test4 = "mountain";
            Debug.Log(test.Contains(test2.Substring(1)));
            Debug.Log(test.Contains(test3.Substring(1)));
            Debug.Log(test.Contains(test4.Substring(1)));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateTileSplitDictionary();
            foreach (KeyValuePair<string, int> kvp in gridTileSplitDictionary)
            {
                Debug.Log(kvp.Key + ": " + kvp.Value);
            }
            //for (int i = 0; i < types.Length; i++)
            //{
            //    Debug.Log($"{types[i]} : {values[i]} out of {values.Sum()} tiles => {Mathf.Round(values[i] / (float)values.Sum() * 100)}%");
            //}
            //Debug.Log($"Majority of : {types[Array.IndexOf(values, values.Max())]}");
        }
    }

    public void UpdateTileSplitDictionary()
    {
        CreateFreshTileDictionary();
        for (int i = 0; i < numberOfTiles; i++)
        {
            string type = transform.GetChild(i).GetChild(0).GetComponent<NeighbourTileProcessor>().tiletype;
            transform.GetChild(i).GetComponent<GridNeighbourHandler>().UpdateNeighbourTiles();
            foreach (string baseType in baseTypes)
            {
                if (baseType.Contains(type.Substring(1)))
                {
                    gridTileSplitDictionary[baseType]++;
                }
            }
        }
    }

    public void UpdateTileSplit()
    {
        CreateFreshTileDictionary();
        values = new int[baseTypes.Length];
        for (int i = 0; i < numberOfTiles; i++)
        {
            string type = transform.GetChild(i).GetChild(0).GetComponent<NeighbourTileProcessor>().tiletype;
            for (int j = 0; j < baseTypes.Length; j++)
            {
                {
                    if (baseTypes[j].Contains(type.Substring(1)))
                    {
                        values[j]++;
                    }
                }
            }
        }
    }
    private void CreateFreshTileDictionary()
    {
        gridTileSplitDictionary = new Dictionary<string, int>()
        {
            { "empty", 0 },
            { "mountain", 0 },
            { "forest", 0 },
            { "lake", 0 },
            { "plain", 0 }
        };
    }
}