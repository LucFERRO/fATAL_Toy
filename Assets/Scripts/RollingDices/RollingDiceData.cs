using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RollingDiceData : MonoBehaviour
{
    [HideInInspector] public int numberOfFaces = 6;
    [HideInInspector] public FaceComponent[] faceComponentArray;
    public bool isInUse;
    private GameManager gameManager;
    public string diceColor;
    public Vector2[] diceVectorArray;
    public string diceRarity;
    //useless?
    public int chosenFaceIndex;


    ///
    public bool hasLanded;
    private Rigidbody diceRb;
    private int closestTileIndex;
    public int maxDisappearanceTimer;
    public float currentDisappearanceTimer;
    public float velocityWatcher;
    public float VelocityWatcher
    {
        get { return velocityWatcher; }
        set
        {
            velocityWatcher = value;

            if (value <= 0.2f)
            {
                GetClosestHexTile();
                if (gameManager.onlyReplacesClosestTile)
                {
                    UpdateSingleHex(traveledTilesGO[closestTileIndex], gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())]);
                }

                else
                {

                    UpdateTraveledHexes(gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())]);
                }
            }
            Debug.Log("DESTROY BY DEFAULT");
            Destroy(gameObject);
        }
    }
    public List<GameObject> traveledTilesGO = new List<GameObject>();

    void Start()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        diceRb = GetComponent<Rigidbody>();
        faceComponentArray = new FaceComponent[numberOfFaces];
        for (int i = 0; i < numberOfFaces; i++)
        {
            faceComponentArray[i] = transform.GetChild(i).GetComponent<FaceComponent>();
        }
        isInUse = true;
        maxDisappearanceTimer = gameManager.diceMaxDisappearanceTimer;
        currentDisappearanceTimer = maxDisappearanceTimer;
        //InitiateVanillaDice();


    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    GetClosestHexTile();
        //    //Debug.Log(GetChosenFace());
        //    //Debug.Log(Array.IndexOf(gameManager.tileTypes,GetChosenFace()));
        //    //Debug.Log(gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())].name);
        //    //UpdateSingleHex(traveledTilesGO[closestTileIndex], gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())]);
        //    UpdateTraveledHexes(gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())]);
        //    Destroy(gameObject);
        //    //CreateBiomeFromDice();
        //}

        if (hasLanded)
        {
            currentDisappearanceTimer -= Time.deltaTime;
        }
        if (currentDisappearanceTimer <= 0 && hasLanded)
        {
            if (traveledTilesGO.Count == 0)
            {
                Debug.Log("EARLY DESTROY");
                Destroy(gameObject);
                return;
            }
            hasLanded = false;
            VelocityWatcher = diceRb.linearVelocity.magnitude;
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
        Vector3Int hexPosition = hexToBeChanged.GetComponent<GridCoordinates>().cellPosition;
        //Debug.Log("Hex Coordinates:" + hexPosition);
        GameObject newHex = Instantiate(newHexPrefab, hexToBeChanged.transform.parent);
        newHex.transform.position = hexToBeChanged.transform.position;
        GridCoordinates newGridCoordinates = newHex.GetComponent<GridCoordinates>();
        newGridCoordinates.tiletype = gameManager.chosenTileType;
        newGridCoordinates.cellPosition = hexPosition;
        //newGridCoordinates.currentPrefab = gameManager.chosenPrefab;
        Destroy(hexToBeChanged.gameObject);
    }
    private void GetClosestHexTile()
    {
        float shortestTileDistance = 10f;
        int closestIndex = 0;
        for (int i = 0; i < traveledTilesGO.Count; i++)
        {
            GameObject tile = traveledTilesGO[i];
            if (tile != null)
            {
                float tileDistance = Vector3.Distance(transform.position, tile.transform.position);
                //Debug.Log(tile.name + " is at " + tileDistance);
                if (tileDistance < shortestTileDistance)
                {
                    shortestTileDistance = tileDistance;
                    closestIndex = i;
                }
            }
        }
        closestTileIndex = closestIndex;
        //Debug.Log($"{traveledTilesGO[closestTileIndex].name} is at the closest");
    }

    private void CreateBiomeFromDice()
    {
        //isStopped = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Untagged"))
        {
            return;
        }

        hasLanded = true;

        if (collision.collider.CompareTag("Hex"))
        {
            Vector3 diceVelocity = diceRb.linearVelocity;
            diceRb.linearVelocity = new Vector3(diceVelocity.x, -0.5f * diceVelocity.y, diceVelocity.z);
        }

        if (!traveledTilesGO.Contains(collision.gameObject))
        {
            if (collision.collider.CompareTag("Hex") && !gameManager.dicesCanReplaceAllHexes)
            {
                return;
            }
            traveledTilesGO.Add(collision.gameObject);
        }
    }

    private string GetChosenFace()
    {
        float[] vectorDotResultArray = new float[numberOfFaces];
        float closestVectorDot = 0;
        int closestIndex = 0;

        for (int i = 0; i < numberOfFaces; i++)
        {
            // /!\ /!\ /!\ AYMERIC A CHANGER LE TRANSFORM.UP EN TRANSFORM.FORWARD /!\ /!\ /!\
            // Produit scalaire entre le vector.up de chaque face et Vector3.up
            vectorDotResultArray[i] = Vector3.Dot(faceComponentArray[i].transform.up, Vector3.up);
            //Debug.Log(vectorDotResultArray[i]);
            // Garde la face qui a son vecteur le plus vertical
            if (vectorDotResultArray[i] >= closestVectorDot)
            {
                //Debug.Log("new index detected " + i);
                closestVectorDot = vectorDotResultArray[i];
                closestIndex = i;
            }
        }
        //chosenFaceIndex = closestIndex;
        int chosenIndex = Array.IndexOf(vectorDotResultArray, vectorDotResultArray.Max());
        return transform.GetChild(chosenIndex).GetComponent<FaceComponent>().faceType;
    }


}