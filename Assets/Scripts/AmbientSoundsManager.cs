using UnityEngine;

public class AmbientSoundsManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance ambientEventInstance;
    private FMOD.Studio.EventInstance musicEventInstance;
    public FMOD.Studio.EventInstance wildlifeEventInstance;

    void Start()
    {
        ambientEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Ambiance");
        musicEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music");
        wildlifeEventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Wildlife");

        musicEventInstance.start();
        ambientEventInstance.start();
        wildlifeEventInstance.setParameterByName("Wildlife", 0);
        wildlifeEventInstance.start();
    }
}
