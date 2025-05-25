using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public static class CrossSceneData
{
    public static bool isTutorial;
    public static string dataDirPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SaveData", "Save"));
    public static string dataFileName = "";
}
