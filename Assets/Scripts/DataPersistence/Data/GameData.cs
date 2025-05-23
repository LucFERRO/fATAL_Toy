using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentNumberOfRolls;

    public SerializableDictionary<string, Vector3> mapElevationDict;

    public SerializableDictionary<string, GameObject> mapTypesDict;

    public GameData()
    {
        currentNumberOfRolls = 3;
        mapElevationDict = new SerializableDictionary<string, Vector3>();
        mapTypesDict = new SerializableDictionary<string, GameObject>();
    }
}
