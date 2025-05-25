using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public FMOD.Studio.EventInstance mainMenuEventInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/MainMenu");
        mainMenuEventInstance.setParameterByName("MainMenu", 0);
    }

    // Update is called once per frame
    public void MainMenuEnter()
    {
        mainMenuEventInstance.start();
    }

    public void MainMenuExit()
    {
        mainMenuEventInstance.start();
        mainMenuEventInstance.setParameterByName("MainMenu", 1);
    }
}
