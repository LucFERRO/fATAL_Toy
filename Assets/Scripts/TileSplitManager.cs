using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TileSplitManager : MonoBehaviour
{
    public int numberOfTiles;
    public Dictionary<string, int> gridTileSplitDictionary = new();
    private string[] types;
    private int[] values;

    void Start()
    {
        numberOfTiles = transform.childCount;
        types = new string[] { "empty", "mountain", "lake", "plain", "forest" };
        values = new int[types.Length];
        CreateTileDictionary();
        UpdateTileSplit();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateTileSplit();
            //foreach (KeyValuePair<string, int> kvp in gridTileSplitDictionary)
            //{
            //    Debug.Log(kvp.Key + ": " + kvp.Value);
            //}
            for (int i = 0; i < types.Length; i++)
            {
                Debug.Log($"{types[i]} : {values[i]} out of {values.Sum()} tiles => {Mathf.Round(values[i]/(float)values.Sum()*100)}%");
            }
            Debug.Log($"Majority of : {types[Array.IndexOf(values, values.Max())]}");
        }
    }

    public void UpdateTileSplit()
    {
        //CreateTileDictionary();
        values = new int[types.Length];
        for (int i = 0; i < numberOfTiles; i++)
        {
            string type = transform.GetChild(i).GetChild(0).GetComponent<NeighbourTileProcessor>().tiletype;
            //    foreach (string tileType in gridTileSplitDictionary.Keys)
            //    {
            //        if (tileType.Contains(type.Substring(1)))
            //        {
            //            gridTileSplitDictionary[tileType]++;
            //            continue;
            //        }
            //    }
            //}
            //gridTileSplitDictionary.Clear();
            //foreach (var key in gridTileSplitDictionary.Keys)
            //{
            //    string type = transform.GetChild(i).GetChild(0).GetComponent<NeighbourTileProcessor>().tiletype;
            //    if (key.Contains(type.Substring(1)))
            //    {
            //        gridTileSplitDictionary[type]++;
            //        continue;
            //    }
            for (int j = 0; j < types.Length; j++)
            {
                if (types[j].Contains(type.Substring(1)))
                {
                    values[j]++;
                }
            }
        }
    }
    private void CreateTileDictionary()
    {
        gridTileSplitDictionary.Clear();

        gridTileSplitDictionary.Add("empty", 0);
        gridTileSplitDictionary.Add("mountain", 0);
        gridTileSplitDictionary.Add("lake", 0);
        gridTileSplitDictionary.Add("plain", 0);
        gridTileSplitDictionary.Add("forest", 0);
    }
}
