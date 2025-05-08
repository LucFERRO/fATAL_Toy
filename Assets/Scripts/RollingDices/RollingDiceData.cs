using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RollingDiceData : MonoBehaviour
{
    [Header("Dice Configuration")]
    [HideInInspector] public int numberOfFaces = 6;
    [HideInInspector] public FaceComponent[] faceComponentArray;
    public string diceColor;
    public string diceRarity;
    public int chosenFaceIndex;
    public Vector2[] diceVectorArray;

    [Header("State")]
    public bool isInUse;
    public bool hasLanded;
    private GameManager gameManager;
    private Rigidbody diceRb;
    private int closestTileIndex;

    [Header("Timers")]
    public int maxDisappearanceTimer;
    public float currentDisappearanceTimer;

    [Header("Velocity Watcher")]
    public float velocityWatcher;
    public List<GameObject> traveledTilesGO = new List<GameObject>();

    public float VelocityWatcher
    {
        get { return velocityWatcher; }
        set
        {
            velocityWatcher = value;

            if (value <= 0.1f)
            {
                GetClosestHexTile();
                //Debug.Log(gameManager.tilePrefabs.Length);
                //Debug.Log(Array.IndexOf(gameManager.tileTypes, GetChosenFace()));
                //Debug.Log(gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())].name);
                GameObject newHexPrefab = gameManager.tilePrefabs[Array.IndexOf(Enum.GetNames(typeof(TileType)), GetChosenFace())];
                Destroy(gameObject);
                UpdateTraveledHexes(newHexPrefab);
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
        if (transform.position.y <= -15)
        {
            for (int i = 0; i < traveledTilesGO.Count; i++)
            {
                traveledTilesGO[i].GetComponent<GlowingHexes>().ToggleGlow(false);
            }
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        diceRb = GetComponent<Rigidbody>();
        maxDisappearanceTimer = gameManager.diceMaxDisappearanceTimer;
        currentDisappearanceTimer = maxDisappearanceTimer;

        faceComponentArray = new FaceComponent[numberOfFaces];
        for (int i = 0; i < numberOfFaces; i++)
        {
            faceComponentArray[i] = transform.GetChild(i).GetComponent<FaceComponent>();
        }

        isInUse = true;
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
                    Destroy(gameObject);
                    return;
                }
                VelocityWatcher = diceRb.linearVelocity.magnitude;
            }
        }
    }

    private void UpdateTraveledHexes(GameObject newHexPrefab)
    {
        List<GameObject> newTiles = new List<GameObject>();

        foreach (GameObject tile in new List<GameObject>(traveledTilesGO))
        {
            if (tile == null) continue;

            NeighbourTileProcessor newTile = UpdateSingleHex(tile, newHexPrefab);
            newTiles.Add(newTile.gameObject);
        }

        gameManager.UpdateNeighboursAfterDiceDestroy(newTiles);
    }

    private NeighbourTileProcessor UpdateSingleHex(GameObject hexToBeChanged, GameObject newHexPrefab)
    {
        Vector3Int hexPosition = hexToBeChanged.GetComponent<NeighbourTileProcessor>().cellPosition;
        Quaternion randomRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 6) * 60, 0));
        if (newHexPrefab == null)
        {
            Debug.LogError("newHexPrefab is null. Ensure the prefab is assigned correctly in the GameManager.");
            return null;
        }
        GameObject newHex = Instantiate(newHexPrefab, hexToBeChanged.transform.position, randomRotation, hexToBeChanged.transform.parent);
        newHex.transform.position = hexToBeChanged.transform.position;

        NeighbourTileProcessor newGridCoordinates = newHex.GetComponent<NeighbourTileProcessor>();
        newGridCoordinates.tileType = GetChosenFace();
        newGridCoordinates.cellPosition = hexPosition;

        // Replace the old tile in traveledTilesGO with the new one
        int index = traveledTilesGO.IndexOf(hexToBeChanged);
        if (index != -1)
        {
            traveledTilesGO[index] = newHex;
        }

        StartCoroutine(hexToBeChanged.GetComponent<GlowingHexes>().TransitionEffect());

        //Destroy(hexToBeChanged.gameObject);
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
            glowingHexes.SplashEffect();
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

            //if (gameManager.onlyReplacesClosestTile)
            //{
            //    foreach (GameObject tile in traveledTilesGO)
            //    {
            //        tile.gameObject.GetComponent<GlowingHexes>().ToggleGlow(false);
            //    }
            //    traveledTilesGO.Clear();
            //    traveledTilesGO.Add(collision.gameObject);
            //    collision.gameObject.GetComponent<GlowingHexes>().ToggleGlow(true);
            //}
            //else
            {
                traveledTilesGO.Add(collision.gameObject);
                collision.gameObject.GetComponent<GlowingHexes>().ToggleGlow(true);
            }
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
            gameManager.diceFaces[i].transform.GetChild(0).GetComponent<Image>().color = gameManager.baseDiceFaceColor;
            if (vectorDotResultArray[i] > closestVectorDot)
            {
                closestVectorDot = vectorDotResultArray[i];
                closestIndex = i;
            }
        }

        gameManager.diceFaces[closestIndex].transform.GetChild(0).GetComponent<Image>().color = Color.blue;
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