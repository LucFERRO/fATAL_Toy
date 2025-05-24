using UnityEngine;

public class UnselectButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Unselect()
    {
        // Deselect the button
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
}
