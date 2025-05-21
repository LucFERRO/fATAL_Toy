using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Drawing;

public class TileSplitManager : MonoBehaviour
{
    public int numberOfTiles;
    public Dictionary<string, int> gridTileSplitDictionary = new();
    public Dictionary<string, int> comboTileSplitDictionary = new();
    public UnityEngine.Color objectiveColor;
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

    public AmbientSoundsManager ambientSoundsManager;
    private FMOD.Studio.EventInstance wildlifeEventInstance;

    [Header("UI Properties")]
    [SerializeField] private float lerpSpeed;
    [SerializeField] private UnityEngine.Color[] biomeNameColors;

    [Header("Objective Positions")]
    private Vector3[] startingPositions;
    [SerializeField] private int speed;
    [SerializeField] private Vector3[] targetPositions;
    [SerializeField] private float moveOffset = 5f;
    [SerializeField] private float snapThreshold;

    [Header("References")]
    public Animator endUiAnimator;
    public Canvas mainCanvas;
    public Canvas winCanvas;
    public GameObject objectiveListGo;
    public TextMeshProUGUI[] objectiveElements;
    public bool[] areObjectiveOpenBools;
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
        wildlifeEventInstance = ambientSoundsManager.wildlifeEventInstance;
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
        //HandleObjectivePositions();
        HandleObjectiveKeyWordColor();
    }

    private void UpdateWildlife()
    {
        if (gridTileSplitDictionary["empty"] >= 30 && gridTileSplitDictionary["empty"] <= 60)
        {
            ambientSoundsManager.wildlifeEventInstance.setParameterByName("AmbianceBalance", 4);
        }
        if (gridTileSplitDictionary["empty"] < 30)
        {
            ambientSoundsManager.wildlifeEventInstance.setParameterByName("AmbianceBalance", 7);
        }
    }

    private void HandleObjectiveKeyWordColor()
    {
        for (int i = 1; i < objectiveElements.Length; i++)
        {
            UnityEngine.Color[] gradientColors = GetFittingGradientColors(objectiveTargets[i]);
            // Calculate the lerp value using Mathf.Sin
            float t = (Mathf.Sin(Time.time * lerpSpeed) + 1f) / 2f;
            UnityEngine.Color lerpedColor = UnityEngine.Color.Lerp(gradientColors[0], gradientColors[1], t);

            // Convert the color to a hex string
            string colorHex = ColorUtility.ToHtmlStringRGB(lerpedColor);

            // Update the text with the animated color for the target word
            objectiveElements[i].text = GetObjectiveString(i, objectiveTargets[i], objectiveCurrentNumbers[i], objectiveMaxNumbers[i], colorHex);
        }
    }

    private void HandleObjectivePositions()
    {
        Vector3 offset = new Vector3(-moveOffset, 0, 0);

        //Set the target positions depending on completed objectives
        for (int i = 0; i < startingPositions.Length; i++)
        {
            targetPositions[i] = objectiveBools[i] ? startingPositions[i] + offset : startingPositions[i];
        }

        //Handle the position of each objective
        for (int i = 0; i < startingPositions.Length; i++)
        {
            if (objectiveBools[i])
            {
                Image leafImage = objectiveListGo.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>();
                UnityEngine.Color targetColor = leafImage.color;
                targetColor.a = Mathf.Lerp(targetColor.a, 1f, lerpSpeed * Time.deltaTime);
                leafImage.color = targetColor;
            }
            objectiveListGo.transform.GetChild(i).transform.position = Vector3.Lerp(objectiveListGo.transform.GetChild(i).transform.position, targetPositions[i], Time.deltaTime * speed);
        }

        //Snap to the position if close enough
        for (int i = 0; i < startingPositions.Length; i++)
        {
            if (Vector3.Distance(objectiveListGo.transform.GetChild(i).transform.position, targetPositions[i]) < snapThreshold)
            {
                objectiveListGo.transform.GetChild(i).transform.position = targetPositions[i];
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

    private UnityEngine.Color[] GetFittingGradientColors(string biomeObjective)
    {
        string[] biomeArray = ProcessTileType(biomeObjective);
        UnityEngine.Color fittingColor1 = biomeNameColors[Array.IndexOf(Enum.GetNames(typeof(TileType)), biomeArray[0])];
        UnityEngine.Color fittingColor2 = biomeNameColors[Array.IndexOf(Enum.GetNames(typeof(TileType)), biomeArray[1])];
        return new UnityEngine.Color[] { fittingColor1, fittingColor2 };
    }

    public void UpdateObjectivePackage()
    {
        UpdateTileSplitDictionary();
        UpdateObjectives();
        CheckObjectives();
        UpdateWildlife();
    }

    private void InitializeObjectives()
    {
        int objectiveNumber = objectiveListGo.transform.childCount;
        objectiveBools = new bool[objectiveNumber];
        ObjectiveBools = new bool[objectiveNumber];
        objectiveTargets = new string[objectiveNumber];
        objectiveMaxNumbers = new int[objectiveNumber];
        objectiveCurrentNumbers = new int[objectiveNumber];
        //areObjectiveOpenBools = new bool[objectiveNumber];
        RandomObjectives(1);
        startingPositions = new Vector3[objectiveNumber];
        targetPositions = new Vector3[objectiveNumber];
        for (int i = 0; i < objectiveNumber; i++)
        {
            startingPositions[i] = objectiveListGo.transform.GetChild(i).position;
            string fittingColor = $"{ColorUtility.ToHtmlStringRGB(biomeNameColors[Array.IndexOf(Enum.GetNames(typeof(TileType)), objectiveTargets[i])])}";
            TextMeshProUGUI targetObjectiveString = objectiveElements[i];
            targetObjectiveString.text = GetObjectiveString(i, objectiveTargets[i], objectiveCurrentNumbers[i], objectiveMaxNumbers[i], fittingColor);
        }
    }

    private TileType GhettoTileTypeToColor(string type)
    {
        TileType res;
        if (Enum.TryParse(type, out res))
        {
            return res;
        }
        return TileType.forest;
    }

    private void RandomObjectives(int difficulty)
    {
        for (int i = 0; i < objectiveTargets.Length; i++)
        {
            if (i == 0)
            {
                objectiveTargets[i] = gridTileSplitDictionary.ElementAt(UnityEngine.Random.Range(1, gridTileSplitDictionary.Count)).Key;
                objectiveMaxNumbers[i] = UnityEngine.Random.Range(8, 12 + difficulty * 3);
            }
            else
            {
                objectiveTargets[i] = comboTileSplitDictionary.ElementAt(UnityEngine.Random.Range(1, comboTileSplitDictionary.Count)).Key;
                objectiveMaxNumbers[i] = UnityEngine.Random.Range(3, 5 + difficulty);
            }
        }
        while (objectiveTargets[1] == objectiveTargets[2])
        {
            objectiveTargets[2] = comboTileSplitDictionary.ElementAt(UnityEngine.Random.Range(1, comboTileSplitDictionary.Count)).Key;
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
                objectiveCurrentNumbers[i] = gridTileSplitDictionary[objectiveTargets[i]];
            }
            else
            {
                objectiveCurrentNumbers[i] = comboTileSplitDictionary[objectiveTargets[i]];
            }

            if (objectiveCurrentNumbers[i] >= objectiveMaxNumbers[i])
            {
                objectiveBools[i] = true;
            }
            else
            {
                objectiveBools[i] = false;
            }

            TextMeshProUGUI targetObjectiveString = objectiveElements[i];
            string fittingColor = $"{ColorUtility.ToHtmlStringRGB(biomeNameColors[Array.IndexOf(Enum.GetNames(typeof(TileType)), objectiveTargets[i])])}";
            targetObjectiveString.text = GetObjectiveString(i, objectiveTargets[i], objectiveCurrentNumbers[i], objectiveMaxNumbers[i], fittingColor);
        }
    }

    private string GetObjectiveString(int objectiveId, string objectiveTarget, int objectiveCurrentNumber, int objectiveMaxNumber, string objectiveColor)
    {
        UpdateObjectiveShortVersion(objectiveElements[objectiveId].transform.parent, objectiveTarget, objectiveCurrentNumber, objectiveMaxNumber);
        string message = $"Create {objectiveMaxNumber} <sprite name={objectiveTarget}> <color=#{objectiveColor}>{objectiveTarget}{(objectiveMaxNumber > 1 ? "s" : "")}</color>.";
        return message;
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

    private void UpdateObjectiveShortVersion(Transform objectiveTransform, string target, int current, int max)
    {
        TextMeshProUGUI objectiveShortText = objectiveTransform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        objectiveShortText.text = $"<size=32>{current}</size>/{max}<size=32><sprite name={target}>";
    }

    private void SwitchCompleteObjectiveAlpha(Transform objectiveTransform, bool objectiveBool)
    {
        Image leafImage = objectiveTransform.GetChild(1).GetComponent<Image>();
        TextMeshProUGUI objectiveShortText = objectiveTransform.GetChild(2).GetComponent<TextMeshProUGUI>();

        UnityEngine.Color newLeafColor = leafImage.color;
        newLeafColor.a = objectiveBool ? 1f : 0f;
        leafImage.color = newLeafColor;

        UnityEngine.Color newDetailsColor = objectiveShortText.color;
        newDetailsColor.a = objectiveBool ? 0f : 1f;
        objectiveShortText.color = newDetailsColor;
    }

    public void ToggleOpenTargetObjective(int objectiveId)
    {
        //areObjectiveOpenBools[objectiveId] = !areObjectiveOpenBools[objectiveId];
        objectiveElements[objectiveId].transform.parent.GetComponent<Animator>().SetTrigger("ToggleTrigger");
    }

    private void CheckObjectives()
    {
        for (int i = 0; i < objectiveBools.Length; i++)
        {
            TextMeshProUGUI textMeshProElement = objectiveElements[i];
            if (objectiveBools[i])
            {
                //textMeshProElement.color = UnityEngine.Color.green;
                textMeshProElement.fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                //textMeshProElement.color = objectiveColor;
                textMeshProElement.fontStyle = FontStyles.Normal;
            }
            //if (areObjectiveOpenBools[i])
            //{
            //    ToggleOpenTargetObjective(i);
            //}
            //SwitchCompleteObjectiveAlpha(objectiveElements[i].transform.parent, objectiveBools[i]);
        }

        if (!objectiveBools.All(b => b))
        {
            return;
        }

        Debug.Log("All objectives are done!");
        //mainCanvas.gameObject.SetActive(false);
        endUiAnimator.gameObject.SetActive(true);
        endUiAnimator.SetTrigger("ToggleTrigger");
    }
    public void RestartGameButton()
    {
        winCanvas.gameObject.SetActive(true);
        Invoke("ReloadScene", 2f);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ambientSoundsManager.wildlifeEventInstance.setParameterByName("AmbianceBalance", 0);
    }

    public void UpdateTileSplitDictionary()
    {
        CreateFreshTileDictionary();

        for (int i = 0; i < numberOfTiles; i++)
        {
            Transform tileTransform = transform.GetChild(i).GetChild(0);
            if (tileTransform.parent.childCount > 1)
            {
                for (int j = 1; j < tileTransform.parent.childCount; j++)
                {
                    if (!tileTransform.parent.GetChild(j).CompareTag("Particles"))
                    {
                        Destroy(tileTransform.parent.GetChild(j).gameObject);
                    }
                }
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