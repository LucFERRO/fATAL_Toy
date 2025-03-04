using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UiManagerV2 : MonoBehaviour
{
    public DiceManagerV2 diceManager;
    public TMP_Text numberOfRollsLeftText;
    public GameObject confirmRollsButton;
    public GameObject addDiceButton;
    public GameObject rollButton;
    public GameObject displayArea;
    public TMP_Text errorMessage;

    void Start()
    {
        diceManager = GetComponent<DiceManagerV2>();
        numberOfRollsLeftText.text = diceManager.maxNumberOfRolls.ToString();
        confirmRollsButton.GetComponent<Button>().interactable = false;
    }

    public void DisplayErrorMessage(string message)
    {
        errorMessage.text = message;
        displayArea.SetActive(true);
    }
    public void ClearErrorMessage()
    {
        errorMessage.text = "";
        displayArea.SetActive(false);
    }

    public void UpdateNumberOfRollsLeft()
    {
        numberOfRollsLeftText.text = diceManager.currentNumberOfRolls.ToString();
    }

    public void EnableConfirmButton()
    {
        confirmRollsButton.GetComponent<Button>().interactable = true;
    }

    public void HideUiGameObject(GameObject objectToHide)
    {
        objectToHide.SetActive(false);
    }
}