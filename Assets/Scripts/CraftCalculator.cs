using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class CraftCalculator : MonoBehaviour
{
    public GameObject craftingPawn;
    public Vector3 craftingPawnStartingPosition;
    private DiceManagerV2 diceManager;
    private NavMeshAgent craftingPawnAgent;
    [HideInInspector] public List<FaceComponent> diceRollResults;

    public Dictionary<string, int> resultDictionary = new();

    //TEST
    public GameObject[] resultGameObjects;
    public Material[] materialArray;
    public float remainingDistance;
    public int craftProgressInt = 5;
    public float vectorStrength;
    public bool isMoving;
    public FaceComponent[] facesResultArray;
    public Vector2[] craftVectorArray;
    public Vector3[] computedDestinationArray;
    public int CraftProgressInt
    {
        get
        {
            return craftProgressInt;
        }
        set
        {
            craftProgressInt = value;
            //if (computedDestinationArray.Length != 0)
            //{
            Debug.Log($"moving to spot number {craftProgressInt}");
            Debug.Log($"Current position: {craftingPawn.transform.position}");
            Debug.Log($"Destination: ({computedDestinationArray[craftProgressInt]})");
            Debug.Log($"Vector distance: {Mathf.Sqrt((Mathf.Pow(computedDestinationArray[craftProgressInt].x - craftingPawn.transform.position.x, 2f)) + (Mathf.Pow(computedDestinationArray[craftProgressInt].y - craftingPawn.transform.position.y, 2f)) + (Mathf.Pow(computedDestinationArray[craftProgressInt].z - craftingPawn.transform.position.z, 2f)))}");
            craftingPawnAgent.SetDestination(computedDestinationArray[craftProgressInt]);
            Debug.Log($"Distance between pos and destination: {craftingPawnAgent.remainingDistance}");
            //}
        }
    }

    void Start()
    {
        diceManager = GetComponent<DiceManagerV2>();
        craftVectorArray = new Vector2[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        craftingPawnStartingPosition = craftingPawn.transform.position;
        craftingPawnAgent = craftingPawn.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        remainingDistance = craftingPawnAgent.remainingDistance;
        if (Input.GetKeyDown(KeyCode.R))
        {
            RollNewStuff();
        }
        if (isMoving)
        {
            UpdateCraftProgressInt();
        }
    }

    private void UpdateCraftProgressInt()
    {
        Debug.Log("In update function: "+craftingPawnAgent.remainingDistance);
        if (craftingPawnAgent.remainingDistance < 0.1f)
        {
            if (craftProgressInt == craftVectorArray.Length - 1)
            {
                isMoving = false;
                return;
            }
            CraftProgressInt++;
        }
    }

    private void ComputeCraftVectorPath(Vector2[] vectorArray)
    {
        Vector3[] vectorPath = new Vector3[vectorArray.Length];
        vectorPath[0] = craftingPawnStartingPosition + new Vector3(vectorArray[0].x, 0, vectorArray[0].y);
        for (int i = 1; i < vectorArray.Length; i++)
        {
            vectorPath[i] = vectorPath[i - 1] + new Vector3(vectorArray[i].x, 0, vectorArray[i].y);
        }
        computedDestinationArray = vectorPath;
    }

    public void RollNewStuff()
    {
        InitiateRoll();
        Debug.Log($"Craft color is {GetCraftAttribute(facesResultArray)}.");
        diceManager.craftStrengthAreaParent.SetActive(true);
        ComputeCraftVectorPath(craftVectorArray);
        UpdateSprites();
        CraftProgressInt = 0;
        isMoving = true;
    }

    public void UpdateFaceResultArray()
    {
        facesResultArray = diceManager.diceResultsArray;
    }

    private void InitiateRoll()
    {
        UpdateFaceResultArray();
        craftingPawn.transform.position = craftingPawnStartingPosition;
        //Vector2[] vectors = diceManager.possibleDiceVectors;
        //string[] colors = diceManager.possibleDiceColors;
        //string[] rarities = diceManager.possibleDiceRarities;

        for (int i = 0; i < facesResultArray.Length; i++)
        {
            FaceComponent face = facesResultArray[i];
            //string randomColor = colors[UnityEngine.Random.Range(0, colors.Length)];
            //Vector2 randomVector = vectors[UnityEngine.Random.Range(0, vectors.Length)];
            //face.faceColor = randomColor;
            //face.faceVector = randomVector;
            craftVectorArray[i] = face.faceVector * vectorStrength;
        }
    }

    private string GetCraftAttribute(FaceComponent[] faceArray)
    {
        resultDictionary.Clear();
        for (int i = 0; i < facesResultArray.Length; i++)
        {
            string possibleNewColor = facesResultArray[i].faceColor;
            if (!resultDictionary.ContainsKey(possibleNewColor))
            {
                resultDictionary.Add(possibleNewColor, 1);
            }
            else
            {
                resultDictionary[possibleNewColor]++;
            }
        }
        //foreach (KeyValuePair<string, int> kvp in resultDictionary)
        //{
        //    Debug.Log(kvp);
        //}
        KeyValuePair<string, int> chosenKvp = resultDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r);
        string chosenAttribute = chosenKvp.Key;
        return chosenAttribute;
    }

    int GetVectorImageFromArray(FaceComponent face)
    {
        int vectorIndex = Array.IndexOf(diceManager.possibleDiceVectors, face.faceVector);
        return vectorIndex;
    }

    public void UpdateSprites()
    {
        UpdateFaceResultArray();
        for (int i = 0; i < facesResultArray.Length; i++)
        {
            GameObject resultSlot = resultGameObjects[i];
            FaceComponent face = facesResultArray[i];
            Color chosenColor;
            if (ColorUtility.TryParseHtmlString(face.faceColor, out chosenColor))
            {
                resultSlot.transform.GetChild(0).GetComponent<RawImage>().color = chosenColor;
            }
            Material chosenMaterial = materialArray[GetVectorImageFromArray(face)];
            resultSlot.transform.GetChild(1).GetComponent<RawImage>().material = chosenMaterial;
        }
    }
}