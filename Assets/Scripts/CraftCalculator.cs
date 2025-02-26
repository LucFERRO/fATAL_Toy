using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CraftCalculator : MonoBehaviour
{
    //[SerializeField] public string[][] rollResults;
    //public List colorList;
    public GameObject craftingPawn;
    private Vector3 craftingPawnStartingPosition;
    private NavMeshAgent craftPawnAgent;
    public List<FaceComponent> diceRollResults;
    public FaceComponent[] diceResultsArray;
    public List<string> colorList;
    public Dictionary<string, int> resultDictionary = new();



    //TEST
    public bool isMoving;
    private FaceComponent[] testFaces;
    public Vector2[] craftVectorArray;
    public Vector3[] computedDestinationArray;
    public int craftProgressInt;
    public int CraftProgressInt
    {
        get
        {
            return craftProgressInt;
        }
        set
        {
            craftProgressInt = value;
            craftPawnAgent.SetDestination(computedDestinationArray[craftProgressInt]);
        }
    }

    void Start()
    {
        craftVectorArray = new Vector2[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        craftingPawnStartingPosition = craftingPawn.transform.position;
        craftPawnAgent = craftingPawn.GetComponent<NavMeshAgent>();
        //craftingPawn.SetActive(false);
        //string[] vectors = new string[5] { "up", "up","right", "up", "right"};
        //string[] colors = new string[5] { "green", "red","green", "blue", "red"};
        //rollResults = new string[][] { vectors, colors };

        //Debug.Log("Liste: "+diceRollResults[2].faceType);
        //Debug.Log(diceRollResults[2].faceColor);
        //Debug.Log(" "+diceResultsArray[4].faceType);
        //Debug.Log(diceResultsArray[4].faceColor);



        //for (int i = 0; i < resultDictionary.Count; i++)
        //{
        //    Debug.Log();
        //}

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RollNewStuff();
            isMoving = true;
        }
        if (isMoving)
        {
            UpdateCraftPawnDestination();
        }
    }

    private void UpdateCraftPawnDestination()
    {
        if (craftPawnAgent.remainingDistance < 0.5f)
        {
            if (craftProgressInt == craftVectorArray.Length - 1)
            {
                isMoving = false;
                return;
            }
            CraftProgressInt++;
        }
    }

    private void UpdateCraftPawnDestination(int destinationIndex)
    {

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
    }

    private void InitiateTest()
    {
        craftProgressInt = 0;
        testFaces = diceResultsArray;
        Vector2[] vectors = new Vector2[8] { Vector2.up, Vector2.right, Vector2.down, Vector2.left, (Vector2.left + Vector2.up).normalized, (Vector2.up + Vector2.right).normalized, (Vector2.right + Vector2.down).normalized, (Vector2.down + Vector2.left).normalized };
        string[] colors = new string[3] { "red", "green", "blue" };

        for (int i = 0; i < testFaces.Length; i++)
        {
            FaceComponent face = testFaces[i];
            string randomColor = colors[Random.Range(0, 3)];
            Vector2 randomVector = vectors[Random.Range(0, 4)];
            face.faceColor = randomColor;
            face.faceVector = randomVector * 4f;
            craftVectorArray[i] = randomVector * 4f;
        }
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
}