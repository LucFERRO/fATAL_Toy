using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Inventory inventory;
    public bool isOnButton;
    FMOD.Studio.EventInstance interfaceEventInstance;
    FMOD.Studio.EventInstance menuSelectEventInstance;
    FMOD.Studio.EventInstance ambianceEventInstance;
    FMOD.Studio.EventInstance mainMenuEventInstance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        inventory = transform.parent.GetComponent<Inventory>();
        interfaceEventInstance = inventory.interfaceEventInstance;

        menuSelectEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/MenuSelect");
        ambianceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Ambiance");
        mainMenuEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/MainMenu");

        ambianceEventInstance.start();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isOnButton)
        {
            if (gameObject.CompareTag("StartGameSelect"))
            {
                mainMenuEventInstance.setParameterByName("MainMenu", 1);
                mainMenuEventInstance.start();
            }
            else if (gameObject.CompareTag("UIEnter"))
            {
                menuSelectEventInstance.setParameterByName("MenuType", 0);
                mainMenuEventInstance.start();
            }
            else
            {
                menuSelectEventInstance.setParameterByName("MenuType", 1);
                menuSelectEventInstance.start();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.PlaySound(2);
        isOnButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOnButton = false;
    }

}
