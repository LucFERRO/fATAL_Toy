using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject quitConfirmationPopup;
    public void ExitGame()
    {
        Debug.Log("QUIT GAME");
        quitConfirmationPopup.SetActive(true);
    }
}
