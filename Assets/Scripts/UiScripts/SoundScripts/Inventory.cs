using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Interface")]
    public FMOD.Studio.EventInstance interfaceEventInstance;
    //public int isObjectivePanel = 1;

    void Awake()
    {
        // Setting Interface instances
        interfaceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/UI");
        interfaceEventInstance.setParameterByName("IsInInventory", 0);
    }

    public void PlaySound(int state)
    {
        interfaceEventInstance.setParameterByName("UIState", state);
        //interfaceEventInstance.setParameterByName("ObjectivePanel", isObjectivePanel);
        interfaceEventInstance.start();
    }
}
