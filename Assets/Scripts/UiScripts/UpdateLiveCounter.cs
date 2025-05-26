using UnityEngine;
using TMPro;

public class UpdateLiveCounter : MonoBehaviour
{
    public DefeatManager defeatManager;
    private TextMeshProUGUI liveCounterText;
    private void Start()
    {
        liveCounterText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void UpdateCounter()
    {
        int currentNumberOfRolls = defeatManager.currentNumberOfRolls;
        string rollMessage = $"{(currentNumberOfRolls == 1 ? $"<color=#ff0000>{currentNumberOfRolls.ToString()}!" : currentNumberOfRolls.ToString())}";
        Debug.Log(rollMessage);
        if (currentNumberOfRolls != 0)
        {
            liveCounterText.text = rollMessage;
        }
    }
}
