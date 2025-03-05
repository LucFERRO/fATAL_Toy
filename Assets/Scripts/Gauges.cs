using Unity.VisualScripting;
using UnityEngine;

public class Gauges : MonoBehaviour
{
    public int hunterMax = 5;
    public int hunterCurrent;
    public int hunterIncrement = 1;
    public int hunterDecrement = 1;
    public int villagesMax = 7;
    public int villagesCurrent;
    public int villagesIncrement = 1;
    public int villagesDecrement = 1;
    public int reputationMax = 9;
    public int reputationCurrent;
    public int reputationIncrement = 1;
    public int reputationDecrement = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Loop entered");
            hunterCurrent = ClampGauge(hunterCurrent, hunterMax, hunterIncrement);
            villagesCurrent = ClampGauge(villagesCurrent, villagesMax, villagesIncrement);
            reputationCurrent = ClampGauge(reputationCurrent, reputationMax, reputationIncrement);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Loop entered");
            hunterCurrent = ClampGauge(hunterCurrent, hunterMax, -hunterDecrement);
            villagesCurrent = ClampGauge(villagesCurrent, villagesMax, -villagesDecrement);
            reputationCurrent = ClampGauge(reputationCurrent, reputationCurrent, -reputationIncrement);
        }
    }

    private int ClampGauge(int gaugeValue, int gaugeMax, int incrementValue)
    {
        return Mathf.Clamp(gaugeValue + incrementValue, 0, gaugeMax);
    }
}
