using UnityEngine;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

public class PhysicalDiceManager : MonoBehaviour
{
    private PhysicalRollUiManager uiManager;

    [Header("Dice Spawner Parameters")]
    public Vector2[] possibleDiceVectors;
    public string[] possibleDiceColors;
    public string[] possibleDiceRarities;

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
    //TO DO
    public FaceComponent[] diceResultsArray;

    [Header("Dice Data")]
    public GameObject[] allDices;
    public DiceData[] allDiceDatas;
    public List<DiceData> dicesInUse;
    public int numberOfDicesInUse;

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
        for (int i = 0; i < possibleDiceVectors.Length; i++)
        {
            possibleDiceVectors[i] = possibleDiceVectors[i].normalized;
        }
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
}