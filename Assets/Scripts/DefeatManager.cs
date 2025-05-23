using TMPro;
using UnityEngine;

public class DefeatManager : MonoBehaviour
{
    public int maxNumberOfRolls;
    public int currentNumberOfRolls;
    public GameObject defeatCanvas;
    public GameObject rollCountGo;
    private GameManager gameManager;
    private TextMeshProUGUI rollsLeftText;
    public bool canRoll;
    void Start()
    {
        InitializeDefeatManager();
    }

    void InitializeDefeatManager()
    {
        currentNumberOfRolls = maxNumberOfRolls;
        canRoll = currentNumberOfRolls > 0;
        rollsLeftText = rollCountGo.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        rollsLeftText.text = currentNumberOfRolls.ToString();
        gameManager = GetComponent<GameManager>();
    }
    public void TriggerDefeat()
    {
        // AYMERIC SON / DEFEAT
        rollCountGo.transform.GetChild(0).gameObject.SetActive(false);
        rollCountGo.transform.GetChild(1).gameObject.SetActive(true);
        defeatCanvas.SetActive(true);
        defeatCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Restart";
        defeatCanvas.transform.GetComponent<Animator>().SetTrigger("ToggleTrigger");
    }

    public void HandleRollCount()
    {
        // AYMERIC SON / PERDRE UNE VIE
        currentNumberOfRolls--;
        if (currentNumberOfRolls <= 0) {
            rollCountGo.transform.GetChild(0).gameObject.SetActive(false);
            canRoll = false;
            TriggerDefeat();
        }
        string rollMessage = $"{(currentNumberOfRolls == 1 ? $"<size=50><color=#ff0000>{currentNumberOfRolls.ToString()}!" : currentNumberOfRolls.ToString())}";
        rollsLeftText.text = rollMessage;
    }
}
