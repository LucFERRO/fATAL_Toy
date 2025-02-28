using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CraftCalculator : MonoBehaviour
{
    //[SerializeField] public string[][] rollResults;
    //public List colorList;
    public GameObject craftingPawn;
    private Vector3 craftingPawnStartingPosition;
    private NavMeshAgent craftingPawnAgent;
    public List<FaceComponent> diceRollResults;
    public FaceComponent[] diceResultsArray;
    public List<string> colorList;
    public Dictionary<string, int> resultDictionary = new();



    //TEST
    public GameObject[] resultGameObjects;
    public Material[] materialArray;
    //public ProtoRandomVectors protoRandomVectors;
    public float remainingDistance;
    public int craftProgressInt;
    public float vectorStrength;
    public bool isMoving;
    public FaceComponent[] testFaces;
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
            if (computedDestinationArray.Length != 0)
            {

                craftingPawnAgent.SetDestination(computedDestinationArray[craftProgressInt]);
            }
        }
    }

    void Start()
    {
        CraftProgressInt = 0;
        craftVectorArray = new Vector2[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        craftingPawnStartingPosition = craftingPawn.transform.position;
        craftingPawnAgent = craftingPawn.GetComponent<NavMeshAgent>();
        //Voir avec Bourhis pourquoi marche pas si mis en Press R (skip progressInt = 0)
        RollNewStuff();
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
            UpdateCraftPawnDestination();
        }
    }

    private void UpdateCraftPawnDestination()
    {
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

    private void RollNewStuff()
    {
        InitiateTest();
        Debug.Log(GetCraftAttribute(testFaces));
        ComputeCraftVectorPath(craftVectorArray);
        //protoRandomVectors.UpdateSprites();
        UpdateSprites();
        CraftProgressInt = 0;
        isMoving = true;
    }

    private void InitiateTest()
    {

        testFaces = diceResultsArray;
        craftingPawn.transform.position = craftingPawnStartingPosition;
        //Vector2[] vectors = new Vector2[8] { Vector2.up, Vector2.right, Vector2.down, Vector2.left, (Vector2.left + Vector2.up).normalized, (Vector2.up + Vector2.right).normalized, (Vector2.right + Vector2.down).normalized, (Vector2.down + Vector2.left).normalized };
        Vector2[] vectors = new Vector2[4] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        string[] colors = new string[3] { "red", "green", "blue" };

        for (int i = 0; i < testFaces.Length; i++)
        {
            FaceComponent face = testFaces[i];
            string randomColor = colors[Random.Range(0, 3)];
            Vector2 randomVector = vectors[Random.Range(0, 4)];
            face.faceColor = randomColor;
            face.faceVector = randomVector * vectorStrength;
            craftVectorArray[i] = randomVector * vectorStrength;
        }
        //protoRandomVectors.facesArray = testFaces;
    }

    private string GetCraftAttribute(FaceComponent[] faceArray)
    {
        resultDictionary.Clear();
        for (int i = 0; i < diceResultsArray.Length; i++)
        {
            string possibleNewColor = diceResultsArray[i].faceColor;
            if (!resultDictionary.ContainsKey(possibleNewColor))
            {
                resultDictionary.Add(possibleNewColor, 1);
            }
            else
            {
                resultDictionary[possibleNewColor]++;
            }
        }
        foreach (KeyValuePair<string, int> kvp in resultDictionary)
        {
            Debug.Log(kvp);
        }
        KeyValuePair<string, int> chosenKvp = resultDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r);
        string chosenAttribute = chosenKvp.Key;
        return chosenAttribute;
    }
    //Color GetColorFromArray(FaceComponent face)
    //{
    //    Color chosenColor;
    //    if (face.faceColor == "red")
    //    {
    //        chosenColor = colorArray[0];
    //    }
    //    else if (face.faceColor == "blue")
    //    {
    //        chosenColor = colorArray[2];
    //    }
    //    else
    //    {
    //        chosenColor = colorArray[1];
    //    }
    //    return chosenColor;
    //}

    int GetVectorImageFromArray(FaceComponent face)
    {
        int chosenIndex = 0;
        if (face.faceVector.x > 0)
        {
            chosenIndex = 1;
        }
        else if (face.faceVector.x < 0)
        {
            chosenIndex = 3;
        }
        if (face.faceVector.y > 0)
        {
            chosenIndex = 0;
        }
        else if (face.faceVector.y < 0)
        {
            chosenIndex = 2;
        }

        return chosenIndex;
    }

    public void UpdateSprites()
    {
        for (int i = 0; i < testFaces.Length; i++)
        {
            GameObject resultSlot = resultGameObjects[i];
            Color chosenColor;
            if (ColorUtility.TryParseHtmlString(testFaces[i].faceColor, out chosenColor))
            {
                resultSlot.transform.GetChild(0).GetComponent<RawImage>().color = chosenColor;
            }
            Material chosenMaterial = materialArray[GetVectorImageFromArray(testFaces[i])];
            resultSlot.transform.GetChild(1).GetComponent<RawImage>().material = chosenMaterial;
            //if (i == 1) { 
            //    Debug.Log(chosenColor.ToString());
            //    Debug.Log(chosenMaterial.name);
            //    Debug.Log("should be "+ facesArray[i].faceColor + " and " + facesArray[i].faceVector.x + " : " + facesArray[i].faceVector.y);
            //}
        }
    }
}