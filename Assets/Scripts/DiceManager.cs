using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    private UiManager uiManager;
    public GameObject diceSpawner;
    public GameObject[] allDices;
    public GameObject[] unusedDices;
    public GameObject[] allRolledDices;
    public DiceData[] allDiceDatas;
    public DiceData[] dicesInUse;
    public DiceData[] globalDicesDataInUse;

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
    public float elapsedTime;
    public float percentageComplete;
    private Vector3[] resultHolderPositions;
    public string[] currentRollResults;
    public string[] globalRollResults;
    public string[] possibleResults;

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
                uiManager.HideButton(uiManager.addDiceButton);
            }
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
            UpdateDicesInUse();
        }
    }
    void Start()
    {
        uiManager = GetComponent<UiManager>();
        CurrentNumberOfRolls = maxNumberOfRolls;
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
            else
            {
                ResetResultInertia();
                isConfirmed = false;
            }
        }
        if (unusedDices.Length > 0)
        {
            return;
        }
        else
        {
            UpdateUnusedDices();
        }

    }

    public void UpdateDiceData()
    {
        allDices = GameObject.FindGameObjectsWithTag("Dice");
        allDiceDatas = new DiceData[allDices.Length];
        globalDicesDataInUse = new DiceData[maxNumberOfDices];
        globalRollResults = new string[maxNumberOfDices];

        for (int i = 0; i < allDices.Length; i++)
        {
            allDiceDatas[i] = allDices[i].GetComponent<DiceData>();
        }
    }

    public void ConfirmRolls()
    {
        CurrentNumberOfRolls = 0;
        isConfirmed = true;
        uiManager.HideButton(uiManager.confirmRollsButton);
        uiManager.HideButton(uiManager.rollButton);
        UpdateRolledDices();
    }

    public void PutDicesToResultHolder()
    {
        for (int i = 0; i < allRolledDices.Length; i++)
        {
            allRolledDices[i].transform.eulerAngles = new Vector3(allRolledDices[i].transform.eulerAngles.x, 0, allRolledDices[i].transform.eulerAngles.z);
            allRolledDices[i].GetComponent<Rigidbody>().freezeRotation = true;
            allRolledDices[i].transform.position = Vector3.Lerp(allRolledDices[i].transform.position, resultHolderPositions[i], percentageComplete);
        }
    }

    private void ResetResultInertia()
    {
        for (int i = 0; i < allRolledDices.Length; i++)
        {
            allRolledDices[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    public void RollDices()
    {
        if (currentNumberOfRolls <= 0)
        {
            Debug.Log("OUT OF REROLLS!");
            return;
        }
        if (dicesInUse.Length == 0)
        {
            Debug.Log("NO DICE SELECTED!");
            return;
        }

        for (int i = 0; i < dicesInUse.Length; i++)
        {
            // Affecte le tag RolledDice aux dés utilisés
            if (!dicesInUse[i].gameObject.CompareTag("RolledDice"))
            {
                dicesInUse[i].gameObject.tag = "RolledDice";
            }

            // Vecteur pour le throw (just up pour l'instant) et random vector pour le random spin, 2 vecteurs pour moins de chances d'avoir une rotation nulle (arrive toujours parfois)
            dicesInUse[i].gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * rollThrowForce, ForceMode.Impulse);
            Vector3 randomSpinVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            dicesInUse[i].gameObject.GetComponent<Rigidbody>().AddTorque(randomSpinVector.normalized * rollSpinForce, ForceMode.Impulse);
        }
        allRolledDices = GameObject.FindGameObjectsWithTag("RolledDice");
        for (int i = 0; i < allRolledDices.Length; i++)
        {
            globalDicesDataInUse[i] = allRolledDices[i].GetComponent<DiceData>();
        }
        // Utilise un roll
        CurrentNumberOfRolls -= 1;
    }
    private void UpdateUnusedDices()
    {
        if (allRolledDices.Length >= maxNumberOfDices)
        {
            // Trie parmi tous les dés ceux qui ont toujours le tag "Dice" <=> ceux qui n'ont pas été roll
            unusedDices = allDices.Where(dice => dice.CompareTag("Dice")).ToArray();
            for (int i = 0; i < unusedDices.Length; i++)
            {
                unusedDices[i].SetActive(false);
            }
        }
    }

    private void UpdateDicesInUse()
    {
        // Trie parmi tous les dés ceux qui ont isInUse
        dicesInUse = allDiceDatas.Where(dice => dice.GetComponent<DiceData>().isInUse).ToArray();
        currentRollResults = new string[dicesInUse.Length];
    }
    private void UpdateRolledDices()
    {
        if (dicesInUse.Length == 0)
        {
            dicesInUse = new DiceData[0];
            currentRollResults = new string[0];
            return;
        }

        for (int i = 0; i < dicesInUse.Length; i++)
        {
            currentRollResults[i] = GetDiceRollResult(dicesInUse[i]);
        }

        UpdateGlobalRollResults();
    }

    private void UpdateGlobalRollResults()
    {
        for (int i = 0; i < globalDicesDataInUse.Length; i++)
        {
            if (globalDicesDataInUse[i] == null)
            {
                return;
            }
            globalRollResults[i] = GetDiceRollResult(globalDicesDataInUse[i]);
        }
    }
    private string GetDiceRollResult(DiceData dice)
    {
        string[] stringResultArray = new string[dice.numberOfFaces];
        float[] vectorDotResultArray = new float[dice.numberOfFaces];
        float closestVectorDot = 0;
        int closestIndex = 0;

        for (int i = 0; i < dice.numberOfFaces; i++)
        {
            stringResultArray[i] = dice.facesArray[i].GetComponent<FaceComponent>().faceType;
            // /!\ /!\ /!\ AYMERIC A CHANGER LE TRANSFORM.UP EN TRANSFORM.FORWARD /!\ /!\ /!\
            // Produit scalaire entre le vector.up de chaque face et Vector3.up
            vectorDotResultArray[i] = Vector3.Dot(dice.facesArray[i].transform.up, Vector3.up);

            // Garde la face qui a son vecteur le plus vertical
            if (vectorDotResultArray[i] >= closestVectorDot)
            {
                closestVectorDot = vectorDotResultArray[i];
                closestIndex = i;
            }
        }
        return stringResultArray[closestIndex];
    }
}