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
    private Animator rollCounterAnimator;
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
        rollCounterAnimator = rollCountGo.GetComponent<Animator>();
        rollsLeftText.text = currentNumberOfRolls.ToString();
        gameManager = GetComponent<GameManager>();
    }
    public void TriggerDefeat()
    {
        rollCountGo.transform.GetChild(0).gameObject.SetActive(false);
        rollCountGo.transform.GetChild(1).gameObject.SetActive(true);
        defeatCanvas.SetActive(true);
        defeatCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Restart";
        defeatCanvas.transform.GetComponent<Animator>().SetTrigger("ToggleTrigger");
    }

    public void HandleRollCount()
    {
        currentNumberOfRolls--;
        if (currentNumberOfRolls >= 1)
        {
            rollCounterAnimator.SetTrigger("UpdateCounter");
        }
        if (currentNumberOfRolls <= 0) {
            rollCountGo.transform.GetChild(0).gameObject.SetActive(false);
            canRoll = false;
            TriggerDefeat();
        }
    }
}
