using UnityEngine;

public class ConfirmationQuitScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ConfirmQuitPopup()
    {
        Debug.Log("Application Quit Confirmed");
        Application.Quit();
    }

    public void CancelQuitPopup()
    {
        gameObject.SetActive(false);
    }
}
