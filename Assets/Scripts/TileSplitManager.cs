using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.SceneManagement;
using System.Drawing;
using UnityEngine.InputSystem;

public class TileSplitManager : MonoBehaviour
{
    public int numberOfTiles;
    public int objectiveIconSize;
    public Dictionary<string, int> gridTileSplitDictionary = new();
    public Dictionary<string, int> comboTileSplitDictionary = new();
    public Dictionary<string, int> previousGridTileSplitDictionary = new();
    public Dictionary<string, int> previousComboTileSplitDictionary = new();
    public UnityEngine.Color objectiveColor;

    [Header("REWORKED Objectives")]
    private List<TileObjective> objectives = new();
    public int finishedBatchCount;
    public int lvlDifficulty;
    private Vector3 startingCameraPos;
    public bool hasCameraMoved;

    [Header("OLD Objectives")]
    private string[] objectiveTargets;
    private int[] objectiveMaxNumbers;
    private int[] objectiveCurrentNumbers;
    public string[] objectiveStrings;
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


    [Header("UI Properties")]
    [SerializeField] private float lerpSpeed;
    [SerializeField] private UnityEngine.Color[] biomeNameColors;

    [Header("FMOD")]
    public AmbientSoundsManager ambientSoundsManager;
    private FMOD.Studio.EventInstance wildlifeEventInstance;
    private FMOD.Studio.EventInstance objectivesEventInstance;

    [Header("References")]
    public Camera mainCamera;
    public GameManager gameManager;
    public Animator endUiAnimator;
    public Animator stencilAnimator;
    public Canvas mainCanvas;
    //public Canvas winCanvas;
    public GameObject objectiveListGo;
    public TextMeshProUGUI[] objectiveElements;
    public bool[] areObjectiveOpenBools;

    void Start()
    {
        startingCameraPos = mainCamera.transform.position;
        numberOfTiles = transform.childCount;
        UpdateTileSplitDictionary();
        UpdatePreviousDictionaries();
        InitializeObjectives();
        wildlifeEventInstance = ambientSoundsManager.wildlifeEventInstance;
        objectivesEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Objectives");
        GlowingHexes.OnDiceDestroyed += ResetAllIsNew;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            objectives[0].ForceComplete();
            UpdateObjectives();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            objectives[1].ForceComplete();
            UpdateObjectives();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            objectives[2].ForceComplete();
            UpdateObjectives();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            CreateNewObjectiveBatch();
            UpdateObjectives();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            UpdateObjectives();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            //UpdateTileSplitDictionary();
            Debug.Log($"Base tiles:");
            //Debug.Log(tileTypeToString[forest]);
            foreach (KeyValuePair<string, int> kvp in gridTileSplitDictionary)
            {
                Debug.Log($"{kvp.Key} : {kvp.Value}");
            }
            foreach (KeyValuePair<string, int> kvp in previousGridTileSplitDictionary)
            {
                Debug.Log($"{kvp.Key} : {kvp.Value}");
            }
            //Debug.Log($"Combo tiles:");
            //foreach (KeyValuePair<string, int> kvp in comboTileSplitDictionary)
            //{
            //    Debug.Log($"{kvp.Key} : {kvp.Value}");
            //}
        }
        if (!CrossSceneData.isTutorial)
        {
            HandleObjectiveKeyWordColor();
        }
        if (!hasCameraMoved)
        {
            if (mainCamera.transform.position != startingCameraPos)
            {
                hasCameraMoved = true;
                UpdateObjectives();
            }
        }
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
        for (int i = 0; i < objectiveElements.Length; i++)
        {
            UnityEngine.Color[] gradientColors = GetFittingGradientColors(objectives[i].Biome);
            // Calculate the lerp value using Mathf.Sin
            float t = (Mathf.Sin(Time.time * lerpSpeed) + 1f) / 2f;
            UnityEngine.Color lerpedColor = UnityEngine.Color.Lerp(gradientColors[0], gradientColors[1], t);

            // Convert the color to a hex string
            string colorHex = ColorUtility.ToHtmlStringRGB(lerpedColor);

            // Update the text with the animated color for the target word
            objectiveElements[i].text = GetObjectiveString(i, objectives[i], colorHex);
        }
    }

    private void HandleObjectiveKeyWordColorOLD()
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
            objectiveElements[i].text = GetObjectiveStringOLD(i, objectiveTargets[i], objectiveCurrentNumbers[i], objectiveMaxNumbers[i], colorHex);
        }
    }

    private string[] ProcessTileType(string tileType)
    {
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
        if (string.IsNullOrEmpty(biomeObjective))
        {
            return new UnityEngine.Color[] { objectiveColor, objectiveColor };
        }
        string[] biomeArray = ProcessTileType(biomeObjective);
        UnityEngine.Color fittingColor1 = biomeNameColors[Array.IndexOf(Enum.GetNames(typeof(TileType)), biomeArray[0])];
        if (biomeArray.Length == 1)
        {
            return new UnityEngine.Color[] { fittingColor1, fittingColor1 };
        }
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

    public void ResetAllIsNew()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            List<NeighbourTileProcessor> newProcessors = transform.GetChild(i).GetComponentsInChildren<NeighbourTileProcessor>().Where(tile => tile.isNew).ToList();
            if (newProcessors.Count() == 0)
            {
                continue;
            }
            Debug.Log(newProcessors.Count());
            foreach (NeighbourTileProcessor processor in newProcessors)
            {
                processor.isNew = false;
            }
        }
        //Debug.Log("ALL IS NEW HAVE BEEN RESET");
    }

    private int[] RandomObjectiveTypes()
    {
        int objectiveCount = objectiveElements.Length;
        List<int> possibleTypes = new List<int> { 1, 2, 3, 4 };

        for (int i = possibleTypes.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = possibleTypes[i];
            possibleTypes[i] = possibleTypes[j];
            possibleTypes[j] = temp;
        }

        List<int> result = new List<int> { 0 };
        result.AddRange(possibleTypes.Take(objectiveCount - 1));

        return result
            //.OrderBy(x => x)
            .ToArray();
    }

    private void InitializeObjectives()
    {
        objectives = new List<TileObjective>();
        int objectiveCount = objectiveElements.Length;
        areObjectiveOpenBools = new bool[objectiveCount];
        gameManager.minTilesRolled = int.MaxValue;
        gameManager.maxTilesRolled = int.MinValue;

        if (CrossSceneData.isTutorial)
        {
            lvlDifficulty = 1;
            for (int i = 0; i < objectiveCount; i++)
            {
                TileObjective obj = ObjectiveFactory.GenerateRandomObjective(i + 10, this);
                objectives.Add(obj);
                //UpdateObjectiveUI(i, obj);
                areObjectiveOpenBools[i] = true;
            }
            CrossSceneData.isTutorial = false;
            return;
        }

        int[] objTypes = RandomObjectiveTypes();
        for (int i = 0; i < objectiveCount; i++)
        {
            TileObjective obj = ObjectiveFactory.GenerateRandomObjective(objTypes[i], this);
            if (objTypes[i] == 1 || objTypes[i] == 4)
            {
                obj = ObjectiveFactory.GenerateRandomObjective(objTypes[i], this, objectives[0].Biome);
            }
            objectives.Add(obj);
            //UpdateObjectiveUI(i, obj);
            areObjectiveOpenBools[i] = true;
        }
    }

    private void UpdatePreviousDictionaries()
    {
        previousGridTileSplitDictionary = gridTileSplitDictionary;
        previousComboTileSplitDictionary = comboTileSplitDictionary;
    }

    private void CreateNewObjectiveBatch()
    {

        InitializeObjectives();
        UpdatePreviousDictionaries();
        CreateFreshTileDictionary();
        gameManager.minTilesRolled = int.MaxValue;
        gameManager.maxTilesRolled = int.MinValue;

        for (int i = 0; i < objectives.Count; i++)
        {
            TextMeshProUGUI textMeshProElement = objectiveElements[i];
            //Debug.Log("obj " + i);
            objectiveElements[i].fontStyle = FontStyles.Normal;
            textMeshProElement.transform.parent.GetChild(1).GetComponent<Animator>().SetBool("DoneBool", false);
            if (!areObjectiveOpenBools[i])
            {
                //Debug.Log($"force open obj {i + 1}");
                ToggleOpenTargetObjective(i);
            }
            SetTargetAnimationIsObjClosed(i, false);
        }
    }

    private string GetObjectiveString(int objectiveId, TileObjective objective, string objectiveColor)
    {
        UpdateObjectiveShortVersion(objectiveElements[objectiveId].transform.parent, objective.Biome, objective.Progress, objective.Target);
        //string message = $"Create {objective.Target} <sprite name={objective.Biome}> <color=#{objectiveColor}>{(TileTypeUtils.TileTypeToString.TryGetValue(objective.Biome, out string result) ? result : "")}{(objective.Target > 1 ? "s" : "")}</color>.";
        string properBiomeName = $"{(TileTypeUtils.TileTypeToString.TryGetValue(objective.Biome, out string result) ? result : "")}{(objective.Target > 1 ? (result == "Marsh" ? "es" : "s") : "")}";
        string message = objective.Description.Replace("TARGET", objective.Target.ToString()).Replace("BIOME", $"<size={objectiveIconSize}><sprite name={objective.Biome}></size> <color=#{objectiveColor}>{properBiomeName}</color>");
        return message;
    }

    private void UpdateObjectiveShortVersion(Transform objectiveTransform, string target, int current, int max)
    {
        TextMeshProUGUI objectiveShortText = objectiveTransform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        objectiveShortText.text = $"<size=25>{current}</size>/{max}{(string.IsNullOrEmpty(target) ? "" : $"<size=30><sprite name={target}>")}";
    }

    public void UpdateObjectives()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].IsCompleted)
            {
                continue;
            }
            objectives[i].Evaluate(this);
            UpdateObjectiveUI(i, objectives[i]);
        }
        CheckObjectives();
    }

    public bool CheckIfCameraHasMoved()
    {
        hasCameraMoved = mainCamera.transform.position != startingCameraPos;
        return hasCameraMoved;
    }
    public bool CheckIfDiceHasChanged()
    {
        return gameManager.diceWasChanged;
    }
    public bool CheckIfDiceWasThrown()
    {
        return gameManager.diceWasThrown;
    }

    public int biomeTilesCreatedSinceReset(string tile)
    {
        int res = gridTileSplitDictionary[tile] - previousGridTileSplitDictionary[tile];
        return Mathf.Max(res, 0);
    }

    public int comboTilesCreatedSinceReset(string combo)
    {
        int res = comboTileSplitDictionary[combo] - previousComboTileSplitDictionary[combo];
        return Mathf.Max(res, 0);
    }

    public int WhatIsMaxRoll()
    {
        return gameManager.MaxTilesRolled;
    }
    public int WhatIsMinRoll()
    {
        return gameManager.MinTilesRolled;
    }

    public bool IsDiceMadeOfOneBiome(string targetBiome)
    {
        return gameManager.IsDiceMadeOfOneBiome(targetBiome);
    }

    private void UpdateObjectiveUI(int index, TileObjective obj)
    {
        if (index < objectiveElements.Length)
        {
            objectiveElements[index].text = GetObjectiveString(index, obj, "ff0000") +
                //$"\n<size=80%>{obj.GetProgressString()}</size>" +
                $"";
            // Optionally, update color/animation as before

        }
    }

    private void CheckObjectives()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            TextMeshProUGUI textMeshProElement = objectiveElements[i];
            if (objectives[i].IsCompleted)
            {
                objectiveElements[i].fontStyle = FontStyles.Strikethrough;
                if (!textMeshProElement.transform.parent.GetChild(1).GetComponent<Animator>().GetBool("DoneBool"))
                {
                    textMeshProElement.transform.parent.GetChild(1).GetComponent<Animator>().SetBool("DoneBool", true);
                    objectivesEventInstance.setParameterByName("ObjectiveState", 1);
                    objectivesEventInstance.start();
                }
                if (areObjectiveOpenBools[i])
                {
                    //Debug.Log($"force close obj {i + 1}");
                    ToggleOpenTargetObjective(i);
                }
                SetTargetAnimationIsObjClosed(i, true);
            }
        }

        if (objectives.All(o => o.IsCompleted))
        {
            finishedBatchCount++;

            if (finishedBatchCount >= lvlDifficulty)
            {
                if (!endUiAnimator.gameObject.activeSelf)
                {
                    Debug.Log("All objectives are done!");
                    objectivesEventInstance.setParameterByName("ObjectiveState", 0);
                    objectivesEventInstance.start();
                    endUiAnimator.gameObject.SetActive(true);
                    endUiAnimator.SetTrigger("ToggleTrigger");
                }
            }
            else
            {
                StartCoroutine(CreateNewObjectiveBatchCoroutine());
            }
        }
    }

    private IEnumerator CreateNewObjectiveBatchCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        CreateNewObjectiveBatch();
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

    public void UpdateObjectivesOLD()
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
            targetObjectiveString.text = GetObjectiveStringOLD(i, objectiveTargets[i], objectiveCurrentNumbers[i], objectiveMaxNumbers[i], fittingColor);
        }
    }

    private string GetObjectiveStringOLD(int objectiveId, string objectiveTarget, int objectiveCurrentNumber, int objectiveMaxNumber, string objectiveColor)
    {
        UpdateObjectiveShortVersionOLD(objectiveElements[objectiveId].transform.parent, objectiveTarget, objectiveCurrentNumber, objectiveMaxNumber);
        string message = $"Create {objectiveMaxNumber} <sprite name={objectiveTarget}> <color=#{objectiveColor}>{TileTypeUtils.TileTypeToString[objectiveTarget]}{(objectiveMaxNumber > 1 ? "s" : "")}</color>.";
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

    private void UpdateObjectiveShortVersionOLD(Transform objectiveTransform, string target, int current, int max)
    {
        TextMeshProUGUI objectiveShortText = objectiveTransform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        objectiveShortText.text = $"<size=25>{current}</size>/{max}<size=30><sprite name={target}>";
    }

    public void ToggleOpenTargetObjective(int objectiveId)
    {
        areObjectiveOpenBools[objectiveId] = !areObjectiveOpenBools[objectiveId];
        SetTargetAnimationIsObjClosed(objectiveId, !areObjectiveOpenBools[objectiveId]);
    }

    public void SetTargetAnimationIsObjClosed(int objectiveId, bool objectiveState)
    {
        objectiveElements[objectiveId].transform.parent.GetComponent<Animator>().SetBool("IsObjectiveClosed", objectiveState);
    }

    private void CheckObjectivesOLD()
    {
        for (int i = 0; i < objectiveBools.Length; i++)
        {
            TextMeshProUGUI textMeshProElement = objectiveElements[i];
            if (objectiveBools[i])
            {
                //textMeshProElement.color = UnityEngine.Color.green;
                textMeshProElement.fontStyle = FontStyles.Strikethrough;
                if (!textMeshProElement.transform.parent.GetChild(1).GetComponent<Animator>().GetBool("DoneBool"))
                {
                    textMeshProElement.transform.parent.GetChild(1).GetComponent<Animator>().SetBool("DoneBool", true);
                }
                if (areObjectiveOpenBools[i])
                {
                    ToggleOpenTargetObjective(i);
                }
            }
            //else
            //{
            //    textMeshProElement.fontStyle = FontStyles.Normal;
            //}
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
        //winCanvas.gameObject.SetActive(true);
        stencilAnimator.SetTrigger("StencielAppearTrigger");
        NeighbourTileProcessor.currentLockedTiles = 0;
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
            if (tileTransform.GetComponent<NeighbourTileProcessor>().isLocked)
            {
                continue;
            }
            if (tileTransform.parent.childCount > 1)
            {
                for (int j = 1; j < tileTransform.parent.childCount; j++)
                {
                    Destroy(tileTransform.parent.GetChild(j).gameObject);
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