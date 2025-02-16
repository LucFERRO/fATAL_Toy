using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public DiceManager diceManager;
    public TMP_Text numberOfRollsLeftText;
    public GameObject confirmRollsButton;
    public GameObject addDiceButton;
    public GameObject rollButton;
    void Start()
    {
        //diceManager = GetComponent<DiceManager>();
        numberOfRollsLeftText.text = diceManager.maxNumberOfRolls.ToString();
        confirmRollsButton.GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateNumberOfRollsLeft()
    {
        numberOfRollsLeftText.text = diceManager.currentNumberOfRolls.ToString();
    }

    public void EnableConfirmButton()
    {
        confirmRollsButton.GetComponent<Button>().interactable = true;
    }

    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }
}
