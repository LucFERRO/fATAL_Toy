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
    private bool hasLanded;
    private Rigidbody diceRb;
    private int closestTileIndex;
    public int maxDisappearanceTimer;
    public float currentDisappearanceTimer;
    public float velocityWatcher
    {
        get { return diceRb.linearVelocity.magnitude; }
        set
        {
            if (value <= 0.2f)
            {
                GetClosestHexTile();

                //UpdateTraveledHexes(gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())]);
                UpdateSingleHex(traveledTilesGO[closestTileIndex], gameManager.tilePrefabs[Array.IndexOf(gameManager.tileTypes, GetChosenFace())]);

                Destroy(gameObject);
            }
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
            hasLanded = false;
            velocityWatcher = diceRb.linearVelocity.magnitude;
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
                Debug.Log(tile.name + " is at " + tileDistance);
                if (tileDistance < shortestTileDistance)
                {
                    shortestTileDistance = tileDistance;
                    closestTileIndex = i;
                }
            }
        }
        closestIndex = closestTileIndex;
        Debug.Log($"{traveledTilesGO[closestTileIndex].name} is at the closest");
    }

    private void CreateBiomeFromDice()
    {
        //isStopped = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("BaseHex"))
        {
            return;
        }

        hasLanded = true;
        //if (!collision.collider.CompareTag("Hex"))
        //{
        //    return;
        //}

        //if (!traveledTilesGO.Contains(collision.gameObject) || !(collision.collider.CompareTag("Hex") && gameManager.dicesCanReplaceAllHexes)) ???
        if (!traveledTilesGO.Contains(collision.gameObject))
        {
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

    //private void InitiateVanillaDice()
    //{
    //    Vector2[] vectors = diceManager.possibleDiceVectors;
    //    string[] colors = diceManager.possibleDiceColors;
    //    string[] rarities = diceManager.possibleDiceRarities;

    //    string randomColor = colors[UnityEngine.Random.Range(0, colors.Length)];
    //    string randomRarity = rarities[UnityEngine.Random.Range(0, rarities.Length)];
    //    diceVectorArray = new Vector2[numberOfFaces];
    //    diceColor = randomColor;
    //    diceRarity = randomRarity;

    //    for (int i = 0; i < faceComponentArray.Length; i++)
    //    {
    //        FaceComponent face = faceComponentArray[i];
    //        Vector2 randomVector = vectors[UnityEngine.Random.Range(0, vectors.Length)].normalized;
    //        diceVectorArray[i] = randomVector;
    //        face.faceVector = randomVector;
    //        face.faceColor = randomColor;
    //    }
    //}

    //public FaceComponent GetRandomFace()
    //{
    //    int randomInt = UnityEngine.Random.Range(0, numberOfFaces);
    //    return faceComponentArray[randomInt];
    //}

    //private void UpdateFacesData()
    //{
    //    for (int i = 0; i < faceComponentArray.Length; i++)
    //    {
    //        FaceComponent face = faceComponentArray[i];
    //        face.faceColor = diceColor;
    //        face.faceVector = diceVectorArray[i];
    //    }
    //}

    //public int UpdateRollResult()
    //{
    //    float[] vectorDotResultArray = new float[numberOfFaces];
    //    float closestVectorDot = 0;
    //    int closestIndex = 0;

    //    for (int i = 0; i < numberOfFaces; i++)
    //    {
    //        // /!\ /!\ /!\ AYMERIC A CHANGER LE TRANSFORM.UP EN TRANSFORM.FORWARD /!\ /!\ /!\
    //        // Produit scalaire entre le vector.up de chaque face et Vector3.up
    //        vectorDotResultArray[i] = Vector3.Dot(faceComponentArray[i].transform.up, Vector3.up);
    //        //Debug.Log(vectorDotResultArray[i]);
    //        // Garde la face qui a son vecteur le plus vertical
    //        if (vectorDotResultArray[i] >= closestVectorDot)
    //        {
    //            //Debug.Log("new index detected " + i);
    //            closestVectorDot = vectorDotResultArray[i];
    //            closestIndex = i;
    //        }
    //    }
    //    //chosenFaceIndex = closestIndex;
    //    int chosenIndex = Array.IndexOf(vectorDotResultArray, vectorDotResultArray.Max());
    //    Debug.Log($"chosenIndex of {transform.gameObject.name} is {chosenIndex}");
    //    return chosenIndex;
    //}

    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // Si clique pour ajouter un dé inactif alors que déjà le nombre max de dés sélectionnés
    //        if (!isInUse && diceManager.numberOfDicesInUse >= diceManager.maxNumberOfDices)
    //        {
    //            string selectedAllDicesAlreadyMessage = $"Cannot use more than {diceManager.maxNumberOfDices} dices!";
    //            Debug.Log(selectedAllDicesAlreadyMessage);
    //            diceManager.uiManager.DisplayErrorMessage(selectedAllDicesAlreadyMessage);
    //            return;
    //        }

    //        isInUse = !isInUse;
    //        diceManager.NumberOfDicesInUse += isInUse ? 1 : -1;
    //    }

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        diceColor = diceManager.currentlyHeldColor;
    //        diceVectorArray = diceManager.currentlyHeldVectors;
    //        diceRarity = diceManager.currentlyHeldRarity;
    //        UpdateFacesData();
    //    }
    //}
}