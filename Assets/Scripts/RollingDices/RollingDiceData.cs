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
                GameObject newHexPrefab = gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())];
                if (gameManager.onlyReplacesClosestTile)
                {
                    UpdateSingleHex(traveledTilesGO[closestTileIndex], newHexPrefab);
                }
                else
                {
                    UpdateTraveledHexes(newHexPrefab);

                }
                Destroy(gameObject);
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
        if (transform.position.y <= -5)
        {
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
        foreach (GameObject tile in traveledTilesGO)
        {
            UpdateSingleHex(tile, newHexPrefab);
        }
    }

    private void UpdateSingleHex(GameObject hexToBeChanged, GameObject newHexPrefab)
    {
        if (hexToBeChanged == null)
        {
            return;
        }

        Vector3Int hexPosition = hexToBeChanged.GetComponent<NeighbourTileProcessor>().cellPosition;
        Quaternion randomRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 6) * 60, 0));
        GameObject newHex = Instantiate(newHexPrefab, hexToBeChanged.transform.position, randomRotation);
        newHex.transform.position = hexToBeChanged.transform.position;

        NeighbourTileProcessor newGridCoordinates = newHex.GetComponent<NeighbourTileProcessor>();
        newGridCoordinates.tiletype = GetChosenFace();
        newGridCoordinates.cellPosition = hexPosition;

        Destroy(hexToBeChanged.gameObject);
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
            return;
        }

        string tileType = collision.collider.GetComponent<NeighbourTileProcessor>().tiletype;

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
