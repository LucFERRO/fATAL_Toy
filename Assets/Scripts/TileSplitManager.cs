using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngineInternal;

public class TileSplitManager : MonoBehaviour
{
    public int numberOfTiles;
    public Dictionary<string, int> gridTileSplitDictionary = new();
    public Dictionary<string, int> comboTileSplitDictionary = new();

    [Header("Objectives Numbers")]
    //private string objectiveNumber1Target;
    //private int objectiveNumber1Max;
    //private int objectiveNumber1Current;    
    //private string objectiveNumber2Target;
    //private int objectiveNumber2Max;
    //private int objectiveNumber2Current;    
    //private string objectiveNumber3Target;
    //private int objectiveNumber3Max;
    //private int objectiveNumber3Current;

    private string[] objectiveTargets;
    private int[] objectiveMaxNumbers;
    private int[] objectiveCurrentNumbers;

    [Header("Objective Positions")]
    private Vector3[] startingPositions;
    [SerializeField] private int speed;
    [SerializeField] private Vector3[] targetPositions;
    [SerializeField] private float moveOffset = 5f;
    [SerializeField] private float snapThreshold;

    [Header("References")]
    public Canvas mainCanvas;
    public Canvas winCanvas;
    public GameObject objectiveListGo;
    public TextMeshProUGUI[] objectiveElements;
    public bool[] objectiveBools;
    public bool[] ObjectiveBools
    {
        get { return objectiveBools; }
        set
        {
            objectiveBools = value;
            CheckObjectives();
        }
    }
    public string[] objectiveStrings;


    void Start()
    {
        numberOfTiles = transform.childCount;
        UpdateTileSplitDictionary();
        InitializeObjectives();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            UpdateObjectives();
            CheckObjectives();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateTileSplitDictionary();
            Debug.Log($"Base tiles:");
            foreach (KeyValuePair<string, int> kvp in gridTileSplitDictionary)
            {
                Debug.Log($"{kvp.Key} : {kvp.Value}");
            }
            Debug.Log($"Combo tiles:");
            foreach (KeyValuePair<string, int> kvp in comboTileSplitDictionary)
            {
                Debug.Log($"{kvp.Key} : {kvp.Value}");
            }
        }
        HandleObjectivePositions();
    }

    public void UpdateObjectivePackage()
    {
        UpdateTileSplitDictionary();
        UpdateObjectives();
        CheckObjectives();
    }

    private void InitializeObjectives()
    {
        int objectiveNumber = objectiveListGo.transform.childCount;
        objectiveBools = new bool[objectiveNumber];
        ObjectiveBools = new bool[objectiveNumber];
        objectiveTargets = new string[objectiveNumber];
        objectiveMaxNumbers = new int[objectiveNumber];
        objectiveCurrentNumbers = new int[objectiveNumber];
        RandomObjectives(1);
        startingPositions = new Vector3[objectiveNumber];
        targetPositions = new Vector3[objectiveNumber];
        for (int i = 0; i < objectiveNumber; i++)
        {
            startingPositions[i] = objectiveListGo.transform.GetChild(i).position;
            TextMeshProUGUI targetObjectiveString = objectiveElements[i];
            targetObjectiveString.text = $"Create {objectiveMaxNumbers[i]} {objectiveTargets[i]}{(objectiveMaxNumbers[i] > 1 ? "s" : "")}. Currently: {objectiveCurrentNumbers[i]}/{objectiveMaxNumbers[i]}.";
        }
    }

    private void RandomObjectives(int difficulty)
    {
        for (int i = 0; i < objectiveTargets.Length; i++)
        {
            if (i == 0)
            {
                objectiveTargets[i] = gridTileSplitDictionary.ElementAt(UnityEngine.Random.Range(1, gridTileSplitDictionary.Count)).Key;
                objectiveMaxNumbers[i] = UnityEngine.Random.Range(3, 5 + difficulty * 2);
            }
            else
            {
                objectiveTargets[i] = comboTileSplitDictionary.ElementAt(UnityEngine.Random.Range(1, comboTileSplitDictionary.Count)).Key;
                objectiveMaxNumbers[i] = UnityEngine.Random.Range(1, 2 + difficulty);
            }
        }
    }

    public void UpdateObjectives()
    {
        for (int i = 0; i < objectiveTargets.Length; i++)
        {
            if (objectiveBools[i])
            {
                continue;
            }

            if (i == 0)
            {
                //Debug.Log($"Objective {i} : {objectiveTargets[i]}");
                objectiveCurrentNumbers[i] = gridTileSplitDictionary[objectiveTargets[i]];
            }
            else
            {
                //Debug.Log($"Objective {i} : {objectiveTargets[i]}");
                objectiveCurrentNumbers[i] = comboTileSplitDictionary[objectiveTargets[i]];
            }

            if (objectiveCurrentNumbers[i] >= objectiveMaxNumbers[i])
            {
                //Debug.Log($"Objective {i} : DONE with {objectiveCurrentNumbers[i]}");
                objectiveBools[i] = true;
                //StartCoroutine(CompletedObjective(objectiveListGo.transform.GetChild(i), 0.2f));
            }
            else
            {
                //Debug.Log($"Objective {i} : NOT DONE with {objectiveCurrentNumbers[i]}");
                objectiveBools[i] = false;
            }

            TextMeshProUGUI targetObjectiveString = objectiveElements[i];
            targetObjectiveString.text = $"Create {objectiveMaxNumbers[i]} {objectiveTargets[i]}{(objectiveMaxNumbers[i] > 1 ? "s" : "")}. Currently: {objectiveCurrentNumbers[i]}/{objectiveMaxNumbers[i]}.";
        }
    }
    public IEnumerator CompletedObjective(Transform objectiveTransform, float time)
    {
        yield return new WaitForSeconds(time);
        Vector3 targetPos = objectiveTransform.position;
        targetPos.x -= 50;
        objectiveTransform.position = Vector3.Lerp(objectiveTransform.position, targetPos, Time.deltaTime);
    }
    public void ChooseObjectiveToComplete(int objectiveId)
    {
        bool[] newObjectiveBoolArray = new bool[objectiveBools.Length];
        for (int i = 0; i < objectiveBools.Length; i++)
        {
            if (i == objectiveId)
            {
                newObjectiveBoolArray[i] = !objectiveBools[i];
            }

            else
            {
                newObjectiveBoolArray[i] = objectiveBools[i];
            }
        }
        ObjectiveBools = newObjectiveBoolArray;
    }

    private void CheckObjectives()
    {
        for (int i = 0; i < objectiveBools.Length; i++)
        {
            TextMeshProUGUI textMeshProElement = objectiveElements[i];
            if (objectiveBools[i])
            {
                textMeshProElement.color = Color.green;
                textMeshProElement.fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                textMeshProElement.color = Color.white;
                textMeshProElement.fontStyle = FontStyles.Normal;
            }
        }

        if (!objectiveBools.All(b => b))
        {
            return;
        }

        Debug.Log("All objectives are done!");
        //mainCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(true);
        Invoke("ReloadScene", 2f);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void UpdateTileSplitDictionary()
    {
        CreateFreshTileDictionary();

        for (int i = 0; i < numberOfTiles; i++)
        {
            Transform tileTransform = transform.GetChild(i).GetChild(0);
            if (tileTransform.parent.childCount > 1)
            {
                Destroy(tileTransform.parent.GetChild(1).gameObject);
            }
            string type = tileTransform.GetComponent<NeighbourTileProcessor>().tileType;

            string[] types = Regex.Split(type, @"(?<!^)(?=[A-Z])");
            if (types.Length > 1)
            {
                types[1] = types[1].ToLower();
                comboTileSplitDictionary[type]++;
            }
            HashSet<string> uniqueTypes = new HashSet<string>(types);
            foreach (string subType in uniqueTypes)
            {
                if (gridTileSplitDictionary.ContainsKey(subType))
                {
                    gridTileSplitDictionary[subType]++;
                }
                else
                {
                    Debug.LogWarning($"Unexpected tile type: {subType}. Ensure all tile types are accounted for.");
                }
            }
        }
    }

    private void CreateFreshTileDictionary()
    {
        gridTileSplitDictionary = new Dictionary<string, int>()
        {
            { "empty", 0 }
        };
        comboTileSplitDictionary = new Dictionary<string, int>();

        foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
        {
            if ((int)tileType < 10)
            {
                gridTileSplitDictionary.Add(tileType.ToString(), 0);
            }
            else
            {
                comboTileSplitDictionary.Add(tileType.ToString(), 0);
            }
        }
    }
}