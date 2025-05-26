using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Unity.VisualScripting;
//using UnityEditor.PackageManager.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    public string sessionId = "defaultSession";
    public bool isGalleryWIP;
    public GameObject hexMap;

    private List<IDataPersistence> dataPersistenceObjects;

    public UiManager uiManager;
    public FileDataHandler dataHandler;
    private ScreenShotMachine screenShotMachine;
    public static DataPersistenceManager instance { get; private set; }

    // Add this field for your save directory
    private string saveDirectory;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("DataPersistenceManager already exists in the scene.");
        }
        sessionId = System.Guid.NewGuid().ToString();
        instance = this;
        dataHandler = new FileDataHandler(Application.persistentDataPath);
        screenShotMachine = GetComponent<ScreenShotMachine>();
        fileName = CrossSceneData.dataFileName;

        // Set the save directory (adjust as needed for your project structure)
        saveDirectory = Path.Combine(Application.dataPath, "..", "SaveData", "Save");

        dataPersistenceObjects = FindAllDataPersistenceObjects();
        NewGame();
        if (isGalleryWIP)
        {
            dataHandler = new FileDataHandler(CrossSceneData.dataFileName);
            AssignIds();
            LoadGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            dataHandler = new FileDataHandler(fileName);
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            //Debug.Log("Loading last save.");
            LoadGame();
        }
    }

    void AssignIds()
    {
        // Use the full path for file operations
        string fullPath = Path.Combine(saveDirectory, fileName);

        if (!File.Exists(fullPath))
        {
            Debug.LogError("JSON file not found: " + fullPath);
            return;
        }

        string json = File.ReadAllText(fullPath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // Build a lookup from position to id
        Dictionary<Vector3, string> positionToId = new Dictionary<Vector3, string>();
        foreach (var kvp in data.mapElevationDict)
        {
            positionToId[kvp.Value] = kvp.Key;
        }

        int assigned = 0;
        const float positionTolerance = 1f;
        foreach (var handler in hexMap.GetComponentsInChildren<GridNeighbourHandler>(true))
        {
            Vector3 pos = handler.transform.position;
            bool found = false;
            foreach (var kvp in positionToId)
            {
                if (Vector3.Distance(pos, kvp.Key) < positionTolerance)
                {
                    //var so = new SerializedObject(handler);
                    //so.FindProperty("id").stringValue = kvp.Value;
                    //so.ApplyModifiedProperties();
                    handler.id = kvp.Value;
                    assigned++;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.LogWarning($"No saved id found for tile at position {pos}");
            }
        }

        Debug.Log($"Assigned {assigned} ids to GridNeighbourHandler components.");
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveGameData()
    {
        screenShotMachine.SaveData(ref gameData);
        screenShotMachine.ScreenShot();
        SaveGame(sessionId);
        uiManager.TogglePause(); // Handle pause state after saving
        // TRANSITION FLASH BLANC
        //SON CLICK PHOTO
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        //Debug.Log("New game started.");
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if (gameData == null)
        {
            //Debug.Log("No game data found. Starting a new game.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        //Debug.Log("Loaded game data: " + gameData.currentNumberOfRolls);
    }

    public void SaveGame(string sessionId)
    {
        if (dataHandler == null)
        {
            //Debug.LogError("DataHandler is not initialized. Cannot save game.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        //Debug.Log("Saved game data: ");

        dataHandler.Save(gameData, sessionId);
    }

    //private void OnApplicationQuit()
    //{
    //    SaveGame();
    //}

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
