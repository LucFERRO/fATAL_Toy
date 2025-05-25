using UnityEngine;
using UnityEngine.EventSystems;

public class LoadMenuSounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    FMOD.Studio.EventInstance interfaceEventInstance;
    FMOD.Studio.EventInstance menuSelectEventInstance;

    public bool isOnButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        interfaceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/UI");
        interfaceEventInstance.setParameterByName("IsInInventory", 0);
        menuSelectEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/MenuSelect");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isOnButton)
        {
            menuSelectEventInstance.setParameterByName("MenuType", 0);
            interfaceEventInstance.start();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        interfaceEventInstance.setParameterByName("UIState", 2);
        interfaceEventInstance.start();
        isOnButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOnButton = false;
    }
}
