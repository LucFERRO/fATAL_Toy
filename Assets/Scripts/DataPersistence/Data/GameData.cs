using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string screenShotId;

    public SerializableDictionary<string, Vector3> mapElevationDict;

    public SerializableDictionary<string, string> mapTypesDict;

    string timestamp;

    public GameData()
    {
        screenShotId = "";
        timestamp = "";
        mapElevationDict = new SerializableDictionary<string, Vector3>();
        mapTypesDict = new SerializableDictionary<string, string>();
    }
}