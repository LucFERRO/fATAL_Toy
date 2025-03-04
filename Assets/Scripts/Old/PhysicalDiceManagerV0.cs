using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhysicalDiceManagerV0 : MonoBehaviour
{
    private PhysicalRollUiManager uiManager;
    public GameObject diceSpawner;
    //public GameObject[] unusedDices;
    //public GameObject[] allRolledDices;
    public GameObject[] allDices;
    public DiceData[] allDiceDatas;
    public List<DiceData> dicesInUse;
    //public DiceData[] globalDicesDataInUse;

    public int numberOfDicesInUse;

    [Header("Roll Parameters")]
    public int maxNumberOfDices = 5;
    public int maxNumberOfRolls = 2;
    public int currentNumberOfRolls;
    public float rollThrowForce;
    public float rollSpinForce;

    [Header("Roll Results")]
    public bool isConfirmed;
    public GameObject resultHolder;
    public float resultMovementDuration;
    private float elapsedTime;
    private float percentageComplete;
    private Vector3[] resultHolderPositions;
    //public List<GameObject> currentRolledDices;
    //public List<DiceData> currentRollResults;
    //public List<DiceData> globalRollResults;
    //public string[] globalRollResultsColors;

    public int CurrentNumberOfRolls
    {
        get
        {
            return currentNumberOfRolls;
        }
        set
        {
            currentNumberOfRolls = value;
            if (currentNumberOfRolls < maxNumberOfRolls)
            {
                uiManager.EnableConfirmButton();
                uiManager.HideUiGameObject(uiManager.addDiceButton);
            }
            uiManager.ClearErrorMessage();
            uiManager.UpdateNumberOfRollsLeft();
        }
    }

    public int NumberOfDicesInUse
    {
        get
        {
            return numberOfDicesInUse;
        }
        set
        {
            numberOfDicesInUse = value;
            uiManager.ClearErrorMessage();
            //Update dices in use
            dicesInUse = allDiceDatas.Where(dice => dice.isInUse).ToList();
        }
    }
    void Start()
    {
        uiManager = GetComponent<PhysicalRollUiManager>();
        CurrentNumberOfRolls = maxNumberOfRolls;
        InitiateDices();
        UpdateDiceData();
        resultHolderPositions = new Vector3[] { resultHolder.transform.GetChild(0).transform.position, resultHolder.transform.GetChild(1).transform.position, resultHolder.transform.GetChild(2).transform.position, resultHolder.transform.GetChild(3).transform.position, resultHolder.transform.GetChild(4).transform.position };
    }

    void Update()
    {

        if (isConfirmed)
        {
            if (elapsedTime < resultMovementDuration)
            {
                elapsedTime += Time.deltaTime;
                percentageComplete = elapsedTime / resultMovementDuration;
                PutDicesToResultHolder();
            }
        }
    }
    public void InitiateDices()
    {
        int numberOfDices = transform.childCount;
        allDices = new GameObject[numberOfDices];
        allDiceDatas = new DiceData[numberOfDices];
        for (int i = 0; i < numberOfDices; i++)
        {
            GameObject diceGameObject = transform.GetChild(i).gameObject;
            DiceData diceData = diceGameObject.GetComponent<DiceData>();
            allDices[i] = diceGameObject;
            allDiceDatas[i] = diceData;
        }
        dicesInUse = allDiceDatas.ToList();
    }

    public void UpdateDiceData()
    {
        for (int i = 0; i < allDices.Length; i++)
        {
            allDiceDatas[i] = allDices[i].GetComponent<DiceData>();
        }
    }
    public void RollDices()
    {
        if (currentNumberOfRolls <= 0)
        {
            string outOfRerollText = "OUT OF REROLLS!";
            Debug.Log(outOfRerollText);
            uiManager.DisplayErrorMessage(outOfRerollText);
            return;
        }
        if (dicesInUse.Count == 0)
        {
            string noDiceText = "NO DICE SELECTED!";
            Debug.Log(noDiceText);
            uiManager.DisplayErrorMessage(noDiceText);
            return;
        }

        for (int i = 0; i < dicesInUse.Count; i++)
        {
            // Vecteur pour le throw (just up pour l'instant) et random vector pour le random spin
            Rigidbody diceInUseRigidbody = dicesInUse[i].gameObject.GetComponent<Rigidbody>();
            Vector3 randomSpinVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            diceInUseRigidbody.AddForce(Vector3.up * rollThrowForce, ForceMode.Impulse);
            diceInUseRigidbody.AddTorque(randomSpinVector.normalized * rollSpinForce, ForceMode.Impulse);
        }

        CurrentNumberOfRolls -= 1;
    }

    public void ConfirmRolls()
    {
        CurrentNumberOfRolls = 0;
        isConfirmed = true;
        uiManager.HideUiGameObject(uiManager.confirmRollsButton);
        uiManager.HideUiGameObject(uiManager.rollButton);
        uiManager.ClearErrorMessage();

        for (int i = 0; i < dicesInUse.Count; i++)
        {
            dicesInUse[i].UpdateRollResult();
        }
        //UpdateRolledDices();
        // 0 0 0
        // 0 0 -90
        // 0 0 90
        //90 0 0
        //-90 0 0
        //-180 0 0
    }

    public void PutDicesToResultHolder()
    {
        for (int i = 0; i < allDices.Length; i++)
        {
            allDices[i].transform.eulerAngles = new Vector3(allDices[i].transform.eulerAngles.x, 0, allDices[i].transform.eulerAngles.z);
            allDices[i].GetComponent<Rigidbody>().freezeRotation = true;
            allDices[i].transform.position = Vector3.Lerp(allDices[i].transform.position, resultHolderPositions[i], percentageComplete);
        }
    }

    //private void UpdateUnusedDices()
    //{
    //    if (allRolledDices.Length >= maxNumberOfDices)
    //    {
    //        // Trie parmi tous les dés ceux qui ont toujours le tag "Dice" <=> ceux qui n'ont pas été roll
    //        unusedDices = allDices.Where(dice => dice.CompareTag("Dice")).ToArray();
    //        for (int i = 0; i < unusedDices.Length; i++)
    //        {
    //            unusedDices[i].SetActive(false);
    //        }
    //    }
    //}

    //private void UpdateDicesInUse()
    //{
    //    Trie parmi tous les dés ceux qui ont isInUse
    //   dicesInUse = allDiceDatas.Where(dice => dice.GetComponent<DiceData>().isInUse).ToArray();
    //    currentRollResults = new string[dicesInUse.Length];
    //    for (int i = 0; i < allDiceDatas.Count(); i++)
    //    {
    //        DiceData dice = allDiceDatas[i];
    //        if (!dice.isInUse)
    //        {

    //        }
    //        if (!dicesInUse.Contains(allDiceDatas[i]))
    //        {
    //            dicesInUse.Add(allDiceDatas[i]);
    //        }
    //    }
    //}
    //private void UpdateRolledDices()
    //{
    //    if (dicesInUse.Length == 0)
    //    {
    //        dicesInUse = new DiceData[0];
    //        currentRollResults = new string[0];
    //        return;
    //    }

    //    for (int i = 0; i < dicesInUse.Length; i++)
    //    {
    //        currentRollResults[i] = GetDiceRollResult(dicesInUse[i]);
    //    }

    //    UpdateGlobalRollResults();
    //}

    //private void UpdateGlobalRollResults()
    //{
    //    for (int i = 0; i < globalDicesDataInUse.Length; i++)
    //    {
    //        if (globalDicesDataInUse[i] == null)
    //        {
    //            return;
    //        }
    //        globalRollResults[i] = GetDiceRollResult(globalDicesDataInUse[i]);
    //        globalRollResultsColors[i] = GetDiceRollColorResult(globalDicesDataInUse[i]);
    //    }
    //}
    //private string GetDiceRollResult(DiceData dice)
    //{
    //    string[] stringResultArray = new string[dice.numberOfFaces];
    //    float[] vectorDotResultArray = new float[dice.numberOfFaces];
    //    float closestVectorDot = 0;
    //    int closestIndex = 0;

    //    for (int i = 0; i < dice.numberOfFaces; i++)
    //    {
    //        stringResultArray[i] = dice.facesArray[i].GetComponent<FaceComponent>().faceType;
    //        // /!\ /!\ /!\ AYMERIC A CHANGER LE TRANSFORM.UP EN TRANSFORM.FORWARD /!\ /!\ /!\
    //        // Produit scalaire entre le vector.up de chaque face et Vector3.up
    //        vectorDotResultArray[i] = Vector3.Dot(dice.facesArray[i].transform.up, Vector3.up);

    //        // Garde la face qui a son vecteur le plus vertical
    //        if (vectorDotResultArray[i] >= closestVectorDot)
    //        {
    //            closestVectorDot = vectorDotResultArray[i];
    //            closestIndex = i;
    //        }
    //    }
    //    return stringResultArray[closestIndex];
    //}    
    //private string GetDiceRollColorResult(DiceData dice)
    //{
    //    string[] stringResultArray = new string[dice.numberOfFaces];
    //    float[] vectorDotResultArray = new float[dice.numberOfFaces];
    //    float closestVectorDot = 0;
    //    int closestIndex = 0;

    //    for (int i = 0; i < dice.numberOfFaces; i++)
    //    {
    //        stringResultArray[i] = dice.facesArray[i].GetComponent<FaceComponent>().faceColor;
    //        // /!\ /!\ /!\ AYMERIC A CHANGER LE TRANSFORM.UP EN TRANSFORM.FORWARD /!\ /!\ /!\
    //        // Produit scalaire entre le vector.up de chaque face et Vector3.up
    //        vectorDotResultArray[i] = Vector3.Dot(dice.facesArray[i].transform.up, Vector3.up);

    //        // Garde la face qui a son vecteur le plus vertical
    //        if (vectorDotResultArray[i] >= closestVectorDot)
    //        {
    //            closestVectorDot = vectorDotResultArray[i];
    //            closestIndex = i;
    //        }
    //    }
    //    return stringResultArray[closestIndex];
    //}
}