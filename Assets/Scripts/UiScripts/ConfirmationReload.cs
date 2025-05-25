using UnityEngine;

public class ConfirmationReload : MonoBehaviour
{


    public void ToggleConfirmPopup(bool confirm)
    {
        // Reload the current scene
        gameObject.SetActive(confirm);
    }
}
