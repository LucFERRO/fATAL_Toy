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
    private Rigidbody diceRb;
    private int closestTileIndex;

    [Header("Timers")]
    public int maxDisappearanceTimer;
    public float currentDisappearanceTimer;

    [Header("FMOD")]
    private FMOD.Studio.EventInstance diceEventInstance;
    public int numberOfBounces;

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
                //Debug.Log(gameManager.tilePrefabs.Length);
                //Debug.Log(Array.IndexOf(gameManager.tileTypes, GetChosenFace()));
                //Debug.Log(gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())].name);
                chosenFaceString = GetChosenFace();
                //Destroy(gameObject);
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                diceRb.isKinematic = true;
                gameObject.GetComponent<MeshCollider>().enabled = false;
                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
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
        //Debug.Log(diceRb.linearVelocity.magnitude);
    }

    private void HandleSelfDestruct()
    {
        if (transform.position.y <= -gameManager.lakituTreshold)
        {
            //transform.position = respawnTransform.position;
            //for (int i = 0; i < traveledTilesGO.Count; i++)
            //{
            //    traveledTilesGO[i].GetComponent<GlowingHexes>().ToggleGlow(false);
            //}
            //diceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //diceEventInstance.release();
            //Destroy(gameObject);


            //
            Vector3 lakituVelocity = (respawnTransform.position - transform.position).normalized;
            lakituVelocity.y *= 2f;
            diceRb.linearVelocity = gameManager.lakitu * diceRb.linearVelocity.magnitude * lakituVelocity;
            if (diceRb.linearVelocity.magnitude > 15)
            {
                diceRb.linearVelocity = diceRb.linearVelocity.normalized * 15f;
            }
            diceRb.angularVelocity = -diceRb.angularVelocity;
        }
    }

    private void Initialize()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        diceRb = GetComponent<Rigidbody>();
        maxDisappearanceTimer = gameManager.diceMaxDisappearanceTimer;
        currentDisappearanceTimer = maxDisappearanceTimer;
        respawnTransform = GameObject.FindWithTag("Lakitu").transform;
        faceComponentArray = new FaceComponent[numberOfFaces];
        for (int i = 0; i < numberOfFaces; i++)
        {
            faceComponentArray[i] = transform.GetChild(i).GetComponent<FaceComponent>();
        }

        isInUse = true;
        diceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/DiceOnTile");
        diceEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
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
        //string[] res = tileType.Split(new[] { "Forest", "Lake", "Mountain", "Plain" }, StringSplitOptions.RemoveEmptyEntries)
        //               .Select(word => word.ToLower())
        //               .Distinct()
        //               .ToArray();        
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
                if (tile == null)
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
                if (tile == null)
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
        GameObject newHex = Instantiate(newHexPrefab, hexToBeChanged.transform.position, randomRotation, hexToBeChanged.transform.parent);

        //Props rotate
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Untagged"))
        {
            return;
        }

        hasLanded = true;

        if (collision.collider.GetComponent<NeighbourTileProcessor>().isLocked)
        {
            GlowingHexes glowingHexes = new GlowingHexes();
            glowingHexes = collision.gameObject.GetComponent<GlowingHexes>();
            StartCoroutine(glowingHexes.ScaleEffect());
            return;
        }

        string tileType = collision.collider.GetComponent<NeighbourTileProcessor>().tileType;

        //if (tileType == gameManager.tileTypes[0]) // mountain
        //{
        //    ApplyMountainEffect();
        //}

        if (!traveledTilesGO.Contains(collision.gameObject))
        {
            if (collision.collider.CompareTag("Hex") && !gameManager.dicesCanReplaceAllHexes)
            {
                return;
            }

            diceEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            // Timer pour pas skip des notes
            diceEventInstance.setParameterByName("numberOfTiles", numberOfBounces);
            diceEventInstance.start();
            //collision.gameObject.GetComponent<NeighbourTileProcessor>().PlayTileBounceSound(numberOfBounces);
            numberOfBounces += 1;
            traveledTilesGO.Add(collision.gameObject);
            collision.gameObject.GetComponent<GlowingHexes>().ToggleGlow(true);

        }
    }

    private void ApplyMountainEffect()
    {
        Vector3 diceVelocity = diceRb.linearVelocity;
        diceRb.linearVelocity = new Vector3(diceVelocity.x, -0.5f * diceVelocity.y, diceVelocity.z);
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
}