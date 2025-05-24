using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    public string sessionId = "defaultSession";

    private List<IDataPersistence> dataPersistenceObjects;

    public UiManager uiManager;
    public FileDataHandler dataHandler;
    private ScreenShotMachine screenShotMachine;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("DataPersistenceManager already exists in the scene.");
        }
        sessionId = System.Guid.NewGuid().ToString();
        instance = this;
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        screenShotMachine = GetComponent<ScreenShotMachine>();
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        NewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            //Debug.Log("Loading last save.");
            LoadGame();
        }
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
