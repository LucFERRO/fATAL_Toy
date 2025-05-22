using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Interface")]
    public FMOD.Studio.EventInstance interfaceEventInstance;
    //public int isObjectivePanel = 1;

    void Start()
    {
        // Setting Interface instances
        interfaceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/UI");
    }

    public void PlaySound(int state)
    {
        interfaceEventInstance.setParameterByName("UIState", state);
        //interfaceEventInstance.setParameterByName("ObjectivePanel", isObjectivePanel);
        interfaceEventInstance.start();
    }
}
