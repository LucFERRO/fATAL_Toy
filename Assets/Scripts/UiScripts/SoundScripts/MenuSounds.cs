using System.Collections;
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
    public GameManager gameManager;
    public bool isObjective;

    void Awake()
    {
        inventory = transform.parent.GetComponent<Inventory>();

        interfaceEventInstance = inventory.interfaceEventInstance;
        InstanciateParentInterfaceEvent();

        menuSelectEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/MenuSelect");
        ambianceEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Ambiance");
        mainMenuEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/MainMenu");

        ambianceEventInstance.start();
        if (gameManager != null)
        {
            isObjective = true;
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isOnButton)
        {
            if (isObjective)
            {
                if (gameManager.isPreviewing)
                {
                    return;
                }
            }
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
    public IEnumerator InstanciateParentInterfaceEvent()
    {
        yield return new WaitForEndOfFrame();

        inventory.interfaceEventInstance = interfaceEventInstance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOnButton = true;
        if (isObjective)
        {
            return;
        }
        inventory.PlaySound(2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOnButton = false;
    }
}