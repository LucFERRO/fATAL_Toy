using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RollingDiceData : MonoBehaviour
{
    public Transform respawnTransform;
    [Header("Dice Configuration")]
    [HideInInspector] public int numberOfFaces = 6;
    [HideInInspector] public FaceComponent[] faceComponentArray;
    public string diceColor;
    public string diceRarity;
    public int chosenFaceIndex;
    public string chosenFaceString;
    public Vector2[] diceVectorArray;
    private bool hasChanged;

    [Header("State")]
    public bool isInUse;
    public bool hasLanded;
    private GameManager gameManager;
    private DefeatManager defeatManager;
    private Rigidbody diceRb;
    private int closestTileIndex;

    [Header("Timers")]
    public int maxDisappearanceTimer;
    public float currentDisappearanceTimer;

    [Header("FMOD")]
    private FMOD.Studio.EventInstance diceEventInstance;
    public int numberOfBounces;
    private FMOD.Studio.EventInstance diceDeathEventInstance;

    [Header("Velocity Watcher")]
    public float velocityWatcher;
    public List<GameObject> traveledTilesGO = new List<GameObject>();

    public float VelocityWatcher
    {
        get { return velocityWatcher; }
        set
        {
            velocityWatcher = value;

            if (value <= 0.1f && !hasChanged)
            {
                GetClosestHexTile();
                chosenFaceString = GetChosenFace();
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                diceRb.isKinematic = true;
                gameObject.GetComponent<MeshCollider>().enabled = false;
                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                CleanseLockedTiles();
                UpdateTraveledHexes(chosenFaceString);
                hasChanged = true;
            }
        }
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        HandleDisappearanceTimer();
        HandleSelfDestruct();
        LiveUpdateChosenFaceUi();
    }

    private void HandleSelfDestruct()
    {
        if (transform.position.y <= -gameManager.lakituTreshold)
        {
            for (int i = 0; i < traveledTilesGO.Count; i++)
            {
                traveledTilesGO[i].GetComponent<GlowingHexes>().ToggleGlow(false);
            }
            diceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            diceEventInstance.release();
            defeatManager.HandleRollCount();
            diceDeathEventInstance.start();
            Destroy(gameObject);
        }
    }

    private void CleanseLockedTiles()
    {
        for (int i = 0; i < traveledTilesGO.Count; i++)
        {
            if (traveledTilesGO[i] == null || traveledTilesGO[i].GetComponent<NeighbourTileProcessor>().isLocked)
            {
                traveledTilesGO.RemoveAt(i);
            }
        }
    }

    private void Initialize()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        gameManager.faceTypes = new string[numberOfFaces];
        defeatManager = transform.parent.GetComponent<DefeatManager>();
        diceRb = GetComponent<Rigidbody>();
        maxDisappearanceTimer = gameManager.diceMaxDisappearanceTimer;
        currentDisappearanceTimer = maxDisappearanceTimer;
        respawnTransform = GameObject.FindWithTag("Lakitu").transform;
        faceComponentArray = new FaceComponent[numberOfFaces];
        for (int i = 0; i < numberOfFaces; i++)
        {
            faceComponentArray[i] = transform.GetChild(i).GetComponent<FaceComponent>();
            gameManager.faceTypes[i] = faceComponentArray[i].faceType;
        }

        isInUse = true;
        diceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/DiceOnTile");
        diceEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        diceDeathEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/DiceDeath");
    }

    private void HandleDisappearanceTimer()
    {
        if (hasLanded && diceRb.linearVelocity.magnitude <= 0.1f)
        {
            currentDisappearanceTimer -= Time.deltaTime;

            if (currentDisappearanceTimer <= 0)
            {
                if (traveledTilesGO.Count == 0)
                {
                    Debug.Log("EARLY DESTROY");
                    print("StopMusic");
                    diceEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                    var result = diceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    if (result != FMOD.RESULT.OK)
                    {
                        Debug.Log($"Failed to stop event with result: {result}");
                    }
                    result = diceEventInstance.release();
                    if (result != FMOD.RESULT.OK)
                    {
                        Debug.Log($"Failed to stop event with result: {result}");
                    }
                    Destroy(gameObject);
                    return;
                }
                VelocityWatcher = diceRb.linearVelocity.magnitude;
            }
        }
    }
    private string[] ProcessTileType(string tileType)
    {
        string[] res = System.Text.RegularExpressions.Regex
        .Matches(tileType, @"[A-Z]?[a-z]+|[A-Z]+(?![a-z])")
        .Cast<System.Text.RegularExpressions.Match>()
        .Select(m => m.Value.ToLower())
        //.Distinct()
        .ToArray();
        return res;
    }

    private void UpdateTraveledHexes(string tileType)
    {
        List<GameObject> newTiles = new List<GameObject>();
        string[] tiles = ProcessTileType(tileType);
        GameObject newHexPrefab = gameManager.tilePrefabs[Array.IndexOf(Enum.GetNames(typeof(TileType)), tileType)];

        if (tiles.Length == 1)
        {
            foreach (GameObject tile in new List<GameObject>(traveledTilesGO))
            {
                if (tile == null || tile.GetComponent<NeighbourTileProcessor>().isLocked)
                {
                    continue;
                }
                NeighbourTileProcessor newTile = UpdateSingleHex(tile, newHexPrefab, tileType);
                newTiles.Add(newTile.gameObject);
            }
        }
        else
        {
            GameObject[] closestNeighbourTilesGOs = traveledTilesGO[closestTileIndex].transform.parent.GetComponent<GridNeighbourHandler>().neighbourTileGOs;
            int randomRingTile = UnityEngine.Random.Range(0, tiles.Length);
            foreach (GameObject tile in new List<GameObject>(traveledTilesGO))
            {
                if (tile == null || tile.GetComponent<NeighbourTileProcessor>().isLocked)
                {
                    continue;
                }
                if (tile == traveledTilesGO[closestTileIndex])
                {
                    UpdateSingleHex(tile, gameManager.tilePrefabs[Array.IndexOf(Enum.GetNames(typeof(TileType)), tileType)], tileType);
                    continue;
                }
                if (closestNeighbourTilesGOs.Contains(tile.transform.parent.gameObject))
                {
                    continue;
                }
                string fittingTileType = gameManager.tilePatternRandom ? tiles[UnityEngine.Random.Range(0, tiles.Length)] : tiles[(int)Mathf.Repeat(randomRingTile + 1, tiles.Length)];
                NeighbourTileProcessor newTile = UpdateSingleHex(tile, gameManager.tilePrefabs[Array.IndexOf(Enum.GetNames(typeof(TileType)), fittingTileType)], fittingTileType);
                newTiles.Add(newTile.gameObject);
            }

            foreach (GameObject tile in closestNeighbourTilesGOs)
            {
                if (tile.transform.GetChild(0).GetComponent<NeighbourTileProcessor>().isLocked)
                {
                    continue;
                }
                NeighbourTileProcessor newTileRing = UpdateSingleHex(tile.transform.GetChild(0).gameObject, gameManager.tilePrefabs[Array.IndexOf(Enum.GetNames(typeof(TileType)), tiles[randomRingTile])], tiles[randomRingTile]);
                newTiles.Add(newTileRing.gameObject);
            }
        }

        gameManager.UpdateNeighboursAfterDiceDestroy(newTiles);
    }

    private NeighbourTileProcessor UpdateSingleHex(GameObject hexToBeChanged, GameObject newHexPrefab, string newTileType)
    {
        if (newHexPrefab == null)
        {
            Debug.LogError("newHexPrefab is null. Ensure the prefab is assigned correctly in the GameManager.");
            return null;
        }
        Vector3Int hexPosition = hexToBeChanged.GetComponent<NeighbourTileProcessor>().cellPosition;
        Quaternion randomRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 6) * 60, 0));
        GameObject newHex = Instantiate(newHexPrefab, hexToBeChanged.transform.position, (newTileType == "plain" || newTileType == "plainPlain") ? Quaternion.identity : randomRotation, hexToBeChanged.transform.parent);

        //Props random rotate
        if (newHex.transform.childCount > 1)
        {
            for (int i = 0; i < newHex.transform.GetChild(1).childCount; i++)
            {
                Transform prop = newHex.transform.GetChild(1).GetChild(i);
                if (prop.CompareTag("RotateImmuneProp"))
                {
                    continue;
                }
                prop.rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 6) * 60, 0));
            }
        }

        newHex.GetComponent<GlowingHexes>().ToggleMaterialize(true);
        StartCoroutine(newHex.GetComponent<GlowingHexes>().TransitionAppear());
        newHex.transform.position = hexToBeChanged.transform.position;

        NeighbourTileProcessor newGridCoordinates = newHex.GetComponent<NeighbourTileProcessor>();
        newGridCoordinates.tileType = newTileType;
        newGridCoordinates.cellPosition = hexPosition;

        // Replace the old tile in traveledTilesGO with the new one
        int index = traveledTilesGO.IndexOf(hexToBeChanged);
        if (index != -1)
        {
            traveledTilesGO[index] = newHex;
        }
        StartCoroutine(hexToBeChanged.GetComponent<GlowingHexes>().TransitionDisappear());
        return newGridCoordinates;
    }

    private void GetClosestHexTile()
    {
        float shortestTileDistance = float.MaxValue;

        for (int i = 0; i < traveledTilesGO.Count; i++)
        {
            GameObject tile = traveledTilesGO[i];
            if (tile != null)
            {
                float tileDistance = Vector3.Distance(transform.position, tile.transform.position);
                if (tileDistance < shortestTileDistance)
                {
                    shortestTileDistance = tileDistance;
                    closestTileIndex = i;
                }
            }
        }
    }

    private void LiveUpdateChosenFaceUi()
    {
        float[] vectorDotResultArray = new float[numberOfFaces];
        float closestVectorDot = float.MinValue;
        int closestIndex = 0;

        for (int i = 0; i < numberOfFaces; i++)
        {
            vectorDotResultArray[i] = Vector3.Dot(faceComponentArray[i].transform.up, Vector3.up);
            //gameManager.diceFaces[i].transform.GetChild(0).GetComponent<Image>().color = gameManager.baseDiceFaceColor;
            gameManager.diceFaces[i].transform.GetComponent<Outline>().effectDistance = new Vector2(2, 2);
            gameManager.diceFaces[i].transform.GetComponent<Outline>().effectColor = Color.black;
            if (vectorDotResultArray[i] > closestVectorDot)
            {
                closestVectorDot = vectorDotResultArray[i];
                closestIndex = i;
            }
        }

        //gameManager.diceFaces[closestIndex].transform.GetChild(0).GetComponent<Image>().color = Color.blue;
        gameManager.diceFaces[closestIndex].transform.GetComponent<Outline>().effectDistance = new Vector2(3, 3);
        gameManager.diceFaces[closestIndex].transform.GetComponent<Outline>().effectColor = Color.yellow;
    }
    private string GetChosenFace()
    {
        float[] vectorDotResultArray = new float[numberOfFaces];
        float closestVectorDot = float.MinValue;
        int closestIndex = 0;

        for (int i = 0; i < numberOfFaces; i++)
        {
            vectorDotResultArray[i] = Vector3.Dot(faceComponentArray[i].transform.up, Vector3.up);
            if (vectorDotResultArray[i] > closestVectorDot)
            {
                closestVectorDot = vectorDotResultArray[i];
                closestIndex = i;
            }
        }

        return transform.GetChild(closestIndex).GetComponent<FaceComponent>().faceType;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Untagged"))
        {
            return;
        }

        hasLanded = true;

        if (!traveledTilesGO.Contains(collision.gameObject))
        {
            if (collision.collider.CompareTag("Hex") && !gameManager.dicesCanReplaceAllHexes)
            {
                return;
            }
            GlowingHexes glowingHexes = collision.gameObject.GetComponent<GlowingHexes>();

            if (collision.collider.GetComponent<NeighbourTileProcessor>().isLocked)
            {
                StartCoroutine(glowingHexes.ScaleEffect());
            }
            else
            {
                collision.gameObject.GetComponent<GlowingHexes>().ToggleGlow(true);
            }

            diceEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            // Timer pour pas skip des notes
            diceEventInstance.setParameterByName("numberOfTiles", numberOfBounces);
            diceEventInstance.start();
            numberOfBounces += 1;
            traveledTilesGO.Add(collision.gameObject);
        }
    }
}